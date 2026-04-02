using UnityEngine;

public class ExpDrop : Drop
{
    public override void OnGrab(Player player)
    {
        
        player.IncreaseExp(expAmount);
    }


}
