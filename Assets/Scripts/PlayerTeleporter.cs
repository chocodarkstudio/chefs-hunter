using UnityEngine;
using UnityEngine.Events;
using UIAnimShortcuts;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTeleporter : MonoBehaviour
{
    [SerializeField] Transform targetPoint;
    [SerializeField] Vector3 cameraPoint;

    public static readonly UnityEvent onPlayerTeleport = new();

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            player.transform.position = targetPoint.position;
            CameraController.SetPos(cameraPoint);

            onPlayerTeleport.Invoke();
        }
    }
}
