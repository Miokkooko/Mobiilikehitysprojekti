using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    int kills;
    public int Kills => kills;

    float gameTimer;
    public float GameTime => gameTimer;

    float lastEnemySpawnTime;
    float lastMiniBossSpawnTime;

    public LevelUpManager levelUpManager;
    public Player player;

    [Header("Prefabs")]
    public EnemyData enemy;
    public EnemyData miniBoss;
   
    [Header("Spawn intervals")]
    public float interval = 2;
    public float miniBossInterval = 60;

    [Header("Spawn distances")]
    public float enemySpawnDistance = 5;

    public int[] intervalChangeKillCounts = { 20, 50, 100, 200 };
    public float[] intervalChangeTimes = { 1.5f, 1.0f, 0.5f, 0.2f };

    void OnDestroy()
    {
        player.OnKill -= OnPlayerKill;

        player.OnDeath -= OnPlayerDeath;
        player.OnPlayerLevelUp -= OnPlayerLevelUp;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance = this;
        if (player == null)
        {
            Debug.Log("Player not found");
            return;
        }

        player.OnKill += OnPlayerKill;
        player.OnDeath += OnPlayerDeath;
        player.OnPlayerLevelUp += OnPlayerLevelUp;
        levelUpManager.player = player;
    }

    
    void OnPlayerLevelUp(int obj)
    {
        TriggerReward();
    }

    public void TriggerReward()
    {
        levelUpManager.TriggerReward();
        
    }

    private void OnPlayerDeath(object sender, KillContext e)
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
            lastEnemySpawnTime = gameTimer;
        }

        if(gameTimer > lastMiniBossSpawnTime + miniBossInterval)
        {
            SpawnEnemy(miniBoss);
            lastMiniBossSpawnTime = gameTimer;
        }
    }

    Vector2 GetSpawnCoordinates()
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

        return new Vector2(xCordinate, yCordinate);
    }

    void SpawnEnemy(EnemyData data)
    {
        PoolManager manager = PoolManager.Instance;
        manager.SpawnEnemy(data, GetSpawnCoordinates());
    }

    private void OnPlayerKill(object sender, KillContext e)
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

    public WeaponInstance GetWeaponFromPlayer(WeaponData weapon)
    {
        return player.GetWeapon(weapon);
    }

    public PassiveInstance GetPassiveFromPlayer(PassiveData data)
    {
        return player.GetPassive(data);
    }
}
