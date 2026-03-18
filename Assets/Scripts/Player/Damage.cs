using System.Xml.Serialization;
using UnityEngine;

public class Damage : MonoBehaviour
{
    public float damage = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Enemy")
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if(enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
