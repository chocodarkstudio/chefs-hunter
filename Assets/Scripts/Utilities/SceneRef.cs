using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
using System.Linq;
#endif

[CreateAssetMenu(fileName = "new SceneRef", menuName = "Custom/SceneRef", order = 1)]
public class SceneRef : ScriptableObject
{

    [field: SerializeField] public string SceneName { get; private set; }
    public string SceneNameLower => SceneName.ToLower();
    [field: SerializeField] public bool MakeActiveOnLoad { get; private set; }

    public static string Get(SceneRef sceneRef)
    {
        if (sceneRef == null)
            return string.Empty;

        return sceneRef.SceneName;
    }

    #region InpsectorEditor
#if UNITY_EDITOR
    [Space()]
    [Space()]
    [SerializeField]
    SceneAsset sceneAsset;


    [CustomEditor(typeof(SceneRef))]
    [CanEditMultipleObjects]
    public class SceneReffEditor : Editor
    {
        SceneRef selfTarget;
        bool renameFile;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            // avoid play mode
            if (Application.isPlaying)
                return;

            selfTarget = (SceneRef)target;

            GUILayout.Space(5);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button(Selection.count > 1 ? "Save all" : "Save"))
            {
                // update all selected (one too)
                foreach (var sceneRef in Selection.objects.Select((gm) => (SceneRef)gm))
                    UpdateSceneReference(sceneRef, save: false);
                AssetDatabase.SaveAssets();
            }
            GUILayout.Space(10);
            renameFile = GUILayout.Toggle(renameFile, "Rename file");
            GUILayout.EndHorizontal();

            // only valid sceneAsset below
            if (selfTarget.sceneAsset == null)
                return;

            // load scene in editor
            GUILayout.Space(20);
            GUILayout.Label("Level mode");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Load Level"))
            {
                string scenePath = AssetDatabase.GetAssetOrScenePath(selfTarget.sceneAsset);
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            }

            /*
            // only levels below
            if (!selfTarget.sceneAsset.name.ToLower().Contains("level"))
            {
                GUILayout.EndHorizontal();
                return;
            }
            */


            GUILayout.Space(10);
            if (GUILayout.Button("Load Level with Main"))
            {
                string scenePath = AssetDatabase.GetAssetOrScenePath(selfTarget.sceneAsset);
                string mainPath = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(0);

                EditorSceneManager.OpenScene(mainPath, OpenSceneMode.Single);
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            }
            GUILayout.EndHorizontal();

        }

        void UpdateSceneReference(SceneRef self, bool save = true)
        {
            // avoid play mode
            if (Application.isPlaying || self == null)
                return;


            // nothing changes
            if (!renameFile && (self.sceneAsset == null && self.SceneName == "empty_reference"
                || self.sceneAsset != null && self.sceneAsset.name == self.SceneName))
                return;

            // no scene
            if (self.sceneAsset == null)
            {
                self.SceneName = "empty_reference";
                EditorUtility.SetDirty(self);
                if (save)
                    AssetDatabase.SaveAssetIfDirty(self);
                return;
            }

            // set SceneName
            self.SceneName = self.sceneAsset.name;

            EditorUtility.SetDirty(self);


            if (renameFile)
            {
                // unselect edited asset
                Selection.activeObject = null;

                // change file name to match scene
                string assetPath = AssetDatabase.GetAssetPath(self.GetInstanceID());
                AssetDatabase.RenameAsset(assetPath, self.SceneName);

                if (save)
                    AssetDatabase.SaveAssets();
            }
            else if (save)
            {
                AssetDatabase.SaveAssetIfDirty(self);
            }

            Debug.Log($"SceneRef updated {self.SceneName}");
        }

    }

#endif
    #endregion

}
