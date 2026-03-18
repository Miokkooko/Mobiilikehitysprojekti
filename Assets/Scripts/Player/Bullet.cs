using UnityEngine;

public class Bullet : MonoBehaviour
{
    
    public float fireForce = 10f;

    void Start()
    {
        Destroy(gameObject, 2f);
    }

    private void Update()
    {
        transform.position += -transform.up * fireForce * Time.deltaTime;
    }



}
