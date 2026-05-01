using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundButton : MonoBehaviour
{
    [Header("Sound Button Text")]
    [SerializeField] public TextForTextManager soundButtonText;

    private void Start()
    {
        if (!AudioManager.Instance.IsSoundOn())
        {
            soundButtonText.UpdateKey("ui.OFF");
        }
    }

    public void UpdateSoundSettings()
    {
        if (AudioManager.Instance.IsSoundOn())
        {
            AudioManager.Instance.SoundOff();
            soundButtonText.UpdateKey("ui.OFF");
        }
        else
        {
            AudioManager.Instance.SoundOn();
            soundButtonText.UpdateKey("ui.ON");
        }
    }
}
