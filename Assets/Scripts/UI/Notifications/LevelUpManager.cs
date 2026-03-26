using System.Collections.Generic;
using UnityEngine;

public class LevelUpManager : MonoBehaviour
{
    public static LevelUpManager Instance;
    public LevelUpData data;
    public List<StatusEffect> StatusUpgrades;
    public List<WeaponData> WeaponUpgrades;
    public Player player;

    NotificationBase n;

    //public List<Weapon> WeaponUpgrades; Mahdollista teh‰ vasta sitten kun weapons scriptable objecti tehty.

    //Mit‰ pelaajalla on jo
    public List<WeaponData> acquiredWeapons = new List<WeaponData>();

    private void Awake()
    {
        Instance = this;
    }
    private void Notification_OnNotificationResult(object sender, NotificationBase.NotificationArgs e)
    {
        n = (NotificationBase)sender;
        n.OnNotificationRaised -= Notification_OnNotificationResult;

        if (e is LevelUpArgs args)
        {
            ApplyUpgrade(args.upgradeChosen);
        }
    }

    public void TriggerLevelUp()
    {
        List<object> choices = GetRandomMixedUpgrades(3);

        // Notificationille dataa
        n = UIManager.Instance.CreateNotification(data);

        if (n is LevelUpNotification lvlNotif)
        {
            lvlNotif.SetUpgradeOptions(choices);
        }

        n.OnNotificationRaised += Notification_OnNotificationResult;
        n.OnNotificationDestroyed += N_OnNotificationDestroyed;

        Time.timeScale = 0f;
    }

    private void N_OnNotificationDestroyed(object sender, NotificationBase.NotificationArgs e)
    {
        n = (NotificationBase)sender;
        n.OnNotificationDestroyed -= N_OnNotificationDestroyed;
        Time.timeScale = 1f;
    }


    private List<object> GetRandomMixedUpgrades(int count)
    {
        List<object> pool = new List<object>();

        // Lis‰t‰‰n aseet
        foreach (var weapon in WeaponUpgrades)
        {
            if (CanShowWeapon(weapon))
                pool.Add(weapon);
        }

        foreach (var status in StatusUpgrades)
        {
            // katotaan onko maxxed
            pool.Add(status);
        }

        List<object> selected = new List<object>();

        //Valitaan 3 p‰ivityst‰ randomilla
        for (int i = 0; i < count && pool.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, pool.Count);
            selected.Add(pool[randomIndex]);
            pool.RemoveAt(randomIndex);
        }

        return selected;
    }

    private bool CanShowWeapon(WeaponData weapon)
    {
        // If player doesn't have it, show as "New Weapon"
        if (!acquiredWeapons.Contains(weapon))
            return true;

        // If player has it, check if it's maxed out (you'll need maxLevel in WeaponData)
        int currentLevel = GetWeaponLevel(weapon);
        return currentLevel < weapon.maxLevel;
    }

    // Lis‰‰ upgradet
    public void ApplyUpgrade(object chosenUpgrade)
    {
        if (chosenUpgrade == null) return;

        switch (chosenUpgrade)
        {
            case ModifierStatusEffect modEffect:
                ApplyPermanentModifiers(modEffect);
                break;

            case StatusEffect statusEffect:
                ApplyStatusEffect(statusEffect);
                break;

            case WeaponData weapon:
                ApplyWeaponUpgrade(weapon);
                break;
        }

        Time.timeScale = 1f;
    }

    // Pysyvi‰ muuttujia pelaajalle
    private void ApplyPermanentModifiers(ModifierStatusEffect modEffect)
    {
        if (modEffect.Modifiers == null) return;

        foreach (var modifier in modEffect.Modifiers)
        {
            float finalValue = modifier.Value;
            if (modifier.Type == ModifierType.Percent)
            {
                finalValue = GetCurrentStatValue(modifier.Stat) * (modifier.Value / 100f);
            }
            ApplyStatToPlayer(modifier.Stat, finalValue);
        }
        Debug.Log($"Applied: {modEffect.Name}");
    }

    private void ApplyStatusEffect(StatusEffect effect)
    {
        Debug.Log($"Applied status: {effect.Name}");
    }

    private void ApplyWeaponUpgrade(WeaponData weapon)
    {
        if (!acquiredWeapons.Contains(weapon))
        {
            // Uus ase
            acquiredWeapons.Add(weapon);
            player.AddWeapon(weapon); // You need this method in Player.cs
            Debug.Log($"New Weapon: {weapon.weaponName}!");
        }
        else
        {
            // P‰ivitys vanhalle aseelle
            int newLevel = GetWeaponLevel(weapon) + 1;
            player.UpgradeWeapon(weapon, newLevel); // You need this method in Player.cs
            Debug.Log($"Upgraded {weapon.weaponName} to Level {newLevel}!");
        }
    }


    // Hankitaan t‰m‰nhetkiset statit prosentteja varten
    private float GetCurrentStatValue(StatType stat)
    {
        switch (stat)
        {
            case StatType.Damage:
                return player.damage;
            case StatType.Speed:
                return player.moveSpeed;
            case StatType.MaxHealth:
                return player.maxHealth;
            default:
                return 0f;
        }
    }


    private void ApplyStatToPlayer(StatType stat, float value)
    {
        switch (stat)
        {
            case StatType.Damage:
                player.damage += value;
                break;
            case StatType.Speed:
                player.moveSpeed += value;
                break;
            case StatType.MaxHealth:
                player.maxHealth += value;
                player.currentHealth += value;
                break;
        }
    }

    private int GetWeaponLevel(WeaponData weapon)
    {
        // Simple tracking - you might want a Dictionary<WeaponData, int> for proper leveling
        return acquiredWeapons.Contains(weapon) ? 1 : 0; // Placeholder
    }


    public void SelectUpgrade(StatusEffect chosenUpgrade)
    {
        ApplyUpgrade(chosenUpgrade);
    }
}
