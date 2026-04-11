
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Unit
{
    protected GameObject playerToFollow;
    protected Player player;
    protected Image healthBar;

    protected Animator animator;
    public bool isWalking = false;
    public EnemyData enemyData;

    protected float lastAttackTime = 0f;
    protected float attackRate = 1f;

    public StatusEffect[] effects;

    float XpValue => enemyData.xpValue;

    public override void InitializeUnit(UnitData data)
    {
        playerToFollow = GameObject.FindGameObjectWithTag("Player");
        player = playerToFollow.GetComponent<Player>();

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
        RemoveAllStatusEffects(this);

        if (playerToFollow == null || player == null)
        {
            playerToFollow = GameObject.FindGameObjectWithTag("Player");
            player = playerToFollow.GetComponent<Player>();
        }
            
        OnDeath += Enemy_OnDeath;

        if(animator == null)
            animator = GetComponent<Animator>();

       
        healthBar = transform.Find("HealthBar/Health")?.GetComponent<Image>();
    }

    public override void Update()
    {
        base.Update();

        

        Move();
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>() is Player p)
        {
            if (Time.time >= lastAttackTime + attackRate)
            {
                Attack(p);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
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
        PoolManager manager = PoolManager.Instance;

        manager.SpawnDrop(DropType.Exp, transform.position, XpValue);

        manager.DisableEnemy(enemyData.poolType, gameObject);
    }

    public void UpdateHealthBar()
    {
        healthBar.fillAmount = Health / MaxHealth;    
    }

    /*
    public virtual void DropCoin()
    {
        float rand = Random.Range(1, coinDropChance);
        if (rand == 1)
        {
            GameObject coinDrop = Instantiate(Resources.Load<GameObject>("Drops/coinDrop"), transform.position, Quaternion.identity);
            CoinDrop coinScript = coinDrop.GetComponent<CoinDrop>();
            coinScript.InitializeCoins(coinValue);
        }
    }
    */
}