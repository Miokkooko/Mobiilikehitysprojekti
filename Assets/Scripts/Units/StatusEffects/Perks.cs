using UnityEngine;

[CreateAssetMenu(menuName = "Perks/PerkData")]
public class PerkData : ScriptableObject
{
    public string perkName;
    public StatType statToBoost;
    public float baseValue; // Esim. 10 (jos HP)
    public float valuePerRank; // Esim. 5 per taso
    public Sprite icon;

    public float GetTotalValue(int currentRank)
    {
        // Laskee bonuksen rankin mukaan
        return baseValue + (valuePerRank * (currentRank - 1));
    }
}