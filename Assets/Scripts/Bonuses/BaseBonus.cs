using System;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class BaseBonus : MonoBehaviour, IBonus
{
    protected Player Player { get; set; }

    private float _timeToLive = 5.0f;

    public BonusState BonusState { get; set; }
    public BonusType BonusType { get; set; } = BonusType.Empty;
    private Vector3 _displayPosition = new (IcwGame.SizeX - 1, IcwGame.SizeY, 0);

    public virtual void Start()
    {
        _displayPosition = new Vector3(IcwGame.SizeX - 1 - (int)BonusType, IcwGame.SizeY, 0);
        BonusState = BonusState.BonusOnField;
        transform.localPosition = new Vector3(Random.Range(3, IcwGame.SizeX - 3), Random.Range(3, IcwGame.SizeY - 3)).GetCenterTile();
    }

    public void Update()
    {
        if (_timeToLive <= 0)
        {
            switch(BonusState)
            {
                case BonusState.BonusOnField : Destroy(this.GameObject()); break;
                case BonusState.BonusPickedUp : OnBonusEndMethod(); break;
                default: Destroy(this.GameObject()); break;
            }
        }

        _timeToLive -= Time.deltaTime;
    }
    
    public virtual void OnPickedUp(Player player)
    {
        _timeToLive = 5.0f;   
        BonusState = BonusState.BonusPickedUp;
        transform.localPosition = _displayPosition;
    }

    public virtual void OnBonusEndMethod()
    {
        Destroy(this.GameObject());
    }
    
}