using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Proj_axe : Projectile
{
    Vector3 velocity;

    public override void Start()
    {
        base.Start();
        velocity = new Vector3(direction.x * 4, 9f, 0f);
    }


    public override void Move()
    {
        velocity.y += -20f * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;
    }

    public override void Rotate()
    {
        angle += 300 * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

}
