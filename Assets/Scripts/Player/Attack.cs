using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Attack : MonoBehaviour
{
    public Transform aim;
    public GameObject bullet;
    private PlayerStats playerStats;
    public float fireForce = 10f;

    float fireRate;
    float shootTimer = 0.5f;

    List<Weapon> weapons;


    void Start()
    {
        weapons = new List<Weapon>();
        playerStats = GetComponent<PlayerStats>();
        fireRate = playerStats.baseFireRate;
       // AddWeapon(new Axe(gameObject, aim));
    }

    
    // Update is called once per frame
    void Update()
    {
        shootTimer += Time.deltaTime;
       
        FireWeapons();
    }
    /*
    void Fire()
    {
        if(shootTimer > fireRate)
        {
            shootTimer = 0;
            GameObject intBullet = Instantiate(bullet, aim.position, aim.rotation);

        
        }
    }
    */
    public virtual void FireWeapons()
    {
        foreach (Weapon w in weapons)
        {
            w.TryFire();
        }
    }

    public void AddWeapon(Weapon w)
    {
        weapons.Add(w);
        w.Initialize(gameObject);
    }
}
