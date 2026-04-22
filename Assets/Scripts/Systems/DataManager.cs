using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    PerkData[] perks;
    PlayerData selectedData;

    public PerkData[] SelectedPerks => perks;
    public PlayerData CharacterData => selectedData;

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

    public void SelectPerkDatas(PerkData[] datas)
    {
        perks = datas;
    }
}
