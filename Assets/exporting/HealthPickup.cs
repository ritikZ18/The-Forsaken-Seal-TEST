public class HealthPickup : BaseOfPickups
{
    public int amtHeal = 1;

    protected override void OnPickup(PlayerHealth player)
    {
        player.Heal(amtHeal);
    }
}
