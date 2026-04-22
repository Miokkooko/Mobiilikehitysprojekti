using System;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.Rendering.DebugUI;

public class lichBoss : Enemy
{
    [Header("Recovery")]
    public float recoverTimer = 0f;
    public float recoverDuration = 1f;

    [Header("Chase")]
    public float chaseDuration = 5f;
    private float chaseTimer = 0f;

    [Header("Charge")]
    public float chargeTimer = 0f;
    public float chargeDuration = 0.5f;

    [Header("Attack")]
    public int tpTimes = 3;
    public float tpCooldown = 0.5f;
    public float tpDistance = 2.5f;

    private bool isAttacking = false;
    private bool isPlayerNear;

    private float isPlayerNearTimer;

    private float lastTp;

    public Tilemap groundTilemap;
    public WeaponData weaponData;

    public bool handActive = false;
    public GameObject lichHand;

    public GameObject continuePortal;
    public GameObject menuPortal;

    private float endTimer;
    private float endDuration = 3f;

    public enum EnemyState
    {
        Recover,
        Chase,
        ChargeAttack,
        Attack
    }

    [SerializeField]
    EnemyState _currentState = EnemyState.Recover;

    public override void Update()
    {

        if (player == null)
            return;

        UpdateHealthBar();
        TrackHealth();

        switch (_currentState)
        {
            case EnemyState.Recover:
                Recover();
                break;
            case EnemyState.Chase:
                Chase();
                break;
            case EnemyState.ChargeAttack:
                ChargeAttack();
                break;
            case EnemyState.Attack:
                if (!isAttacking)
                {
                    int chooseAtk = UnityEngine.Random.Range(0, 3);
                    if (chooseAtk == 0)
                    {
                        StartCoroutine(TpAttack());
                    }else if(chooseAtk == 1)
                    {
                        SpawnSkeletons();
                    }else if(chooseAtk == 2)
                    {
                        ShootProjectiles();
                    }

                    isAttacking = true;
                }
                break;

        }
    }

    public void Recover()
    {
        isAttacking = false;
        recoverTimer += Time.deltaTime;

        if (recoverTimer >= recoverDuration)
        {
            recoverTimer = 0f;
            _currentState = EnemyState.Chase;
        }
    }

    public void Chase()
    {
        Move();

        chaseTimer += Time.deltaTime;

        if (chaseTimer >= chaseDuration)
        {
            chaseTimer = 0f;
            _currentState = EnemyState.ChargeAttack;
        }


    }

    public override void Move()
    {

        if (!isPlayerNear)
        {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(playerToFollow.transform.position.x, playerToFollow.transform.position.y, 0), Speed * Time.deltaTime);
        }else
        {
            Vector3 directionAway = (transform.position - playerToFollow.transform.position).normalized;
            Vector3 targetPos = transform.position + directionAway;

            transform.position = Vector3.MoveTowards(transform.position, targetPos, (Speed*0.70f) * Time.deltaTime);
        }
    }

    public void ChargeAttack()
    {
        chargeTimer += Time.deltaTime;

        if (chargeTimer >= chargeDuration)
        {
            chargeTimer = 0f;
            _currentState = EnemyState.Attack;
        }
    }

    public IEnumerator TpAttack()
    {
        for (int i = 0; i < tpTimes; i++)
        {
            Vector2 tpPosition = GetSpawnCoordinates();
            transform.position = tpPosition;

            ShootBullets();

            yield return new WaitForSeconds(tpCooldown);
        }

        _currentState = EnemyState.Recover;
    }

    public void SpawnSkeletons()
    {

        PoolManager manager = PoolManager.Instance;
        EnemyData data = Resources.Load<EnemyData>("UnitData/Enemies/SkeletonData");

        Vector3[] directions = new Vector3[]
        {
            Vector3.right,
            Vector3.left,
            Vector3.up,
            Vector3.down,

            new Vector3(1, 1, 0).normalized,
            new Vector3(-1, 1, 0).normalized,
            new Vector3(1, -1, 0).normalized,
            new Vector3(-1, -1, 0).normalized
        };
        float radius = 3f; 

        foreach (Vector3 dir in directions)
        {
            Vector3 spawnPos = transform.position + dir * radius;

            PoolManager.Instance.SpawnEnemy(data, spawnPos);
        }

        _currentState = EnemyState.Recover;
    }


    public void ShootBullets()
    {
        Vector3[] directions = new Vector3[]
        {
            Vector3.right,
            Vector3.left,
            Vector3.up,
            Vector3.down,

            new Vector3(1, 1, 0).normalized,
            new Vector3(-1, 1, 0).normalized,
            new Vector3(1, -1, 0).normalized,
            new Vector3(-1, -1, 0).normalized
        };


        foreach (Vector3 dir in directions)
        {
            PoolManager manager = PoolManager.Instance;
            GameObject proj = manager.SpawnProjectile(weaponData.poolType, transform.position);

            if (proj.GetComponent<EnemyProjectile>() is EnemyProjectile ep)
            {
                ep.Initialize(weaponData, this, dir);
            }
        }
    }

    public void ShootProjectiles()
    {
        float spreadAngle = 22.5f;

        for (int i = 0; i < 4; i++)
        {
            PoolManager manager = PoolManager.Instance;
            GameObject proj = manager.SpawnProjectile(weaponData.poolType, transform.position);

            if (proj.GetComponent<Projectile>() is Projectile p)
            {
                float angle = 0;

                
                    float even = i % 2;

                    if (i == 0)
                    {
                        angle = 0;
                    }
                    else
                    {
                        int side;
                        if (i % 2 == 0)
                        {
                            side = 1;
                        }
                        else side = -1;

                        int step = (i + 1) / 2;

                        angle = side * step * spreadAngle;
                    
                }

                Vector2 dir = (player.transform.position - transform.position).normalized;

                Vector3 rotatedDir = Quaternion.Euler(0, 0, angle) * dir;

                if (proj.GetComponent<EnemyProjectile>() is EnemyProjectile ep)
                {
                    ep.Initialize(weaponData, this, rotatedDir);
                }

            }

            proj.SetActive(true);
        }

        _currentState = EnemyState.Recover;
    }

    public Vector2 GetSpawnCoordinates()
    {
        float minDistance = 3.5f;
        float maxDistance = 6f;

        for (int i = 0; i < 50; i++)
        {
            Vector2 dir = UnityEngine.Random.insideUnitCircle.normalized;
            float distance = UnityEngine.Random.Range(minDistance, maxDistance);

            Vector2 pos = (Vector2)player.transform.position + dir * distance;

            if (IsValidGround(pos))
                return pos;
        }

        Debug.LogWarning("Failed to find spawn position");
        return player.transform.position;
    }

    bool IsValidGround(Vector2 position)
    {
        Vector3Int cell = groundTilemap.WorldToCell(position);
        return groundTilemap.HasTile(cell);

    }

    public void SetPlayerNear(bool isNear)
    {
        isPlayerNear = isNear;
    }

    public void TrackHealth()
    {
        if (Health <= MaxHealth * 0.7f && !handActive)
        {
            lichHand.SetActive(true);
            handActive = true;
        }

        if(Health <= MaxHealth / 2f)
        {
            chargeDuration = chargeDuration * 0.6f;
        }
    }

    public override void Enemy_OnDeath(object sender, KillContext e)
    {
        lichHand.SetActive(false);
        Debug.Log("Lich dead");

        for (int i = 0; i < 5; i++)
        {
            Instantiate(Resources.Load<ParticleSystem>("Particles/HitParticles"), gameObject.transform.position, Quaternion.identity);
        }

        PoolManager manager = PoolManager.Instance;

        for (int i = 0; i < 10; i++)
        {
            manager.SpawnDrop(DropType.Exp, transform.position, XpValue);
        }

        for (int i = 0; i < 10; i++)
        {
            DropCoin();
        }

        continuePortal.transform.position = new Vector2(player.transform.position.x -5, player.transform.position.y + 5);
        menuPortal.transform.position = new Vector2(player.transform.position.x + 5, player.transform.position.y + 5);
        continuePortal.SetActive(true);
        menuPortal.SetActive(true);
        gameObject.SetActive(false);
        
    }
}

