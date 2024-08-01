using ChocoDark.GlobalAudio;
using DG.Tweening;
using System.Collections;
using UnityEngine;

public class DualMusicTransition : MonoBehaviour
{
    [SerializeField] GlobalAudioSource kitchenSource;
    [SerializeField] GlobalAudioSource battleSource;

    string lastSourceName;

    private void Awake()
    {
        PlayerTeleporter.onPlayerTeleport.AddListener(OnPlayerTeleport);
        GlobalAudio.onChannelVolumeChange.AddListener(OnChannelVolumeChange);

        SourceTransition("kitchen");
    }

    void Start()
    {
        StartCoroutine(SynchronizeAudioSources());
    }

    IEnumerator SynchronizeAudioSources()
    {
        // Ensure all audio sources are prepared
        kitchenSource.AudioSource.Stop();
        battleSource.AudioSource.Stop();

        /*
        // Wait until all sources are prepared
        yield return new WaitForEndOfFrame();
        */

        // Start all audio sources at the exact same time
        kitchenSource.AudioSource.PlayScheduled(AudioSettings.dspTime);
        battleSource.AudioSource.PlayScheduled(AudioSettings.dspTime);
        yield break;
    }

    void OnPlayerTeleport(string teleportName)
    {
        SourceTransition(teleportName);
    }

    void SourceTransition(string sourceName)
    {
        float currentVol = GlobalAudio.GetChannelVolume(kitchenSource.Channel);

        float transitionDuration = 0.25f;

        // do volume transitions
        kitchenSource.AudioSource.DOFade(sourceName == "kitchen" ? currentVol : 0f, transitionDuration).SetEase(Ease.InOutQuad);
        battleSource.AudioSource.DOFade(sourceName == "battle" ? currentVol : 0f, transitionDuration).SetEase(Ease.InOutQuad);

        lastSourceName = sourceName;
    }

    void OnChannelVolumeChange(Channel channel, float vol)
    {
        if (channel != kitchenSource.Channel)
            return;

        // default, no teleport touched yet
        if (string.IsNullOrEmpty(lastSourceName))
        {
            kitchenSource.AudioSource.volume = vol;
            battleSource.AudioSource.volume = 0f;
        }

        // Update the source volume if the last teleport was it.
        // Otherwise, keep the current volume.

        // kitchen
        kitchenSource.AudioSource.volume = lastSourceName == "kitchen" ? vol : kitchenSource.AudioSource.volume;
        // battle
        battleSource.AudioSource.volume = lastSourceName == "battle" ? vol : battleSource.AudioSource.volume;
    }
}
