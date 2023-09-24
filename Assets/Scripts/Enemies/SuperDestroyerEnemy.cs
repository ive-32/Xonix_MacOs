using System.Linq;
using UnityEngine;

public class SuperDestroyerEnemy : BaseEnemy
{
    private float _destroySpeed = 1.0f;
    protected new void Update()
    {
        if (_destroySpeed < 1.0f)
            _destroySpeed += 2 * Time.deltaTime;
        
        var (reflectionNormalVector, tiles) = GetCollision(transform.position,
            new[] { TileType.Border });

        foreach (var tile in tiles.Where(tile => tile.TileType == TileType.Trace))
            Field.HitTraceTile(tile.Position);

        foreach (var tile in tiles.Where(tile => tile.TileType == TileType.Filled))
        {
            Field.PutTile(TileType.Empty, tile.Position);
            _destroySpeed = 0.3f;
        }
        
        if (reflectionNormalVector.magnitude > 0)
            Direction = Vector3.Reflect(Direction, reflectionNormalVector.normalized).normalized;

        transform.SetPositionAndRotation(transform.position + Direction * 
            (EnemySpeed * IcwGame.GameSpeed * _destroySpeed * Time.deltaTime), 
            Quaternion.identity);
    }
    
}