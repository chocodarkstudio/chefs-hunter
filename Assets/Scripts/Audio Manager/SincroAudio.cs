using UnityEngine;
using System.Collections;

public class SincroAudio : MonoBehaviour
{
    [SerializeField] AudioSource[] sources;

    void Start() 
    {
        StartCoroutine(SynchronizeAudioSources());
    }

    IEnumerator SynchronizeAudioSources()
    {
        // Ensure all audio sources are prepared
        foreach (var source in sources)
        {
            source.Play();
            source.Pause();
        }

        // Wait until all sources are prepared
        yield return new WaitForSeconds(0.1f);

        // Start all audio sources at the exact same time
        foreach (var source in sources)
        {
            source.UnPause();
        }
    }
}
