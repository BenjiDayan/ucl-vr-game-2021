using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    private static GameObject instance;
    public float score;
    public float highscore = 10000000.00f;
   // public static float highscore;
    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance == null)
            instance = gameObject;
        else
            Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        score = Time.timeSinceLevelLoad;
    }
    
    public void GetHighScore()
    {
        if(score < highscore)
        {
            highscore = score;
        }
    } 
}
