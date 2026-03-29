using System.Collections;
using UnityEngine;



public class WeaponInstance
{

    protected Player owner;
    public WeaponData data;

    //Weapon stats
    public float lastFireTime;

    public int upgradeRank = 0;

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
    }

    public void UpgradeWeapon()
    {
        if (upgradeRank >= data.upgradeList.Length)
            return;

        upgradeRank++;
        statSystem.AddModifier(data.upgradeList[upgradeRank - 1]);
    }

    public void TryFire()
    {
        if (Time.time >= lastFireTime + Firerate) 
        {
            if (fire != null)
                owner.StopCoroutine(fire);

            fire = owner.StartCoroutine(FireProjectiles());
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
}
