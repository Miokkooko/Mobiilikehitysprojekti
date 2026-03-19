using System.Numerics;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    
    public Rigidbody2D rigidbody;
    public float movementSpeed = 1f;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //rigidbody.linearVelocity = UnityEngine.Vector2.down;

        transform.position = UnityEngine.Vector3.MoveTowards(transform.position, new UnityEngine.Vector3(0,0,0), movementSpeed * Time.deltaTime);
    }

    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }
}
