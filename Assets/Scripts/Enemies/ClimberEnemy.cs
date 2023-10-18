using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ClimberEnemy : BaseEnemy
{
    private Vector3 _targetTilePoint;
    protected Vector2Int Direction2Int;
    
    [NonSerialized] public RotationType RotationType = RotationType.RotationLeft;
    
    protected override void Start()
    {
        base.Start();
        var availablePositions = new List<Vector2Int>(200);
        
        for (var i = 0; i < IcwGame.SizeX; i++)
        for (var j = 0; j < IcwGame.SizeY; j++)
        {
            var tile = Field.GetTileType(i, j);
            if (!tile.IsGround()) continue;
            
            foreach (var neighbour in Neghbours.Vector2INT)
            {
                var position = new Vector2Int(i + neighbour.x, j + neighbour.y);
                if (position.IsPositionValid() && Field.GetTileType(position) == TileType.Empty)
                    availablePositions.Add(position);
            }
        }

        if (!availablePositions.Any())
            Destroy(gameObject);
        
        var index = Random.Range(0, availablePositions.Count);
        _targetTilePoint = new Vector3(availablePositions[index].x, availablePositions[index].y, 0);
        transform.localPosition = _targetTilePoint;
        Direction2Int = GetNextClimbDirection(availablePositions[index], Direction2Int);
        EnemySpeed = 1.0f;
    }
    
    protected override void Update()
    {
        var position = transform.position;
        var step = Direction2Int.ToVector3() * (Time.deltaTime * EnemySpeed * IcwGame.GameSpeed);
        var stepToTargetPoint = _targetTilePoint - position;

        if (step.magnitude >= stepToTargetPoint.magnitude)
        {
            var currentTile = position.GetCenterTile2Int();
            step = stepToTargetPoint;
            Direction2Int = GetNextClimbDirection(currentTile, Direction2Int);
            _targetTilePoint = (currentTile + Direction2Int).ToVector3();
            
            if (Field.GetTileType(currentTile) == TileType.Trace)
                Field.HitTraceTile(currentTile);
        }
        
        transform.SetPositionAndRotation(position + step,
            transform.rotation * Quaternion.AngleAxis(360 * Time.deltaTime * IcwGame.GameSpeed / IcwGame.DefaultGameSpeed, 
                RotationType == RotationType.RotationLeft 
                    ? Vector3.forward
                    : Vector3.back));
    }

    protected Vector2Int GetPositiveRotation(Vector2Int direction)
        => RotationType == RotationType.RotationLeft 
            ? direction.RotateToLeft()
            : direction.RotateToRight();

    protected Vector2Int GetNegativeRotation(Vector2Int direction)
        => RotationType == RotationType.RotationLeft 
            ? direction.RotateToRight()
            : direction.RotateToLeft();

    protected Vector2Int GetNextClimbDirection(Vector2Int currentTile, Vector2Int direction)
    {
        TileType forwardTileType;
        TileType neighbourTileType;
        var counter = 0;

        if (direction == Vector2Int.zero)
        {
            direction = Vector2Int.left;

            forwardTileType = Field.GetTileType(currentTile + direction);
            neighbourTileType = Field.GetTileType(currentTile + GetPositiveRotation(direction));

            while ((forwardTileType.IsGround() || !neighbourTileType.IsGround()) && counter < 4)
            {
                direction = GetPositiveRotation(direction);
                forwardTileType = Field.GetTileType(currentTile + direction);
                neighbourTileType = Field.GetTileType(currentTile + GetPositiveRotation(direction));
                counter++;
            }

            if (counter == 4)
                return Vector2Int.zero;
        }

        counter = 0;
        neighbourTileType = Field.GetTileType(currentTile + GetPositiveRotation(direction));
        while (!neighbourTileType.IsGround() && counter < 4)
        {
            direction = GetPositiveRotation(direction);
            neighbourTileType = Field.GetTileType(currentTile + direction + GetPositiveRotation(direction));
            counter++;
        }

        if (counter == 4)
            return Vector2Int.zero;

        counter = 0;
        forwardTileType = Field.GetTileType(currentTile + direction);
        while (forwardTileType.IsGround() && counter < 4)
        {
            direction = GetNegativeRotation(direction);
            forwardTileType = Field.GetTileType(currentTile + direction);
            counter++;
        }
        
        return counter == 4 ? Vector2Int.zero: direction;
    }
}