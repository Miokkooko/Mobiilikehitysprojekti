using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;

public class BulletMiniBoss : Enemy
{
    [Header("Recover")]
    float recoverTimer = 0f;
    float recoverDuration = 1f;

    [Header("Charge")]
    float chargeTimer = 0f;
    float chargeDuration = 0.5f;

    [Header("Firing")]
    public float fireDistance = 3f;

    
    public WeaponData data;

    public override void Start()
    {
        base.Start();
        
    }

    Vector3 lungeDir;
    public enum EnemyState
    {
        Recover,
        Chase,
        ChargeLunge,
        Attack
    }

    [SerializeField]
    EnemyState _currentState = EnemyState.Recover;

    public override void Update()
    {
        switch (_currentState)
        {
            case EnemyState.Recover:
                Recover();
                break;
            case EnemyState.Chase:
                Chase();
                break;
            case EnemyState.ChargeLunge:
                ChargeLunge();
                break;
            case EnemyState.Attack:
                Attack();
                break;

        }
    }

    public void Recover()
    {
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

        float distance = Vector3.Distance(transform.position, playerToFollow.transform.position);

        if (distance < fireDistance)
        {
            _currentState = EnemyState.Attack;
        }
    }

    public void ChargeLunge()
    {
        chargeTimer += Time.deltaTime;

        if (chargeTimer >= chargeDuration)
        {
            chargeTimer = 0f;
            _currentState = EnemyState.Attack;
        }
    }

    public void Attack()
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
            GameObject proj = Instantiate(data.projectilePrefab, transform.position, Quaternion.identity);
            EnemyProjectile eProj = proj.GetComponent<EnemyProjectile>();

            eProj.EnemyInitialize(data,this, dir);
        }
        _currentState = EnemyState.Recover;
    }


}
