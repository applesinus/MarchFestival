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

    public void PlayMainMix() => AudioManager.Instance.ChangeMusic(MainMix);
    public void PlayHGMix() => AudioManager.Instance.ChangeMusic(HGMix);
    public void PlayPianoMix() => AudioManager.Instance.ChangeMusic(PianoMix);
}
