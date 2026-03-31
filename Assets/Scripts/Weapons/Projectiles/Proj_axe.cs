using UnityEngine;

public class Proj_axe : Projectile
{
    Vector3 velocity;
    float dir;

    public override void Start()
    {

        base.Start();

        dir = Random.Range(0, 2);

        if (dir == 0)
        {
            velocity = new Vector3(1 * 4, 9f, 0f);
        }
        else
        {
            velocity = new Vector3(-1 * 4, 9f, 0f);
        }
        
    }


    public override void Move()
    {
        velocity.y += -20f * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;
    }

    

}
