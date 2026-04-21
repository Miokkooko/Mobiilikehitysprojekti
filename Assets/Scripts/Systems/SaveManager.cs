using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class RunData
{
    public string playerName;
    public int kills;
    public float time;
    public int coinsEarned;
    public List<WeaponType> weaponsSelected;
}
[Serializable]
public class CharacterEntry
{
    public CharacterType type;
    public int value;

    public CharacterEntry(CharacterType _type, int _value)
    {
        type = _type;
        value = _value;
    }
}
[Serializable]
public class SaveData
{
    public int coins;
    public int kills;
    public float totalPlayTime;

    public List<CharacterEntry> charactersOwned;

    public List<RunData> runs;
}

public class SaveManager
{
    static string savePath = Path.Combine(Application.persistentDataPath, "SaveData.json");
    static SaveData saveData;

    public static SaveData GetSaveData()
    {
        if (saveData == null)
            LoadSave();

        return saveData;
    }

    public static CharacterEntry GetCharacterEntry(CharacterType type)
    {
        if (saveData == null)
            LoadSave();

        foreach (var item in saveData.charactersOwned)
        {
            if (item.type == type)
                return item;
        }

        CharacterEntry newChar = new CharacterEntry(type, 0);

        saveData.charactersOwned.Add(newChar);

        return newChar;
    }
    public static List<CharacterEntry> GetCharacterEntries()
    {
        if (saveData == null)
            LoadSave();

        return saveData.charactersOwned;
    }

    public static void DeleteSave()
    {
        saveData = null;

        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Save file deleted.");
        }
        else
        {
            Debug.Log("No save file to delete.");
        }
    }

    public static void AddCharacter(CharacterType character, int amount = 1)
    {
        if (saveData == null)
            LoadSave();

        bool owned = false;

        foreach (var item in saveData.charactersOwned)
        {
            if (item.type == character)
            {
                owned = true;
                item.value = Math.Clamp(item.value + amount, 0, RankManager.MaxCopies);
                break;
            }   
        }

        if (!owned)
        {
            saveData.charactersOwned.Add(new CharacterEntry(character, amount));
        }
    }

    public static void SaveRun(Player player, int kills, float gameTimer, int coins)
    {
        List<WeaponType> weapons = new List<WeaponType>();
        foreach (var weapon in player.GetWeapons)
        {
            weapons.Add(weapon.weaponType);
        }

        RunData data = new RunData()
        {
            playerName = player.playerData.name,
            kills = kills,
            time = gameTimer,
            weaponsSelected = weapons,
            coinsEarned = coins
        };

        if(saveData == null)
            LoadSave();

        saveData.runs.Add(data);
        saveData.coins += coins;
        saveData.kills += kills;
        saveData.totalPlayTime += gameTimer;

        Save();
    }

    public static void Save()
    {
        string newJson = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(savePath, newJson);

        Debug.Log("Game saved to: " + savePath);
    }

    static void LoadSave()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            saveData = JsonUtility.FromJson<SaveData>(json);

            if (saveData.runs == null)
                saveData.runs = new List<RunData>();
        }
        else
        {
            saveData = new SaveData()
            {
                coins = 0,
                charactersOwned = new List<CharacterEntry>(),
                runs = new List<RunData>()
            };

            saveData.charactersOwned.Add(new CharacterEntry(CharacterType.Knight, 1));
            saveData.charactersOwned.Add(new CharacterEntry(CharacterType.Noble, 1));
            saveData.charactersOwned.Add(new CharacterEntry(CharacterType.DarkWitch, 1));
        }
    }
}
