public interface IBonus
{
    BonusType BonusType { get; set; }

    void OnPickedUp(Player player);
}