using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;



public class WeaponInstance
{

    protected GameObject owner;
    private WeaponData data;

    //Weapon stats
    public float lastFireTime;


    public WeaponInstance(GameObject owner, WeaponData data)
    {
        this.owner = owner;
        this.data = data;
    }

    public virtual void Initialize(GameObject o)
    {
        owner = o;
    }


    public void TryFire()
    {
        if (Time.time >= lastFireTime + data.cooldown)
        {
            Fire();
            lastFireTime = Time.time;
        }
    }

    //------ Jos jätän tähän tämmöisen käyttäytymisen defaultiksi kun aika moni ase saattais käyttää tätä? ----------
    public virtual void Fire()
    {

            //hae viimeisimmän inputin suunta + luo projectile + anna projectilelle viimeisimmän inputin suunta
            Vector3 dir = owner.GetComponent<PlayerMovement>().GetMoveDirection();
            GameObject proj = Object.Instantiate(data.projectilePrefab, owner.transform.position, Quaternion.identity);
            proj.GetComponent<Projectile>().SetDirection(dir); 
    }

}
