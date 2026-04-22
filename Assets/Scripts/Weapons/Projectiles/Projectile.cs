using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public List<StatusEffect> OnHitEffects = new List<StatusEffect>();

    [Header("ProjectileStats")]
    protected float projectileSpeed = 5f;
    protected float projectilePiercing = 1f;
    protected float damage = 1f;
    protected float projectileLifetime = 2f;

    protected float aoeRadius = 1f;

    [Header("Projectiles")]
    public bool enableParticles;
    public GameObject hitParticles;

    //Movement
    protected Vector3 direction;
    protected Unit owner;
    protected WeaponData weaponData;
    protected WeaponInstance weaponInstance;
    public ProjectilePoolType AoEFallBack = ProjectilePoolType.None;
    public ProjectilePoolType PoolType => weaponData.poolType;

    protected Transform ownerPos => owner.transform;
    protected float angle;

    DetectionRadius detRadius;
    public List<Enemy> _enemies;

    public virtual void Start()
    {
        if (hitParticles == null && enableParticles)
        {
            hitParticles = Resources.Load<GameObject>("Particles/HitParticles");
        }
    }

    public virtual void Update()
    {
        Move();
        Rotate();
    }

    #region Movement and direction
    public virtual void Move()
    {
        transform.position += direction * projectileSpeed * Time.deltaTime;
    }

    public virtual void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(DestroyAfterdelay(projectileLifetime));
    }
    public virtual void Disable()
    {
        PoolManager.Instance.DisableProjectile(AoEFallBack != ProjectilePoolType.None ? AoEFallBack : PoolType, gameObject);
    }
   
    public virtual void Initialize(WeaponInstance w, Unit p, Vector3 dir)
    {
        weaponInstance = w;
        weaponData = w.data;
        damage = w.Damage;
        projectilePiercing = w.Piercing;
        projectileSpeed = w.ProjectileSpeed;
        direction = dir.normalized;
        owner = p;
        projectileLifetime = w.data.projectileLifeTime;
        aoeRadius = w.AoERadius;

        OnHitEffects = w.OnHitEffects;

        if (owner is Player player)
        {
            detRadius = player.GetComponentInChildren<DetectionRadius>();
            _enemies = detRadius._enemies;
        }

        gameObject.SetActive(true);
    }

    public virtual void Initialize(WeaponData w, Unit p, Vector3 dir)
    {
        weaponData = w;
        damage = w.baseDamage;
        projectilePiercing = w.piercing;
        projectileSpeed = w.projectileSpeed;
        direction = dir.normalized;
        owner = p;
        projectileLifetime = w.projectileLifeTime;
        aoeRadius = w.aoeRadius;

        if (owner is Player player)
        {
            detRadius = player.GetComponentInChildren<DetectionRadius>();
            _enemies = detRadius._enemies;
        }

        gameObject.SetActive(true);
    }

    public virtual void Rotate()
    {
        //käännä projectile pelaajan viimeisimpään kävelysuuntaan
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    #endregion

    #region Collision
    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            return;

        if (collision.GetComponent<IDamageable>() is IDamageable d)
        {
            Unit.DealDamage(new DamageContext(owner, d, damage));
            OnHit();    
        }

        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            if (OnHitEffects != null)
            {
                foreach (var effect in OnHitEffects)
                {
                    if (effect is KnockBack kb)
                    {
                        Vector2 dir = (enemy.transform.position - owner.transform.position).normalized;
                        enemy.ApplyKnockback(dir, kb.force, kb.duration);
                    } // if effect is KnockBack

                    else
                    {
                        Unit.ApplyStatusEffect(effect, enemy);
                    }
                }
            }
        }

    }



    public virtual void OnHit()
    {
        if (hitParticles != null)
        {
            Instantiate(hitParticles, gameObject.transform.position, Quaternion.identity);
        }


        if (projectilePiercing != 1)
        {
            projectilePiercing -= 1;
        }
        else
        {
            StopAllCoroutines();
            Disable();
        }
    }

    public virtual void SpawnAoE()
    {
        GameObject proj = PoolManager.Instance.SpawnProjectile(ProjectilePoolType.Projectile_AoE, transform.position);
   
        Projectile aoe = proj.GetComponent<AoE>();

        aoe.Initialize(weaponInstance, owner, Vector3.zero);
        proj.SetActive(true);
    }

    #endregion

    public virtual Enemy GetRandomEnemy()
    {
        if (_enemies.Count == 0)
        {
            return null;
        }
        int random = Random.Range(0, _enemies.Count);
        return _enemies[random];
    }

    public Enemy GetRandomEnemiesCheck(Player player)
    {
        detRadius = player.GetComponentInChildren<DetectionRadius>();
        _enemies = detRadius._enemies;

        return GetRandomEnemy();
    }

    public IEnumerator DestroyAfterdelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Disable();
    }
}
