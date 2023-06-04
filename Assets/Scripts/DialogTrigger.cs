using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    public DialogContent dialogContent;

    public DialogContent GetDialogContent()
    {
        gameObject.SetActive(false);
        return dialogContent;
    }
}