using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("ProjectileStats")]
    public float projectileSpeed = 5f;
    public float projectileHealth = 1f;
    public float damage = 1f;

    [Header("Movement")]
    public Vector3 direction;
    public float angle;


    public virtual void Start()
    {
        //Projectile tuhoaa ittensä kahen sekunnin jälkeen
        Destroy(gameObject, 2f);
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
                enemy.TakeDamage(damage);
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
