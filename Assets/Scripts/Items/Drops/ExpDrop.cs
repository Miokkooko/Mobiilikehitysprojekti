using UnityEngine;

public class ExpDrop : Drop
{
    public override void OnGrab(Player player)
    {
        Debug.Log("XP got!");
    }
}
