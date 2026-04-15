using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    StatusEffect[] perks;
    PlayerData selectedData;
    public PlayerData CharacterData => selectedData;

    int kills;
    public int Kills => kills;

    int coins;
    public int Coins => coins;

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
