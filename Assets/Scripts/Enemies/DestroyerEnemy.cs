using System.Linq;
using UnityEngine;

public class DestroyerEnemy : BaseEnemy
{
    
    protected new void Update()
    {
        var (reflectionNormalVector, tiles) = GetCollision(transform.position,
            new[] { TileType.Border, TileType.Filled });

        foreach (var tile in tiles.Where(tile => tile.TileType == TileType.Trace))
            Field.HitTraceTile(tile.Position);

        foreach (var tile in tiles.Where(tile => tile.TileType == TileType.Filled))
            Field.PutTile(TileType.Empty, tile.Position);
        
        if (reflectionNormalVector.magnitude > 0)
            Direction = Vector3.Reflect(Direction, reflectionNormalVector.normalized).normalized;

        transform.SetPositionAndRotation(transform.position + Direction * (EnemySpeed * IcwGame.GameSpeed * Time.deltaTime), 
                Quaternion.identity);
    }
    
}
