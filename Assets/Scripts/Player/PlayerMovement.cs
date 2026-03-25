using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    private PlayerStats playerStats;
    public float moveSpeed;
    public bool disableWeapon;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 lastMoveDirection;


    private Animator animator;

    public Transform aim;
    bool isWalking = false;

    //Attacking
    public List<WeaponInstance> weapons;
    float shootTimer = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        weapons = new List<WeaponInstance>();

        if (!disableWeapon)
        {
            weapons.Add(new WeaponInstance(gameObject, Resources.Load<WeaponData>("WeaponData/KnifeData")));
        }

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerStats = GetComponent<PlayerStats>();
        spriteRenderer = GetComponent<SpriteRenderer>();
       
        //moveSpeed = playerStats.baseMoveSpeed;
    }

    private void FixedUpdate()
    {
        //liikuttaa hahmoa
        
        


        Animate();

        //kääntää tähtäämisen suunnan
        if(isWalking)
        {
            Vector3 vector3 = Vector3.left * moveInput.x + Vector3.down * moveInput.y;
            aim.rotation = Quaternion.LookRotation(Vector3.forward, vector3);
        }
    }

    // Update is called once per frame
    void Update()
    {
        shootTimer += Time.deltaTime;
        //transform.position += new Vector3(moveInput.x, moveInput.y) * moveSpeed * Time.deltaTime;

        transform.Translate(moveInput * moveSpeed * Time.deltaTime);

        FireWeapons();
    }

    public void Move(InputAction.CallbackContext context)
    {
        //jos napista päästetään irti tallentaa viimeisimmän suunnan. Muuten pidetään isWalking = true
        if (context.canceled)
        {
            isWalking = false;
            lastMoveDirection = moveInput;
            
        //Aim pitää viimeisimmän suunnan
            Vector3 vector3 = Vector3.left * lastMoveDirection.x + Vector3.down * lastMoveDirection.y;
            aim.rotation = Quaternion.LookRotation(Vector3.forward, vector3);
        } else
        {
            isWalking = true;
        }
        
        //lukee kävelysuunnan
        moveInput = context.ReadValue<Vector2>();
        

        //kääntää spriten
        if (moveInput.x != 0)
        {
            spriteRenderer.flipX = moveInput.x < 0;
        }
    }

    void Animate()
    {
            animator.SetBool("isWalking", isWalking);

            animator.SetFloat("LastInputX", lastMoveDirection.x);
            

            animator.SetFloat("InputX", moveInput.x);
            animator.SetFloat("InputY", moveInput.y);
    }

    public Vector3 GetMoveDirection()
    {
        if (moveInput != Vector2.zero)
        {
            lastMoveDirection = moveInput.normalized;
        }

        Vector3 dir = new Vector3(lastMoveDirection.x, lastMoveDirection.y, 0);

        if (dir == Vector3.zero)
        {
            dir = Vector3.right;
        }

        return dir;
    }


    public virtual void FireWeapons()
    {
        foreach (WeaponInstance w in weapons)
        {
            w.TryFire();
        }
    }

    public void AddWeapon(WeaponInstance w)
    {
        weapons.Add(w);
        w.Initialize(gameObject);
    }

    private void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            Destroy(gameObject);
            Debug.Log("Pelaaja tuhottu. Peli ohi.");
        }
    }
}
