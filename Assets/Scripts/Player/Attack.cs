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



    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        fireRate = playerStats.baseFireRate;
    }

    // Update is called once per frame
    void Update()
    {
        shootTimer += Time.deltaTime;
       
        Fire();
    }

    void Fire()
    {
        if(shootTimer > fireRate)
        {
            shootTimer = 0;
            GameObject intBullet = Instantiate(bullet, aim.position, aim.rotation);

        
        }
    }
    
}
