using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Singleton;

    [SerializeField] Camera mainCamera;
    public static Camera MainCamera => Singleton.mainCamera;

    [Space]
    [SerializeField] Transform pivot;
    [SerializeField] Transform innerPivot;

    // Target
    [field: SerializeField] Transform objectTarget;
    static Vector3? pointTarget;
    static Vector3 targetOffset;
    public Vector3? GetTargetPosition()
    {
        if (objectTarget != null)
            return objectTarget.position + targetOffset;
        return pointTarget + targetOffset;
    }
    static bool targetCapZ;


    // Zoom
    public static float DefaultZoom { get; private set; }
    public static float CurrentZoom { get; private set; }
    public static float TargetZoom { get; private set; }
    static Coroutine smoothZoomCoroutine;

    float diffAspectFactor;

    public static Quaternion DefaultRotation { get; private set; }
    public static Quaternion CurrentRotation { get; private set; }


    [SerializeField] float movementSpeed = 5;
    public static float MovementSpeedMultiplier { get; set; } = 1;


    private void Awake()
    {
        Singleton = this;

        CurrentRotation = DefaultRotation = pivot.localRotation;

        // calculate the difference in aspect ratios in others resolutions
        float currentAspect = (float)Screen.width / (float)Screen.height;
        diffAspectFactor = 1.3333333333333333f / currentAspect;
        diffAspectFactor = 1f;

        if (MainCamera.orthographic)
            TargetZoom = CurrentZoom = mainCamera.orthographicSize;

        DefaultZoom = TargetZoom;
        SetOrthoZoom(DefaultZoom);
    }

    public static void SetFocusTarget(Transform target, Vector3 offset = default, bool capZ = false)
    {
        // remove point target
        pointTarget = null;

        Singleton.objectTarget = target;
        targetOffset = offset;
        targetCapZ = capZ;
    }
    public static void SetFocusTarget(Vector3 position, Vector3 offset = default, bool capZ = false)
    {
        // remove obj target
        Singleton.objectTarget = null;

        if (capZ)
        {
            pointTarget = RemoveZRelative(position);
            return;
        }

        pointTarget = position;
        targetOffset = offset;
    }

    public static void SetPos(Vector3 pos, bool capZ = false)
    {
        if (capZ)
        {
            pointTarget = Singleton.pivot.position = RemoveZRelative(pos);
            return;
        }

        Singleton.pivot.position = pos;
    }


    public static void SetZoom(float zoom)
    {
        if (zoom == 0)
        {
            Debug.Log("Cannot set zoom at 0"); return;
        }

        TargetZoom = Mathf.Abs(zoom);

        // isometric mode only change the orthographicSize
        if (MainCamera.orthographic)
        {
            //Singleton.MainCamera.orthographicSize = Singleton.ZoomDistance;

            // prevent run twice the coroutine
            if (smoothZoomCoroutine != null)
                Singleton.StopCoroutine(smoothZoomCoroutine);
            smoothZoomCoroutine = Singleton.StartCoroutine(Singleton.SmoothZoom(speedMultiplier: 1f));
            return;
        }

        // perseptive mode
        Singleton.innerPivot.localPosition = new(
            Singleton.innerPivot.localPosition.x,
            Singleton.innerPivot.localPosition.y,
            -TargetZoom
        );
    }

    private void Update()
    {
        Vector3? pos = GetTargetPosition();
        if (!pos.HasValue)
            return;

        // multiply this to make smooth movement when is near to target
        float distance = Vector3.Distance(pivot.position, pos.Value);

        pivot.position = Vector3.MoveTowards(pivot.position, pos.Value,
            distance * movementSpeed * MovementSpeedMultiplier * Time.deltaTime);
    }


    IEnumerator SmoothZoom(float speedMultiplier = 1)
    {

        float currentVelocity = 0;
        while (true)
        {
            // multiply this to make smooth movement when is near to target
            float distance = Mathf.Abs(CurrentZoom - TargetZoom);

            CurrentZoom = Mathf.SmoothDamp(CurrentZoom, TargetZoom, ref currentVelocity, Time.smoothDeltaTime, speedMultiplier * 10 * distance);
            SetOrthoZoom(CurrentZoom);

            if (distance <= 0.01f)
                break;

            yield return null;
        }

        SetOrthoZoom(CurrentZoom);
    }

    void SetOrthoZoom(float zoom)
    {
        MainCamera.orthographicSize = diffAspectFactor * zoom;
    }



    public static void SetBackgroundColor(Color color)
    {
        MainCamera.backgroundColor = color;
    }


    public static Vector3 RemoveZRelative(Vector3 globalPos)
    {
        //EditorDraw.DrawPoint(globalPos, Color.magenta);

        // get target pos relative to local camera
        Vector3 camLocalPos = MainCamera.transform.InverseTransformPoint(globalPos);

        // remove z axis
        camLocalPos.Set(camLocalPos.x, camLocalPos.y, 0);

        // get back to global pos
        Vector3 camPos = MainCamera.transform.TransformPoint(camLocalPos);
        //EditorDraw.DrawPoint(camPos, Color.yellow);


        Vector3 camDisToPivot = Singleton.transform.position - MainCamera.transform.position;


        return camPos + camDisToPivot;
    }

    private void OnDrawGizmos()
    {
        if (mainCamera == null)
            return;
        /*
        DebugExtension.DrawCircle(mainCamera.transform.position, mainCamera.transform.forward, Color.gray);

        float arrowSize = 4;
        //DebugExtension.DrawArrow(MainCamera.transform.position, MainCamera.transform.up * arrowSize, Color.green);
        DebugExtension.DrawArrow(mainCamera.transform.position, mainCamera.transform.forward * arrowSize, Color.gray);
        //DebugExtension.DrawArrow(MainCamera.transform.position, MainCamera.transform.right * arrowSize, Color.red);
        */
    }


    public static void SetRotation(Vector3 rot)
    {
        CurrentRotation = Quaternion.Euler(rot);
        Singleton.pivot.localRotation = CurrentRotation;
    }

    public static void ResetRotation()
    {
        SetRotation(DefaultRotation.eulerAngles);
    }
}
