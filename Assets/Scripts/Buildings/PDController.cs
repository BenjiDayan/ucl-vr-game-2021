using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PDController : MonoBehaviour
{
    List<GameObject> availableDrones = new List<GameObject>();
    List<GameObject> debris = new List<GameObject>();
    List<GameObject> processedDebris = new List<GameObject>();

    public GameObject droneMaskPrefab;

    //Custom .Contains function from the internet
    bool ArrayContains(GameObject[] array, GameObject g)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == g) return true;
        }
        return false;
    }

    void Start()
    {
        GameObject droneMask;
        foreach (GameObject drone in GameObject.FindGameObjectsWithTag("Player Drone"))
        {
            droneMask = Instantiate(droneMaskPrefab);
            droneMask.SendMessage("ReceiveDroneObject", drone);
        }
    }

    void Update()
    {
        debris = new List<GameObject>();
        availableDrones = new List<GameObject>();
        foreach (GameObject segment in GameObject.FindGameObjectsWithTag("Debris"))
        {
            if (!processedDebris.Contains(segment))
            {
                debris.Add(segment);
            }
        }
        if (debris.Count != 0)
        {
            foreach (GameObject drone in GameObject.FindGameObjectsWithTag("Player Drone"))
            {
                if(drone.GetComponent<PlayerDrone>().mode == "follow player")
                {
                    availableDrones.Add(drone);
                }
            }
            for (int i = 0; i < debris.Count && i < availableDrones.Count; i++)
            {
                availableDrones[i].SendMessage("AssignDebrisTarget", debris[i]);
                processedDebris.Add(debris[i]);
            }
        }
    }
}
