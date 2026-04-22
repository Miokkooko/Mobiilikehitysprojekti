using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;
using static UnityEditor.PlayerSettings;

public class SceneHand : Enemy
{
    public GameObject playerBossSpawn;
    public Camera mainCamera;

    private bool hasTeleported = false;

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
    int animState = 0;

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
            mainCamera.GetComponent<CameraFollowObject>().TriggerShake();
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

        transform.position = player.transform.position;

        if (chargeTimer >= chargeDuration)
        {
            chargeTimer = 0f;
            _currentState = EnemyState.Attack;
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
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0) ;

            if (state.IsName("Retreat") && state.normalizedTime >= 1f && !hasTeleported)
            {
                hasTeleported = true;

                Vector3 pos = playerBossSpawn.transform.position;

                mainCamera.enabled = false;
             
                GameManager.Instance.SetBossSettings();

                player.transform.position = playerBossSpawn.transform.position;
                player.GetComponent<SpriteRenderer>().enabled = true;
                player.enabled = true;
                mainCamera.transform.position = new Vector3(pos.x, pos.y, -10f);
                mainCamera.enabled = true;
                this.enabled = false;
                //grabStarted = false;
                //_currentState = EnemyState.Recover;
            }

        }
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>() is Player p && canAttack)
        {
            player.enabled = false;
            player.GetComponent<SpriteRenderer>().enabled = false;
            GameManager.Instance._currentState = GameManager.GameState.SceneChange;
        }
    }
}
