
using System.Collections.Generic;
using UnityEngine;


public class LevelUpManager : MonoBehaviour
{
    public LevelUpData data;
    public List<PassiveData> PassiveUpgrades;
    public List<WeaponData> WeaponUpgrades;

    int queuedNotifications = 0;

    public Player player;

    NotificationBase n;

    private void Notification_OnNotificationResult(object sender, NotificationBase.NotificationArgs e)
    {
        n = (NotificationBase)sender;
        n.OnNotificationRaised -= Notification_OnNotificationResult;

        if (e is LevelUpArgs args)
        {
            ApplyUpgrade(args.upgradeChosen);
        }
    }

    public void TriggerReward()
    {
        List<object> choices = GetRandomMixedUpgrades(3);

        if (choices.Count == 0)
            return;

        data.upgradeList = choices;
        Time.timeScale = 0f;

        n = UIManager.Instance.CreateNotification(data);

        n.OnNotificationRaised += Notification_OnNotificationResult;
        n.OnNotificationDestroyed += N_OnNotificationDestroyed;
        queuedNotifications++;
    }

    private void N_OnNotificationDestroyed(object sender, NotificationBase.NotificationArgs e)
    {
        n = (NotificationBase)sender;
        queuedNotifications--;
        n.OnNotificationDestroyed -= N_OnNotificationDestroyed;

        if(queuedNotifications == 0)
            Time.timeScale = 1f;
    }

    private List<object> GetRandomMixedUpgrades(int count)
    {
        List<object> pool = new List<object>();

        // Lisätään aseet
        foreach (var weapon in WeaponUpgrades)
        {
            if (CanShowWeapon(weapon))
                pool.Add(weapon);
        }

        foreach (var passive in PassiveUpgrades)
        {
            if (CanShowPassive(passive)) 
                pool.Add(passive);
        }

        if (pool.Count == 0)
            return pool;

        List<object> selected = new List<object>();

        //Valitaan 3 päivitystä randomilla
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
        WeaponInstance instance = player.GetWeapon(weapon);

        if (instance != null)
            return instance.CanUpgrade;
        else
            return player.CanGetWeapon;
    }
    private bool CanShowPassive(PassiveData data)
    {
        PassiveInstance instance = player.GetPassive(data);
        if (instance != null)
            return instance.CanUpgrade;
        else
            return player.CanGetPassive;
    }

    // Lisää upgradet
    public void ApplyUpgrade(object chosenUpgrade)
    {
        if (chosenUpgrade == null) return;

        switch (chosenUpgrade)
        {
            case PassiveData passive:
                ApplyPassiveUpgrade(passive);
                break;

            case WeaponData weapon:
                ApplyWeaponUpgrade(weapon);
                break;
        }
    }

    private void ApplyPassiveUpgrade(PassiveData data)
    {
        if (player.GetPassive(data) == null)
        {
            player.AddPassive(data);
        }
        else
        {
            player.UpgradePassive(data); 
        }
    }

    private void ApplyWeaponUpgrade(WeaponData weapon)
    {
        if (player.GetWeapon(weapon) == null)
        {
            player.AddWeapon(weapon); 
        }
        else
        {
            player.UpgradeWeapon(weapon); 
        }
    }
}
