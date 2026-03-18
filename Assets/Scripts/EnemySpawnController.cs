using System.Numerics;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
    public GameObject enemy;
    [SerializeField] public float interval = 2;
    private float timer = 0;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < interval)
        {
            timer += Time.deltaTime;
        }
        else
        {
            Instantiate(enemy, new UnityEngine.Vector3(Random.Range(-10,10), Random.Range(5,20), 0), transform.rotation);
            timer = 0;
        }   
    }
}
