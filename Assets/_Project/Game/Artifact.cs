using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Artifact : MonoBehaviour
{
    private enum character {
        unknown = 0,
        boy = 1,
        butcher = 2,
        carpenter = 3,
        girl = 4,
        oldLady = 5,
        oldMan = 6,
        trader = 7,
        woman = 8,
    }

    private character parseFromString(string str)
    {
        str = str.ToLower();

        if (str.Contains("boy")) return character.boy;
        if (str.Contains("butcher")) return character.butcher;
        if (str.Contains("carpenter")) return character.carpenter;
        if (str.Contains("girl")) return character.girl;
        if (str.Contains("oldlady")) return character.oldLady;
        if (str.Contains("oldman")) return character.oldMan;
        if (str.Contains("trader")) return character.trader;
        if (str.Contains("woman")) return character.woman;

        return character.unknown;
    }

    [SerializeField] public UnityEvent<string> OnArtifactUsed;

    private bool isBad = false;

    private void Start()
    {
        GenerateRunSettings();
        Hide();
    }
    private void OnDestroy()
    {
        EndGame();
    }

    public void Use()
    {
        if (isBad)
        {
            OnArtifactUsed.Invoke("bad");
        }
        else
        {
            OnArtifactUsed.Invoke("good");
        }

        Hide();
    }

    public void EnableArtifactUsage(string messagePreffix)
    {
        Debug.Log("Artifact message: " + messagePreffix);

        Show();

        if (TextManager.Instance.GetText($"{messagePreffix}.characterID") == $"{messagePreffix}.characterID")
        {
            Debug.LogError("No character ID in artifact message!");
        }
        else
        {
            character character = parseFromString(TextManager.Instance.GetText($"{messagePreffix}.characterID"));
            if (character == character.unknown)
            {
                Debug.LogError($"Unknown character {TextManager.Instance.GetText($"{messagePreffix}.characterID")} in artifact message!");
                isBad = false;
            }
            else
            {
                if (PlayerPrefs.GetInt($"runSettings.{character}.isBad") == 1)
                {
                    isBad = true;
                }
                else
                {
                    isBad = false;
                }
            }
        }
    }

    public void Hide()
    {
        Debug.Log("Artifact hidden!");
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }

    public void Show()
    {
        Debug.Log("Artifact shown!");
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }

    public void GenerateRunSettings()
    {
        if (!PlayerPrefs.HasKey("runSet")) {
            // Generate new run settings
            foreach (character character in Enum.GetValues(typeof(character)))
            {
                PlayerPrefs.SetInt($"runSettings.{character}.isVisited", 0);

                if (character == character.woman) continue;

                int randValue = UnityEngine.Random.Range(0, 2);
                PlayerPrefs.SetInt($"runSettings.{character}.isBad", randValue);
                Debug.Log($"{character}: {PlayerPrefs.GetInt($"runSettings.{character}.isBad")}");

                if (character == character.carpenter)
                {
                    if (randValue == 1) randValue = 0; else randValue = 1;
                    PlayerPrefs.SetInt($"runSettings.{character.woman}.isBad", randValue);
                    Debug.Log($"{character}: {PlayerPrefs.GetInt($"runSettings.{character.woman}.isBad")}");
                }
            }
            PlayerPrefs.SetInt("runSet", 1);
            PlayerPrefs.SetInt("lastDayArtifactUsed", -1);
            PlayerPrefs.Save();

            // Generate other prefs
        }
    }

    public void EndGame()
    {
        // Erase run settings
        foreach (character character in Enum.GetValues(typeof(character)))
        {
            PlayerPrefs.DeleteKey($"runSettings.{character}.isBad");
            PlayerPrefs.DeleteKey($"runSettings.saved.{character}");
        }
        PlayerPrefs.DeleteKey("runSet");
        PlayerPrefs.DeleteKey("lastDayArtifactUsed");
        PlayerPrefs.Save();

        // Erase other prefs
    }
}
