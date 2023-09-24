using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BaseEnemy : MonoBehaviour
{
    [NonSerialized] public float EnemySpeed = 2.0f;
    [NonSerialized] public Field Field;
    private Vector3 _direction;
    
    protected virtual void Start()
    {
        transform.localPosition = new(Random.Range(3, IcwGame.SizeX - 3), Random.Range(3, IcwGame.SizeY - 3));

        do
        {
            _direction = Random.insideUnitCircle.normalized;
        } while (Mathf.Abs(_direction.x) < 0.3f || Mathf.Abs(_direction.y) < 0.3f);
    }

    protected void Update()
    {
        var pos = transform.position;
        
        var reflectionNormalVector = Vector3.zero;

        var tilePos = new Vector2Int(
            Mathf.RoundToInt(pos.x + Mathf.Clamp(_direction.x * 100, -0.5f, 0.5f)),
            Mathf.RoundToInt(pos.y));

        var tile = Field.GetTile(tilePos);

        if (tile != TileType.Empty)
        {
            reflectionNormalVector += new Vector3(_direction.x, 0, 0);
            if (tile == TileType.Trace)
                Field.HitTraceTile(tilePos.x, tilePos.y);
        }

        tilePos = new Vector2Int(
            Mathf.RoundToInt(pos.x), 
            Mathf.RoundToInt(pos.y + Mathf.Clamp(_direction.y * 100, -0.5f, 0.5f)));

        tile = Field.GetTile(tilePos); 
        
        if (tile != TileType.Empty)
        {
            reflectionNormalVector += new Vector3(0, _direction.y, 0);
            if (tile == TileType.Trace)
                Field.HitTraceTile(tilePos.x, tilePos.y);
        }   
        
        if (reflectionNormalVector.magnitude > 0)
            _direction = Vector3.Reflect(_direction, reflectionNormalVector.normalized).normalized;

        transform.SetPositionAndRotation(transform.position + _direction * (EnemySpeed * IcwGame.GameSpeed * Time.deltaTime), 
                Quaternion.identity);
    }
    
}
