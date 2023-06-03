using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Pause();
        }
    }

    public void Pause()
    {
        Time.timeScale = 0;
        pausePanel.gameObject.SetActive(true);
    }

    public void Continue()
    {
        Time.timeScale = 1;
        pausePanel.gameObject.SetActive(false);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
