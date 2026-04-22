using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


[Serializable]
public class RunData
{
    public CharacterType selectedCharacter;
    public int kills;
    public float time;
    public int coinsEarned;
    public List<WeaponType> weaponsSelected;
    public List<PassiveType> passivesSelected;
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
public class PerkEntry
{
    public PerkType type;
    public int value;

    public PerkEntry(PerkType _type, int _value)
    {
        type = _type;
        value = _value;
    }
}
[Serializable]
public class SaveData
{
    public long lastUpdated;

    public int coins;
    public int kills;
    public float totalPlayTime;

    public List<CharacterEntry> charactersOwned;
    public List<PerkEntry> perksOwned;
}

public class SaveManager
{
    static string savePath = Path.Combine(Application.persistentDataPath, "SaveData.json");
    static SaveData saveData;

    static string userId;
    static DatabaseReference dbRef => FirebaseDatabase.DefaultInstance.RootReference;
    static DatabaseReference userRef;

    public static Action OnSaved;
    public static Action OnSaveLoaded;

    #region firebase
    public static void InitializeFirebase(string uid)
    {
        userId = uid;
        userRef = dbRef.Child("users").Child(userId);

        LoadFromFirebase();
    }

    static void LoadFromFirebase()
    {
        userRef.Child("saveData").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogWarning("Firebase load failed, using local save.");
                LoadSave();
                return;
            }
            
            DataSnapshot snapshot = task.Result;
            

            if (snapshot.Exists)
            {
                SaveData cloudSaveData = new SaveData();
                string json = snapshot.GetRawJsonValue();
                cloudSaveData = JsonUtility.FromJson<SaveData>(json);

                string localJson = "";
                SaveData localSaveData = new SaveData();
               
                if (File.Exists(savePath))
                {
                    localJson = File.ReadAllText(savePath);
                    localSaveData = JsonUtility.FromJson<SaveData>(localJson);
                }

                if (localSaveData.lastUpdated > cloudSaveData.lastUpdated)
                {
                    saveData = localSaveData;
                    Debug.Log("Local SaveData is newer than firebase, updating firebase SaveData");
                    SaveToFirebase();
                }
                else
                {
                    saveData = cloudSaveData;
                    Debug.Log("Loaded save from Firebase.");
                }

                if (saveData.charactersOwned == null)
                    saveData.charactersOwned = new List<CharacterEntry>();
            }
            else
            {
                Debug.Log("No Firebase save found, creating new.");
                LoadSave();
                SaveToFirebase();
            }
        });
    }

    static void SaveToFirebase()
    {
        if (saveData == null || userRef == null)
            return;

        saveData.lastUpdated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        string json = JsonUtility.ToJson(saveData);

        userRef.Child("saveData").SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                    Debug.Log("Saved to Firebase.");
                else
                    Debug.LogError("Firebase save failed.");
            });
    }

    static void DeleteFromFirebase()
    {
        if (userRef == null)
            return;

        userRef.Child("saveData").RemoveValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                    Debug.Log("Deleted from Firebase.");
                else
                    Debug.LogError("Firebase save deletion failed.");
            });
    }

    public static void SaveRunToFirebase(RunData run)
    {
        string runId = dbRef.Child("users").Child(userId).Child("runs").Push().Key;

        dbRef.Child("users")
            .Child(userId)
            .Child("runs")
            .Child(runId)
            .SetRawJsonValueAsync(JsonUtility.ToJson(run));
    }
    #endregion

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
   

    public static PerkEntry GetPerkEntry(PerkType type)
    {
        if (saveData == null)
            LoadSave();

        foreach (var item in saveData.perksOwned)
        {
            if (item.type == type)
                return item;
        }

        PerkEntry newPerk = new PerkEntry(type, 0);

        saveData.perksOwned.Add(newPerk);

        return newPerk;
    }
    public static List<PerkEntry> GetPerkEntries()
    {
        if (saveData == null)
            LoadSave();

        return saveData.perksOwned;
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

    public static void AddPerk(PerkType perk, int amount = 1)
    {
        if (saveData == null)
            LoadSave();

        bool owned = false;

        foreach (var item in saveData.perksOwned)
        {
            if (item.type == perk)
            {
                owned = true;
                item.value = Math.Clamp(item.value + amount, 0, RankManager.MaxCopies);
                break;
            }   
        }

        if (!owned)
        {
            saveData.perksOwned.Add(new PerkEntry(perk, amount));
        }
    }

    public static void SaveRun(Player player, int kills, float gameTimer, int coins)
    {
        List<WeaponType> weapons = new List<WeaponType>();
        foreach (var weapon in player.GetWeapons)
        {
            weapons.Add(weapon.weaponType);
        }

        List<PassiveType> passives = new List<PassiveType>();
        foreach (var passive in player.GetPassives)
        {
            passives.Add(passive.passiveType);
        }

        RunData data = new RunData()
        {
            selectedCharacter = player.playerData.characterType,
            passivesSelected = passives,
            kills = kills,
            time = gameTimer,
            weaponsSelected = weapons,
            coinsEarned = coins
        };

        if(saveData == null)
            LoadSave();

        SaveRunToFirebase(data);

        saveData.coins += coins;
        saveData.kills += kills;
        saveData.totalPlayTime += gameTimer;

        Save();
    }

    public static void Save()
    {
        saveData.lastUpdated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        string newJson = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(savePath, newJson);
        Debug.Log("Game saved to: " + savePath);

        SaveToFirebase(); 
        OnSaved?.Invoke();
    }

    static void LoadSave()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            saveData = JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            saveData = new SaveData()
            {
                coins = 0,
                charactersOwned = new List<CharacterEntry>(),
            };

            saveData.charactersOwned.Add(new CharacterEntry(CharacterType.Knight, 1));
            saveData.charactersOwned.Add(new CharacterEntry(CharacterType.Noble, 1));
            saveData.charactersOwned.Add(new CharacterEntry(CharacterType.DarkWitch, 1));
        }

        OnSaveLoaded?.Invoke();
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

        DeleteFromFirebase();
        LoadSave();
    }
}
