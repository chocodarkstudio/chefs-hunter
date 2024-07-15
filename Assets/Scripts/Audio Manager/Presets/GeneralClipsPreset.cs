using UnityEngine;

[CreateAssetMenu(fileName = "new GeneralClipsPreset", menuName = "Custom/ClipsPreset/GeneralClipsPreset", order = 1)]
[System.Serializable]
public class GeneralClipsPreset : ScriptableObject
{

    public AudioClip enemyHitClip;
    public AudioClip playerHitClip;
    public AudioClip customerBellClip;
    public AudioClip missHitClip;
    public AudioClip enemyDeathClip;

    public AudioClip completeCustomerClip;


    [Header("Tutorial")]
    public AudioClip carboardOpenFlapClip;

}
