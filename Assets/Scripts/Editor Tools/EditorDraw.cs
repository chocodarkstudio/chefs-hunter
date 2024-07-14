using UnityEngine;


public static class EditorDraw
{
    public static bool Enable = true;

    public static void DrawPoint(Vector3 pos, Color c)
    {
        if (!Enable)
            return;

        DebugExtension.DebugCircle(pos, Vector3.up, c, duration: 5);
        DebugExtension.DebugArrow(pos + Vector3.up * 3, -Vector3.up * 3, c, duration: 5);
    }

    public static void DrawBounds(Bounds b, Vector3 pos, Color c)
    {
        b.center += pos;
        DebugExtension.DrawBounds(b, c);
    }

    public static void DrawBounds(Bounds b, Vector3 pos)
    {
        b.center += pos;
        DebugExtension.DrawBounds(b, Color.yellow);
    }
}
