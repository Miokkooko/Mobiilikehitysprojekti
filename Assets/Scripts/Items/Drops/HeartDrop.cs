using UnityEngine;

public class HeartDrop : Drop
{
    public override void OnGrab(Player player)
    {
        Debug.Log("Heart Got!");
        player.Heal(new HealContext(player, health));
    }
}
