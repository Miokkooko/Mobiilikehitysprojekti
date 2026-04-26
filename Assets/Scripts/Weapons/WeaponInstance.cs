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
    private int timesFired;

    public int upgradeRank = 0;
    public bool CanUpgrade => upgradeRank < data.upgradeList.Length;
    #region stats
    float baseDamage = 1;
    public float Damage => statSystem.Calculate(StatType.Damage, baseDamage + owner.Damage);

    float basePiercing = 1;
    public float Piercing => statSystem.Calculate(StatType.Piercing, basePiercing + owner.Piercing);

    float baseProjectileCount = 1;
    public float ProjectileCount => statSystem.Calculate(StatType.ProjectileCount, baseProjectileCount + owner.ProjectileCount);

    float baseProjectileBurst = 1;
    public float ProjectileBurst => statSystem.Calculate(StatType.ProjectileBurst, baseProjectileBurst + owner.ProjectileBurst);

    float baseProjectileSpread = 1f;

    float baseFirerate = 1f;
    public float FireratePercent => statSystem.Calculate(StatType.FirerateBonus, owner.FireratePercent);
    public float Firerate => Mathf.Max((1 - (FireratePercent - 1)) * baseFirerate, 0.2f);


    float baseProjectileSpeed = 1;
    public float ProjectileSpeed => statSystem.Calculate(StatType.Speed, baseProjectileSpeed);


    float baseAoeRadius = 1f;
    public float AoERadius => statSystem.Calculate(StatType.AoERadius, baseAoeRadius + owner.AoERadius);
    #endregion

    Coroutine fire;
    public StatSystem statSystem = new StatSystem();

    public WeaponInstance(Player owner, WeaponData data)
    {
        if (data == null)
        {
            Debug.LogError("Wtf data is null");
        }

        this.owner = owner;
        this.data = data;

        baseProjectileCount = data.projectileCount;
        baseProjectileSpread = data.projectileSpread;
        baseProjectileBurst = data.projectileBurst;
        baseDamage = data.baseDamage;
        baseProjectileSpeed = data.projectileSpeed;
        basePiercing = data.piercing;
        baseAoeRadius = data.aoeRadius;
        baseFirerate = data.firerate;

        OnHitEffects = new List<StatusEffect>();
        OnHitEffects.AddRange(owner.OnHitEffects);
        OnHitEffects.AddRange(data.effectList);
    }

    public void UpgradeWeapon()
    {
        if (!CanUpgrade)
            return;

        statSystem.AddModifier(new StatModifierInstance(data.upgradeList[upgradeRank]));
        upgradeRank++;
        Debug.Log(AoERadius);
    }

    public void TryFire()
    {
        if (Time.time >= lastFireTime + Firerate)
        {
            if (data.usesProjectileBurst)
            {
                if (fire != null)
                    owner.StopCoroutine(fire);

                fire = owner.StartCoroutine(FireProjectiles());
            }
            else
            {
                lastFireTime = Time.time;
                Fire();
            }
        }
    }

    //------ Jos jätän tähän tämmöisen käyttäytymisen defaultiksi kun aika moni ase saattais käyttää tätä? ----------
    public virtual void Fire()
    {

        //hae viimeisimmän inputin suunta + luo projectile + anna projectilelle viimeisimmän inputin suunta
        Vector3 dir = owner.GetComponent<PlayerMovement>().GetMoveDirection();
        Transform playerPos = owner.GetComponent<Transform>();
        PoolManager manager = PoolManager.Instance;

        if (!data.usesProjectileSpread)
        {
            GameObject proj = manager.SpawnProjectile(data.poolType, owner.transform.position);
            if (proj.GetComponent<Projectile>() is Projectile projectile)
            {
                if (data.poolType == ProjectilePoolType.Projectile_Fireball)
                {
                    if (projectile.GetRandomEnemiesCheck(owner) == null)
                    {
                        manager.DisableProjectile(data.poolType, proj);
                        return;
                    }
                }
                projectile.Initialize(this, owner, dir);
            }
            proj.SetActive(true);
            return;
        }

        float spreadAngle = baseProjectileSpread;
        for (int i = 0; i < ProjectileCount; i++)
        {
            GameObject proj = manager.SpawnProjectile(data.poolType, owner.transform.position);

            if (proj.GetComponent<Projectile>() is Projectile p)
            {
                float angle = 0;

                if (ProjectileCount > 1)
                {
                    float even = i % 2;

                    if (i == 0)
                    {
                        angle = 0;
                    }
                    else
                    {
                        int side;
                        if (i % 2 == 0)
                        {
                            side = 1;
                        }
                        else side = -1;

                        int step = (i + 1) / 2;

                        angle = side * step * spreadAngle;
                    }
                }

                Vector3 rotatedDir = Quaternion.Euler(0, 0, angle) * dir;


                if (data.poolType == ProjectilePoolType.Projectile_Fireball)
                {
                    if (p.GetRandomEnemiesCheck(owner) == null)
                    {
                        manager.DisableProjectile(data.poolType, proj);
                        return;
                    }
                }
                p.Initialize(this, owner, rotatedDir);

                proj.SetActive(true);
            }
        }
    }

    IEnumerator FireProjectiles()
    {
        for (int i = 0; i < ProjectileBurst; i++)
        {
            Fire();
            lastFireTime = Time.time;
            yield return new WaitForSeconds(data.projectileBurstDelay);
        }

    }

    public string GetRankUpDescription()
    {
        if ((upgradeRank) >= data.upgradeList.Length)
            return "";

        StatModifier nextUpgrade = data.upgradeList[upgradeRank];
        string statName = nextUpgrade.Stat.ToString();

        float currentValue = 0;
        bool percentStuff = false;
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
            case StatType.FirerateBonus:
                currentValue = (FireratePercent - 1) * 100;
                percentStuff = true;
                break;
            case StatType.AoERadius:
                currentValue = AoERadius;
                break;
            default:
                break;
        }

        float nextValue = nextUpgrade.Type == ModifierType.Percent ? currentValue + (currentValue * nextUpgrade.Value) : currentValue + (percentStuff ? nextUpgrade.Value * 100 : nextUpgrade.Value);

        return $"{statName} {Mathf.RoundToInt(currentValue)} -> {Mathf.RoundToInt(nextValue)}";
    }

    public string GetRankUpText()
    {
        return $"{upgradeRank + 1} > {upgradeRank + 2}";
    }
}
