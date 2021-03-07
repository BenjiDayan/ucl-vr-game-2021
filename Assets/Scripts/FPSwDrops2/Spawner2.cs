using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner2 : MonoBehaviour
{
    [SerializeField] bool spawnedDrop;  // Spawner can only spawn one item at a time

    [SerializeField] float cooldown = 15;
    [SerializeField] float cooldownCount = 0;

    [SerializeField] float distance = 15;

    [SerializeField] SpawnerData2[] spawns;
    [SerializeField] bool random;
    [SerializeField] int index;

    [SerializeField] List<Drop2> drops = new List<Drop2>();

    public bool _spawnedDrop
    {
        get => spawnedDrop;
        set => spawnedDrop = value;
    }

    private void Start()
    {
        foreach (var spawn in spawns)
        {
            for (int i = 0; i < spawn._weight; i++)
            {
                drops.Add(spawn._drop);
            }
        }
    }

    private void Update()
    {
        cooldownCount += Time.deltaTime;

        if (!spawnedDrop && cooldownCount >= cooldown)
        {
            if (random) SpawnObjectRandom();
            else SpawnObject(spawns[index]._drop);

            cooldownCount = 0;
        }
    }

    public void ResetCooldown()
    {
        cooldownCount = 0;
    }

    public void SpawnObjectRandom()
    {
        int i = Random.Range(0, drops.Count);

        SpawnObject(drops[i]);
    }

    public void SpawnObject(Drop2 dropPrefab)
    {
        if (spawnedDrop) return;

        GameObject obj = Instantiate(dropPrefab.gameObject,
            transform.position + distance * Vector3.up, Quaternion.identity) as GameObject;

        Drop2 drop = obj.GetComponent<Drop2>();

        drop._spawner = this;
        spawnedDrop = true;
    }
}
