using MusicClips;
using UnityEngine;
using UnityEngine.Events;

public class GlobalAudio : MonoBehaviour
{
    static GlobalAudio Singleton;

    [Header("Sources")]
    [SerializeField]
    AudioSource musicAudioSource;
    [SerializeField]
    AudioSource effectsAudioSource;


    // Effect Volume
    float effectVolume = 1;
    public static float EffectVolume
    {
        get => Singleton.effectVolume;
        set
        {
            Singleton.effectVolume = value;

            // apply to audio source
            if (Singleton.effectsAudioSource != null)
                Singleton.effectsAudioSource.volume = value;

            onEffectVolumeChange.Invoke(value);
        }
    }

    // Music Volume
    float musicVolume = 1;
    public static float MusicVolume
    {
        get => Singleton.musicVolume;
        set
        {
            Singleton.musicVolume = value;

            // apply to audio source
            if (Singleton.musicAudioSource != null)
                Singleton.musicAudioSource.volume = value;

            onMusicVolumeChange.Invoke(value);
        }
    }

    [Header("Presets")]
    [SerializeField]
    UIClipsPreset uiClipsPreset;
    public static UIClipsPreset UIClips => Singleton.uiClipsPreset;


    [SerializeField]
    MusicClipsPreset musicClipsPreset;
    public static MusicClipsPreset MusicClips => Singleton.musicClipsPreset;
    public static bool IsMusicLoopPaused { get; private set; }

    [SerializeField]
    GeneralClipsPreset generalClipsPreset;
    public static GeneralClipsPreset GeneralClips => Singleton.generalClipsPreset;

    // Events
    public static readonly UnityEvent<float> onEffectVolumeChange = new();
    public static readonly UnityEvent<float> onMusicVolumeChange = new();

    private void Awake()
    {
        Singleton = this;
    }



    /// <summary>
    /// Play audio without an audio source </summary>
    /// <param name="worldPos">
    /// if worldPos is null, it will be effectsAudioSource position </param>
    public static void PlayAtPoint(AudioClip clip, Vector3? worldPos = null,
        float volume = 1, float pitchRandomizer = 0, float spatialBlend = 0.7f)
    {
        if (clip == null)
            return;

        // worldPos is null, set effectsAudioSource position 
        if (!worldPos.HasValue)
            worldPos = Singleton.effectsAudioSource.transform.position;

        // Create the audio source
        GameObject gameObject = new("One shot audio");
        gameObject.transform.position = worldPos.Value;
        AudioSource audioSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));

        // configure
        audioSource.clip = clip;
        audioSource.spatialBlend = spatialBlend;
        audioSource.volume = Mathf.Min(volume, EffectVolume);
        audioSource.pitch = 1 + Random.Range(-pitchRandomizer, pitchRandomizer);

        // play and destroy
        audioSource.Play();
        Destroy(gameObject, clip.length * ((Time.timeScale < 0.01f) ? 0.01f : Time.timeScale));
    }

    public static void PlayEffect(AudioClip clip, float volume = 1)
    {
        if (clip == null)
            return;

        /*
        //  randomize pitch
        float pitch = 1 + Random.Range(-pitchRandomizer, pitchRandomizer);
        Singleton.effectsAudioSource.pitch = pitch;
        */

        Singleton.effectsAudioSource.PlayOneShot(clip, Mathf.Min(volume, EffectVolume));
    }


    public static void StopAllEffects()
    {
        Singleton.effectsAudioSource.Stop();
    }

    #region Music
    public static void PlayMusic(AudioClip clip, float volume = 1, bool pauseLoop = false)
    {
        if (clip == null)
            return;

        // Debug.Log($"Playing Music ({clip.name})");

        Singleton.musicAudioSource.Stop();
        Singleton.musicAudioSource.clip = clip;
        Singleton.musicAudioSource.volume = Mathf.Min(volume, MusicVolume);
        //Singleton.musicAudioSource.PlayDelayed(5);
        Singleton.musicAudioSource.Play();

        IsMusicLoopPaused = pauseLoop;
    }


    public static void PlayMusicLoop(LoopOption loopOption, bool randomize = false)
    {
        if (Singleton == null)
        {
            Debug.Log("GlobalAudio not found!");
            return;
        }

        // already in that state
        if (MusicClipsPreset.CurrentLoop == loopOption &&
            MusicClipsPreset.RandomizeIndex == randomize)
            return;

        MusicClipsPreset.CurrentLoop = loopOption;
        MusicClipsPreset.RandomizeIndex = randomize;
        MusicClips.RestartLoop();

        // play now if loop isnt paused
        if (!IsMusicLoopPaused)
            PlayMusic(MusicClips.Next());
    }

    public static void StopMusicClip(AudioClip clip)
    {
        // invalid clip
        if (clip == null)
            return;

        // no playing clip
        if (Singleton.musicAudioSource.clip == null)
            return;

        // no matching clip
        if (Singleton.musicAudioSource.clip.name != clip.name)
            return;

        // stop it
        Singleton.musicAudioSource.Stop();
        Singleton.musicAudioSource.clip = null;
    }
    #endregion
}
