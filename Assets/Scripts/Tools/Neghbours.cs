using UnityEngine;

public static class Neghbours
{
    public static Vector2Int[] Vector2INT = new Vector2Int[4] { Vector2Int.down, Vector2Int.left, Vector2Int.up, Vector2Int.right };

    public static Vector2Int RotateToLeft(this Vector2Int vec) => new Vector2Int(vec.y * -1, vec.x);
    
    public static Vector2Int RotateToRight(this Vector2Int vec) => new Vector2Int(vec.y, vec.x * -1);

    public static Vector3 RotateToLeft(this Vector3 vec) => new Vector3(vec.y * -1, vec.x, 0);
    
    public static Vector3 RotateToRight(this Vector3 vec) => new Vector3(vec.y, vec.x * -1, 0);
}