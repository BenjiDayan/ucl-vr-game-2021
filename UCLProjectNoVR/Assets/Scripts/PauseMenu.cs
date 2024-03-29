﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.XR;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;

    public GameObject pauseFirstButton;

    [SerializeField] KeyCode pauseKey = KeyCode.T;


    void Update()
    {
        bool pausePushed;
        if  (   
                Input.GetKeyDown(pauseKey)
            )
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Pause()
    {
        Debug.Log("Pause/Unpause!");
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        //Cursor.lockState = CursorLockMode.None;
        //clear selected object
        EventSystem.current.SetSelectedGameObject(null);
        //set a new selected object
        EventSystem.current.SetSelectedGameObject(pauseFirstButton);
    }

    public void LoadMenu()
    {
        Debug.Log("load menu!");
        SceneManager.LoadScene("MenuScreen");
    }

    public void ReloadGame()
    {
        Debug.Log("reload game!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
