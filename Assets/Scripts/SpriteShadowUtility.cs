#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class SpriteShadowUtility
{
    private const string MaterialPath = "Assets/Materials/SpriteShadows/SpriteShadow.mat";

#if UNITY_EDITOR
    [MenuItem("Tools/Set SpriteShadow to Selected Objects")]
#endif
    private static void AddSpriteRendererToSelectedObjects()
    {
#if UNITY_EDITOR
        Material material = AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);

        if (material == null)
        {
            Debug.LogError($"Material not found at path: {MaterialPath}");
            return;
        }

        ProcessSelectedObjects(obj =>
        {
            if (obj.TryGetComponent(out SpriteRenderer spriteRenderer))
            {
                spriteRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                spriteRenderer.receiveShadows = true;
                spriteRenderer.material = material;

                if (!obj.TryGetComponent(out AlignToCam _))
                    obj.AddComponent<AlignToCam>();
            }
        });
#endif
    }

    private static void ProcessSelectedObjects(System.Action<GameObject> processAction)
    {
#if UNITY_EDITOR
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length == 0)
        {
            Debug.Log("No objects selected.");
            return;
        }
        Undo.RecordObjects(selectedObjects, "SpriteShadow");
        foreach (GameObject obj in selectedObjects)
        {
            processAction(obj);
        }
#endif
    }
}
