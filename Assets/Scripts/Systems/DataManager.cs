using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    int kills;
    public int Kills => kills;

    int coins;
    public int Coins => coins;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // important!
        }
        else
        {
            Destroy(gameObject);
        }

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
