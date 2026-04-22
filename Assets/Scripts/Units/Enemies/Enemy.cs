
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Unit
{
    protected ParticleSystem particles;
    protected string unitName;
    protected SpriteRenderer spriteRenderer;
    protected GameObject playerToFollow;
    protected Player player;
    protected Image healthBar;
    protected Canvas fullHealthBar;

    protected bool canAttack = true;
    protected CircleCollider2D hitBox;
    protected BoxCollider2D hitBox2;

    protected Animator animator;
    public bool isWalking = false;
    public EnemyData enemyData;

    protected float lastAttackTime = 0f;
    protected float attackRate = 1f;

    public StatusEffect[] effects;

    protected float XpValue => enemyData.xpValue;

    public bool canShoot = false;

    private float lastShootTime;
    public float shootInterval = 5f;

    public override void InitializeUnit(UnitData data)
    {
        playerToFollow = GameObject.FindGameObjectWithTag("Player");
        player = playerToFollow.GetComponent<Player>();
        hitBox = GetComponent<CircleCollider2D>();
        hitBox2 = GetComponent<BoxCollider2D>();
        
        particles = gameObject?.GetComponentInChildren<ParticleSystem>();

        Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), player.GetComponent<Collider2D>());

        base.InitializeUnit(data);
        enemyData = (EnemyData)data;
    }
    private void Start()
    {
        if(enemyData != null)
            InitializeUnit(enemyData);

        
    }
    public virtual void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        RemoveAllStatusEffects(this);

        if (playerToFollow == null || player == null)
        {
            playerToFollow = GameObject.FindGameObjectWithTag("Player");
            player = playerToFollow.GetComponent<Player>();
        }



        OnDeath += Enemy_OnDeath;

        if(animator == null)
            animator = GetComponent<Animator>();

        //vaihtaa vihun värin nimen mukaan
        ChangeColor();

        fullHealthBar = transform.Find("HealthBar")?.GetComponent<Canvas>();

        healthBar = transform.Find("HealthBar/Health")?.GetComponent<Image>();
    }

    public override void Update()
    {
        base.Update();

        Move();

        if(enemyData.unitName.Contains("Spider"))
        {
            Rotate();
        }

    }
    private void OnDisable()
    {
        //DropCoin();

        OnDeath -= Enemy_OnDeath;
    }


    public virtual void Move()
    {
        if(playerToFollow != null)
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(playerToFollow.transform.position.x, playerToFollow.transform.position.y, 0), Speed * Time.deltaTime);

        if (isKnockedBack) return;
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

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canAttack) return;

        if (Time.time < lastAttackTime + attackRate)
            return;

        if (collision.GetComponent<Player>() is not Player p)
            return;

        // ONLY allow main hitbox
        if (hitBox == null || !hitBox.IsTouching(collision))
            return;

        Attack(p);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!canAttack) return;

        if (Time.time < lastAttackTime + attackRate)
            return;

        if (collision.GetComponent<Player>() is not Player p)
            return;

        // ONLY allow main hitbox
        if (hitBox == null || !hitBox.IsTouching(collision))
            return;

        Attack(p);
    }

    public virtual void Enemy_OnDeath(object sender, KillContext e)
    {
        PoolManager manager = PoolManager.Instance;

        manager.SpawnDrop(DropType.Exp, transform.position, XpValue);

        DropCoin();

        manager.DisableEnemy(enemyData.poolType, gameObject);
    }

    public void UpdateHealthBar()
    {
        healthBar.fillAmount = Health / MaxHealth;    
    }

    
    public virtual void DropCoin()
    {
        float rand = Random.Range(1, enemyData.coinDropChance);
        if (rand == 1)
        {
            PoolManager.Instance.SpawnDrop(DropType.Coin, transform.position, enemyData.coinValue);
        }
    }

    public void Rotate()
    {
        Vector2 direction = player.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + 90f);
    }

    public void ChangeColor()
    {
        if (enemyData.name.Contains("Red"))
        {
            spriteRenderer.color = Color.red;
        }
    }

    private void ShootProjectile()
    {
        int chooseAtk = UnityEngine.Random.Range(0, 2);
        if (chooseAtk == 1)
            return;
        PoolManager manager = PoolManager.Instance;
        GameObject proj = manager.SpawnProjectile(ProjectilePoolType.Projectile_Enemy, transform.position);

        Vector2 dir = (player.transform.position - transform.position).normalized;

        if (proj.GetComponent<EnemyProjectile>() is EnemyProjectile ep)
        {
            ep.Initialize(Resources.Load<WeaponData>("WeaponData/EnemyWeaponData"), this, dir);
        }
    }
}