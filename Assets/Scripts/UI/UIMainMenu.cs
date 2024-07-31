using ChocoDark.GlobalAudio;
using UIAnimShortcuts;
using UnityEngine;
using UnityEngine.UI;

public class UIMainMenu : UIShowPanel
{
    [SerializeField] Transform blackBackgroundPanel;
    [SerializeField] Transform playBtn;
    [SerializeField] Transform exitBtn;

    [SerializeField] Slider musicSlider;
    [SerializeField] Slider soundSlider;

    [SerializeField] MenuIntroVideo menuIntroVideo;

    private void Awake()
    {
        if (musicSlider != null)
            musicSlider.value = GlobalAudio.MusicVolume;
        if (soundSlider != null)
            soundSlider.value = GlobalAudio.SFXVolume;
    }

    public void OnPlayBtn()
    {
        UIAnim.BtnClick(playBtn);


        if (menuIntroVideo.PlayerNeverSawTheIntro)
        {
            // load game level on video completed
            menuIntroVideo.onVideoCompleted.RemoveAllListeners();
            menuIntroVideo.onVideoCompleted.AddListener(() =>
            {
                LevelLoader.LoadGameplayLevels();
            });

            menuIntroVideo.PlayVideo();
        }
        else
        {
            LevelLoader.LoadGameplayLevels();
        }
    }

    public void OnExitBtn()
    {
        UIAnim.BtnClick(exitBtn);
        Application.Quit();
    }



    public void OnMusicSlider()
    {
        if (musicSlider == null)
            return;

        GlobalAudio.MusicVolume = musicSlider.value;
    }

    public void OnSoundSlider()
    {
        if (soundSlider == null)
            return;

        GlobalAudio.SFXVolume = soundSlider.value;
    }


    public override void Show(bool show)
    {
        base.Show(show);
        blackBackgroundPanel.gameObject.SetActive(show);
    }
}
