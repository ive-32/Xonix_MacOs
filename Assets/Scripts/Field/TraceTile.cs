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
        var pos = Vector2Int.RoundToInt(transform.position);
        _field.PutTile(TileType.Empty, pos.x, pos.y, 1.0f / (IcwGame.GameSpeed * 8.0f));
        _spriteRenderer.color = Color.red;
    }
    
    public void OnDestroy()
    {
        if (_field is null) return;
        
        var pos = Vector2Int.RoundToInt(transform.position);
        var neighbours = new[] { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

        foreach (var neighbour in neighbours)
        {
            var neighbourPos = pos + neighbour;
            if (Field.IsPositionValid(neighbourPos) && _field.GetTileType(neighbourPos) == TileType.Trace) 
                _field.HitTraceTile(neighbourPos.x, neighbourPos.y);
        }
    }
}
