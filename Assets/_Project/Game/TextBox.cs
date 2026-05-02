using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class TextBox : MonoBehaviour
{
    [SerializeField] public bool DebugMode = false;
    [SerializeField] public string DebugDialogueID = "elderIntroduction";
    [SerializeField] public int animationSpeed = 1;
    [SerializeField] public TextMeshProUGUI textBox;
    [SerializeField] public TextMeshProUGUI characterName;
    [SerializeField] public Dictionary<string, GameObject> characters;
    [SerializeField] public UnityEvent<string> ChoiceEvent;

    private bool isSpacePressed = false;
    private bool isAnimationActive = false;
    
    private string dialogueID;
    private int messageID;
    private List<(string, int)> choices;
    private bool isChoiceActive = false;

    private void Start()
    {
        if (DebugMode)
        {
            Debug.LogWarning("DEBUG MODE ENABLED!");
            InitiateDialogue(DebugDialogueID);
        }
        animationProgress = 0;
        choices = new List<(string, int)>();
    }

    private void Update()
    {
        if (isChoiceActive) return;

        checkPressingKeys();
        if (isAnimationActive) animate();
    }

    private void checkPressingKeys()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isSpacePressed)
        {
            isSpacePressed = true;
            MessageBoxClicked();
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            isSpacePressed = false;
        }
    }

    private int animationProgress;
    private string message;
    private void animate()
    {
        if (charProgress() >= textBox.text.Length) textBox.text += message[charProgress()];
        animationProgress++;

        if (charProgress() == message.Length) messageEnd();
    }

    private int charProgress()
    {
        // TODO: use time evaluation instead of framerate
        return animationProgress / animationSpeed;
    }

    private void forceForwardAnimation()
    {
        textBox.text = message;
        messageEnd();
    }

    private void messageEnd()
    {
        isAnimationActive = false;
        animationProgress = 0;

        if (TextManager.Instance.GetText($"dialogue.{dialogueID}[{messageID}].choices[0].text") != $"dialogue.{dialogueID}[{messageID}].choices[0].text")
        {
                isChoiceActive = true;
        }

        if (isChoiceActive)
        {
            ChoiceEvent.Invoke($"dialogue.{dialogueID}[{messageID}]");
        }
        else
        {
            messageID++;
        }

        if (DebugMode) Debug.Log("Animation Reset");
    }

    public void MessageBoxClicked()
    {
        if (isChoiceActive) return;

        if (isAnimationActive)
        {
            forceForwardAnimation();
        }
        else
        {
            characterName.text = getCharacterName();

            message = getNewMessage();
            textBox.text = "";

            isAnimationActive = true;
        }

        if (DebugMode) Debug.Log("Message Box Clicked");
    }

    private string getNewMessage()
    {
        return TextManager.Instance.GetText($"dialogue.{dialogueID}[{messageID}].message");
    }

    private string getCharacterName()
    {
        return TextManager.Instance.GetText($"dialogue.{dialogueID}[{messageID}].speaker");
    }

    public void InitiateDialogue(string id)
    {
        gameObject.SetActive(true);
        isChoiceActive = false;

        dialogueID = id;
        messageID = 0;

        MessageBoxClicked();
    }

    private void endDialogue()
    {
        gameObject.SetActive(false);
    }
}
