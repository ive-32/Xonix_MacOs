using UnityEngine;

public static class Tools
{
    public static Vector3 GetCenterTile(this Vector3 pos)
        => new (Mathf.Round(pos.x), Mathf.Round(pos.y), 0);

}