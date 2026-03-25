using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Splines.ExtrusionShapes;

public class WeaponPickup : MonoBehaviour
{
    public WeaponData data;
    public Sprite sprite;


    void Start()
    {
        if (sprite != null)
        {
            GetComponent<SpriteRenderer>().sprite = sprite;
            GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

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

        if(player.GetComponent<Player>() is Player p)
            p.AddWeapon(new WeaponInstance(p, data));

        Destroy(gameObject);
    }
}
