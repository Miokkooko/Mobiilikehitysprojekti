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
        if (collision.tag == "Player")
            return;

        if (collision.GetComponent<IDamageable>() is IDamageable d)
        {
            //Unit.DealDamage(new DamageContext(player, d, 1));
        }

        if (collision.tag == "Enemy")
        {
            Enemy dummy = collision.GetComponent<Enemy>();

            if (dummy != null)
            {
                
            }
        }
    }
}
