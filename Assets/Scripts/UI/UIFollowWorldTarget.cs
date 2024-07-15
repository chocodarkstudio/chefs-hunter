using System.Collections;
using UnityEngine;

public class UIFollowWorldTarget : MonoBehaviour
{
    [SerializeField] Transform target;
    Vector3? targetPos;

    [SerializeField] Vector2 uiOffset;
    [SerializeField] Vector3 worldOffset;
    [SerializeField] float smoothness = 1;

    [SerializeField] bool startFollowOnAwake = false;

    public bool IsFollowing { get; private set; }
    Coroutine followCorotuine;

    private void Awake()
    {
        if (startFollowOnAwake && target != null)
            StartFollow();
    }

    /// <summary>
    /// can be used during use, if the target type changes, the routine will be restarted </summary>
    public void SetFollowTarget(Transform target)
    {
        // target is in 'position' mode
        bool isInOtherMode = targetPos != null;

        if (isInOtherMode)
            RemoveFollowTarget();

        this.target = target;

        // restart if it has been following and in other mode
        if (isInOtherMode && IsFollowing)
            StartFollow();
    }

    /// <summary>
    /// can be used during use, if the target type changes, the coroutine will be restarted </summary>
    public void SetFollowTarget(Vector3 targetPos)
    {
        // target is in 'transfrom' mode
        bool isInOtherMode = target != null;

        if (isInOtherMode)
            RemoveFollowTarget();

        this.targetPos = targetPos;

        // restart if it has been following
        if (IsFollowing)
            StartFollow();
    }

    /// <summary>
    /// Stop the coroutine and remove target </summary>
    public void RemoveFollowTarget()
    {
        StopFollow();
        target = null;
        targetPos = null;
    }


    public void StartFollow()
    {
        // prevent run twice
        StopFollow();

        // start follow transform coroutine
        if (target != null)
            followCorotuine = StartCoroutine(FollowTransform());
        // start follow position coroutine
        else if (targetPos != null)
            followCorotuine = StartCoroutine(FollowPosition());
        // no target defined, cancel
        else return;

        IsFollowing = true;
    }

    /// <summary>
    /// Stop the coroutine but dont remove the target </summary>
    public void StopFollow()
    {
        if (followCorotuine == null)
            return;

        IsFollowing = false;
        StopCoroutine(followCorotuine);
    }

    /// <summary>
    /// Sets a offset in pixels of ui  </summary>
    public void SetUIOffset(Vector2 uiOffset) => this.uiOffset = uiOffset;

    /// <summary>
    /// Sets a offset in world position </summary>
    public void SetWOffset(Vector3 worldOffset) => this.worldOffset = worldOffset;

    public void SetSmoothness(float sm) => this.smoothness = sm;


    public void UpdatePosition()
    {
        if (target != null)
            MoveTo(target.position);
        else
            MoveTo(targetPos.Value);
    }

    IEnumerator FollowTransform()
    {
        while (target != null)
        {
            MoveSmoothTo(target.position);
            yield return null;
        }
    }

    IEnumerator FollowPosition()
    {
        while (targetPos.HasValue)
        {
            MoveSmoothTo(targetPos.Value);
            yield return null;
        }

    }

    void MoveTo(Vector3 worldPosition)
    {
        Vector3 uiPos = CameraController.MainCamera.WorldToScreenPoint(worldPosition + worldOffset);
        Vector3 newPos = uiPos + (Vector3)uiOffset;
        transform.position = newPos;
    }

    void MoveSmoothTo(Vector3 worldPosition)
    {
        Vector3 uiPos = CameraController.MainCamera.WorldToScreenPoint(worldPosition + worldOffset);
        Vector3 newPos = uiPos + (Vector3)uiOffset;

        Vector3 diff = newPos - transform.position;

        transform.position += diff * smoothness;
    }

    #region static creation methods
    public static UIFollowWorldTarget Follow(GameObject obj, Vector3 target)
    {
        // get or create the component in the obj
        if (!obj.TryGetComponent(out UIFollowWorldTarget followTarget))
            followTarget = obj.AddComponent<UIFollowWorldTarget>();

        // configure follow
        followTarget.SetFollowTarget(target);
        followTarget.StartFollow();

        return followTarget;
    }

    public static UIFollowWorldTarget Follow<T1>(T1 obj, Vector3 target)
    where T1 : Component
    {
        if (obj == null || target == null)
            return null;

        return Follow(obj.gameObject, target);
    }


    public static UIFollowWorldTarget Follow(GameObject obj, Transform target)
    {
        // get or create the component in the obj
        if (!obj.TryGetComponent(out UIFollowWorldTarget followTarget))
            followTarget = obj.AddComponent<UIFollowWorldTarget>();

        // configure follow
        followTarget.SetFollowTarget(target);
        followTarget.StartFollow();

        return followTarget;
    }

    public static UIFollowWorldTarget Follow<T1, T2>(T1 obj, T2 target)
        where T1 : Component where T2 : Component
    {
        if (obj == null || target == null)
            return null;

        return Follow(obj.gameObject, target.transform);
    }
    #endregion
}
