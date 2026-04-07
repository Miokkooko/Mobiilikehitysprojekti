using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;

public class BulletMiniBoss : Enemy
{
    [Header("Recover")]
    public float recoverTimer = 0f;
    public float recoverDuration = 1f;

    [Header("Charge")]
    public float chargeTimer = 0f;
    public float chargeDuration = 0.5f;

    [Header("Firing")]
    public float fireDistance = 3f;

    
    public WeaponData data;
    

    public enum EnemyState
    {
        Recover,
        Chase,
        Attack
    }

    [SerializeField]
    EnemyState _currentState = EnemyState.Recover;

    public override void Update()
    {
        animator.SetBool("isWalking", isWalking);
        UpdateHealthBar();

        switch (_currentState)
        {
            case EnemyState.Recover:
                Recover();
                break;
            case EnemyState.Chase:
                Chase();
                break;
            case EnemyState.Attack:
                Attack();
                break;

        }
    }

    public void Recover()
    {
        isWalking = false;

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

        isWalking = true;

        chargeTimer += Time.deltaTime;

        if (chargeTimer >= chargeDuration)
        {
            chargeTimer = 0f;
            _currentState = EnemyState.Attack;
        }

    }

    public void Attack()
    {
        isWalking = false;

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
