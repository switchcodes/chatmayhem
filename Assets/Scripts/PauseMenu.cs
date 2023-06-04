using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public int currentSceneIndex=1;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape) && !gameOverPanel.activeSelf)
        {
            Pause();
        }
    }

    private void Pause()
    {
        Time.timeScale = 0;
        pausePanel.gameObject.SetActive(true);
    }

    public void GameOver()
    {
        Time.timeScale = 0;
        gameOverPanel.gameObject.SetActive(true);
    }

    public void Continue()
    {
        Time.timeScale = 1;
        pausePanel.gameObject.SetActive(false);
    }

    public void Restart()
    {
        //restart
        Time.timeScale = 1;
        SceneManager.LoadScene(currentSceneIndex);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
