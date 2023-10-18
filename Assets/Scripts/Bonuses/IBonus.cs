using UnityEngine;

public interface IBonus
{
    BonusType BonusType { get; set; }
    public GameObject SplashTextPrefab { get; set; }
    public GameObject ProgressPrefab { get; set; }
    
    void OnPickedUp(Player player);
}