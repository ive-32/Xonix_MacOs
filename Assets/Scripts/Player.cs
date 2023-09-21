using UnityEngine;

public class Player : MonoBehaviour
{
    private readonly Vector3 _startPosition = new (Mathf.Round(IcwGame.SizeX / 2.0f), 1, 0);
    
    private Vector2 _direction = Vector2.zero;

    public float playerSpeed = 2.0f;
    
    private void Awake()
    {
        transform.SetPositionAndRotation(_startPosition, Quaternion.identity);
    }

    void Update()
    {
        _direction = GetDirection();
        var position = transform.position + (Vector3) _direction * (Time.deltaTime * IcwGame.GameSpeed * playerSpeed); 
        transform.SetPositionAndRotation(position, Quaternion.identity);
    }

    private Vector2 GetDirection()
    {
        var direction = _direction;
        
        var x = Input.GetAxisRaw("Horizontal");
        var y = Input.GetAxisRaw("Vertical");
        
        if (x != 0 || y != 0)
            direction = Mathf.Abs(x) > Mathf.Abs(y) ? new Vector2(x, 0).normalized : new Vector2(0, y).normalized;
       
        if (!PositionIsValid(transform.position.x + direction.x, transform.position.y + direction.y))
            direction = Vector2.zero; 
        
        return direction;
    }
    
    private static bool PositionIsValid(float x, float y) 
        => Mathf.Round(x) is >= 0 and < IcwGame.SizeX && Mathf.Round(y) is >= 0 and < IcwGame.SizeY;

}
