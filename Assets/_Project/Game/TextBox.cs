using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.AI;
using System.ComponentModel;

public class TextBox : MonoBehaviour
{
    [SerializeField] public bool DebugMode = false;
    [SerializeField] public string DebugDialogueID = "elderIntroduction";
    [SerializeField] public int animationSpeed = 3;
    [SerializeField] public TextMeshProUGUI textBox;
    [SerializeField] public TextMeshProUGUI characterName;
    [SerializeField] public Dictionary<string, GameObject> characters;
    [SerializeField] public UnityEvent<string> ChoiceEvent;
    [SerializeField] public UnityEvent<string, string, string> CharacterEvent;
    [SerializeField] public UnityEvent<string> ArtifactEvent;
    [SerializeField] public UnityEvent StartArtifactUsed;
    [SerializeField] public UnityEvent EndArtifactUsed;
    [SerializeField] public UnityEvent NewMessage;
    [SerializeField] public UnityEvent<int, int, bool> UpdateClock;
    [SerializeField] public UnityEvent PlayHGMix;
    [SerializeField] public UnityEvent PlayPianoMix;

    private int timeOfDay = 2;
    private int day = 0;

    private bool isArtifactUsed = false;
    private string thoughtSuffix = "";
    private bool isArtifactAvailable = true;

    private bool isSpacePressed = false;
    private bool isAnimationActive = false;
    
    private string dialogueID;
    private int messageID;
    private bool isChoiceActive = false;
    private Dictionary<string, bool> visited = new Dictionary<string, bool>();

    private void Start()
    {
        if (DebugMode)
        {
            Debug.LogWarning("DEBUG MODE ENABLED!");
            InitiateDialogue(DebugDialogueID);
        }
        animationProgress = 0;
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
        else if (isArtifactUsed)
        {
            MessageBoxClicked();
        }

        if (DebugMode) Debug.Log("Animation Reset");
    }

    public void MessageBoxClicked()
    {   
        Debug.Log($"MessageBoxClicked, IsAnimationActive: {isAnimationActive}");

        if (isChoiceActive) return;

        if (isAnimationActive)
        {
            forceForwardAnimation();
        }
        else
        {
            NewMessage.Invoke();

            if (!isArtifactUsed)
            {
                messageID++;
            }

            characterName.text = getCharacterName();
            if (isArtifactUsed) characterName.text += " " + TextManager.Instance.GetText("game.realThoughts");

            message = getNewMessage();
            textBox.text = "";

            isAnimationActive = true;

            if (!isArtifactUsed)
            {
                List<(string, string, string)> characterUpdates = getCharacterUpdate();
                if (characterUpdates != null)
                {
                    foreach ((string name, string sprite, string animation) in characterUpdates)
                    {
                        CharacterEvent.Invoke(name, sprite, animation);
                    }
                }

                if (
                    TextManager.Instance.GetText($"dialogue.{dialogueID}[{messageID}].thought") != $"dialogue.{dialogueID}[{messageID}].thought"
                    && isArtifactAvailable
                )
                {
                    ArtifactEvent.Invoke($"dialogue.{dialogueID}[{messageID}]");
                }
            } else
            {
                isArtifactUsed = false;
            }
        }
    }

    private string getNewMessage()
    {
        if (isArtifactUsed)
        {
            if (TextManager.Instance.GetText($"dialogue.{dialogueID}[{messageID}].thought") == $"dialogue.{dialogueID}[{messageID}].thought")
            {
                return TextManager.Instance.GetText($"dialogue.{dialogueID}[{messageID}].message");
            }
            else
            {
                PlayerPrefs.SetInt("lastDayArtifactUsed", day);
                return TextManager.Instance.GetText($"dialogue.{dialogueID}[{messageID}].thought{thoughtSuffix}");
            }
        }
        return TextManager.Instance.GetText($"dialogue.{dialogueID}[{messageID}].message");
    }

    private string getCharacterName()
    {
        return TextManager.Instance.GetText($"dialogue.{dialogueID}[{messageID}].speaker");
    }

    private List<(string, string, string)> getCharacterUpdate()
    {
        string character0 = TextManager.Instance.GetText($"dialogue.{dialogueID}[{messageID}].characters[0].name");
        if (character0 == $"dialogue.{dialogueID}[{messageID}].characters[0].name") return null;

        List<(string, string, string)> characters = new List<(string, string, string)>();
        int i = 0;
        
        while (true)
        {
            string character = TextManager.Instance.GetText($"dialogue.{dialogueID}[{messageID}].characters[{i}].name");
            if (character == $"dialogue.{dialogueID}[{messageID}].characters[{i}].name") break;

            string emotion = TextManager.Instance.GetText($"dialogue.{dialogueID}[{messageID}].characters[{i}].sprite");
            if (emotion == $"dialogue.{dialogueID}[{messageID}].characters[{i}].sprite") break;

            string animation = TextManager.Instance.GetText($"dialogue.{dialogueID}[{messageID}].characters[{i}].animation");
            if (animation == $"dialogue.{dialogueID}[{messageID}].characters[{i}].animation") break;

            characters.Add((character, emotion, animation));
            i++;
        }

        return characters;
    }

    public void InitiateDialogue(string id)
    {
        gameObject.SetActive(true);
        isChoiceActive = false;
        isAnimationActive = false;

        dialogueID = id;
        messageID = -1;

        if (id == "tavernRelax" && timeOfDay == 2) PlayHGMix.Invoke(); else PlayPianoMix.Invoke();
        Debug.Log($"Dialogue initiated: {id}");

        MessageBoxClicked();
    }

    public void EnterToHouse(string houseOccupant)
    {
        Debug.LogWarning($"Enter to house: {houseOccupant}");

        // TODO: change
        string rootID = $"{houseOccupant}_root_1";
        if (visited.ContainsKey(houseOccupant))
        {
            if (houseOccupant == "woman")
            {
                if (timeOfDay == 2) rootID = "tavernRelax"; else rootID = "tavernRelaxDay";
            }
            else
            {
                rootID = "no";
            }
        }
        else
        {
            visited.Add(houseOccupant, true);
        }
        
        InitiateDialogue(rootID);
    }

    public void UseArtifact(string thoughtType)
    {
        Debug.LogWarning("Artifact used!");

        if (isChoiceActive)
        {
            Debug.LogWarning("Using artifact while a choice box is active!");
            return;
        }

        if (thoughtType == "bad") thoughtSuffix = "_bad"; else thoughtSuffix = "";

        isArtifactUsed = true;
        isArtifactAvailable = false;
        StartArtifactUsed.Invoke();
        messageEnd();
    }

    private void updateCharacters()
    {
        if (isArtifactUsed)
        {
            isArtifactUsed = false;
            EndArtifactUsed.Invoke();
        }
    }

    public void ExitHouse()
    {
        if (dialogueID == "no") return;

        timeOfDay = (timeOfDay+1)%3;
        if (timeOfDay == 0)
        {
            day++;
            isArtifactAvailable = true;
        }

        UpdateClock.Invoke(day, timeOfDay, isArtifactAvailable);

        if (DebugMode) Debug.Log($"Day: {day}, Time of day: {timeOfDay}");
    }
}
