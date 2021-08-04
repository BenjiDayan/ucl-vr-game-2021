using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PDController : MonoBehaviour
{
    List<GameObject> availableDrones = new List<GameObject>();
    List<GameObject> debris = new List<GameObject>();
    List<GameObject> processedDebris = new List<GameObject>();
    List<GameObject> scrambledDrones = new List<GameObject>();
    List<GameObject> dronesBeingHacked = new List<GameObject>();
    Dictionary<GameObject, GameObject> collectorDrones = new Dictionary<GameObject, GameObject>();

    public float buildingConstructionTime = 5f;
    public float glowFadeTime = 1f;

    
    bool creatingBuilding = false;
    bool constructionUnderway = false;
    bool fadingUnderway = false;
    List<GameObject> builderDrones;
    GameObject hologram;
    List<List<float>> buildingOffsets = new List<List<float>>();
    int dronesInPosition;
    float constructionStartTime;
    float constructionCompletedTime;
    List<float> droneOffsets;

    //Dealing with the material for the building under construction
    public Material glowMaterial;

    [Header("Leave as default")]
    [SerializeField] public Transform raycatcherTransform;
    [SerializeField] public List<List<float>> buildingDimensions = new List<List<float>>();
    [SerializeField] public int buildingIndex;
    [SerializeField] public float radius;
    [SerializeField] public List<GameObject> dronesFollowingPlayer;

    GameObject buildingInProgress;
    MeshRenderer renderer;
    MeshFilter filter;

    List<GameObject> buildingPrefabs;

    //Custom .Contains function from the internet
    bool ArrayContains(GameObject[] array, GameObject g)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == g) return true;
        }
        return false;
    }

    void ReceiveDimensions(List<List<float>> trueBuildingDimensions)
    {
        buildingDimensions = trueBuildingDimensions;
    }

    void ReceiveOffsets(List<List<float>> prefabOffsets)
    {
        buildingOffsets = prefabOffsets;
    }

    void InPosition()
    {
        dronesInPosition++;
    }

    void Start()
    {
        hologram = GameObject.Find("Hologram");
        buildingPrefabs = GameObject.Find("City Builder").GetComponent<CityBuilder>().buildingPrefabs;
    }

    void Update()
    {
        //Begin constructing the building
        if (creatingBuilding && dronesInPosition == 3)
        {
            creatingBuilding = false;
            constructionUnderway = true;
            constructionStartTime = Time.realtimeSinceStartup;
            
            //Create the glowing building preview
            buildingInProgress = new GameObject();
            if (buildingIndex == 0 || buildingIndex == 4)
            {
                buildingInProgress.transform.localScale = new Vector3(1.02f, 1.02f, 1.02f);
            }
            Vector3 relativePosition = new Vector3(
                -buildingOffsets[buildingIndex][0],
                0,
                -buildingOffsets[buildingIndex][1]
                );
            switch (buildingIndex)
            {
                case 0:
                    relativePosition.y -= 2.5f;
                    break;
                case 4:
                    relativePosition.y -= 4.0528f;
                    break;
            }
            buildingInProgress.transform.position = raycatcherTransform.position + relativePosition;
            renderer = buildingInProgress.AddComponent<MeshRenderer>();
            filter = buildingInProgress.AddComponent<MeshFilter>();
            try
            {
                filter.mesh = buildingPrefabs[buildingIndex].GetComponent<SkinnedMeshRenderer>().sharedMesh;
            }
            catch (MissingComponentException e)
            {
                filter.mesh = buildingPrefabs[buildingIndex].GetComponent<MeshFilter>().sharedMesh;
            }
            Material[] materialsTemp = new Material[filter.mesh.subMeshCount];
            for (int i = 0; i < filter.mesh.subMeshCount; i++)
            {
                materialsTemp[i] = glowMaterial;
            }
            renderer.materials = materialsTemp;
            glowMaterial.SetColor("_EmissionColor", Color.black);
            
            //Message the drones
            for (int i = 0; i < builderDrones.Count; i++)
            {
                builderDrones[i].SendMessage("BeginOrbit", droneOffsets[i]);
            }
        }
        
        //Build the building
        if (constructionUnderway)
        {
            if (Time.realtimeSinceStartup - constructionStartTime < buildingConstructionTime)
            {
                float emissionStrength = ((Time.realtimeSinceStartup - constructionStartTime) / buildingConstructionTime);
                glowMaterial.SetColor("_EmissionColor", new Color(emissionStrength, emissionStrength, emissionStrength, 1));
            }
            else
            {
                constructionUnderway = false;
                fadingUnderway = true;
                hologram.SendMessage("ConstructionComplete");
                foreach (GameObject drone in builderDrones)
                {
                    drone.SendMessage("EndTask");
                }
                constructionCompletedTime = Time.realtimeSinceStartup;
                Instantiate(
                    buildingPrefabs[buildingIndex],
                        new Vector3(
                            raycatcherTransform.position.x - buildingOffsets[buildingIndex][0],
                            0,
                            raycatcherTransform.position.z - buildingOffsets[buildingIndex][1]
                            ),
                        Quaternion.identity,
                        raycatcherTransform
                    );
            }
        }

        //Make the glowing building preview fade away after the construction is complete
        if (fadingUnderway)
        {
            if (Time.realtimeSinceStartup - constructionCompletedTime < glowFadeTime)
            {
                float emissionStrength = (1 - (Time.realtimeSinceStartup - constructionCompletedTime) / glowFadeTime);
                glowMaterial.SetColor("_EmissionColor", new Color(emissionStrength, emissionStrength, emissionStrength, 1));
            }
            else
            {
                fadingUnderway = false;
                Destroy(buildingInProgress);
            }
        }

        //Look for floating debris, available drones and scrambled enemy drones
        debris = new List<GameObject>();
        availableDrones = new List<GameObject>();
        scrambledDrones = new List<GameObject>();
        Vector3 segmentPosition;
        foreach (GameObject segment in GameObject.FindGameObjectsWithTag("Debris"))
        {
            //Check if any debris is out of bounds and deal with it
            segmentPosition = segment.transform.position;
            if (segmentPosition.y < 0f || segmentPosition.y > 1000f)
            {
                if (collectorDrones.ContainsKey(segment))
                {
                    collectorDrones[segment].SendMessage("EndTask");
                }
                Destroy(segment);
            }
            else if (!processedDebris.Contains(segment))
            {
                debris.Add(segment);
            }
        }
        dronesFollowingPlayer = new List<GameObject>();
        foreach (GameObject drone in GameObject.FindGameObjectsWithTag("Player Drone"))
        {
            if (drone.GetComponent<Drone>().mode == "follow player")
            {
                dronesFollowingPlayer.Add(drone);
                availableDrones.Add(drone);
            }
        }
        foreach (GameObject drone in GameObject.FindGameObjectsWithTag("Enemy Drone"))
        {
            if (drone.GetComponent<Drone>().mode == "scrambled" && !dronesBeingHacked.Contains(drone))
            {
                scrambledDrones.Add(drone);
            }
        }

        //Find up to three drones to build my building
        while (creatingBuilding && builderDrones.Count < 3 && availableDrones.Count != 0)
        {
            builderDrones.Add(availableDrones[0]);
            availableDrones[0].SendMessage("GoToConstructionSite", raycatcherTransform.position + new Vector3(
                Mathf.Sin(2 * Mathf.PI * (builderDrones.Count / 3f)) * radius,
                0,
                Mathf.Cos(2 * Mathf.PI * (builderDrones.Count / 3f)) * radius
                ));
            droneOffsets.Add(builderDrones.Count / 3f);
            availableDrones.RemoveAt(0);
        }

        //Assign drones scrambled enemy drones to hack
        float distance;
        float distanceTemp;
        int distanceIndex = 0;
        int numOfAvailableDrones = availableDrones.Count;
        for (int i = 0; i < scrambledDrones.Count && i < numOfAvailableDrones; i++)
        {
            distance = 9999f;
            for (int j = 0; j < availableDrones.Count; j++)
            {
                distanceTemp = Vector3.Distance(availableDrones[j].transform.position, scrambledDrones[i].transform.position);
                if (distanceTemp < distance)
                {
                    distance = distanceTemp;
                    distanceIndex = j;
                }
            }
            availableDrones[distanceIndex].SendMessage("ReceiveEnemyDroneTarget", scrambledDrones[i]);
            availableDrones.RemoveAt(distanceIndex);
            dronesBeingHacked.Add(scrambledDrones[i]);
        }

        //Assign drones debris to collect
        if (debris.Count != 0)
        {
            for (int i = 0; i < debris.Count && i < availableDrones.Count; i++)
            {
                distance = 9999f;
                for (int j = 0; j < debris.Count; j++)
                {
                    if (!processedDebris.Contains(debris[j]))
                    {
                        distanceTemp = Vector3.Distance(debris[j].transform.position, availableDrones[i].transform.position);
                        if (distanceTemp < distance)
                        {
                            distance = distanceTemp;
                            distanceIndex = j;
                        }
                    }
                }
                availableDrones[i].SendMessage("AssignDebrisTarget", debris[distanceIndex]);
                processedDebris.Add(debris[distanceIndex]);
                collectorDrones.Add(debris[distanceIndex], availableDrones[i]);
            }
        }
    }

    void CreateBuilding(Transform passedTransform)
    {
        raycatcherTransform = passedTransform;
        creatingBuilding = true;
        builderDrones = new List<GameObject>();
        buildingIndex = hologram.GetComponent<Hologram>().listIndex;
        radius = buildingDimensions[buildingIndex][0] / 2;
        if (radius > buildingDimensions[buildingIndex][1] / 2)
        {
            radius = buildingDimensions[buildingIndex][1] / 2;
        }
        dronesInPosition = 0;
        droneOffsets = new List<float>();
    }
}
