using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class BaseEnemy : MonoBehaviour
{
    [NonSerialized] public float EnemySpeed = 2.0f;
    [NonSerialized] public Field Field;
    [NonSerialized] public GameObject Player;
    protected Vector3 Direction;
    
    protected virtual void Start()
    {
        transform.localPosition = new(Random.Range(3, IcwGame.SizeX - 3), Random.Range(3, IcwGame.SizeY - 3));

        do
        {
            Direction = Random.insideUnitCircle.normalized;
        } while (Mathf.Abs(Direction.x) < 0.3f || Mathf.Abs(Direction.y) < 0.3f);
    }

    protected virtual void Update()
    {
        var (reflectionNormalVector, tiles) = GetCollision(transform.position,
            new[] { TileType.Border, TileType.Filled });

        foreach (var tile in tiles.Where(tile => tile.TileType == TileType.Trace))
            Field.HitTraceTile(tile.Position);
        
        if (reflectionNormalVector.magnitude > 0)
            Direction = Vector3.Reflect(Direction, reflectionNormalVector.normalized).normalized;

        transform.SetPositionAndRotation(transform.position + Direction * (EnemySpeed * IcwGame.GameSpeed * Time.deltaTime), 
                Quaternion.identity);
    }

    protected (Vector3 reflectionNormalVector, List<FieldTile> tiles) GetCollision(Vector3 pos, TileType[] tileTypes)
    {
        var reflectionNormalVector = Vector3.zero;

        var collidedTiles = new List<FieldTile>();
        
        var tilePos = new Vector2Int(
            Mathf.RoundToInt(pos.x + Mathf.Clamp(Direction.x * 100, -0.5f, 0.5f)),
            Mathf.RoundToInt(pos.y));

        var tile = Field.GetTileWithValidation(tilePos);

        if (tile == null || tileTypes.Contains(tile.TileType))
            reflectionNormalVector += new Vector3(Direction.x, 0, 0);
        
        if (tile != null) 
            collidedTiles.Add(tile);
        
        tilePos = new Vector2Int(
            Mathf.RoundToInt(pos.x), 
            Mathf.RoundToInt(pos.y + Mathf.Clamp(Direction.y * 100, -0.5f, 0.5f)));

        tile = Field.GetTileWithValidation(tilePos);

        if (tile == null || tileTypes.Contains(tile.TileType))
            reflectionNormalVector += new Vector3(0, Direction.y, 0);
        if (tile != null) 
            collidedTiles.Add(tile);

        return (reflectionNormalVector, collidedTiles);
    }
    
}
