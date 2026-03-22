using System;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    private void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        if (collision.gameObject.name == "Enemy(Clone)")
        {
            Destroy(gameObject);
            Debug.Log("Pelaaja tuhottu. Peli ohi.");
        }
    }
}
