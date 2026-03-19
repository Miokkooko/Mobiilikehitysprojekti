using UnityEngine;

public class Bullet : Projectile
{
    
    public float fireForce = 10f;

    void Start()
    {
        Destroy(gameObject, 2f);
    }

    public override void Move()
    {
        transform.position += -transform.up * fireForce * Time.deltaTime;
    }

}
