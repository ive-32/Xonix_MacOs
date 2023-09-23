using System;
using Unity.VisualScripting;
using UnityEngine;

public class TraceTile : MonoBehaviour
{
    private Field _field;
    private SpriteRenderer _spriteRenderer;
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void HitByEnemy(Field field)
    {
        _field = field;
        Destroy(this, 1.0f / (IcwGame.GameSpeed * 5.0f));
        _spriteRenderer.color = Color.red;
    }
    
    public void OnDestroy()
    {
        var pos = Vector2Int.RoundToInt(transform.position);
        var neighbours = new[] { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

        foreach (var neighbour in neighbours)
        {
            var neighbourPos = pos + neighbour;
            if (Field.IsPositionValid(neighbourPos) && _field.GetTile(neighbourPos) == TileType.Trace) 
                _field.HitTraceTile(neighbourPos.x, neighbourPos.y);
        }
    }
}