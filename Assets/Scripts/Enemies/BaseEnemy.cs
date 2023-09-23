using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BaseEnemy : MonoBehaviour
{
    [NonSerialized] public float EnemySpeed = IcwGame.GameSpeed;
    [NonSerialized] public Field Field;
    protected Vector3 Direction;
    public float timetolive = 10000;
    public float agro = 0;
    public bool saveMyParameters = false;
    
    protected virtual void Start()
    {
        if (!saveMyParameters)
        {
            transform.localPosition = new(Random.Range(3, IcwGame.SizeX - 3), Random.Range(3, IcwGame.SizeY - 3));

            do
            {
                Direction = Random.insideUnitCircle.normalized;
            } while (Mathf.Abs(Direction.x) < 0.3f || Mathf.Abs(Direction.y) < 0.3f);
        }
    }

    protected void Update()
    {
        var pos = transform.position;
        
        var reflectionNormalVector = Vector3.zero;
        
        var tile = Field.GetTile(Mathf.RoundToInt(pos.x + Mathf.Clamp(Direction.x * 100, -0.5f, 0.5f)), 
            Mathf.RoundToInt(pos.y));
        
        if (tile != TileType.Empty) 
            reflectionNormalVector += new Vector3(Direction.x, 0, 0);

        tile = Field.GetTile(Mathf.RoundToInt(pos.x), 
            Mathf.RoundToInt(pos.y + Mathf.Clamp(Direction.y * 100, -0.5f, 0.5f)));
        
        if (tile != TileType.Empty) 
            reflectionNormalVector += new Vector3(0, Direction.y, 0);
        
        if (reflectionNormalVector.magnitude > 0)
            Direction = Vector3.Reflect(Direction, reflectionNormalVector.normalized).normalized;

        transform.SetPositionAndRotation(transform.position + Direction * (EnemySpeed * Time.deltaTime), 
                Quaternion.identity);
    }
    
}
