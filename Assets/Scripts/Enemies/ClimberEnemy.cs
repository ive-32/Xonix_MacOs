using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

public class ClimberEnemy : BaseEnemy
{
    protected const float EnemySize = 0.5f;
    private const float RotationSpeed = 360 / (Mathf.PI * EnemySize);
    [NonSerialized] public RotationType RotationType = RotationType.RotationLeft;
    
    protected override void Start()
    {
        base.Start();
        var availablePositions = new List<(Vector3 pos, Vector3 dir)>(200);
        
        for (var i = 0; i < IcwGame.SizeX; i++)
        for (var j = 0; j < IcwGame.SizeY; j++)
        {
            var tile = Field.GetTileType(i, j);
            if (!tile.IsGround()) continue;
            
            foreach (var neighbour in Neghbours.Vector2INT)
            {
                var position = new Vector2Int(i + neighbour.x, j + neighbour.y);
                if (position.IsPositionValid() && Field.GetTileType(position) == TileType.Empty)
                {
                    
                    availablePositions.Add(
                        (new Vector3(i, j, 0) + neighbour.ToVector3() * (0.5f + EnemySize), 
                            GetPositiveRotation(neighbour).ToVector3()));
                }
            }
        }

        if (!availablePositions.Any())
            Destroy(this.gameObject);
        
        var index = Random.Range(0, availablePositions.Count);
        transform.localPosition = availablePositions[index].pos;
        Direction = availablePositions[index].dir;
        EnemySpeed = 1.0f;
    }

    protected new virtual void Update()
    {
        var currentPosition = Climb(Time.deltaTime);
        
        transform.SetPositionAndRotation(currentPosition,
            transform.rotation * Quaternion.AngleAxis(RotationSpeed * Time.deltaTime * EnemySpeed * IcwGame.GameSpeed, 
                RotationType == RotationType.RotationLeft 
                    ? Vector3.forward
                    : Vector3.back));
    }

    protected Vector3 Climb(float deltaTime)
    {
        var step = deltaTime * EnemySpeed * IcwGame.GameSpeed;
        
        var currentPosition = transform.position;
        do
        {
            var currentStep = step > EnemySize ? EnemySize : step;

            Direction = GetClimbDirection(currentPosition, Direction);

            currentPosition = Glue(currentPosition);
            currentPosition += Direction * currentStep;
            
            step -= currentStep;
        } while (step > 0);

        return currentPosition;
    }

    protected Vector3 Glue(Vector3 currentPosition)
    {
        var glueDirection = GetPositiveRotation(Direction);
        var neighbourTile = (currentPosition + glueDirection).GetCenterTile();
        if (glueDirection.x != 0)
            currentPosition.x = neighbourTile.x - glueDirection.x * (0.5f + EnemySize * 0.5f);
        if (glueDirection.y != 0)
            currentPosition.y = neighbourTile.y - glueDirection.y * (0.5f + EnemySize * 0.5f);

        return currentPosition;
    }
    protected Vector3 GetClimbDirection(Vector3 position, Vector3 direction)
    {
        var neighbourTile = (position - direction * (EnemySize * 0.495f) + GetPositiveRotation(direction)).GetCenterTile2Int();
        var neighbourTile2 = (position + direction * (EnemySize * 0.505f) + GetPositiveRotation(direction)).GetCenterTile2Int();
        var counter = 0;

        while (neighbourTile.IsPositionValid() 
               && !Field.GetTileType(neighbourTile).IsGround() 
               && neighbourTile2.IsPositionValid() 
               && !Field.GetTileType(neighbourTile2).IsGround()
               && counter < 5)
        {
            direction = GetPositiveRotation(direction);
            
            neighbourTile = (position - direction * (EnemySize * 0.495f) + GetPositiveRotation(direction)).GetCenterTile2Int();
            neighbourTile2 = (position + direction * (EnemySize * 0.505f) + GetPositiveRotation(direction)).GetCenterTile2Int();
            counter++;
        }
        
        if (counter == 5)
            direction = Vector3.zero;
        
        counter = 0;
        
        var targetTile = position + direction * (EnemySize * 0.505f);
        while ((!targetTile.IsPositionValid() || Field.GetTileType(targetTile).IsGround()) && counter < 5)
        {
            direction = GetNegativeRotation(direction);
                    
            targetTile = position + direction * (EnemySize * 0.505f);
            counter++;
        }
        
        if (counter == 5)
            direction = Vector3.zero;

        return direction;
    }
    
    protected Vector2Int GetPositiveRotation(Vector2Int direction)
        => RotationType == RotationType.RotationLeft 
            ? direction.RotateToLeft()
            : direction.RotateToRight();

    protected Vector2Int GetNegativeRotation(Vector2Int direction)
        => RotationType == RotationType.RotationLeft 
            ? direction.RotateToRight()
            : direction.RotateToLeft();

    
    protected Vector3 GetPositiveRotation(Vector3 direction)
        => RotationType == RotationType.RotationLeft 
            ? direction.RotateToLeft()
            : direction.RotateToRight();

    protected  Vector3 GetNegativeRotation(Vector3 direction)
        => RotationType == RotationType.RotationLeft 
            ? direction.RotateToRight()
            : direction.RotateToLeft();

}