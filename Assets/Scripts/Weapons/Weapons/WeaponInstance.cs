using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WeaponInstance
{
    public List<StatusEffect> OnHitEffects = new List<StatusEffect>();

    protected Player owner;
    public WeaponData data;

    //Weapon stats
    public float lastFireTime;

    public int upgradeRank = 0;
    public bool CanUpgrade => upgradeRank < data.upgradeList.Length;
    #region stats
    float baseDamage = 1;
    public float Damage => statSystem.Calculate(StatType.Damage, baseDamage + owner.Damage);

    float basePiercing = 1;
    public float Piercing => statSystem.Calculate(StatType.Piercing, basePiercing + owner.Piercing);

    float baseProjectileCount = 1;
    public float ProjectileCount => statSystem.Calculate(StatType.ProjectileCount, baseProjectileCount + owner.ProjectileCount);

    float baseFireRate = 1;
    public float Firerate => statSystem.Calculate(StatType.Firerate, baseFireRate);

    float baseProjectileSpeed = 1;
    public float ProjectileSpeed => statSystem.Calculate(StatType.Speed, baseProjectileSpeed);

    float baseAoeDamage = 1f;
    public float AoEDamage => statSystem.Calculate(StatType.AoEDamage, baseAoeDamage);

    float baseAoeRadius = 1f;
    public float AoERadius => statSystem.Calculate(StatType.AoERadius, baseAoeRadius);
    #endregion

    Coroutine fire;
    public StatSystem statSystem = new StatSystem();

    public WeaponInstance(Player owner, WeaponData data)
    {
        this.owner = owner;
        this.data = data;

        baseFireRate = data.firerate;
        baseProjectileCount = data.projectileCount;
        baseDamage = data.baseDamage;
        baseProjectileSpeed = data.projectileSpeed;
        basePiercing = data.piercing;
        baseAoeDamage = data.aoeDamage;
        baseAoeRadius = data.aoeRadius;

        OnHitEffects = new List<StatusEffect>();

        OnHitEffects.AddRange(owner.OnHitEffects);
        OnHitEffects.AddRange(data.effectList);
    }

    public void UpgradeWeapon()
    {
        if (!CanUpgrade)
            return;

        statSystem.AddModifier(data.upgradeList[upgradeRank]); 
        upgradeRank++;
    }

    public void TryFire()
    {
        if (Time.time >= lastFireTime + Firerate) 
        {
            if (data.usesProjectileCount)
            {
                if (fire != null)
                    owner.StopCoroutine(fire);

                fire = owner.StartCoroutine(FireProjectiles());
            }
            else
                Fire();

            lastFireTime = Time.time;
        }
    }

    //------ Jos jätän tähän tämmöisen käyttäytymisen defaultiksi kun aika moni ase saattais käyttää tätä? ----------
    public virtual void Fire()
    {

        //hae viimeisimmän inputin suunta + luo projectile + anna projectilelle viimeisimmän inputin suunta
        Vector3 dir = owner.GetComponent<PlayerMovement>().GetMoveDirection();
        Transform playerPos = owner.GetComponent<Transform>();

        GameObject proj = Object.Instantiate(data.projectilePrefab, owner.transform.position, Quaternion.identity);

        if(proj.GetComponent<Projectile>() is Projectile p)
        {
            p.Initialize(this, owner, dir);
        }
    }


    IEnumerator FireProjectiles()
    {
        for (int i = 0; i < ProjectileCount; i++)
        {
            Fire();
            yield return new WaitForSeconds(0.1f);
        }
    }

    public string GetRankUpDescription()
    {
        if ((upgradeRank) >= data.upgradeList.Length)
            return "";

        StatModifier nextUpgrade = data.upgradeList[upgradeRank];
        string statName = nextUpgrade.Stat.ToString();
        
        float currentValue = 0;

        switch (nextUpgrade.Stat)
        {
            case StatType.Damage:
                currentValue = Damage;
                break;
            case StatType.Piercing:
                currentValue = Piercing;
                break;
            case StatType.ProjectileCount:
                currentValue = ProjectileCount;
                break;
            case StatType.Firerate:
                currentValue = Firerate;
                break;
            case StatType.AoERadius:
                currentValue = AoERadius;
                break;
            case StatType.AoEDamage:
                currentValue = AoEDamage;
                break;
            default:
                break;
        }

        float nextValue = nextUpgrade.Type == ModifierType.Percent ? currentValue + (currentValue * nextUpgrade.Value) : currentValue + nextUpgrade.Value;

        return $"{statName} {currentValue} -> {nextValue}";
    }

    public string GetRankUpText()
    {
        return $"{upgradeRank + 1} > {upgradeRank + 2}";
    }
}
