using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Characters : MonoBehaviour
{
    [Serializable]
    public struct CharacterData
    {
        public string Character;
        public List<EmotionData> Emotions;
        public Sprite BackgroundDay;
        public Sprite BackgroundNight;
        public Sprite BackgroundDayBad;
        public Sprite BackgroundNightBad;
    }

    [Serializable]
    public struct EmotionData
    {
        public string Emotion;
        public Sprite Sprite;
    }

    [SerializeField] public List<CharacterData> Sprites;
    [SerializeField] public GameObject characterPrefab;
    [SerializeField] public Image background;

    private Dictionary<string, Dictionary<string, Sprite>> SpritesDictionary = new();
    private struct backgrounds
    {
        public Sprite dayGood;
        public Sprite nightGood;
        public Sprite dayBad;
        public Sprite nightBad;
    }
    private Dictionary<string, backgrounds> characterBackgrounds = new();

    private struct onScreenCharacterData
    {
        public GameObject gameObject;
        public string Emotion;
    }
    private Dictionary<string, onScreenCharacterData> onScreenCharacters = new();

    private void Awake()
    {
        Debug.LogWarning($"Characters started! Count: {Sprites.Count}");

        foreach (CharacterData spriteData in Sprites)
        {
            if (!SpritesDictionary.ContainsKey(spriteData.Character))
            {
                SpritesDictionary.Add(spriteData.Character, new Dictionary<string, Sprite>());
            }
            
            foreach (EmotionData emotionData in spriteData.Emotions)
            {
                SpritesDictionary[spriteData.Character].Add(emotionData.Emotion, emotionData.Sprite);
            }

            if (!characterBackgrounds.ContainsKey(spriteData.Character))
            {
                backgrounds bgs = new();
                if (spriteData.BackgroundDay == null)
                {
                    Debug.LogError($"Background for day of character {spriteData.Character} not found!");
                    bgs.dayGood = Sprite.Create(new Texture2D(10, 10), new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
                }
                else bgs.dayGood = spriteData.BackgroundDay;
                if (spriteData.BackgroundNight == null) bgs.nightGood = bgs.dayGood; else bgs.nightGood = spriteData.BackgroundNight;
                if (spriteData.BackgroundDayBad == null) bgs.dayBad = bgs.dayGood; else bgs.dayBad = spriteData.BackgroundDayBad;
                if (spriteData.BackgroundNightBad == null) bgs.nightBad = bgs.nightGood; else bgs.nightBad = spriteData.BackgroundNightBad;

                characterBackgrounds.Add(spriteData.Character, bgs);
            }
        }
    }

    private void Start()
    {
        OpenHouse("oldMan", 2);
    }

    private bool isUpset = false;
    public void SwitchUpset()
    {
        foreach (string character in onScreenCharacters.Keys)
        {
            if (isUpset)
            {
                UpdateSprite(character, onScreenCharacters[character].Emotion, "update");
            }
            else
            {
                if (PlayerPrefs.GetInt($"runSettings.{character}.isBad") == 1)
                {
                    UpdateSprite(character, "upset", "update");
                }
                else
                {
                    UpdateSprite(character, "neutral", "update");
                }
            }
        }

        isUpset = !isUpset;
    }

    public void UpdateSprite(string character, string emotion, string animation)
    {
        Debug.Log($"Character animation: {character} {emotion} {animation}");
        Sprite sprite = getSprite(character, emotion);

        switch (animation)
        {
            case "appear":
                if (onScreenCharacters.ContainsKey(character))
                {
                    Debug.LogWarning($"Character {character} already on screen!");
                    onScreenCharacters[character].gameObject.GetComponent<Image>().sprite = sprite;
                }
                else
                {
                    GameObject characterObject = Instantiate(characterPrefab, transform);
                    characterObject.transform.localPosition = new Vector3(0, 0, 0);
                    characterObject.GetComponent<Image>().sprite = sprite;
                    onScreenCharacters.Add(character, new onScreenCharacterData { gameObject = characterObject, Emotion = emotion });
                }
                break;

            case "disappear":
                if (onScreenCharacters.ContainsKey(character))
                {
                    Destroy(onScreenCharacters[character].gameObject);
                    onScreenCharacters.Remove(character);
                }
                else
                {
                    Debug.LogWarning($"Character {character} already not on screen!");
                }
                break;
            
            case "update":
                if (onScreenCharacters.ContainsKey(character))
                {
                    onScreenCharacters[character].gameObject.GetComponent<Image>().sprite = sprite;
                }
                else
                {
                    Debug.LogWarning($"Character {character} not on screen!");
                }
                break;

            default:
                Debug.LogError($"Animation {animation} not found! Simply changing sprite");
                if (onScreenCharacters.ContainsKey(character))
                {
                    onScreenCharacters[character].gameObject.GetComponent<Image>().sprite = sprite;
                }
                break;
        }
    }

    private Sprite getSprite(string character, string emotion)
    {
        if (SpritesDictionary.ContainsKey(character))
        {
            if (SpritesDictionary[character].ContainsKey(emotion))
            {
                if (PlayerPrefs.GetInt($"runSettings.{character}.isBad") == 1 && SpritesDictionary[character].ContainsKey(emotion + "_bad"))
                {
                    return SpritesDictionary[character][emotion + "_bad"];
                }
                else
                {
                    return SpritesDictionary[character][emotion];
                }
            }
            else
            {
                Debug.LogError($"Emotion {emotion} of character {character} not found!");

                if (SpritesDictionary[character].ContainsKey("neutral")) return SpritesDictionary[character]["neutral"];

                Debug.LogError($"Emotion neutral of character {character} not found! too");
                Sprite sprite = null;
                foreach (string emotionName in SpritesDictionary[character].Keys)
                {
                    sprite = SpritesDictionary[character][emotionName];
                    if (PlayerPrefs.GetInt($"runSettings.{character}.isBad") == 1 && emotionName.Contains("_bad")) return sprite;
                    if (PlayerPrefs.GetInt($"runSettings.{character}.isBad") == 0 && !emotionName.Contains("_bad")) return sprite;
                }
                return sprite;
            }
        }
        else
        {
            Debug.LogError($"Character {character} not found!");
        }

        if (characterPrefab != null)
        {
            Debug.Log($"Character {character} found! Emotion is {emotion}! Sprite is {characterPrefab.GetComponent<Image>().sprite.name}");
            return characterPrefab.GetComponent<Image>().sprite;
        }
        return null;
    }

    public void OpenHouse(string character, int time)
    {
        if (characterBackgrounds.ContainsKey(character))
        {
            if (PlayerPrefs.GetInt($"runSettings.{character}.isBad") == 1)
            {
                if (time < 2) background.sprite = characterBackgrounds[character].dayBad;
                else background.sprite = characterBackgrounds[character].nightBad;
            }
            else
            {
                if (time < 2) background.sprite = characterBackgrounds[character].dayGood;
                else background.sprite = characterBackgrounds[character].nightGood;
            }
        }
        else
        {
            Debug.LogError($"Character {character} not found!");
            background.sprite = characterBackgrounds["unknown"].dayGood;
        }
        gameObject.SetActive(true);
    }

    public void CloseHouse()
    {
        clearCharacters();
        gameObject.SetActive(false);
    }

    private void clearCharacters()
    {
        foreach (onScreenCharacterData character in onScreenCharacters.Values)
        {
            Destroy(character.gameObject);
        }
        onScreenCharacters.Clear();
    }
}
