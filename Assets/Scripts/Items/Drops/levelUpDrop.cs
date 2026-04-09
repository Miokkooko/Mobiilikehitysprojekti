using UnityEngine;

public class levelUpDrop : Drop
{
    public override void OnGrab(Player player)
    {
        GameManager.Instance.TriggerReward();
    }
}
