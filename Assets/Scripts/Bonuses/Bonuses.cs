using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Bonuses : MonoBehaviour
{
    [FormerlySerializedAs("bonusesPrefabsList")] 
    public List<GameObject> BonusesPrefabs = new();

    [FormerlySerializedAs("Progress")] public GameObject progressPrefab;

    [NonSerialized] public GameObject SplashTextPrefab;
    [NonSerialized] public Field Field;
    
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

            var attempts = 5;
            var position = Vector2Int.zero;
            do
            {
                position.x = Random.Range(2, IcwGame.SizeX - 1);
                position.y = Random.Range(2, IcwGame.SizeY - 1);
                attempts--;
            } while (Field.GetTileType(position).IsGround() &&  attempts > 0);
            
            if (attempts == 0) return;
            
            var bonus = Instantiate(BonusesPrefabs[bonusPrefabIndex], transform);
            bonus.transform.localPosition = position.ToVector3();
            
            var bonusClass = bonus.GetComponent<IBonus>(); 
            bonusClass.SplashTextPrefab = SplashTextPrefab;
            bonusClass.ProgressPrefab = progressPrefab;
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

    private void SetTimeToNextBonus() => _timeToNextBonus = Random.Range(3.0f, 10.0f);

    private static BonusType MapToBonusType(int index) => index switch
        {
            0 => BonusType.BorderShield,
            1 => BonusType.FieldShield,
            2 => BonusType.SpeedUp,
            _ => BonusType.Empty
        };

}