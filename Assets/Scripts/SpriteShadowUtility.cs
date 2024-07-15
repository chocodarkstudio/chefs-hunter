using UnityEditor;
using UnityEngine;

public class SpriteShadowUtility
{
    private const string MaterialPath = "Assets/Materials/SpriteShadows/SpriteShadow.mat";

    [MenuItem("Tools/Set SpriteShadow to Selected Objects")]
    private static void AddSpriteRendererToSelectedObjects()
    {
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
    }

    private static void ProcessSelectedObjects(System.Action<GameObject> processAction)
    {
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
    }
}
