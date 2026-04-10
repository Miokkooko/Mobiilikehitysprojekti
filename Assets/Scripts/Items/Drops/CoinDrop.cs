using UnityEngine;

public class CoinDrop : Drop
{
    public override void OnGrab(Player player)
    {
        GameManager.instance.AddCoins(coinAmount);
        Debug.Log("Coin got: "+GameManager.instance.Coins);
    }

    public void InitializeCoins(int coins)
    {
        coinAmount = coins;
    }
}
