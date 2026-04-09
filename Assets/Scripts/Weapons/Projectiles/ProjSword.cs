using UnityEngine;

public class ProjSword : Projectile
{
    public override void Move()
    {
        transform.position = playerPos.position + direction * 1.5f;
    }

}
