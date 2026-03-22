using Unity.Cinemachine;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Pickup(other);
        }
    }

    void Pickup(Collider2D player)
    {
        Debug.Log("Picked up: Axe");

        
        player.GetComponent<PlayerMovement>().AddWeapon(new Axe(player.gameObject));

        Destroy(gameObject);
    }
}
