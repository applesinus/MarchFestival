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
    [SerializeField] public UnityEvent ExitDialogue;
    private struct Choice
    {
        public string text;
        public string nextRoot;
        public GameObject button;
        public string[] effects;
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

            string[] requires = TextManager.Instance.GetList($"{preffix}.choices[{i}].requires");
            if (requires != null)
            {
                bool ok = true;

                foreach (string r in requires)
                {
                    if (r.StartsWith("no"))
                    {
                        string key = r.Substring(2);
                        Debug.Log("key: " + key);
                        if (PlayerPrefs.HasKey($"runSettings.saved.{key}") && PlayerPrefs.GetInt($"runSettings.saved.{key}") == 1)
                        {
                            ok = false;
                            break;
                        }
                    }
                    else
                    {
                        if (PlayerPrefs.GetInt($"runSettings.saved.{r}") == 0)
                        {
                            ok = false;
                            break;
                        }
                    }
                }

                if (!ok)
                {
                    i++;
                    continue;
                }
            }

            string[] effects = TextManager.Instance.GetList($"{preffix}.choices[{i}].effects");
            if (effects != null)
            {
                foreach (string e in effects)
                {
                    if (e.StartsWith("no"))
                    {
                        string key = e.Substring(2);
                        PlayerPrefs.SetInt($"runSettings.saved.{key}", 0);
                    }
                    else
                    {
                        PlayerPrefs.SetInt($"runSettings.saved.{e}", 1);
                    }
                }
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

            choices.Add(new Choice{
                text = text,
                nextRoot = nextRoot,
                button = button
            });
            {
                int choicesCount = choices.Count-1;
                clickEvent.AddListener(() => ChoiceDone(choicesCount));
            }

            if (DebugMode) Debug.Log($"Choice {i}: {text} -> {nextRoot}");

            i++;
        }
    }

    public void ChoiceDone(int idx)
    {
        if (DebugMode) Debug.Log($"Choice {idx} done");
        
        foreach (Choice choice in choices) Destroy(choice.button);
        if (choices[idx].nextRoot == "exit")
        {
            ExitDialogue.Invoke();
        }
        else
        {
            TextBoxObject.InitiateDialogue(choices[idx].nextRoot);
        }
    }
}
