﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement2 : MonoBehaviour
{
    private GameManager gameManager;
    //public Transform goal;

    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.destination = gameManager.flagPos;
        //agent.destination = goal.position;
    }
}
