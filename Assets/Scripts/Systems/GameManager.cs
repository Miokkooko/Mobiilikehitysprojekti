using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    int kills;
    public int Kills => kills;

    float gameTimer;
    public float GameTime => gameTimer;

    float lastEnemySpawnTime;
    float lastMiniBossSpawnTime;

    [Header("Prefabs")]
    public GameObject enemy;
    public GameObject miniBoss;

    private Player player;

    [Header("Spawn intervals")]
    public float interval = 2;
    public float miniBossInterval = 60;

    [Header("Spawn distances")]
    public float enemySpawnDistance = 5;

    public int[] intervalChangeKillCounts = { 20, 50, 100, 200 };
    public float[] intervalChangeTimes = { 1.5f, 1.0f, 0.5f, 0.2f };

    void OnDestroy()
    {
        player.OnKill -= Player_OnKill;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.OnKill += Player_OnKill;

        player.OnDeath += Player_OnDeath;
        instance = this;    
    }

    private void Player_OnDeath(object sender, KillContext e)
    {
        SceneManager.LoadScene(0);
    }

    // Update is called once per frame
    void Update()
    {
        gameTimer += Time.deltaTime;

        if (gameTimer > lastEnemySpawnTime + interval)
        {
            SpawnEnemy(enemy);
        }

        if(gameTimer > lastMiniBossSpawnTime + miniBossInterval)
        {
            SpawnMiniBoss(miniBoss);
            
        }
    }

    void SpawnEnemy(GameObject prefab)
    {
        float xCordinate = 0;
        float yCordinate = 0;
        while (xCordinate == 0 && yCordinate == 0)
        {
            xCordinate = UnityEngine.Random.Range(-27, 23);
            yCordinate = UnityEngine.Random.Range(-17, 20);
            if (Math.Abs(player.transform.position.x - xCordinate) < enemySpawnDistance && Math.Abs(player.transform.position.y - yCordinate) < enemySpawnDistance)
            {
                xCordinate = 0;
                yCordinate = 0;
            }
        }

        Instantiate(prefab, new Vector3(xCordinate, yCordinate, 0), transform.rotation);

        lastEnemySpawnTime = gameTimer;
    }

    void SpawnMiniBoss(GameObject prefab)
    {
        float xCordinate = 0;
        float yCordinate = 0;
        while (xCordinate == 0 && yCordinate == 0)
        {
            xCordinate = UnityEngine.Random.Range(-27, 23);
            yCordinate = UnityEngine.Random.Range(-17, 20);
            if (Math.Abs(player.transform.position.x - xCordinate) < enemySpawnDistance && Math.Abs(player.transform.position.y - yCordinate) < enemySpawnDistance)
            {
                xCordinate = 0;
                yCordinate = 0;
            }
        }

        Instantiate(prefab, new Vector3(xCordinate, yCordinate, 0), transform.rotation);

        lastMiniBossSpawnTime = gameTimer;
    }


    private void Player_OnKill(object sender, KillContext e)
    {
        kills += 1;

        if (Kills == intervalChangeKillCounts[0])
        {
            interval = intervalChangeTimes[0];
        }
        if (Kills == intervalChangeKillCounts[1])
        {
            interval = intervalChangeTimes[1];
        }
        if (Kills == intervalChangeKillCounts[2])
        {
            interval = intervalChangeTimes[2];
        }
        if (Kills == intervalChangeKillCounts[3])
        {
            interval = intervalChangeTimes[3];
        }
    }

    public void ResetKillCount()
    {
        kills = 0;
    }

    
}
