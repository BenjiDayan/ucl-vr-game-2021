using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    Drone droneScript;
    MeshRenderer renderer;
    float random = 0f;
    bool projectileAlreadyInstantiated = false;
    public GameObject droneProjectilePrefab;
    GameObject player;
    Vector3 previousPlayerPosition;
    float projectileSpeed;

    void Start()
    {
        renderer = GetComponent<MeshRenderer>();
        player = GameObject.Find("FPS_Player");

        projectileSpeed = droneProjectilePrefab.GetComponent<Projectile>().speed;
    }

    void ReceiveDroneObject(GameObject assignedDrone)
    {
        droneScript = assignedDrone.GetComponent<Drone>();
    }

    void Update()
    {
        Vector3 playerPosition = player.transform.position;

        if (droneScript.mode == "attack" && ((Time.realtimeSinceStartup + random) % 0.125f) > 0.0875f)
        {
            renderer.enabled = true;
            if (!projectileAlreadyInstantiated)
            {
                //Work out how far we need to lead the shot
                Vector3 lead = (playerPosition - previousPlayerPosition) / Time.deltaTime;
                lead *= Vector3.Distance(playerPosition, transform.position) / projectileSpeed;

                projectileAlreadyInstantiated = true;
                GameObject projectile = Instantiate(droneProjectilePrefab);
                projectile.transform.position = transform.position;
                projectile.transform.forward = playerPosition + lead - projectile.transform.position;
                projectile.GetComponent<Projectile>()._trajectory = projectile.transform.forward;
            }
        }
        else
        {
            transform.Rotate(0, 0, Random.Range(0f, 120f));
            float brightness = Random.Range(0.65f, 1.5f);
            renderer.material.SetColor("_EmissionColor", new Color(4.237095f * brightness, 3.43848f * brightness, 2.617682f * brightness, 1));
            renderer.enabled = false;
            transform.localScale = Vector3.one * Random.Range(0.8f, 1.3f);
            projectileAlreadyInstantiated = false;
        }
        random += Random.Range(0f, 0.02f);

        previousPlayerPosition = playerPosition;
    }
}
