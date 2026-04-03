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

    //Jono jos on monta upgradea tulossa

    private Queue<List<object>> levelUpQueue = new Queue<List<object>>();
    private bool isLevelUpActive = false;


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

        // Katotaan onko lis‰‰ leveluppeja tulossa
        isLevelUpActive = false;
        ProcessNextLevelUp();
    }

    public void TriggerLevelUp()
    {
        List<object> choices = GetRandomMixedUpgrades(3);

        // Jos on jo yks leveluppi -> seuraava jonoon
        if (isLevelUpActive)
        {
            levelUpQueue.Enqueue(choices);
            Debug.Log("Level up queued. Queue count: " + levelUpQueue.Count);
            return;
        }

        ShowLevelUp(choices);
    }

    private void ShowLevelUp(List<object> choices)
    {
        isLevelUpActive = true;
        data.upgradeList = choices;
        Time.timeScale = 0f;

        n = UIManager.Instance.CreateNotification(data);

        n.OnNotificationRaised += Notification_OnNotificationResult;
        n.OnNotificationDestroyed += N_OnNotificationDestroyed;
    }


    private void ProcessNextLevelUp()
    {
        if (levelUpQueue.Count > 0)
        {
            var nextChoices = levelUpQueue.Dequeue();
            ShowLevelUp(nextChoices);
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    private void N_OnNotificationDestroyed(object sender, NotificationBase.NotificationArgs e)
    {
        n = (NotificationBase)sender;
        n.OnNotificationDestroyed -= N_OnNotificationDestroyed;

        //jos notificaatio tuhoutuu tuhotaan que
        if (!isLevelUpActive && levelUpQueue.Count == 0)
        {
            Time.timeScale = 1f;
        }
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
     
        if (player.GetWeapon(weapon) == null)
            return true;

       
        int currentLevel = player.GetWeapon(weapon).upgradeRank;

        return currentLevel < weapon.upgradeList.Length;
    }
    public WeaponInstance GetWeapon(WeaponData weapon)
    {
        return player.GetWeapon(weapon);
    }

    // Lis‰‰ upgradet
    public void ApplyUpgrade(object chosenUpgrade)
    {
        if (chosenUpgrade == null) return;

        switch (chosenUpgrade)
        {
            case StatusEffect statusEffect:
                ApplyStatusEffect(statusEffect);
                break;

            case WeaponData weapon:
                ApplyWeaponUpgrade(weapon);
                break;
        }

        Time.timeScale = 1f;
    }

    private void ApplyStatusEffect(StatusEffect effect)
    {
        if (effect == null) return;

        Unit.ApplyStatusEffect(effect, player);
    }

    private void ApplyWeaponUpgrade(WeaponData weapon)
    {
        if (player.GetWeapon(weapon) == null)
        {
            // Uus ase
            player.AddWeapon(weapon);
            Debug.Log($"New Weapon: {weapon.weaponName}!");
        }
        else
        {
            // P‰ivitys vanhalle aseelle
            player.UpgradeWeapon(weapon);
            Debug.Log($"Upgraded {weapon.weaponName}!");
        }
    }
}
