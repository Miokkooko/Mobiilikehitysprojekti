using System;
using System.Numerics;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
    public GameObject enemy;
    public GameObject player;
    public KillCounter counter;
    public float interval = 2;
    public float enemySpawnDistance = 5;
    private float timer = 0;
    public int[] intervalChangeKillCounts = {20, 50, 100, 200};
    public float[] intervalChangeTimes = {1.5f, 1.0f, 0.5f, 0.2f};
    
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
        if (counter.killCount == intervalChangeKillCounts[0])
        {
            interval = intervalChangeTimes[0];
        }
        if (counter.killCount == intervalChangeKillCounts[1])
        {
            interval = intervalChangeTimes[1];
        }
        if (counter.killCount == intervalChangeKillCounts[2])
        {
            interval = intervalChangeTimes[2];
        }
        if (counter.killCount == intervalChangeKillCounts[3])
        {
            interval = intervalChangeTimes[3];
        }
    }
}
