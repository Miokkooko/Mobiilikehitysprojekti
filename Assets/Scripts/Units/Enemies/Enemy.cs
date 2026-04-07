using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Unit
{
    protected GameObject playerToFollow;
    protected Player player;
    protected Image healthBar;

    protected Animator animator;
    public bool isWalking = false;
    private Vector2 lastMoveDirection;


    protected float lastAttackTime = 0f;
    protected float attackRate = 1f;

    public StatusEffect[] effects;

    public virtual void Start()
    {
        playerToFollow = GameObject.FindGameObjectWithTag("Player");
        player = playerToFollow.GetComponent<Player>();
        OnDeath += Enemy_OnDeath;

        
        animator = GetComponent<Animator>();
        healthBar = transform.Find("HealthBar/Health")?.GetComponent<Image>();
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

        if(effects != null && target is Player p)
        {
            foreach(var effect in effects)
            {
                ApplyStatusEffect(effect, p);
            }
        }
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

    private void OnCollisionStay2D(Collision2D collision)
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
        GameObject expDrop = Instantiate(Resources.Load<GameObject>("Drops/ExpDrop"), transform.position, Quaternion.identity);
        ExpDrop expScript = expDrop.GetComponent<ExpDrop>();
        expScript.Initialize(expAmount);
        Destroy(gameObject);
    }

    public void UpdateHealthBar()
    {
        healthBar.fillAmount = Health / MaxHealth;    
    }
}