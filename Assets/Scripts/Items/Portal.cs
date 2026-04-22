using UnityEngine;
using UnityEngine.Splines.ExtrusionShapes;
using UnityEngine.UIElements;

public class Portal : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || !gameObject.activeInHierarchy)
        {
            return;
        }

        
            if (other.GetComponent<Player>() is Player p)
            {
                
            }
        
    }
}
