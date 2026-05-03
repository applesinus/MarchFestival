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
    }

    [Serializable]
    public struct EmotionData
    {
        public string Emotion;
        public Sprite Sprite;
    }

    [SerializeField] public List<CharacterData> Sprites;
    [SerializeField] public GameObject characterPrefab;

    private Dictionary<string, Dictionary<string, Sprite>> SpritesDictionary = new Dictionary<string, Dictionary<string, Sprite>>();
    private Dictionary<string, GameObject> onScreenCharacters = new Dictionary<string, GameObject>();

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
        }
    }

    private void Start()
    {
        CloseHouse();
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
                    onScreenCharacters[character].GetComponent<Image>().sprite = sprite;
                }
                else
                {
                    GameObject characterObject = Instantiate(characterPrefab, transform);
                    characterObject.transform.localPosition = new Vector3(0, 0, 0);
                    characterObject.GetComponent<Image>().sprite = sprite;
                    onScreenCharacters.Add(character, characterObject);
                }
                break;

            case "disappear":
                if (onScreenCharacters.ContainsKey(character))
                {
                    Destroy(onScreenCharacters[character]);
                    onScreenCharacters.Remove(character);
                }
                else
                {
                    Debug.LogWarning($"Character {character} already not on screen!");
                }
                break;

            default:
                Debug.LogError($"Animation {animation} not found! Simply changing sprite");
                if (onScreenCharacters.ContainsKey(character))
                {
                    onScreenCharacters[character].GetComponent<Image>().sprite = sprite;
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
                if (PlayerPrefs.GetInt($"runSettings.{character}.isBad") == 1)
                {
                    if (SpritesDictionary[character].ContainsKey($"{emotion}_bad")) return SpritesDictionary[character][$"{emotion}_bad"];
                }
                return SpritesDictionary[character][emotion];
            }
            else
            {
                Debug.LogError($"Emotion {emotion} of character {character} not found!");
                foreach (string emotionName in SpritesDictionary[character].Keys)
                {
                    return SpritesDictionary[character][emotionName];
                }
            }
        }
        else
        {
            Debug.LogError($"Character {character} not found!");
        }

        if (characterPrefab != null) return characterPrefab.GetComponent<Image>().sprite;
        return null;
    }

    public void OpenHouse() => gameObject.SetActive(true);
    public void CloseHouse() => gameObject.SetActive(false);
}
