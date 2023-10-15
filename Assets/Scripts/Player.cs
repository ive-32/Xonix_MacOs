using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    private readonly Vector3 _startPosition = new (Mathf.Round(IcwGame.SizeX / 2.0f), 1, 0);

    private Vector3 _lastGroundPosition = Vector3.zero;
    private bool _playerIsKilled = false;
    
    private Vector3 _currentTilePosition;
    private Vector2Int _currentTilePositionInt;

    private Vector2 _direction = Vector2.zero;
    private Vector2 _currentDirection = Vector2.zero;
    private static float _minimalDelta = 0.01f;
    
    [NonSerialized] public const float PlayerDefaultSpeed = 1.5f;
    [NonSerialized] public float PlayerSpeed = PlayerDefaultSpeed;
    [NonSerialized] public Field Field;
    [NonSerialized] public TileType TraceTile = TileType.Trace;
    [NonSerialized] public Bonuses Bonuses;

    private AnimationClip _playerAppear;
    private AnimationClip _playerDisappear;
    private Animation _playerAnimation;
    
    private void Awake()
    {
        transform.SetPositionAndRotation(_startPosition, Quaternion.identity);
        _currentTilePosition = _startPosition;

        _playerAppear = GetComponent<Animation>().GetClip("PlayerAppear");
        _playerDisappear = GetComponent<Animation>().GetClip("PlayerDisappear");
        _playerAnimation = GetComponent<Animation>();
    }

    private void Start()
    {
        _playerAnimation.Rewind();
        _playerAnimation.clip = _playerAppear;
        _playerAnimation.Play();
        _lastGroundPosition = _startPosition;
    }

    public void Update()
    {
        if (_playerIsKilled)
        {
            if (Field.HasTraceTiles()) return;
            _playerIsKilled = false;
            
            PlayerKilled();
        }
        
        _minimalDelta = Time.deltaTime * IcwGame.GameSpeed * PlayerSpeed;
        
        _direction = GetDirection();
        _direction = CorrectDirection(_direction);
        
        if (transform.position.IsOnCenterTile(_minimalDelta))
        {
            var newTilePosition = transform.position.GetCenterTile();
            transform.SetPositionAndRotation(newTilePosition, Quaternion.identity);
            
            while (_currentTilePosition != newTilePosition)
            {
                var movingVector = (Vector3)_currentDirection.normalized;
                var currentTileToFill = (_currentTilePosition + movingVector).GetCenterTile();
                PlayerReachNewTile(currentTileToFill, _currentTilePosition);
                _currentTilePosition = currentTileToFill;
            } 

            if (_currentDirection.x + _direction.x + _currentDirection.y + _direction.y == 0)
                StopPlayer();
            
            _currentDirection = _direction;

        }

        if (!IsOnGround() && Field.GetTileType(_currentTilePositionInt) == TileType.Empty)
        {
            _playerIsKilled = true;
            _playerAnimation.clip = _playerDisappear;
            _playerAnimation.Play();
        }
        
        var position = transform.position + (Vector3) _currentDirection * 
            (Time.deltaTime * IcwGame.GameSpeed * PlayerSpeed); 
        transform.SetPositionAndRotation(position, Quaternion.identity);
    }

    public void KillPlayer()
    {
        _playerIsKilled = true;
        _lastGroundPosition = _startPosition;
    }
    
    private void PlayerKilled()
    {
        transform.SetPositionAndRotation(_lastGroundPosition, Quaternion.identity);
        _currentTilePosition = _lastGroundPosition;
        StopPlayer();
        _lastGroundPosition = _startPosition;
        _playerAnimation.Rewind();
        _playerAnimation.clip = _playerAppear;
        _playerAnimation.Play();
        IcwGame.Lives--;
    }
    
    private void PlayerReachNewTile(Vector3 newPosition, Vector3 oldPosition)
    {
        var newFieldPos = new Vector2Int(Mathf.RoundToInt(newPosition.x), Mathf.RoundToInt(newPosition.y));
        var oldFieldPos = new Vector2Int(Mathf.RoundToInt(oldPosition.x), Mathf.RoundToInt(oldPosition.y));
        _currentTilePositionInt = newFieldPos;
        
        var oldTile = Field.GetTileType(oldFieldPos);
        var newTile = Field.GetTileType(newFieldPos);
        var nextNewTile = Field.GetTileType(newFieldPos + Vector2Int.RoundToInt(_direction));

        //stop before going from ground
        if (nextNewTile == TileType.Empty && newTile.IsGround())
        {
            StopPlayer();
        }
        
        // step from ground
        if (IsOnGround() && newTile == TileType.Empty)
            _lastGroundPosition = oldPosition;
        
        // return to ground
        if (!IsOnGround() && newTile.IsGround())
        {
            _lastGroundPosition = Vector3.zero;
            StopPlayer();
            Field.FillFieldAfterFlow();
        }
        
        // set trace
        if (!newTile.IsGround())
            Field.PutTile(TraceTile, newFieldPos);
        
        // CheckBonuses
        Bonuses.CheckBonusInTile(_currentTilePosition)?.OnPickedUp(this);
    }

    private void StopPlayer()
    {
        _direction = Vector2.zero;
        if (transform.position.IsOnCenterTile(_minimalDelta))
            _currentDirection = Vector2.zero;
    }
    
    private Vector2 CorrectDirection(Vector2 dir)
    {
        var nextTile = transform.position.GetCenterTile() + dir.ToVector3();
        
        return nextTile.IsPositionValid() ? dir : Vector2.zero;
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
    
    public bool IsOnGround()
        => _lastGroundPosition == Vector3.zero;
    
}
