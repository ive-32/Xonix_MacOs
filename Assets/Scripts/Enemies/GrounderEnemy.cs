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

        var startPosititon = availableTiles[Random.Range(0, availableTiles.Count)].ToVector3();

        transform.position = startPosititon;
        EnemySpeed = 1;
    }
    
    protected new void Update()
    {
        var (reflectionNormalVector, _) = GetCollision(transform.position,
            new[] { TileType.Empty });
        
        if (reflectionNormalVector.magnitude > 0)
            Direction = Vector3.Reflect(Direction, reflectionNormalVector.normalized).normalized;

        transform.SetPositionAndRotation(transform.position + Direction * (EnemySpeed * IcwGame.GameSpeed * Time.deltaTime), 
                Quaternion.identity);
    }
    
}
