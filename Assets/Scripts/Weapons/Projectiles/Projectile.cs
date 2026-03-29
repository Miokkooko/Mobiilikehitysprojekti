using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("ProjectileStats")]
    float projectileSpeed = 5f;
    float projectilePiercing = 1f;
    float damage = 1f;
    float projectileLifetime = 2f;

    [Header("Projectiles")]
    public bool enableParticles;
    public GameObject hitParticles;

    //Movement
    protected Vector3 direction;
    protected Player player;
    protected Transform playerPos => player.transform;
    protected float angle;


    public virtual void Start()
    {
        //Projectile tuhoaa ittensä kahen sekunnin jälkeen
        if(hitParticles == null && enableParticles)
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
    }

    public virtual void Rotate()
    {
        //käännä projectile pelaajan viimeisimpään kävelysuuntaan
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    #endregion

    #region Collision
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            return;

        if(collision.GetComponent<IDamageable>() is IDamageable d)
        {
            Unit.DealDamage(new DamageContext(player, d, damage));
        }

        if(collision.tag == "Enemy")
        {
            Enemy dummy = collision.GetComponent<Enemy>();

            if (dummy != null)
            {
                OnHit();
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
            Destroy(gameObject);
        }
    }

    #endregion
}
