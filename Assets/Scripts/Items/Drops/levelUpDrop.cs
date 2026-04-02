using UnityEngine;

public class levelUpDrop : Drop
{
    public override void OnGrab(Player player)
    {
        LevelUpManager.Instance.TriggerLevelUp();
    }
}
