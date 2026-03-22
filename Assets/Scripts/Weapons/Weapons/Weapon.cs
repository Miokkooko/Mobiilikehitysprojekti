using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class Weapon 
{

    protected GameObject owner;
    protected GameObject projectilePrefab;

    //Weapon stats
    public float cooldown = 1f;
    public float lastFireTime;
    public float projectileCount = 1f;

    public Weapon(GameObject owner)
    {
        this.owner = owner;
    }
   
    public virtual void Initialize(GameObject o)
    {
        owner = o;
    }
    public void TryFire()
    {
        if (Time.time >= lastFireTime + cooldown)
        {
            Fire();
            lastFireTime = Time.time;
        }
    }

    public virtual void Fire()
    {
        //get the direction of the last input
        Vector3 dir = owner.GetComponent<PlayerMovement>().GetMoveDirection();
        //spawn projectile
        GameObject proj = Object.Instantiate(projectilePrefab, owner.transform.position, Quaternion.identity);
        //give the projectile the direction of the last input
        proj.GetComponent<Projectile>().SetDirection(dir);
    }

}
