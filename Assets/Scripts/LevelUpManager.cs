using System.Collections.Generic;
using UnityEngine;

public class LevelUpManager : MonoBehaviour
{
    public static LevelUpManager Instance;
    public LevelUpData data;
    public List<StatusEffect> StatusUpgrades;
    public Player player;

    NotificationBase n;
    //public List<Weapon> WeaponUpgrades; Mahdollista tehä vasta sitten kun weapons scriptable objecti tehty.

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
            //SelectUpgrade(args.);
        }
    }

    public void TriggerLevelUp()
    {
        List<StatusEffect> choises = GetRandomUpgrades(3);
        data.upgradeList = choises;

        n = UIManager.Instance.CreateNotification(data);

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

    private List<StatusEffect> GetRandomUpgrades(int count)
    {
        if (StatusUpgrades == null || StatusUpgrades.Count == 0)
        {
            return new List<StatusEffect>();
        }

        List<StatusEffect> pool = new List<StatusEffect>(StatusUpgrades);
        List<StatusEffect> selected = new List<StatusEffect>();

        for (int i = 0; i < count; i++)
        {
            if (pool.Count == 0) break;

            int randomIndex = Random.Range(0, pool.Count);
            selected.Add(pool[randomIndex]);
            pool.RemoveAt(randomIndex);
        }

        return selected;
    }

    public void SelectUpgrade(StatusEffect chosenUpgrade)
    {
        if (chosenUpgrade == null) return;

        // Lisää pelaajalle, pelaaja statti systeemi jne
       
    }
}
