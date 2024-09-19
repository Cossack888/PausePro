using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource soundSource;
    public AudioSource musicSource;

    public Slider soundVolumeSlider;
    public Slider musicVolumeSlider;

    public AudioClip[] fleshMusic;
    public AudioClip[] ghostMusic;

    private AudioClip[] currentPlaylist;
    private int currentTrackIndex = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        musicSource.loop = false;
        if (soundVolumeSlider != null)
        {
            soundVolumeSlider.value = soundSource.volume;
            soundVolumeSlider.onValueChanged.AddListener(SetSoundVolume);
        }

        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = musicSource.volume;
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        }
        if (fleshMusic.Length > 0)
        {
            currentPlaylist = fleshMusic;
            PlayNextTrack();
        }
    }
    public void PlaySound(AudioClip clip)
    {
        soundSource.PlayOneShot(clip);
    }
    public void SwitchToAlbum(string albumName)
    {
        if (albumName == "flesh")
        {
            currentPlaylist = fleshMusic;
        }
        else if (albumName == "ghost")
        {
            currentPlaylist = ghostMusic;
        }
        else
        {
            Debug.LogWarning("Invalid album name.");
            return;
        }
        currentTrackIndex = 0;
        PlayNextTrack();
    }
    private void PlayNextTrack()
    {
        if (currentPlaylist.Length == 0) return;

        musicSource.clip = currentPlaylist[currentTrackIndex];
        musicSource.Play();
        musicSource.loop = false;
        currentTrackIndex = (currentTrackIndex + 1) % currentPlaylist.Length;
    }

    private void Update()
    {
        if (!musicSource.isPlaying && currentPlaylist.Length > 0)
        {
            PlayNextTrack();
        }
    }
    public void SetSoundVolume(float volume)
    {
        soundSource.volume = Mathf.Clamp(volume, 0f, 1f);
    }
    public void SetMusicVolume(float volume)
    {
        musicSource.volume = Mathf.Clamp(volume, 0f, 1f);
    }
}
