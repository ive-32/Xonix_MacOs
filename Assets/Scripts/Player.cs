using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    private readonly Vector3 _startPosition = new (Mathf.Round(IcwGame.SizeX / 2.0f), 1, 0);

    private Vector3 _startFlowPosition = Vector3.zero;
    
    private Vector3 _currentTilePosition;

    private Vector2 _direction = Vector2.zero;
    private Vector2 _currentDirection = Vector2.zero;

    private static float _minimalDelta = 0.01f;
    public float playerSpeed = 2.0f;
    [NonSerialized] public Field _field;
    
    private void Awake()
    {
        transform.SetPositionAndRotation(_startPosition, Quaternion.identity);
    }

    public void Update()
    {
        _minimalDelta = Time.deltaTime * IcwGame.GameSpeed * playerSpeed;
        
        _direction = GetDirection();
        _direction = CorrectDirection(_direction);
        
        if (IsOnCenterTile(transform.position))
        {
            var newTilePosition = GetCenterTile(transform.position);
            transform.SetPositionAndRotation(newTilePosition, Quaternion.identity);
            
            if (_currentDirection.x + _direction.x + _currentDirection.y + _direction.y == 0)
                _direction = Vector2.zero;
            
            _currentDirection = _direction;

            if (newTilePosition != _currentTilePosition)
            {
                PlayerReachNewTile(newTilePosition, _currentTilePosition);
                _currentTilePosition = newTilePosition;
            }
        }            
        
        var position = transform.position + (Vector3) _currentDirection * 
            (Time.deltaTime * IcwGame.GameSpeed * playerSpeed); 
        transform.SetPositionAndRotation(position, Quaternion.identity);
    }

    private void PlayerReachNewTile(Vector3 newPosition, Vector3 oldPosition)
    {
        var newFieldPos = new Vector2Int(Mathf.RoundToInt(newPosition.x), Mathf.RoundToInt(newPosition.y));
        var oldFieldPos = new Vector2Int(Mathf.RoundToInt(oldPosition.x), Mathf.RoundToInt(oldPosition.y));

        var oldTile = _field.GetTile(oldFieldPos);
        var newTile = _field.GetTile(newFieldPos);

        // step from ground
        if (_startFlowPosition == Vector3.zero && newTile == TileType.Empty)
            _startFlowPosition = newPosition;
        
        // return to ground
        if (_startFlowPosition != Vector3.zero && newTile.IsGround())
        {
            _startFlowPosition = Vector3.zero;
            _direction = Vector2.zero;
            _field.FillFieldAfterFlow();
        }
        
        // set trace
        if (!newTile.IsGround())
            _field.PutTile(TileType.Trace, newFieldPos);
    }
    
    private Vector2 CorrectDirection(Vector2 dir)
    {
        var nextTile = GetCenterTile(transform.position);
        
        return Field.IsPositionValid(nextTile.x + dir.x, nextTile.y + dir.y) ? dir : Vector2.zero;
    }
    
    private Vector2 GetDirection()
    {
        var direction = _direction;
        
        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");
        
        if (x != 0 || y != 0)
            direction = Mathf.Abs(x) > Mathf.Abs(y) ? new Vector2(x, 0).normalized : new Vector2(0, y).normalized;

        if (Input.GetKey(KeyCode.Space)) 
            direction = Vector2.zero;

        return direction;
    }
    
    private static Vector3 GetCenterTile(Vector3 pos)
        => new (Mathf.Round(pos.x), Mathf.Round(pos.y), 0);
    
    private static bool IsOnCenterTile(Vector3 pos)
        => (pos - GetCenterTile(pos)).magnitude < _minimalDelta ;
    
}
