using UnityEngine;

public class GlobalSystemsGO : MonoBehaviour
{
    static GlobalSystemsGO Singleton;

    private void Awake()
    {
        if (Singleton != null)
        {
            //DestroyImmediate(gameObject);
            Debug.LogWarning($"Another instance of GlobalSystems already exists!! Make sure only one exists");
            DestroyImmediate(gameObject);
            return;
        }
        Singleton = this;
    }
}
