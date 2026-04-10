using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class ProjScythe : Projectile
{
    public float timer = 0f;
    public float duration = 1f;
    
    bool returnBack = false;

    public override void OnEnable()
    {
        base.OnEnable();
        timer = 0f;
        duration = 1f;

        returnBack = false;
    }

    public override void Move()
    {
        if (timer < duration)
        {
            transform.position += direction * projectileSpeed * Time.deltaTime;
            timer += Time.deltaTime;
        }
        else
        {
            returnBack = true;
        }

        if(returnBack == true)
        {
            transform.position += (owner.transform.position - transform.position).normalized * projectileSpeed * Time.deltaTime;
        }
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if(collision.tag == "Player" && returnBack)
        {
            Disable();
            //Destroy(gameObject);
        }
    }
}
