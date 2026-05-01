using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageButton : MonoBehaviour
{
    private int i = 0;
    TextManager.LanguageInfo[] languages;

    private void Start()
    {
        languages = TextManager.Instance.AvailableLanguages.ToArray();
        for (int j = 0; j < languages.Length; j++)
        {
            if (languages[j].IsoCode == TextManager.Instance.CurrentLanguageCode)
            {
                i = j;
                break;
            }
        }
    }

    public void ChangeLanguage()
    {
        i = (i + 1) % languages.Length;
        TextManager.Instance.SetLanguage(languages[i].IsoCode);
    }
}
