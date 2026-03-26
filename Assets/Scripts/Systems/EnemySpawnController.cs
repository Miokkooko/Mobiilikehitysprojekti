using System;
using System.Numerics;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
    public GameObject enemy;
    public GameObject player;
    public float interval = 2;
    public float enemySpawnDistance = 5;
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
            float xCordinate = 0;
            float yCordinate = 0;
            while (xCordinate == 0 && yCordinate == 0)
            {
                xCordinate = UnityEngine.Random.Range(-27,23);
                yCordinate = UnityEngine.Random.Range(-17,20);
                if(Math.Abs(player.transform.position.x - xCordinate) < enemySpawnDistance && Math.Abs(player.transform.position.y - yCordinate) < enemySpawnDistance)
                {
                    xCordinate = 0;
                    yCordinate = 0;
                }
            }
            
            Instantiate(enemy, new UnityEngine.Vector3(xCordinate, yCordinate, 0), transform.rotation);
            timer = 0;
        }   
    }
}
