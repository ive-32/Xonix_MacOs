using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Bonuses : MonoBehaviour
{
    [FormerlySerializedAs("bonusesPrefabsList")] 
    public List<GameObject> BonusesPrefabs = new();
    
    [NonSerialized] public Player Player;
    [NonSerialized] public Field Field;
    [NonSerialized] public Enemies Enemies;

    private float _timeToNextBonus = 1.0f;
    
    public void Update()
    {
        _timeToNextBonus -= Time.deltaTime;
        if (_timeToNextBonus <= 0)
        {
            SetTimeToNextBonus();
            
            var bonusPrefabIndex = Random.Range(0, BonusesPrefabs.Count);
            var newBonusType = MapToBonusType(bonusPrefabIndex);

            if (newBonusType is BonusType.Empty)
                return;
            
            var bonuses = new List<IBonus>();
            for(var i = 0; i < transform.childCount; i++)
                bonuses.Add(transform.GetChild(i).GetComponent<IBonus>());

            if (bonuses.Any(b => b.BonusType == newBonusType ))
                return;
            
            if (newBonusType is BonusType.BorderShield or BonusType.FieldShield && 
                bonuses.Any(b => b.BonusType is BonusType.BorderShield or BonusType.FieldShield))
                return;
                
            Instantiate(BonusesPrefabs[bonusPrefabIndex], transform);
        }
    }

    public IBonus CheckBonusInTile(Vector3 tile)
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            var bonus = transform.GetChild(i);
            if ((bonus.transform.position - tile).magnitude < 0.5f)
                return bonus.GetComponent<IBonus>();
        }

        return null;
    }

    private void SetTimeToNextBonus() => _timeToNextBonus = 4.0f; //Random.Range(3.0f, 3.0f);

    private BonusType MapToBonusType(int index) => index switch
        {
            0 => BonusType.BorderShield,
            1 => BonusType.FieldShield,
            2 => BonusType.SpeedUp,
            _ => BonusType.Empty
        };

}