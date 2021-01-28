using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadScene : MonoBehaviour
{
    private ScoreManager myScoreManager;

    void Awake() {
        myScoreManager = FindObjectOfType<ScoreManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadGame();
        }   
    }
    public void ReloadGame()
    {
        myScoreManager.GetHighScore();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Debug.Log("Scene loaded");
        
    }

    
    /*
    bool isPaused = false;
    public void pauseGame()
    {
        if(isPaused)
        {
            Time.timeScale = 1;
            isPaused = false;
        } else
        {
            Time.timeScale = 0;
            isPaused = true;
        }
    } */
}
