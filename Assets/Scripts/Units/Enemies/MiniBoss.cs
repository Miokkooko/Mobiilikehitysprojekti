using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class MiniBoss : Enemy
{
    [Header("Recovery")]
    public float recoverTimer = 0f;
    public float recoverDuration = 1f;

    [Header("Charge")]
    public float chargeTimer = 0f;
    public float chargeDuration = 0.5f;

    [Header("Lunge")]
    public float lungeSpeed = 5f;
    public float lungeDuration = 1f;
    public float lungeDistance = 3f;

    float lungeTimer = 0f;
    bool lungeStarted = false;

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
        UpdateHealthBar();

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

        if(distance < lungeDistance)
        {
            _currentState = EnemyState.ChargeLunge;
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
        if (!lungeStarted)
        {
            lungeStarted = true;
            lungeTimer = 0f;

            lungeDir = (playerToFollow.transform.position - transform.position).normalized;
        }

        transform.position += lungeDir * lungeSpeed * Time.deltaTime;

        lungeTimer += Time.deltaTime;

        if(lungeTimer >= lungeDuration)
        {
            lungeStarted = false;
            _currentState = EnemyState.Recover;
        }
    }

}
