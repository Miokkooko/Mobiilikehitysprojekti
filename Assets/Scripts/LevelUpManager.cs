using System.Collections.Generic;
using UnityEditor.Rendering.Universal;
using UnityEngine;

public class LevelUpManager : MonoBehaviour
{
    public static LevelUpManager Instance;

    public List<StatusEffect> StatusUpgrades;

    public event System.Action<List<StatusEffect>> OnUpgradeChoisesReady;

    //public List<Weapon> WeaponUpgrades; Mahdollista tehä vasta sitten kun weapons scriptable objecti tehty.

    private void Awake()
    {
        Instance = this;
    }

    public void TriggerLevelUp()
    {
        List<StatusEffect> choises = GetRandomUpgrades(3);

        OnUpgradeChoisesReady?.Invoke(choises);

        Time.timeScale = 0f;
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

        Time.timeScale = 1f;

        //notification kutsu

    }

}
