using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyHealth : MonoBehaviour{

    public int maxHealth;
    private int health;

    private TextMeshProUGUI healthUI;
    private GameManager gameManager;

    void Awake(){
        health = maxHealth;
    }

    void Start(){
	gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
	healthUI = GameObject.Find("Health UI").GetComponent<TextMeshProUGUI>();
	UpdateHealthUI();
    }

    void OnTriggerEnter(Collider col){
	if (col.name == "Hazard(Clone)"){
	    if (health > 0){
	    	health = health - 1;
		UpdateHealthUI();
	    }
	    if (health == 0){
		gameManager.HealthFeedback();
	        StartCoroutine(gameManager.DelayedRestart());
	    }
	}
    }

    void UpdateHealthUI(){
	healthUI.text = "Health: " + health.ToString();
    }

}
