using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    #region stats

    float baseDamage;
    public float Damage => baseDamage;

    float baseMaxHealth;
    public float Health => baseMaxHealth;

    float baseSpeed;

    public float Speed => statSystem.Calculate(StatType.Speed, baseSpeed); 

    List<Debuff> debuffList;
    public List<Debuff> Debuffs => debuffList;

    List<Buff> buffList;
    public List<Buff> Buffs => buffList;

    StatSystem statSystem;
    #endregion

}
