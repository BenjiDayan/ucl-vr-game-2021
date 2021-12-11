using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoController : MonoBehaviour
{
    List<GameObject> spawners;
    float timeLastSpawned = 0;
    GameObject player;

    public float minDistance = 100f;
    public float maxDistance = 300f;
    public float spawnCooldown = 50f;
    public GameObject ammoPickupPrefab;

    void Start()
    {

        player = GameObject.Find("FPS_Player");
    }

    float HorizontalDistance(Vector3 firstPosition, Vector3 secondPosition)
    {
        Vector3 temp = firstPosition - secondPosition;
        temp.y = 0;
        return (temp.magnitude);
    }

    void Update()
    {
        if (Time.realtimeSinceStartup - timeLastSpawned > spawnCooldown)
        {
            timeLastSpawned = Time.realtimeSinceStartup;

            //Get all spawners
            spawners = new List<GameObject>();
            foreach (GameObject spawner in GameObject.FindGameObjectsWithTag("Ammo Spawner"))
            {
                if (spawner.transform.childCount == 0)
                {
                    spawners.Add(spawner);
                }
            }

            //Get spawners which are the correct distance from the player
            List<GameObject> validSpawners = new List<GameObject>();
            foreach (GameObject spawner in spawners)
            {
                float distance = HorizontalDistance(spawner.transform.position, player.transform.position);
                if (distance > minDistance && distance < maxDistance && spawner.transform.childCount == 0)
                {
                    validSpawners.Add(spawner);
                }
            }

            //If there are no spawners the correct distance, get spawners within the max distance
            if (validSpawners.Count == 0)
            {
                foreach (GameObject spawner in spawners)
                {
                    if (HorizontalDistance(spawner.transform.position, player.transform.position) < maxDistance && spawner.transform.childCount == 0)
                    {
                        validSpawners.Add(spawner);
                    }
                }
            }

            //If there are no spawners within max distance, get all spawners
            if (validSpawners.Count == 0)
            {
                foreach (GameObject spawner in spawners)
                {
                    if (spawner.transform.childCount == 0)
                    {
                        validSpawners.Add(spawner);
                    }
                }
            }

            //Shuffle the list of spawners (not actually necessary as it turns out)
            /*
            List<GameObject> spawnersTemp = new List<GameObject>();
            int index;
            while (validSpawners.Count != 0)
            {
                index = Random.Range(0, validSpawners.Count);
                spawnersTemp.Add(validSpawners[index]);
                validSpawners.RemoveAt(index);
            }
            */

            Instantiate(ammoPickupPrefab, validSpawners[Random.Range(0, validSpawners.Count)].transform, false);

        }
    }
}
