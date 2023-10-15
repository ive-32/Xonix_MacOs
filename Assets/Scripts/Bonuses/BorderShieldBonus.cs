public class BorderShieldBonus : BaseBonus
{
    public override void Start()
    {
        BonusType = BonusType.BorderShield;
        base.Start();
    }
    
    public override void OnPickedUp(Player player)
    {
        Player = player;
        Player.TraceTile = TileType.Border;
        base.OnPickedUp(player);
    }

    public override void OnBonusEndMethod()
    {
        if (Player is not null)
            Player.TraceTile = TileType.Trace;
        base.OnBonusEndMethod();
    }

}