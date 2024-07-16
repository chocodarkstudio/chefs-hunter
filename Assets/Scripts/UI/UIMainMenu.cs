using UIAnimShortcuts;
using UnityEngine;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] Transform playBtn;
    [SerializeField] Transform exitBtn;

    [SerializeField] Slider musicSlider;
    [SerializeField] Slider soundSlider;

    [SerializeField] AudioSource[] musicSources;


    public void OnPlayBtn()
    {
        UIAnim.BtnClick(playBtn);
        LevelLoader.LoadGameplayLevels();
    }

    public void OnExitBtn()
    {
        UIAnim.BtnClick(exitBtn);
        Application.Quit();
    }



public void OnMusicSlider(){
 if (musicSlider==null) return;
 GlobalAudio.MusicVolume = musicSlider.value;
 foreach(AudioSource adsrc in musicSources){
if (adsrc == null)
continue;
  adsrc.volume = GlobalAudio.MusicVolume;
 }
}

public void OnSoundSlider(){
 if (soundSlider==null)return;
 GlobalAudio.EffectVolume = soundSlider.value;
}

}
