using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Clock : MonoBehaviour
{
    [Header("Day")]
    [SerializeField] public TextMeshProUGUI day;
    [SerializeField] public string dayKey = "game.day";

    [Header("Time")]
    [SerializeField] public TextMeshProUGUI time;
    [SerializeField] public string timeKey = "game.time";
    [SerializeField] public string morningKey = "game.morning";
    [SerializeField] public string afternoonKey = "game.afternoon";
    [SerializeField] public string eveningKey = "game.evening";

    [Header("Artifact")]
    [SerializeField] public TextMeshProUGUI artifact;
    [SerializeField] public string artifactKey = "game.artifact";
    [SerializeField] public string availableKey = "game.available";
    [SerializeField] public string usedKey = "game.used";


    private void Awake()
    {
        TextManager.Instance.OnLanguageChanged += BuildStrings;
    }

    private void OnDestroy()
    {
        if (TextManager.Instance != null) TextManager.Instance.OnLanguageChanged -= BuildStrings;
    }

    private int dayProgress = 0;
    public int timeProgress = 0;
    private bool isArtifactAvailable = false;

    public void UpdateClock(int day, int time, bool artifactState)
    {
        dayProgress = day;
        timeProgress = time;
        isArtifactAvailable = artifactState;

        Debug.Log($"Clock updated! Day: {dayProgress}, Time: {timeProgress}, Artifact: {isArtifactAvailable}");

        BuildStrings();
    }

    public void BuildStrings()
    {
        string timeOfDayKey = morningKey;
        switch (timeProgress)
        {
            case 0:
                break;
            case 1:
                timeOfDayKey = afternoonKey;
                break;
            case 2:
                timeOfDayKey = eveningKey;
                break;
            default:
                Debug.LogError("Unknown time of day!");
                break;
        }

        string artifactStateKey = usedKey;
        if (isArtifactAvailable) artifactStateKey = availableKey;

        day.text = $"{TextManager.Instance.GetText(dayKey)} {dayProgress}";
        time.text = $"{TextManager.Instance.GetText(timeKey)} {TextManager.Instance.GetText(timeOfDayKey)}";
        artifact.text = $"{TextManager.Instance.GetText(artifactKey)} {TextManager.Instance.GetText(artifactStateKey)}";
    }
}
