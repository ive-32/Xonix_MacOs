using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    private readonly Vector3 _startPosition = new (Mathf.Round(IcwGame.SizeX / 2.0f), 1, 0);

    private Vector3 _lastGroundPosition = Vector3.zero;
    private bool _playerIsKilled = false;
    private const float DefaultTimeToAppear = 5.0f;
    private float _timeToAppear = DefaultTimeToAppear;
    private float _timeForUnbreakable = DefaultTimeToAppear;
    private bool _isFloating = false;
    
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
    [NonSerialized] public List<SlitherEnemy> slitherEnemies = new List<SlitherEnemy>();

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
        _lastGroundPosition = _startPosition;
        SetUpNewPlayer();
    }

    public void Update()
    {
        if (_timeForUnbreakable > 0) 
            _timeForUnbreakable -= Time.deltaTime;
        
        if (_playerIsKilled)
        {
            if (_timeToAppear > 0)
                _timeToAppear -= Time.deltaTime;
            
            if (Field.HasTraceTiles()) return;
            
            SetUpNewPlayer();
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

        if (_isFloating && Field.GetTileType(_currentTilePositionInt) == TileType.Empty)
        {
            KillPlayer();
            return;
        }

        if (slitherEnemies.Any(e => (transform.position - e.transform.position).magnitude <= 1.3f) && _timeForUnbreakable <= 0)
            KillPlayer();
        
        var position = transform.position + (Vector3) _currentDirection * 
            (Time.deltaTime * IcwGame.GameSpeed * PlayerSpeed); 
        transform.SetPositionAndRotation(position, Quaternion.identity);
    }

    public void KillPlayer()
    {
        if (_playerIsKilled) return;

        _playerIsKilled = true;
        _isFloating = false;
        _timeToAppear = DefaultTimeToAppear;
        StopPlayer();
        IcwGame.Lives--;
        _playerAnimation.Rewind();
        _playerAnimation.clip = _playerDisappear;
        _playerAnimation.Play();
    }
    
    private void SetUpNewPlayer()
    {
        transform.SetPositionAndRotation(_lastGroundPosition, Quaternion.identity);
        _currentTilePosition = _lastGroundPosition;
        _lastGroundPosition = _startPosition;
        StopPlayer();
        _playerAnimation.Rewind();
        _playerAnimation.clip = _playerAppear;
        _playerAnimation.Play();
        _playerIsKilled = false;
        _isFloating = false;
        _timeForUnbreakable = 2 / (IcwGame.GameSpeed * slitherEnemies.FirstOrDefault()?.EnemySpeed ?? 100);
    }
    
    
    private void PlayerReachNewTile(Vector3 newPosition, Vector3 oldPosition)
    {
        var newFieldPos = new Vector2Int(Mathf.RoundToInt(newPosition.x), Mathf.RoundToInt(newPosition.y));
        _currentTilePositionInt = newFieldPos;
        
        var newTile = Field.GetTileType(newFieldPos);
        var nextNewTile = Field.GetTileType(newFieldPos + Vector2Int.RoundToInt(_direction));

        //stop before going from ground
        if (nextNewTile == TileType.Empty && newTile.IsGround())
        {
            StopPlayer();
        }
        
        // step from ground
        if (!_isFloating && newTile == TileType.Empty)
        {
            _lastGroundPosition = oldPosition;
            _isFloating = true;
        }
        
        // return to ground
        if (_isFloating && newTile.IsGround())
        {
            _isFloating = false;
            _lastGroundPosition = _startPosition;
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
}
