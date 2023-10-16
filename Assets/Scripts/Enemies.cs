using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemies : MonoBehaviour
{
    public List<GameObject> enemiesPrefabs = new ();
    [NonSerialized] public Field Field;
    private GameObject _player;
    private int _slitherCount = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        if (Field.FieldMeta?.Enemies?.Any() == true)
        {
            foreach (var enemyType in Field.FieldMeta.Enemies)
                AddEnemy(enemyType);
        }
        else
        {
            AddEnemy(EnemyType.BaseEnemy);
            AddEnemy(EnemyType.BaseEnemy);
            AddEnemy(EnemyType.BaseEnemy);
            AddEnemy(EnemyType.Destroyer);
            AddEnemy(EnemyType.Destroyer);
            AddEnemy(EnemyType.SuperDestroyer);
            AddEnemy(EnemyType.Slither);
            AddEnemy(EnemyType.Slither);
        }
    }

    public void SetPlayer(GameObject player)
    {
        _player = player;
        for (var i = 0; i < transform.childCount; i++)
        {
            var enemyObject = transform.GetChild(i);
            var enemy = enemyObject.GetComponent<BaseEnemy>();
            enemy.Player = _player;
        }    
    }
    
    private GameObject GetEnemyByType(EnemyType enemyType)
        => enemyType switch
        {
            EnemyType.BaseEnemy => enemiesPrefabs[0],
            EnemyType.Destroyer => enemiesPrefabs[1],
            EnemyType.SuperDestroyer => enemiesPrefabs[2],
            EnemyType.Slither => enemiesPrefabs[3],
            _ => enemiesPrefabs[0]
        };

    private void AddEnemy(EnemyType enemyType)
    {
        var enemyObject = Instantiate(GetEnemyByType(enemyType), transform);
        var enemy = enemyObject.GetComponent<BaseEnemy>();
        enemy.Field = Field;
        enemy.Player = _player;
        
        if (enemyType == EnemyType.Slither && enemy is SlitherEnemy slitherEnemy)
        {
            slitherEnemy.RotationType = (RotationType)(_slitherCount % 2);
            _slitherCount++;
        }
    }

}
