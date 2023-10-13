using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FieldMeta
{
    public Vector2Int StartPosition { get; set; } = new(Mathf.RoundToInt(IcwGame.SizeX / 2.0f), 1);

    public List<EnemyType> Enemies { get; set; } = new List<EnemyType>();
        //{ EnemyType.BaseEnemy, EnemyType.BaseEnemy, EnemyType.Destroyer, EnemyType.SuperDestroyer };

    public string[] Field { get; set; } = new string[IcwGame.SizeY];
    
}