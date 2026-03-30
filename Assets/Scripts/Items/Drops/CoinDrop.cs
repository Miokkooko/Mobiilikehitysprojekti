using UnityEngine;

public class CoinDrop : Drop
{
    public override void OnGrab(Player player)
    {
        Debug.Log("Coin got!");
    }
}
