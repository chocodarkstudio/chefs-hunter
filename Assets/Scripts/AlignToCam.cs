using UnityEngine;

public class AlignToCam : MonoBehaviour
{

    [SerializeField] Vector3 alignAxis;

    private void Update()
    {
        Vector3 newRotation = transform.eulerAngles;
        newRotation.x = CameraController.MainCamera.transform.eulerAngles.x; // Align y-axis rotation
        transform.eulerAngles = newRotation;
    }
}
