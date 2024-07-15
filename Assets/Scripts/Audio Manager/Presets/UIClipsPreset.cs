using UnityEngine;

[CreateAssetMenu(fileName = "new UIClipsPreset", menuName = "Custom/ClipsPreset/UIClipsPreset", order = 1)]
[System.Serializable]
public class UIClipsPreset : ScriptableObject
{
    public AudioClip buttonClip;
    public AudioClip operationErrorClip;


}
