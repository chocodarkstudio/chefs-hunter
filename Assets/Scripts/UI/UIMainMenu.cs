using UIAnimShortcuts;
using UnityEngine;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] Transform playBtn;
    [SerializeField] Transform exitBtn;


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
}
