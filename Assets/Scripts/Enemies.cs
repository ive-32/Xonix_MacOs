using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemies : MonoBehaviour
{
    public List<GameObject> enemiesPrefabs = new ();
    [NonSerialized] public Field Field;
    
    // Start is called before the first frame update
    void Start()
    {
        if (Field.FieldMeta?.Enemies?.Any() == true)
        {
            foreach (var enemyType in Field.FieldMeta.Enemies)
            {
                var enemy = Instantiate(GetEnemyByType(enemyType), transform);
                enemy.GetComponent<BaseEnemy>().Field = Field;
            }
        }
        else
        {
            var enemy = Instantiate(enemiesPrefabs[0], transform);
            enemy.GetComponent<BaseEnemy>().Field = Field;
        
            enemy = Instantiate(enemiesPrefabs[1], transform);
            enemy.GetComponent<BaseEnemy>().Field = Field;

            enemy = Instantiate(enemiesPrefabs[2], transform);
            enemy.GetComponent<BaseEnemy>().Field = Field;   
        }
    }

    private GameObject GetEnemyByType(EnemyType enemyType)
        => enemyType switch
        {
            EnemyType.BaseEnemy => enemiesPrefabs[0],
            EnemyType.Destroyer => enemiesPrefabs[1],
            EnemyType.SuperDestroyer => enemiesPrefabs[2],
            _ => enemiesPrefabs[0]
        };


}
