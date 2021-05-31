using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hologram : MonoBehaviour
{

    Transform gun;
    Renderer rend;
    RaycastHit[] hits;

    public GameObject cyberbuilding;

    //Sorting algorithm from the internet
    public static void RaycastHitsSort(RaycastHit[] data)
    {
        int i, j;
        RaycastHit temp;
        int N = data.Length;

        for (j = N - 1; j > 0; j--)
        {
            for (i = 0; i < j; i++)
            {
                if (data[i].distance > data[i + 1].distance)
                {
                    temp = data[i];
                    data[i] = data[i + 1];
                    data[i + 1] = temp;
                }
            }
        }
    }

    void Start()
    {
        gun = GameObject.Find("Gun").transform;
        rend = transform.GetChild(0).GetComponent<Renderer>();
    }

    void Update()
    {
        hits = Physics.RaycastAll(gun.position, gun.forward, 1000f, 1 << 10);
        RaycastHitsSort(hits);
        if (hits.Length != 0)
        {
            //Create new building
            if (Input.GetKey(KeyCode.Mouse2))
            {
                Instantiate(cyberbuilding, hits[0].transform);
                hits[0].collider.enabled = false;
            }
            else
            {
                transform.position = hits[0].transform.position;
                rend.enabled = true;
            }
        }
        else
        {
            rend.enabled = false;
        }
    }
}
