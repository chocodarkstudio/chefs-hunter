using GameUtils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GlobalMouse : MonoBehaviour
{
    public static bool IgnoreClicks { get; set; }

    public static bool IsMouseOverUI
    {
        get
        {
            // EventSystem.GameObject means UIGameObject
            return EventSystem.current.IsPointerOverGameObject();
        }
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1))
        {
            OnClick();
        }
    }



    public static Vector3 GetPos()
    {
        // get ray
        Ray r = CameraController.MainCamera.ScreenPointToRay(Input.mousePosition);

        // raycast without the grid, only other objects
        bool hitted = Physics.Raycast(r, out RaycastHit hit, maxDistance: Mathf.Infinity);

        if (!hitted)
        {
            // set the given state
            LayerMask lm = LayerMask.GetMask("GridPlane");
            hitted = Physics.Raycast(r, out hit, maxDistance: Mathf.Infinity, layerMask: lm);
        }

        // return to previous active state

        if (hitted)
            return hit.point;
        return Vector3.zero;
    }




    /// <summary>
    /// Get all objects the mouse passes over. </summary>
    /// <param name="gridDiscard"> When the grid is hit, discard the following objects. Keep the current verticalLayer </param>
    public static int GetObjsBehindMouse(out RaycastHit[] result)
    {
        result = GUtils.raycastHitAlloc;

        // get ray
        Ray r = CameraController.MainCamera.ScreenPointToRay(Input.mousePosition);

        // send raycast
        GUtils.CleanRaycastHitAlloc();
        int hittedCount = Physics.RaycastNonAlloc(r, GUtils.raycastHitAlloc, maxDistance: Mathf.Infinity);

        // sort by distance  
        for (int it = 0; it < hittedCount; it++)
        {
            for (int i = 0; i < hittedCount; i++)
            {
                if (i >= hittedCount - 1)
                    break;

                RaycastHit x = GUtils.raycastHitAlloc[i];
                RaycastHit y = GUtils.raycastHitAlloc[i + 1];

                if (x.collider == null || x.distance > y.distance)
                {
                    GUtils.raycastHitAlloc[i] = y;
                    GUtils.raycastHitAlloc[i + 1] = x;
                }
            }
        }


        // sort by IClickeable.SortOrder
        for (int it = 0; it < hittedCount; it++)
        {
            for (int i = 0; i < hittedCount; i++)
            {
                if (i >= hittedCount - 1)
                    break;

                RaycastHit x = GUtils.raycastHitAlloc[i];
                RaycastHit y = GUtils.raycastHitAlloc[i + 1];

                // pass if Y is null
                if (y.collider == null)
                    continue;

                IClickeable xC = GUtils.raycastHitAlloc[i].collider.GetComponent<IClickeable>();
                IClickeable yC = GUtils.raycastHitAlloc[i + 1].collider.GetComponent<IClickeable>();

                if (x.collider == null || (xC != null && yC != null && yC.IsSortHigherThan(xC)))
                {
                    GUtils.raycastHitAlloc[i] = y;
                    GUtils.raycastHitAlloc[i + 1] = x;
                }
            }
        }


        return hittedCount;
    }

    /// <summary>
    /// Get all T objects the mouse passes over. </summary>
    /// <param name="includeGrid"> When the grid is hit, discard the following objects. Keep the current verticalLayer </param>
    public static int TryGetObjsBehindMouse<T>(out T[] result)
        where T : Component
    {
        result = null;

        // get objs
        int hittedCount = GetObjsBehindMouse(out RaycastHit[] hits);

        // nothing was hitted
        if (hittedCount <= 0)
            return 0;

        // temporally store
        List<T> resultList = new();

        // get only T
        for (int i = 0; i < hittedCount; i++)
        {
            if (hits[i].collider.TryGetComponent(out T obj))
                resultList.Add(obj);
        }

        // return
        result = resultList.ToArray();
        return result.Length;
    }


    /// <summary>
    /// Get the first object the mouse passes over. </summary>
    /// <param name="includeGrid"> Allow to hit the grid, discard the following objects. Keep the current verticalLayer </param>
    public static RaycastHit GetFirstObjBehindMouse()
    {
        // get ray
        Ray r = CameraController.MainCamera.ScreenPointToRay(Input.mousePosition);

        // send raycast
        Physics.Raycast(r, out RaycastHit hit, maxDistance: Mathf.Infinity);

        return hit;
    }


    /// <summary>
    /// Get the first T object the mouse passes over. </summary>
    /// <param name="includeGrid"> Allow to hit the grid, discard the following objects. Keep the current verticalLayer </param>
    public static bool TryGetFirstObjBehindMouse<T>(out T result)
    {
        result = default;

        // get obj
        RaycastHit hit = GetFirstObjBehindMouse();

        // nothing was hitted
        if (hit.collider == null)
            return false;

        // try get component
        return hit.collider.TryGetComponent(out result);
    }


    // send a raycast and iterate over it calling all IClickable found
    void OnClick()
    {
        if (IsMouseOverUI || IgnoreClicks)
            return;

        // get objs
        int hittedCount = GetObjsBehindMouse(out RaycastHit[] hits);

        for (int i = 0; i < hittedCount; i++)
        {
            // only get clickeables
            bool hasComponent = hits[i].collider.TryGetComponent(out IClickeable clickeable);

            // call click
            if (hasComponent)
                clickeable.OnClickeableClick(hits[i].point, hitOrder: i);
        }
    }


}

/// <summary>
/// Only works if the gameobject associated with this script has a collider </summary>
public interface IClickeable
{
    public int ClickSortOrder { get; set; }

    /// <summary>
    /// Called for both mouse clicks </summary>
    public void OnClickeableClick(Vector3 clickedPoint, int hitOrder = 0);

    public bool IsSortHigherThan(IClickeable other) => ClickSortOrder > other.ClickSortOrder;

}

