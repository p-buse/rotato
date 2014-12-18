using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour {
    static MusicTheme.LevelTheme currentTheme = MusicTheme.LevelTheme.none;
    public static MusicManager instance;
    private static AudioSource deckA;
    private static AudioSource deckB;
    private static bool musicIsPlaying = false;
    private enum Deck { A, B };
    private static Deck deck = Deck.A;
    private static float fadeTime = 0.5f;
    private static float fader = 0f;
    public float musicVolume = 1f;
    private static float _fxVolume = .75f;
    public float fxVolume
    {
        get
        {
            return _fxVolume;
        }
        set
        {
            if (value != _fxVolume)
                this.UpdateSceneAudio(value);
            _fxVolume = value;
        }
    }

    void Awake()
    {
        if (MusicManager.instance == null)
        {
            MusicManager.instance = this;
            MusicManager.deckA = gameObject.AddComponent<AudioSource>();
            MusicManager.deckB = gameObject.AddComponent<AudioSource>();
            deckA.loop = true;
            deckB.loop = true;
            OnLevelWasLoaded();
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void UpdateSceneAudio(float newVolume)
    {
        // Set the volume of each thing in the scene to the current volume.
        // At time of writing, this is only Lasers
        AudioSource[] sceneAudio = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audioSource in sceneAudio)
        {
            if (audioSource.GetInstanceID() != this.GetInstanceID())
            {
                audioSource.volume = newVolume;
            }
        }
    }

    void OnLevelWasLoaded()
    {
        
        MusicTheme musicTheme = FindObjectOfType<MusicTheme>();
            if (musicTheme == null)
            {
                //musicIsPlaying = false;
                //currentTheme = MusicTheme.LevelTheme.none;
                //FadeOut();
            }
            else
            {
                if (!musicIsPlaying)
                {
                    musicIsPlaying = true;
                    currentTheme = musicTheme.theme;
                    StartSongImmediate(musicTheme.theme);
                }
                else if (musicIsPlaying && musicTheme.restartOnLoad)
                {
                    musicIsPlaying = true;
                    currentTheme = musicTheme.theme;
                    StartSongImmediate(musicTheme.theme);
                }
                else if (musicIsPlaying && musicTheme.theme != currentTheme)
                {
                    musicIsPlaying = true;
                    currentTheme = musicTheme.theme;
                    StartSong(musicTheme.theme);
                }
                else if (musicIsPlaying && musicTheme.theme == MusicTheme.LevelTheme.none)
                {
                    FadeOut();
                }
                else
                {
                    // Music is the same as what's playing, do nothing
                }
            }
    }

    private void StartSongImmediate(MusicTheme.LevelTheme levelTheme)
    {
        fader = 0f;
        deckA.clip = LoadSong(levelTheme);
        deckA.Play();
        deck = Deck.A;
    }

    private void StartSong(MusicTheme.LevelTheme levelTheme)
    {
        AudioClip song = LoadSong(levelTheme);
        if (deck == Deck.A)
        {
            deckB.Stop();
            deckB.clip = song;
            deckB.Play();
            deck = Deck.B;
        }
        else
        {
            deckA.Stop();
            deckA.clip = song;
            deckA.Play();
            deck = Deck.A;
        }
    }

    private static AudioClip LoadSong(MusicTheme.LevelTheme levelTheme)
    {
        AudioClip song = new AudioClip();
        switch (levelTheme)
        {
            case MusicTheme.LevelTheme.farm: song = (AudioClip)Resources.Load("farm"); break;
            case MusicTheme.LevelTheme.city: song = (AudioClip)Resources.Load("city2"); break;
            case MusicTheme.LevelTheme.lab: song = (AudioClip)Resources.Load("lab"); break;
            case MusicTheme.LevelTheme.space: song = (AudioClip)Resources.Load("space"); break;
            case MusicTheme.LevelTheme.escape: song = (AudioClip)Resources.Load("escape"); break;
			case MusicTheme.LevelTheme.menu: song = (AudioClip)Resources.Load("menu"); break;
        }
        return song;
    }

    private void FadeOut()
    {
        if (deck == Deck.A)
        {
            deck = Deck.B;
            deckB.Stop();
        }
        else
        {
            deck = Deck.A;
            deckA.Stop();
        }
    }

    private void Crossfade()
    {
        if (MusicManager.deck == Deck.A)
        {
            fader = Mathf.Clamp01(fader -= Time.deltaTime * fadeTime);
        }
        else
            fader = Mathf.Clamp01(fader += Time.deltaTime * fadeTime);
    }
    void Update()
    {
        Crossfade();
        deckA.volume = (1f - fader) * musicVolume;
        deckB.volume = fader * musicVolume;
    }

}
