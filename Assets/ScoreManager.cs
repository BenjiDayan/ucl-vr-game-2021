using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public float score;
    public float highscore = 10000000.00f;
   // public static float highscore;
    // Start is called before the first frame update
    void Start()
    {
        
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
