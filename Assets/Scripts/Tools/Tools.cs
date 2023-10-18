using UnityEngine;

public static class Tools
{
    public static Vector3 GetCenterTile(this Vector3 pos)
        => new (Mathf.Round(pos.x), Mathf.Round(pos.y), 0);

    public static Vector2Int GetCenterTile2Int(this Vector3 pos)
        => Vector2Int.RoundToInt(pos);
    
    public static bool IsOnCenterTile(this Vector3 pos, float minimalDelta = 0.01f)
        => Vector3.Magnitude(pos - Vector3Int.RoundToInt(pos)) <= minimalDelta + Mathf.Epsilon;

    public static bool ReachCenterTile(this Vector3 pos, Vector2Int centerTile, Vector2Int direction)
    {
        if ((pos - centerTile.ToVector3()).magnitude < Mathf.Epsilon) return true;
        if (direction == Vector2Int.zero) return false;

        return Vector2.Angle(centerTile - (Vector2)pos, direction) > Mathf.Epsilon;
    }

    public static bool ReachTargetPoint(this Vector3 pos, Vector3 targetPoint, Vector3 direction)
    {
        if ((pos - targetPoint).magnitude < Mathf.Epsilon) return true;
        if (direction == Vector3.zero) return false;

        return Vector3.Angle(targetPoint - pos, direction) > Mathf.Epsilon;
    }

    public static bool IsPositionValid(this Vector2Int pos)
        => pos.x is >= 0 and < IcwGame.SizeX && pos.y is >= 0 and < IcwGame.SizeY;

    public static bool IsPositionValid(this Vector3 pos)
        => pos.x is >= 0 and < IcwGame.SizeX && pos.y is >= 0 and < IcwGame.SizeY;

    public static Vector3 ToVector3(this Vector2Int pos)
        => new Vector3(pos.x, pos.y, 0);

    public static Vector3 ToVector3(this Vector2 pos)
        => new Vector3(pos.x, pos.y, 0);

}