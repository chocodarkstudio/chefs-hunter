#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

public class EnviromentRestorer : MonoBehaviour
{
    [System.Serializable]
    public struct EnviromentSetting
    {
        public Material skyboxMaterial;
        public Light Sun;
        public Color RealtimeShadowColor;

        [Header("Enviroment Lighting")]
        public UnityEngine.Rendering.AmbientMode Source;
        public float intensityMultiplier;

        /*
        [Header("Enviroment Reflections")]
        public UnityEngine.Rendering.AmbientMode Source;
        public float intensityMultiplier;
        */

        [Header("Fog")]
        public bool fog;
        public Color Color;
        public FogMode fogMode;

        public float start;
        public float end;
    }

    [SerializeField] EnviromentSetting enviromentSetting;


    private void Awake()
    {
        Restore();
    }

    /// <summary>
    /// Restore the saved enviromentSetting to actual loaded scene </summary>
    public void Restore()
    {
        RenderSettings.skybox = enviromentSetting.skyboxMaterial;
        RenderSettings.sun = enviromentSetting.Sun;
        RenderSettings.subtractiveShadowColor = enviromentSetting.RealtimeShadowColor;

        // Enviroment Lighting
        RenderSettings.ambientMode = enviromentSetting.Source;
        RenderSettings.ambientIntensity = enviromentSetting.intensityMultiplier;

        // fog
        RenderSettings.fog = enviromentSetting.fog;
        RenderSettings.fogColor = enviromentSetting.Color;
        RenderSettings.fogMode = enviromentSetting.fogMode;

        RenderSettings.fogStartDistance = enviromentSetting.start;
        RenderSettings.fogEndDistance = enviromentSetting.end;
    }


    /// <summary>
    /// Save the enviromentSetting from actual loaded scene </summary>
    [ContextMenu("SaveArray enviroment setting")]
    public void Save()
    {
        enviromentSetting = new()
        {
            skyboxMaterial = RenderSettings.skybox,
            Sun = RenderSettings.sun,
            RealtimeShadowColor = RenderSettings.subtractiveShadowColor,

            // Enviroment Lighting
            Source = RenderSettings.ambientMode,
            intensityMultiplier = RenderSettings.ambientIntensity,

            // fog
            fog = RenderSettings.fog,
            Color = RenderSettings.fogColor,
            fogMode = RenderSettings.fogMode,

            start = RenderSettings.fogStartDistance,
            end = RenderSettings.fogEndDistance
        };

#if UNITY_EDITOR
        Debug.Log("Enviroment setting saved");
        EditorUtility.SetDirty(this);
#endif
    }
}










#if UNITY_EDITOR

[CustomEditor(typeof(EnviromentRestorer))]
[CanEditMultipleObjects]
public class EnviromentRestorerEditor : Editor
{
    EnviromentRestorer enviromentRestorer;
    SerializedProperty enviromentSetting;

    void OnEnable()
    {
        enviromentRestorer = (EnviromentRestorer)target;
        enviromentSetting = serializedObject.FindProperty("enviromentSetting");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(enviromentSetting);
        serializedObject.ApplyModifiedProperties();


        if (!Application.isPlaying)
        {
            GUILayout.Space(5);

            // "Click to save settings"
            if (GUILayout.Button("SaveArray"))
                enviromentRestorer.Save();
        }
    }
}
#endif