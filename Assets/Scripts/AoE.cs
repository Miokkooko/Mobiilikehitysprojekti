using UnityEngine;

public class AoE : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.tag == "Enemy")
        {
            EnemyController enemy = collision.GetComponent<EnemyController>();
            Enemy dummy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(1);
                
            }
            if (dummy != null)
            {
                dummy.TakeDamage(1);
                
            }
        }
    }
}
