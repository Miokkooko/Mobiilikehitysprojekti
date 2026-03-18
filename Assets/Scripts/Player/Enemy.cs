using System.Threading;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    float moveTimer;
    private Vector2 moveDirection;
    private Rigidbody2D rb;
    

    public float hp = 4;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
    }

    void FixedUpdate()
    {
        Timer();
        rb.linearVelocity = moveDirection * 2;
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Timer()
    {
        moveTimer += Time.deltaTime;
        if (moveTimer >= 2)
        {
            moveTimer = 0;
            if (moveDirection.y == 1)
            {
                moveDirection.y = -1;
            }
            else { 
                moveDirection.y = 1; }

        }
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}

