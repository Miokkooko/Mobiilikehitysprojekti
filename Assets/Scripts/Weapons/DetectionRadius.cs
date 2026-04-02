using System.Collections.Generic;
using UnityEngine;

public class DetectionRadius : MonoBehaviour
{
    CircleCollider2D circle;
    public List<Enemy> _enemies = new List<Enemy>();

    void Start()
    {
        circle = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            if (enemy != null && !_enemies.Contains(enemy))
            {
                _enemies.Add(enemy);
                Debug.Log("Added enemy:"+_enemies);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            if (enemy != null)
            {
                _enemies.Remove(enemy);
                Debug.Log("Removed enemy:"+_enemies);
            }
        }
    }
}
