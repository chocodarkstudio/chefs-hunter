using ChocoDark.GlobalAudio;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

public class MenuIntroVideo : MonoBehaviour
{
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] GameObject gameCanvas;
    [SerializeField] GameObject videoCanvas;

    [SerializeField] Timer stopTimer;

    [SerializeField] UIBarFiller uiFiller;

    public bool PlayerNeverSawTheIntro { get; private set; }

    public readonly UnityEvent onVideoCompleted = new();

    private void Awake()
    {
        videoPlayer.loopPointReached += OnVideoCompleted;
        stopTimer.onCompleted.AddListener(OnVideoSkipped);

        PlayerNeverSawTheIntro = PlayerPrefs.GetInt("PlayerNeverSawTheIntro", 1) == 1;
    }

    private void Update()
    {
        if (videoPlayer.isPlaying)
        {
            // start timer
            if (!stopTimer.IsRunning &&
                (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return)))
            {
                stopTimer.Restart();
                uiFiller.Show(true);
            }

            // stop timer
            if (stopTimer.IsRunning &&
                (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(KeyCode.Return)))
            {
                stopTimer.Stop();
                uiFiller.Show(false);
            }
        }
        stopTimer.Update();
        uiFiller.SetFillerAmount(stopTimer.RemainingPercent);
    }

    [ContextMenu(nameof(PlayVideo))]
    public void PlayVideo()
    {
        GlobalAudio.StopChannel(Channel.Music);

        videoPlayer.Stop();
        videoPlayer.Play();
        stopTimer.Stop();

        gameCanvas.SetActive(false);
        videoCanvas.SetActive(true);
    }


    public void StopVideo()
    {
        GlobalAudio.StopChannel(Channel.Music);
        videoPlayer.Stop();
        gameCanvas.SetActive(true);
        videoCanvas.SetActive(false);

        // save intro
        PlayerNeverSawTheIntro = true;
        PlayerPrefs.SetInt("PlayerNeverSawTheIntro", 1);
    }

    void OnVideoCompleted(VideoPlayer _)
    {
        StopVideo();
        onVideoCompleted.Invoke();
    }

    void OnVideoSkipped()
    {
        StopVideo();
        uiFiller.Show(false);
        onVideoCompleted.Invoke();
    }
}

