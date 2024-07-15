using UnityEditor;
using UnityEngine;

public class AlignToCam : MonoBehaviour
{
    [SerializeField] Transform pivotSortPoint;

    [SerializeField] Vector3 alignAxis;
    private void Awake()
    {
        Align();
    }

    public void Align(Transform point)
    {
        Vector3 newRotation = transform.eulerAngles;
        newRotation.x = point.eulerAngles.x; // Align y-axis rotation
        transform.eulerAngles = newRotation;
    }

    public void Align() => Align(CameraController.MainCamera.transform);


    public void SortSpriteOrderByHeight()
    {
        if (!TryGetComponent(out SpriteRenderer spriteRenderer))
            return;

        if (pivotSortPoint == null)
        {
            pivotSortPoint = transform;
        }

        spriteRenderer.sortingOrder = (int)(pivotSortPoint.position.y * 10);
    }


    [MenuItem("Tools/AlignToCam SelectedObjects")]
    private static void AlignToCamSelectedObjects()
    {
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length == 0)
        {
            Debug.Log("No objects selected.");
            return;
        }
        Undo.RecordObjects(selectedObjects, "AlignToCam");

        Transform cameraT = Camera.main.transform;
        foreach (GameObject obj in selectedObjects)
        {
            if (!obj.TryGetComponent(out AlignToCam alignToCam))
                continue;

            alignToCam.Align(cameraT);
        }
    }
}
