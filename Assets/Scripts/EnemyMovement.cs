using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour{

    private float startTime;
    private Vector3 startPos;
    private Vector3 targetPos;
    private Vector3 midPos1;
    private Vector3 midPos2;
    private Vector3 midPos3;
    private Vector3 midPos4;
    private float mapXY;

    public float speed;

    private GameManager gameManager;

    void Start(){
    	gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        startTime = Time.time;
    	startPos = gameManager.enemyPos;
    	targetPos = gameManager.flagPos;
    	mapXY = gameManager.mapXY;
    	midPos1 = new Vector3(Random.Range(-mapXY, mapXY), 0.25f, Random.Range(-mapXY, mapXY));
	midPos2 = new Vector3(Random.Range(-mapXY, mapXY), 0.25f, Random.Range(-mapXY, mapXY));
	midPos3 = new Vector3(Random.Range(-mapXY, mapXY), 0.25f, Random.Range(-mapXY, mapXY));
	midPos4 = new Vector3(Random.Range(-mapXY, mapXY), 0.25f, Random.Range(-mapXY, mapXY));
    }
 
    void Update(){
   	var t = (Time.time - startTime) * 0.1f * speed;
        transform.position = Bezier(t, startPos, midPos1, midPos2, midPos3, midPos4, targetPos);
    }

    void OnTriggerEnter(Collider col){
	if (col.name == "Flag(Clone)"){
	    gameManager.DistanceFeedback();
            StartCoroutine(gameManager.DelayedRestart());
	}
    }

    Vector3 Bezier(float t, Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector3 e, Vector3 f){
    	var ab = Vector3.Lerp(a, b, t);
    	var bc = Vector3.Lerp(b, c, t);
	var abc = Vector3.Lerp(ab, bc, t);
	var de = Vector3.Lerp(d, e, t);
	var ef = Vector3.Lerp(e, f, t);
	var def = Vector3.Lerp(de, ef, t);
    	return Vector3.Lerp(abc, def, t);
    }

}