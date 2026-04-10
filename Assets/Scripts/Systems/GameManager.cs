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

    int coins;
    public float Coins => coins;
    public event Action<int> OnCoinChanged;

    float lastEnemySpawnTime;
    float lastMiniBossSpawnTime;

    [Header("Enemies")]
    
    //[SerializeField] private EnemySpawn[] enemyList;

    [SerializeField] private EnemyGroup[] enemyGroups;

    private EnemySpawn[] currentList; 

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
    void Awake()
    {
        instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.OnKill += Player_OnKill;

        player.OnDeath += Player_OnDeath;
        //instance = this;

        currentList = enemyGroups[0].enemies;
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
            SpawnEnemy();
        }

        if(gameTimer > lastMiniBossSpawnTime + miniBossInterval)
        {
            SpawnMiniBoss(miniBoss);
            
        }

        ChangeLists();
    }

    void SpawnEnemy()
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


        CalculateEnemy(xCordinate, yCordinate);


        

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

    public void CalculateEnemy(float xCordinate, float yCordinate)
    {
        float totalChance = 0f;
        foreach (EnemySpawn spawnEvents in currentList)
        {
            totalChance += spawnEvents.SpawnChance;
        }

        float rand = UnityEngine.Random.Range(0f, totalChance);
        float cumulaticeChance = 0f;

        foreach (EnemySpawn spawnEvents in currentList)
        {
            cumulaticeChance += spawnEvents.SpawnChance;

            if (rand <= cumulaticeChance)
            {
                Instantiate(spawnEvents.enemyPrefab, new Vector3(xCordinate, yCordinate, 0), transform.rotation);
                return;
            }
        }
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

    private void ChangeLists()
    {
        if(gameTimer > 60)
        {
            currentList = enemyGroups[2].enemies;
        }else if(gameTimer > 30)
        {
            currentList = enemyGroups[1].enemies;
        }
        else
        {
            currentList = enemyGroups[0].enemies;
        }
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        OnCoinChanged?.Invoke(coins);
    }
    
}


[System.Serializable]
public class EnemySpawn
{
    public string EventName;
    [Space]
    public GameObject enemyPrefab;
    [Range(0f, 1f)] public float SpawnChance = 0.5f;

}

[System.Serializable]
public class EnemyGroup
{
    public string groupName;         
    public EnemySpawn[] enemies;    
}