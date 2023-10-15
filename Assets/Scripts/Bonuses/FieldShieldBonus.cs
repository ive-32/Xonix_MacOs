public class FieldShieldBonus : BaseBonus
{
    public override void Start()
    {
        BonusType = BonusType.FieldShield;
        base.Start();
    }
    
    public override void OnPickedUp(Player player)
    {
        Player = player;
        Player.TraceTile = TileType.Filled;
        base.OnPickedUp(player);
    }

    public override void OnBonusEndMethod()
    {
        if (Player is not null)
            Player.TraceTile = TileType.Trace;
        base.OnBonusEndMethod();
    }

}