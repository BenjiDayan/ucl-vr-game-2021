using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour{

    private TextMeshProUGUI feedbackUI;

    public GameObject enemyPrefab;
    public GameObject flagPrefab;
    public GameObject hazardPrefab;

    public Vector3 enemyPos;
    public Vector3 flagPos;
    public float mapXY;

    public Transform floor;
    public int hazardNumber;

    private GameObject enemy;
    private GameObject flag;
    private GameObject hazard;

    void Start(){
	feedbackUI = GameObject.Find("Feedback UI").GetComponent<TextMeshProUGUI>();
	feedbackUI.text = "";
	InitialiseScene();
    }

    public void Restart(){ // Replace with Athina's function
	string scene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }

    public IEnumerator DelayedRestart(){
	Time.timeScale = 0;
	yield return new WaitForSecondsRealtime(1);
	Restart();
	Time.timeScale = 1;
    }

    public void HealthFeedback(){
	feedbackUI.text = "The enemy is dead!";
    }

    public void DistanceFeedback(){
	feedbackUI.text = "The enemy has captured the flag!";
    }

    void InitialiseScene(){
	mapXY = (0.5f * floor.localScale.x) - 1;
	enemyPos = new Vector3(Random.Range(-mapXY, mapXY), 0.25f, Random.Range(mapXY, mapXY));
	flagPos = new Vector3(Random.Range(-mapXY/4, mapXY/4), 0.25f, Random.Range(-mapXY/4, mapXY/4));
        enemy = Instantiate(enemyPrefab, enemyPos, Quaternion.identity);
	flag = Instantiate(flagPrefab, flagPos, Quaternion.identity);
	for (int i = 0; i < hazardNumber; i++){
	    Vector3 hazardPos = new Vector3(Random.Range(-mapXY, mapXY), 0.25f, Random.Range(-mapXY, 0.9f * mapXY));
	    hazard = Instantiate(hazardPrefab, hazardPos, Quaternion.identity);
	}
    }

}
