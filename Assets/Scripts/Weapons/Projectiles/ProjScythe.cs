using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class ProjScythe : Projectile
{
    public float timer = 0f;
    public float duration = 1f;
    
    bool retrn = false;
    public override void Move()
    {
        if (timer < duration)
        {
            transform.position += direction * projectileSpeed * Time.deltaTime;
            timer += Time.deltaTime;
        }
        else
        {
            retrn = true;
        }

        if(retrn == true)
        {
            transform.position += (player.transform.position - transform.position).normalized * projectileSpeed * Time.deltaTime;
        }
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if(collision.tag == "Player" && retrn)
        {
            Destroy(gameObject);
        }
    }
}
