using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance;

    StatusEffect[] perks;
    PlayerData selectedData;
    public PlayerData CharacterData => selectedData;
    public int Coins;


    private void Awake()
    {
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
}
