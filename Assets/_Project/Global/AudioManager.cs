using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Sources")]
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _sfxSource;

    private float _defaultMusicVolume = 0.5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        Instance.UpdateSettings();
        
        if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void UpdateSettings()
    {
        if (!PlayerPrefs.HasKey("sound"))
        {
            PlayerPrefs.SetInt("sound", 0);
        }

        if (PlayerPrefs.GetInt("sound") == 0)
        {
            SoundOn();
        }
        else
        {
            SoundOff();
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (_musicSource.clip == clip) return;

        _musicSource.clip = clip;
        _musicSource.loop = true;
        _musicSource.Play();
    }

    public void PlayClick(AudioClip clip)
    {
        _sfxSource.PlayOneShot(clip);
    }

    public void SoundOn()
    {
        _sfxSource.volume = _defaultMusicVolume;
        _musicSource.volume = _defaultMusicVolume;

        PlayerPrefs.SetInt("sound", 0);
    }
    public void SoundOff()
    {
        _sfxSource.volume = 0f;
        _musicSource.volume = 0f;

        PlayerPrefs.SetInt("sound", 1);
    }

    public bool IsSoundOn()
    {
        return PlayerPrefs.GetInt("sound") == 0;
    }
}