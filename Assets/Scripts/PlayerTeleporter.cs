using UnityEngine;
using UnityEngine.Events;

public class PlayerTeleporter : MonoBehaviour
{
    [SerializeField] Transform targetPoint;
    [SerializeField] Vector3 cameraPoint;
    [field: SerializeField] public string TeleporterName { get; private set; }

    public static readonly UnityEvent<string> onPlayerTeleport = new();

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            player.transform.position = targetPoint.position;
            CameraController.SetPos(cameraPoint);

            onPlayerTeleport.Invoke(TeleporterName.ToLower());
        }
    }
}
