using System.Numerics;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    
    public Rigidbody2D rigidbody;
    public float movementSpeed = 1f;
    public KillCounter counter;

    private GameObject playerToFollow;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerToFollow = GameObject.FindGameObjectWithTag("Player");
        counter = GameObject.FindGameObjectWithTag("KillCounter").GetComponent<KillCounter>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = UnityEngine.Vector3.MoveTowards(transform.position, new UnityEngine.Vector3(playerToFollow.transform.position.x, playerToFollow.transform.position.y, 0), movementSpeed * Time.deltaTime);
    }

    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        if (collision.gameObject.name == "Bullet(Clone)")
        {
            DestroyEnemy();
            counter.addOneKill();
        }
    }
}
