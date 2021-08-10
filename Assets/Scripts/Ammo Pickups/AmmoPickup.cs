using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [Header("Drop settings")]
    [SerializeField] public float rotationRate = 80f;
    [SerializeField] public float lifetime = 90f;

    [Header("Ammo settings")]
    [SerializeField] public int laserStock = 24;
    [SerializeField] public int bulletStock = 48;
    [SerializeField] public int rocketStock = 8;

    [Header("Dynamic scale settings")]
    [SerializeField] public bool dynamicScale = false;
    [SerializeField] public float maxDistance = 50f;
    [SerializeField] public float minDistance = 5f;
    [SerializeField] public float maxScale = 10f;
    [SerializeField] public float minScale = 3f;

    Transform playerTransform;
    PlayerGunFPS2 gunScript;
    Magazine magazine;

    int ammoType;

    float collectionDistance = 0.63562f;

    float timeOfDeath;

    int WeightedRandom(float[] weights)
    {
        float totalWeight = 0;
        foreach(float weight in weights)
        {
            totalWeight += weight;
        }
        for (int i = 0; i < weights.Length; i++)
        {
            if (Random.Range(0f, 1f) < weights[i] / totalWeight)
            {
                return (i);
            }
            else
            {
                totalWeight -= weights[i];
            }
        }
        return (weights.Length - 1);
    }

    float[] GetWeights()
    {
        Magazine laserMagazine = gunScript.magazines[0];
        Magazine bulletMagazine = gunScript.magazines[1];
        Magazine rocketMagazine = gunScript.magazines[2];

        float laserAndBulletWeight = Mathf.Clamp((float)(laserMagazine._size + laserMagazine._stockSize + bulletMagazine._size + bulletMagazine._stockSize)
            / (float)(laserMagazine._ammo + laserMagazine._stock + bulletMagazine._ammo + bulletMagazine._stock) - 1f, 0f, 99999999f);
        float rocketWeight = Mathf.Clamp((float)(rocketMagazine._size + rocketMagazine._stockSize)
            / (float)(rocketMagazine._ammo + rocketMagazine._stock) - 1f, 0f, 99999999f);

        return (new float[] { laserAndBulletWeight, laserAndBulletWeight, rocketWeight });
    }

    void Start()
    {
        playerTransform = GameObject.Find("FPS_Player").transform;
        gunScript = GameObject.Find("Gun").GetComponent<PlayerGunFPS2>();

        timeOfDeath = Time.realtimeSinceStartup + lifetime;

        //Decide what kind of ammo I am going to be
        float[] ammoWeights = GetWeights();
        ammoType = WeightedRandom(ammoWeights);
        magazine = gunScript.magazines[ammoType];
        transform.GetChild(ammoType).GetComponent<MeshRenderer>().enabled = true;
    }

    void Update()
    {
        //Player is close enough to pick up ammo
        if (Vector3.Distance(transform.position, playerTransform.position) < collectionDistance * transform.localScale.x)
        {
            if (magazine._stock < magazine._stockSize)
            {
                magazine.ChangeStock(new int[] { laserStock, bulletStock, rocketStock}[ammoType]);
                Destroy(gameObject);
            }
        }

        //Time to die
        if (Time.realtimeSinceStartup > timeOfDeath)
        {
            Destroy(gameObject);
        }

        //If the pickups are large enough to be seen from a distance, they are comically massive up close
        //so I tried to vary the scale with the distance from the player
        //unfortunately it doesn't look very good
        if (dynamicScale)
        {
            float a = (maxScale - minScale) / (maxDistance - minDistance);
            float b = minScale - a * minDistance;
            transform.localScale = Vector3.one * (a * Mathf.Clamp(Vector3.Distance(transform.position, playerTransform.position), minDistance, maxDistance) + b);
        }

        //Rotation
        transform.Rotate(Vector3.up * rotationRate * Time.deltaTime);
    }
}
