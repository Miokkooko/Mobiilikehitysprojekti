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

    protected float aoeDamage = 1f;
    protected float aoeRadius = 1f;

    [Header("Projectiles")]
    public bool enableParticles;
    public GameObject hitParticles;

    //Movement
    protected Vector3 direction;
    protected Player player;
    protected Enemy enemy;
    protected Transform playerPos => player.transform;
    protected float angle;

    DetectionRadius detRadius;
    public List<Enemy> _enemies;

    public virtual void Start()
    {

        //Projectile tuhoaa ittensä kahen sekunnin jälkeen
        if (hitParticles == null && enableParticles)
        {
            hitParticles = Resources.Load<GameObject>("Particles/HitParticles");
        }
        Destroy(gameObject, projectileLifetime);
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

    public virtual void Initialize(WeaponInstance w, Player p, Vector3 dir)
    {
        damage = w.Damage;
        projectilePiercing = w.Piercing;
        projectileSpeed = w.ProjectileSpeed;
        direction = dir.normalized;
        player = p;
        projectileLifetime = w.data.projectileLifeTime;
        aoeDamage = w.data.aoeDamage;
        aoeRadius = w.data.aoeRadius;

        OnHitEffects = w.OnHitEffects;

        detRadius = player.GetComponentInChildren<DetectionRadius>();
        _enemies = detRadius._enemies;
    }



    public virtual void InitializeAoE(Player p, float d, float r)
    {
        player = p;
        aoeDamage = d;
        aoeRadius = r;
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
        if (collision.tag == "Player")
            return;

        if (collision.GetComponent<IDamageable>() is IDamageable d)
        {
            Unit.DealDamage(new DamageContext(player, d, damage));
            OnHitParticles();
        }

        if (collision.tag == "Enemy")
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            if (OnHitEffects != null)
            {
                foreach (var effect in OnHitEffects)
                {
                    Unit.ApplyStatusEffect(effect, enemy);
                }
            }
        }

    }



    public virtual void OnHitParticles()
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
            Destroy(gameObject);
        }
    }

    public virtual void SpawnAoE()
    {
        GameObject proj = Instantiate(Resources.Load<GameObject>("Particles/FireballAoE"), gameObject.transform.position, Quaternion.identity);
        Projectile aoe = proj.GetComponent<AoE>();
        aoe.InitializeAoE(player, aoeDamage, aoeRadius);
    }

    #endregion

    public Enemy GetRandomEnemy()
    {
        if (_enemies.Count == 0)
        {
            return null;
        }
        int random = Random.Range(0, _enemies.Count);
        return _enemies[random];
    }
}
