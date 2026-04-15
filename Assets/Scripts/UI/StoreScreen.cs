using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreScreen : MonoBehaviour
{
    public TMP_Text CoinsText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CoinsText.text = DataManager.Instance.Coins.ToString() + " G";
    }

    // Update is called once per frame
    void Update()
    {
        
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
