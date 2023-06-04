using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DreamTimer : MonoBehaviour
{

    public float dreamTimer = 160f;

    public int sceneIndex = 2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        dreamTimer -= Time.deltaTime;
        if (dreamTimer < 0)
        {
            SceneManager.LoadScene(sceneIndex);
        }
    }
}
