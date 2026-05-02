using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ChoiceBox : MonoBehaviour
{
    [SerializeField] public bool DebugMode = false;
    [SerializeField] public GameObject ButtonPrefab;
    [SerializeField] public TextBox TextBoxObject;
    private struct Choice
    {
        public string text;
        public string nextRoot;
        public GameObject button;
    }
    private List<Choice> choices;

    public void ShowChoices(string preffix)
    {
        choices = new List<Choice>();

        if (DebugMode) Debug.Log("Showing choices: " + preffix);

        int i = 0;
        while (true)
        {
            string text = TextManager.Instance.GetText($"{preffix}.choices[{i}].text");
            if (text == $"{preffix}.choices[{i}].text") break;

            string nextRoot = TextManager.Instance.GetText($"{preffix}.choices[{i}].nextRoot");
            if (nextRoot == $"{preffix}.choices[{i}].nextRoot")
            {
                Debug.LogError($"No associated root for choice {i}");
                break;
            }

            // TODO: remove instantiation, use preplaced prefabs instead
            GameObject button = Instantiate(ButtonPrefab, transform);
            button.transform.SetParent(transform, false);

            RectTransform rect = button.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1);
            rect.anchorMax = new Vector2(0.5f, 1);
            rect.anchoredPosition = new Vector2(0, -120*choices.Count);

            TextMeshProUGUI tmpro = button.transform.Find("Button").Find("Text").GetComponent<TextMeshProUGUI>();
            tmpro.autoSizeTextContainer = false;
            tmpro.fontSize = 30;

            TextForTextManager textForTM = button.transform.Find("Button").Find("Text").GetComponent<TextForTextManager>();
            textForTM.UpdateKey($"{preffix}.choices[{i}].text");
            textForTM.IsSet = true;

            UnityEvent clickEvent = button.GetComponent<ButtonBox>().OnClick;
            int choiceIdx = i;
            clickEvent.AddListener(() => ChoiceDone(choiceIdx));

            choices.Add(new Choice{
                text = text,
                nextRoot = nextRoot,
                button = button
            });

            if (DebugMode) Debug.Log($"Choice {i}: {text} -> {nextRoot}");

            i++;
        }
    }

    public void ChoiceDone(int idx)
    {
        foreach (Choice choice in choices) Destroy(choice.button);
        if (choices[idx].nextRoot == "exit")
        {
            Debug.LogWarning("Exit");
            // TODO
        }
        else
        {
            TextBoxObject.InitiateDialogue(choices[idx].nextRoot);
        }
    }
}
