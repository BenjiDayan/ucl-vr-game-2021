using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpawnerData2
{
    [SerializeField] Drop2 drop;
    [SerializeField] int weight = 1;    // Affects likelihood of drop spawning

    public Drop2 _drop => drop;
    public int _weight => weight;
}
