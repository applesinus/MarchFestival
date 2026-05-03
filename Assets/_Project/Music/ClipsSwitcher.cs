using UnityEngine;

public class ClipsSwitcher : MonoBehaviour
{
    [SerializeField] AudioClip MainMix;
    [SerializeField] AudioClip HGMix;
    [SerializeField] AudioClip PianoMix;

    private void Start()
    {
        PlayPianoMix();
    }

    public void PlayMainMix() => AudioManager.Instance.PlayMusic(MainMix);
    public void PlayHGMix() => AudioManager.Instance.PlayMusic(HGMix);
    public void PlayPianoMix() => AudioManager.Instance.PlayMusic(PianoMix);
}
