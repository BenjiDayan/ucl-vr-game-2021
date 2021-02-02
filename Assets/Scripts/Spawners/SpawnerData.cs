using System;
using UnityEngine;

[Serializable]
public class SpawnerData
{
    [SerializeField] Drop drop;
    [SerializeField] int weight = 1;    // Affects likelihood of drop spawning

    public Drop _drop => drop;
    public int _weight => weight;
}
