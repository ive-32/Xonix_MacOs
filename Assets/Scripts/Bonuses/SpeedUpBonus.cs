public class SpeedUpBonus : BaseBonus
{
    public override void Start()
    {
        BonusType = BonusType.SpeedUp;
        base.Start();
    }
    
    public override void OnPickedUp(Player player)
    {
        Player = player;
        Player.PlayerSpeed = Player.PlayerDefaultSpeed * 2f;
        base.OnPickedUp(player);
    }

    public override void OnBonusEndMethod()
    {
        if (Player is not null)
            Player.PlayerSpeed = Player.PlayerDefaultSpeed;
        base.OnBonusEndMethod();
    }

}