using ChocoDark.GlobalAudio;
using DG.Tweening;
using System.Collections;
using UnityEngine;

public class DualMusicTransition : MonoBehaviour
{
    [SerializeField] GlobalAudioSource kitchenSource;
    [SerializeField] GlobalAudioSource battleSource;

    string lastTeleportName;

    private void Awake()
    {
        PlayerTeleporter.onPlayerTeleport.AddListener(OnPlayerTeleport);
        GlobalAudio.onChannelVolumeChange.AddListener(OnChannelVolumeChange);
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
        yield return new WaitForSeconds(0.1f);
        */

        // Start all audio sources at the exact same time
        kitchenSource.AudioSource.Play();
        battleSource.AudioSource.Play();

        yield break;
    }

    void OnPlayerTeleport(string teleportName)
    {
        float currentVol = GlobalAudio.GetChannelVolume(kitchenSource.Channel);

        float transitionDuration = 0.25f;

        // do volume transitions
        kitchenSource.AudioSource.DOFade(teleportName == "kitchen" ? currentVol : 0f, transitionDuration).SetEase(Ease.InOutQuad);
        battleSource.AudioSource.DOFade(teleportName == "battle" ? currentVol : 0f, transitionDuration).SetEase(Ease.InOutQuad);

        lastTeleportName = teleportName;
    }

    void OnChannelVolumeChange(Channel channel, float vol)
    {
        if (channel != kitchenSource.Channel)
            return;

        // default, no teleport touched yet
        if (string.IsNullOrEmpty(lastTeleportName))
        {
            kitchenSource.AudioSource.volume = vol;
            battleSource.AudioSource.volume = 0f;
        }

        // Update the source volume if the last teleport was it.
        // Otherwise, keep the current volume.

        // kitchen
        kitchenSource.AudioSource.volume = lastTeleportName == "kitchen" ? vol : kitchenSource.AudioSource.volume;
        // battle
        battleSource.AudioSource.volume = lastTeleportName == "battle" ? vol : battleSource.AudioSource.volume;
    }
}
