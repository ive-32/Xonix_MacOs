using System.Collections.Generic;
using UnityEngine;

public class GrounderEnemy : BaseEnemy
{
    protected override void Start()
    {
        base.Start();
        var availableTiles = new List<Vector2Int>();
        
        for (var i = 0; i < IcwGame.SizeX; i++)
        for (var j = 0; j < IcwGame.SizeY; j++)
        {
            if (Field.GetTileType(i, j).IsGround())
                availableTiles.Add(new Vector2Int(i, j));
        }

        var startPosition = availableTiles[Random.Range(0, availableTiles.Count)];

        transform.position = startPosition.ToVector3();
        EnemySpeed = 1;
        
        Direction = GetDirection();
    }
    
    protected new void Update()
    {
        var (reflectionNormalVector, _) = GetCollision(transform.position, new[] { TileType.Empty });

        if (reflectionNormalVector.magnitude > 0)
            Direction = Vector3.Reflect(Direction, reflectionNormalVector.normalized).normalized;
        
        transform.SetPositionAndRotation(transform.position + Direction 
            * (EnemySpeed * IcwGame.GameSpeed * Time.deltaTime), 
            Quaternion.identity);
    }

    private static Vector3 GetDirection()
        => Random.Range(0, 4) switch
            {
                0 => new Vector3(1, 1, 0),
                1 => new Vector3(1, -1, 0),
                2 => new Vector3(-1, 1, 0),
                3 => new Vector3(-1, -1, 0),
                _ => Vector3.zero
            };
    
}
