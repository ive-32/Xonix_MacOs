using UnityEngine;

public class FieldTile
{
    public readonly Vector2Int Position;
    public TileType TileType = TileType.Empty;
    public GameObject GameObject;

    public FieldTile(int x, int y)
        => Position = new Vector2Int(x, y);
};