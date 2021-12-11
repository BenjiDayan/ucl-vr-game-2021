using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MainEnemy : MonoBehaviour
{
    [Header("Balance settings")]
    [SerializeField] public float minDestinationDistance = 500f;
    [SerializeField] public float totalDistanceToCover = 2800f;
    [SerializeField] public int totalDronesToRelease = 12;
    [SerializeField] public float startingHP = 100f;

    [Header("Prefabs")]
    [SerializeField] public GameObject dronePrefab;
    [SerializeField] public GameObject destinationMarkerPrefab;
    PlayerUI ui;

    GameObject destinationMarker;

    LineRenderer line;

    NavMeshAgent agent;
    List<Vector3> destinations;
    Vector3 destination;

    Vector3 previousPosition = Vector3.zero;
    float timeSinceStuck = 0f;

    float hp;

    int dronesReleased = 0;

    float destinationCompletenessChange;
    float completeness;
    float currentPathLength;

    List<Vector3> Shuffle(List<Vector3> inputList)
    {
        int randomIndex;
        List<Vector3> outputList = new List<Vector3>();
        while (inputList.Count != 0)
        {
            randomIndex = Random.Range(0, inputList.Count);
            outputList.Add(inputList[randomIndex]);
            inputList.RemoveAt(randomIndex);
        }
        return(outputList);
    }

    float PathLength(NavMeshPath path)
    {
        float length = 0;
        for (int i = 1; i < path.corners.Length; i++)
        {
            length += Vector3.Distance(path.corners[i - 1], path.corners[i]);
        }
        return length;
    }

    public void Awake() {
        Debug.Log("Enemy searches for UI");
        ui = (PlayerUI)FindObjectOfType(typeof(PlayerUI));
        Debug.Log("Has enemy found UI?");
        Debug.Log(ui);
    }

    void RandomDestination()
    {
        float distance;

        destinations = Shuffle(destinations);
        for (int i = 0; i < destinations.Count; i++)
        {
            distance = Vector3.Distance(transform.position, destinations[i]);
            NavMeshPath path = new NavMeshPath();
            if (distance > minDestinationDistance && agent.CalculatePath(destinations[i], path) && path.status == NavMeshPathStatus.PathComplete)
            {
                agent.destination = destinations[i];
                destination = destinations[i];
                float pathLengthTemp = PathLength(path);
                destinationCompletenessChange = pathLengthTemp / totalDistanceToCover;
                if (destinationCompletenessChange + completeness > 1f)
                {
                    destinationCompletenessChange = 1f - completeness;
                }
                currentPathLength = pathLengthTemp;
                break;
            }
        }
    }

    void ReceiveDestinations(List<Vector3> inputDestinations)
    {
        hp = startingHP;
        ui.UpdateEnemyHealth(hp);

        agent = GetComponent<NavMeshAgent>();

        destinations = Shuffle(inputDestinations);
        for (int i = 0; i < destinations.Count; i++)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(destinations[i], out hit, 4f, NavMesh.AllAreas))
            {
                transform.position = hit.position;
                break;
            }
        }

        agent.enabled = true;

        line = transform.Find("Line").GetComponent<LineRenderer>();

        destinationMarker = Instantiate(destinationMarkerPrefab);

        RandomDestination();
    }

    void Update()
    {
        ui.UpdateEnemyHealth(hp);
        ui.UpdateEnemyHealthBar(hp / startingHP);

        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(destination, path);
        ui.UpdateCompleteness(completeness + destinationCompletenessChange * (1 - PathLength(path) / currentPathLength));

        if (hp > 0)
        {
            //Mark the path we're following
            line.SetVertexCount(agent.path.corners.Length);
            for (int i = 0; i < agent.path.corners.Length; i++)
            {
                line.SetPosition(i, agent.path.corners[i]);
            }

            //If we are trapped, and we're as close as we're going to get to the destination, blow up the closest building between me and the destination
            //Or if we haven't moved for five seconds
            if ((agent.pathStatus != NavMeshPathStatus.PathComplete && Vector3.Distance(transform.position, agent.destination) < 10) || previousPosition == transform.position)
            {
                timeSinceStuck += Time.deltaTime;
                if (timeSinceStuck > 5)
                {
                    GameObject[] landmarkBuildings = GameObject.FindGameObjectsWithTag("Landmark");
                    GameObject[] genericBuildings = GameObject.FindGameObjectsWithTag("Generic Building");
                    int arrayOriginalSize = landmarkBuildings.Length;
                    System.Array.Resize(ref landmarkBuildings, arrayOriginalSize + genericBuildings.Length);
                    System.Array.Copy(genericBuildings, 0, landmarkBuildings, arrayOriginalSize, genericBuildings.Length);
                    float distanceTemp = Mathf.Infinity;
                    GameObject closestBuilding = landmarkBuildings[0];
                    foreach (GameObject building in landmarkBuildings)
                    {
                        if (Vector3.Distance(transform.position, building.transform.position) < distanceTemp)
                        {
                            distanceTemp = Vector3.Distance(transform.position, building.transform.position);
                            closestBuilding = building;
                        }
                    }
                    closestBuilding.SendMessage("Collapse", transform.position.y);
                }
            }
            else
            {
                timeSinceStuck = 0;
            }

            //You have arrived at your destination
            if (Vector3.Distance(transform.position, destination) < 30f)
            {
                completeness += destinationCompletenessChange;
                if (completeness >= 1)
                {
                    PauseMenu pauseMenu = (PauseMenu)FindObjectOfType(typeof(PauseMenu));
                    pauseMenu.ReloadGame();
                }
                else
                {
                    int dronesToRelease = (int)Mathf.Ceil(totalDronesToRelease * Mathf.Pow(completeness, 2f) / 4f) - dronesReleased;
                    if (dronesToRelease < 1)
                    {
                        dronesToRelease = 1;
                    }

                    GameObject newDrone;
                    Drone droneScript;
                    for (int i = 0; i < dronesToRelease; i++)
                    {
                        newDrone = Instantiate(dronePrefab);
                        droneScript = newDrone.GetComponent<Drone>();
                        newDrone.tag = "Enemy Drone";
                        droneScript.mode = "attack cooldown";
                        droneScript.futureTime = Time.realtimeSinceStartup + Random.Range(0, droneScript.attackRunCooldown);

                        newDrone.transform.position = new Vector3(destination.x + Mathf.Sin(i / dronesToRelease) * 15,
                            destination.y * i / dronesToRelease,
                            destination.z + Mathf.Cos(i / dronesToRelease) * 15);
                    }
                }

                RandomDestination();
            }

            previousPosition = transform.position;

            Vector3 destinationTemp = destination;
            destinationTemp.y = 0;
            destinationMarker.transform.position = destinationTemp;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag == "Debris")
        {
            hp -= float.Parse(collision.collider.gameObject.name);

            if (hp <= 0)
            {
                transform.Find("Mask").gameObject.SendMessage("BeginCrash");
                line.enabled = false;
            }
        }
    }
}
