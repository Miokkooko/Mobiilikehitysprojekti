using UnityEngine;

public class ProjSword : Projectile
{
    Vector3 velocity;

    public override void Start()
    {
        base.Start();
    }


    public override void Move()
    {
        transform.position = playerPos.position + direction * 1.5f;
    }

}
