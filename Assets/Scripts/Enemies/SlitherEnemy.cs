using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SlitherEnemy : BaseEnemy
{
    private Vector2Int _currentTile;
    private Vector2Int _direction;
    
    protected override void Start()
    {
        base.Start();
        var availablePositions = new List<Vector2Int>(200);
        
        //get all available StartPositions
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
            Destroy(this.gameObject);
        
        var index = Random.Range(0, availablePositions.Count);
        transform.localPosition = new Vector3(availablePositions[index].x, availablePositions[index].y, 0);
        _currentTile = availablePositions[index];
        GetDirection();
        EnemySpeed = 1.0f;
    }



    protected override void Update()
    {
        var currentPosition = transform.position;
        var frameWholeStep = Time.deltaTime * IcwGame.GameSpeed * EnemySpeed;
        const float atomicStep = 0.5f;
        do
        {
            var currentStep = frameWholeStep > atomicStep ? atomicStep : frameWholeStep;

            frameWholeStep -= currentStep;

            var targetTile = _currentTile + _direction;
            if (currentPosition.ReachCenterTile(targetTile, _direction)) 
            {
                _currentTile = currentPosition.GetCenterTile2Int();

                var oldDirection = _direction;
                if (_direction == Vector2Int.zero)
                    GetDirection();

                var neighbourTileType = Field.GetTileType(_currentTile + _direction.RotateToLeft());
                var counter = 0;

                while (!neighbourTileType.IsGround() && counter < 5)
                {
                    _direction = _direction.RotateToLeft();
                    neighbourTileType = Field.GetTileType(_currentTile + _direction + _direction.RotateToLeft());
                    counter++;
                }

                if (counter == 5)
                    _direction = Vector2Int.zero;

                counter = 0;
                var targetTileType = Field.GetTileType(_currentTile + _direction);
                while (targetTileType.IsGround() && counter < 5)
                {
                    _direction = _direction.RotateToRight();
                    targetTileType = Field.GetTileType(_currentTile + _direction);
                    counter++;
                }

                if (counter == 5)
                    _direction = Vector2Int.zero;

                if (oldDirection != _direction)
                    currentPosition = _currentTile.ToVector3();

                TryToHitPlayer();
            }

            currentPosition += _direction.ToVector3() * currentStep;
        } while (frameWholeStep > atomicStep);
        
        transform.SetPositionAndRotation(currentPosition,
            transform.rotation * Quaternion.AngleAxis(360 * Time.deltaTime, Vector3.forward));
    }

    private void TryToHitPlayer()
    {
        if (Field.GetTileType(_currentTile) == TileType.Trace)
            Field.HitTraceTile(_currentTile);

        if (Player is null) return;
        
        var playerPos = Player.transform.position.GetCenterTile2Int();
        foreach (var neighbour in Neghbours.Vector2INT)
        {
            var pos = neighbour + _currentTile;
            if (pos == playerPos)
                Player.GetComponent<Player>()?.KillPlayer();
        }
    }
    
    private void GetDirection()
    {
        _direction = Vector2Int.right;
        
        foreach (var neighbour in Neghbours.Vector2INT)
        {
            var neighbourTile = _currentTile + neighbour;
            if (!neighbourTile.IsPositionValid() || !Field.GetTileType(neighbourTile).IsGround()) continue;

            _direction = (_currentTile - neighbourTile).RotateToRight();
            break;
        }

    }

}