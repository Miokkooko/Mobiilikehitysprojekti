using UnityEngine;

public class HandMiniBoss : Enemy
{
    

    [Header("Recovery")]
    public float recoverTimer = 0f;
    public float recoverDuration = 1f;
    

    [Header("Charge")]
    public float chargeTimer = 0f;
    public float chargeDuration = 0.5f;
    public float stopFollowingWhen = 0.8f;

    [Header("Grab")]
    public float grabDuration = 1f;

    float grabTimer = 0f;
    bool grabStarted = false;
    int animState = 0 ;

    Vector3 lungeDir;
    public enum EnemyState
    {
        Recover,
        ChargeAttack,
        Attack
    }

    [SerializeField]
    EnemyState _currentState = EnemyState.Recover;

    public override void Update()
    {
        base.Update();

        if (player == null)
            return;

        animator.SetInteger("State", animState);

        UpdateHealthBar();

        switch (_currentState)
        {
            case EnemyState.Recover:
                Recover();
                break;
            case EnemyState.ChargeAttack:
                ChargeAttack();
                break;
            case EnemyState.Attack:
                Attack();
                break;

        }
    }

    public void Recover()
    {

        animState = 0;
        canAttack = false;
        spriteRenderer.enabled = false;
        hitBox.enabled = false;
        hitBox2.enabled = false;
        fullHealthBar.enabled = false;
        

        recoverTimer += Time.deltaTime;

        if (recoverTimer >= recoverDuration)
        {
            recoverTimer = 0f;
            _currentState = EnemyState.ChargeAttack;
        }
    }

    public override void Move()
    {

    }

   
    public void ChargeAttack()
    {
        isWalking = false;
        spriteRenderer.enabled = true;

        chargeTimer += Time.deltaTime;

        if (chargeTimer >= chargeDuration)
        {
            chargeTimer = 0f;
            _currentState = EnemyState.Attack;
        }
        else if (chargeTimer < chargeDuration * stopFollowingWhen)
        {
            transform.position = player.transform.position;
        }

    }

    public void Attack()
    {
        
        canAttack = true;
        hitBox.enabled = true;
        hitBox2.enabled = true;
        fullHealthBar.enabled = true;
        animState = 1;
        particles.Play();

        if (!grabStarted)
        {
            grabStarted = true;
            grabTimer = 0f;
        }

        

        grabTimer += Time.deltaTime;

        if (grabTimer >= grabDuration)
        {
            animState = 2;
            particles.Play();
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

            if (state.IsName("Retreat") && state.normalizedTime >= 1f)
            {
                grabStarted = false;
                _currentState = EnemyState.Recover;
            }

        }
    }
}
