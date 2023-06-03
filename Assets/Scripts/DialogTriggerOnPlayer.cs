using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogTriggerOnPlayer : MonoBehaviour
{
    public Dialog dialog;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "DialogTrigger")
        {
            dialog.StartDialog(other.GetComponent<DialogTrigger>().GetDialogContent());
        }
    }

    void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            dialog.startNextLine();
        }
    }
}