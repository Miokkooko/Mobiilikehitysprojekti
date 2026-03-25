using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("ProjectileStats")]
    public float projectileSpeed = 5f;
    public float projectileHealth = 1f;
    public float damage = 1f;
    public float projectileLifetime = 2f;

    [Header("Projectiles")]
    public bool enableParticles;
    public GameObject hitParticles;

    //Movement
    protected Vector3 direction;
    protected Transform playerPos;
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

    public virtual void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    public virtual void SetPlayerPos(Transform pos)
    {
        playerPos = pos;
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
        if(collision.tag == "Enemy")
        {
            EnemyController enemy = collision.GetComponent<EnemyController>();
            Enemy dummy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                //enemy.TakeDamage(damage);
                OnHit();
            }
            if (dummy != null)
            {
                dummy.TakeDamage(damage);
                OnHit();
            }
        }
    }

    public virtual void OnHit()
    {
        
        if (hitParticles != null)
        {
            Object.Instantiate(hitParticles, gameObject.transform.position, Quaternion.identity);
        }

        if (projectileHealth != 1)
        {
            projectileHealth -= 1;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    


}
