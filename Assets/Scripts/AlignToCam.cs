using UnityEngine;

public class AlignToCam : MonoBehaviour
{

    [SerializeField] Vector3 alignAxis;

    private void Update()
    {
        Camera camera = Camera.main;
        transform.LookAt(camera.transform.position);
    }
}
