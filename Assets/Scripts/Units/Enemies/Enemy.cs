using System.Threading;
using UnityEngine;

public class Enemy : Unit
{
    private GameObject playerToFollow;

    float lastAttackTime = 0f;
    float attackRate = 1f;

    private void Start()
    {
        playerToFollow = GameObject.FindGameObjectWithTag("Player");
        OnDeath += Enemy_OnDeath;
    }

    public override void Update()
    {
        base.Update();

        Move();
    }

    private void OnDestroy()
    {
        OnDeath -= Enemy_OnDeath;
    }


    public virtual void Move()
    {
        if(playerToFollow != null)
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(playerToFollow.transform.position.x, playerToFollow.transform.position.y, 0), Speed * Time.deltaTime);
    }
    void Attack(IDamageable target)
    {
        lastAttackTime = Time.time;
        DealDamage(new DamageContext(this, target, Damage));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Player>() is Player p)
        {
            if (Time.time >= lastAttackTime + attackRate)
            {
                Attack(p);
            }
        }
    }
    private void Enemy_OnDeath(object sender, KillContext e)
    {
        Destroy(gameObject);
    }
}