using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    PerkData[] perks;
    PlayerData selectedData;

    public PerkData[] selectedPerks => perks;
    public PlayerData CharacterData => selectedData;

    int kills;
    public int Kills => kills;

    int coins;
    public int Coins => coins;

    // Apuluokka, jotta saadaan data ja rankki samaan pakettiin
    [System.Serializable]
    public class PurchasedPerk
    {
        public PerkData data;
        public int currentRank;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SelectPlayerData(PlayerData data)
    {
        selectedData = data;
    }

    public void LoadSave()
    {

    }

    public void SaveData()
    {

    }


    public void AddCoins(int amount)
    {
        coins += amount;
    }

    public bool SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            return true;
        }
        return false;
    }

    public void AddKills(int amount)
    {
        kills += amount;
    }
}
