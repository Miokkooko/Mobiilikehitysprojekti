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


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            if (enemy != null && !_enemies.Contains(enemy))
            {
                _enemies.Add(enemy);
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
            }
        }
    }
}
