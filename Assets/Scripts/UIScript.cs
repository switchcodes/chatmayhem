using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIScript : MonoBehaviour
{
    private GameObject[] pauseObjects;
    void OnResumeClick()
    {
        Time.timeScale = 1f;
       
    }

    void OnPauseClick()
    {
        Time.timeScale = 0;   
    }

    void OnRestartClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
       
    }
    void OnReturnClick()
    {
        SceneManager.LoadScene("Insert Main Menu Scene Name");
    }
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
        pauseObjects = GameObject.FindGameObjectsWithTag("ShowOnPause");
        hidePauseUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (Time.timeScale == 1)
            {
                Time.timeScale = 0;
                showPaused();
            }
            else if (Time.timeScale == 0)
            {
                Time.timeScale = 1;
                hidePaused();
            }
        }
    }
    public void showPaused()
    {
        foreach (GameObject g in pauseObjects)
        {
            g.SetActive(true);
        }
    }
    public void hidePaused()
    {
        foreach (GameObject g in pauseObjects)
        {
            g.SetActive(false);
        }
    }

    void hidePauseUI()
    {

    }
}
