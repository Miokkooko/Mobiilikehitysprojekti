using UnityEngine;

public class ProjSword : Projectile
{
    public float radius = 1f;
    public float _angle = 0f;
    public override void Move()
    {
        float x = owner.transform.position.x + Mathf.Cos(_angle) * radius;
        float y = owner.transform.position.y + Mathf.Sin(_angle) * radius;

        transform.position = new Vector3(x, y);

        _angle += projectileSpeed * Time.deltaTime;
    }
}


