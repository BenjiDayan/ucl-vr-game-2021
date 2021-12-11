using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    //public GameObject pauseMenuUI;
    public GameObject menuFirstButton;

   public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        Time.timeScale = 1f;
        //SceneManager.LoadScene("FPSWithDropsVR"); 
        //If we want to load a particular scene we can just write its name or put 
        //it in the build index settings as number 1 with the MainMenu Scene being number 0
    }

    public void OnEnable() {
        EventSystem.current.SetSelectedGameObject(null);
        //set a new selected object
        EventSystem.current.SetSelectedGameObject(menuFirstButton);
    }

    public void Start() {
        EventSystem.current.SetSelectedGameObject(null);
        //set a new selected object
        EventSystem.current.SetSelectedGameObject(menuFirstButton);
    }

    public void QuitGame()
    {
        Debug.Log("Quit"); //indication that we actually quit the game
        Application.Quit();
    }
}
