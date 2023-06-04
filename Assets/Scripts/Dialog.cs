using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Dialog : MonoBehaviour
{
    public enum DialogState { closed, typing, finishedTyping};
    private DialogState currentDialogState;
    private int currentDialogIndex;
    private DialogContent currentDialog;
    public TextMeshProUGUI textBox;
    public Image profile;

    public AudioSource audioSource;

    public Queue<DialogContent> dialogStack = new Queue<DialogContent>();
    
    private float currentTimer;
    private bool timerIsOn = false;

    public AudioSource startMusic;

    // Start is called before the first frame update
    void Start()
    {
        //currentDialogState = DialogState.closed;
        
    }

    private void Update()
    {
    }

    public void startNextLine()
    {
        if(currentDialogState == DialogState.closed)
        {
            return;
        }
        if(currentDialogIndex >= currentDialog.dialogLines.Count)
        {
            currentDialogState = DialogState.closed;
            if(dialogStack.Count > 0)
            {
                StartQueuedDialog();
            }
            else
            {
                transform.GetChild(0).gameObject.SetActive(false);
            }
            return;
        }
        //audioSource.Play();
        textBox.text = currentDialog.dialogLines[currentDialogIndex++];
    }

    public void StartDialog(DialogContent dialogContent) {
        
        dialogStack.Enqueue(dialogContent);

        if (currentDialogState != DialogState.closed)
        {
            return;
        }
        StartQueuedDialog();
        
    }

    private void StartQueuedDialog()
    {
        //audioSource.Play();
        transform.GetChild(0).gameObject.SetActive(true);

        currentDialog = dialogStack.Dequeue();

        currentDialogIndex = 0;
        currentDialogState = DialogState.typing;
        /*
        if (currentDialog.doesDialogComeFromZucc)
        {
            profile.sprite = zuccSprite;
        }
        else
        {
            profile.sprite = gamebertSprite;
        }
        */
        startNextLine();
    }
}
