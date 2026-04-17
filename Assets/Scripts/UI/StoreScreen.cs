using TMPro;
using UnityEngine;

public class StoreScreen : MonoBehaviour
{
    public TMP_Text CoinsText;

    public PullAnimator pullAnimator;

    public Banner[] banners;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CoinsText.text = DataManager.Instance.Coins.ToString() + " G";
    }

    public void Pull(Banner banner)
    {
        pullAnimator.StartAnimation(banner.Pull());
    }

    public void PullFive(Banner banner)
    {
        Rarity highest = Rarity.Common;
        Rarity latest;
        for (int i = 0; i < 5; i++)
        {
            latest = banner.Pull();
            if (latest > highest)
                highest = latest;
        }

        pullAnimator.StartAnimation(highest);
    }
   
 
    public void OnPurchaseCharacter(int cost)
    {
        if(DataManager.Instance.SpendCoins(cost))
        {
            Debug.Log("Character bought!");
            UpdateUI();
        }
        else
        {
            Debug.Log("Not enough money!");
        }
    }

    public void OnPurchasePerk(int cost)
    {
        if (DataManager.Instance.SpendCoins(cost))
        {
            Debug.Log("Perk bought!");
            UpdateUI();
        }
        else
        {
            Debug.Log("Not enough money!");
        }
    }

    public void UpdateUI()
    {
        CoinsText.text = DataManager.Instance.Coins.ToString() + " G";
    }
}
