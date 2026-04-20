using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    int kills;
    public int Kills => kills;

    float gameTimer;
    public float GameTime => gameTimer;

    protected float lastIntervalChange;
    protected float lastIntervalChangeBoss;
    protected float lastListChange = 0;
    public int listNumber = 0;

    int coins;
    public int Coins => coins;
    public event Action<int> OnCoinChanged;

    float lastEnemySpawnTime;
    float lastMiniBossSpawnTime;

    [Header("UI References")]
    [SerializeField] private DeathMenu deathMenu;

    [Header("Enemies")]
    
    //[SerializeField] private EnemySpawn[] enemyList;

    [SerializeField] private EnemyGroup[] enemyGroups;
    [SerializeField] private EnemyGroup[] miniBossGroups;

    private EnemySpawn[] currentEnemyList;
    private EnemySpawn[] currentMiniBossList;

    public LevelUpManager levelUpManager;
    public Player player;
    public Tilemap groundTilemap;

    [Header("Spawn intervals")]
    public float interval = 2;
    public float spawnDecreaseTime;
    public float spawnDecreaseAmount;
    public float miniBossInterval = 60;
    public float spawnDecreaseTimeBoss;
    public float spawnDecreaseAmountBoss;

    [Header("Spawn distances")]
    public float enemySpawnDistance = 5;

    [System.Serializable]
    private class SingleGameData
    {
        public string playerName;
        public string datetime;
        public int kills;
        public float time;
    }

    private string savePath;


    void OnDestroy()
    {
        player.OnKill -= OnPlayerKill;

        player.OnDeath -= OnPlayerDeath;
        player.OnPlayerLevelUp -= OnPlayerLevelUp;
    }
    void Awake()
    {
        Instance = this;
        savePath = Path.Combine(Application.persistentDataPath, "singleGameSave.json");
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

        currentEnemyList = enemyGroups[0].enemies;
        currentMiniBossList = miniBossGroups[0].enemies;

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
        SingleGameData data = new SingleGameData()
        {
            playerName = player.playerData.name,
            datetime = DateTime.Now.ToString(),
            kills = kills,
            time = gameTimer
        };

        string json = JsonUtility.ToJson(data, true);
        File.AppendAllText(savePath, json);
        Debug.Log("Game saved to: " + savePath);
        

        enabled = false;

        if (DataManager.Instance != null)
        {
            DataManager.Instance.AddCoins(GameManager.Instance.Coins);
            DataManager.Instance.AddKills(GameManager.Instance.Kills);
        }

        ResetCoinCount();

        if (deathMenu != null)
            deathMenu.ShowDeathMenu();
        else
            Debug.LogError("DeathMenu not assigned in Inspector!");
    }

    // Update is called once per frame
    void Update()
    {
        gameTimer += Time.deltaTime;

        if (gameTimer > lastEnemySpawnTime + interval)
        {
            CalculateEnemy(currentEnemyList);
            lastEnemySpawnTime = gameTimer;
        }

        if(gameTimer > lastMiniBossSpawnTime + miniBossInterval)
        {
            CalculateEnemy(currentMiniBossList);
            lastMiniBossSpawnTime = gameTimer;
        }

        UpdateSpawnInterval();
    }


    Vector2 GetSpawnCoordinates()
    {
        while (true)
        {
            float xCordinate = player.transform.position.x + UnityEngine.Random.Range(-20, 20);
            float yCordinate = player.transform.position.y + UnityEngine.Random.Range(-20, 20);

            Vector2 pos = new Vector2(xCordinate, yCordinate);

            bool invalidGround = !IsValidGround(pos);
            bool tooCloseToPlayer = TooCloseToPlayer(xCordinate, yCordinate);

            if (!tooCloseToPlayer && !invalidGround)
            {
                return pos;
            }
        }
    }

    #region spawnChecks
    bool TooCloseToPlayer(float xCordinate, float yCordinate)
    {
        return Math.Abs(player.transform.position.x - xCordinate) < enemySpawnDistance &&
                Math.Abs(player.transform.position.y - yCordinate) < enemySpawnDistance;
    }
    bool IsValidGround(Vector2 position)
    {
        Vector3Int cell = groundTilemap.WorldToCell(position);
        return groundTilemap.HasTile(cell);
    }
    #endregion

    void SpawnEnemy(EnemyData data)
    {
        PoolManager manager = PoolManager.Instance;
        manager.SpawnEnemy(data, GetSpawnCoordinates());
    }

    public void CalculateEnemy(EnemySpawn[] enemyList)
    {
        float totalChance = 0f;
        foreach (EnemySpawn spawnEvents in enemyList)
        {
            totalChance += spawnEvents.SpawnChance;
        }

        float rand = UnityEngine.Random.Range(0f, totalChance);
        float cumulaticeChance = 0f;

        foreach (EnemySpawn spawnEvents in enemyList)
        {
            cumulaticeChance += spawnEvents.SpawnChance;

            if (rand <= cumulaticeChance)
            {
                //Instantiate(spawnEvents.enemyPrefab, new Vector3(xCordinate, yCordinate, 0), transform.rotation);

                SpawnEnemy(spawnEvents.data);
                return;
            }
        }
    }


    private void OnPlayerKill(object sender, KillContext e)
    {
        kills += 1;
    }

    public void UpdateSpawnInterval()
    {
        if(gameTimer > lastIntervalChange + spawnDecreaseTime)
        {
            interval -= spawnDecreaseAmount;
            interval = Mathf.Max(0.2f, interval);
            lastIntervalChange = gameTimer;
        }
        if (gameTimer > lastIntervalChangeBoss + spawnDecreaseTimeBoss)
        {
            miniBossInterval -= spawnDecreaseAmountBoss;
            miniBossInterval = Mathf.Max(1f, miniBossInterval);
            lastIntervalChangeBoss = gameTimer;
        }
    }

    public void ResetKillCount()
    {
        kills = 0;
    }
    public void ResetCoinCount()
    {
        coins = 0;
    }

    private void ChangeLists()
    {

        
        if(gameTimer > lastListChange + 30)
        {
            listNumber += 1;
            if (enemyGroups[listNumber] != null)
            {
                currentEnemyList = enemyGroups[listNumber].enemies;
            }
            lastListChange = gameTimer;
        }
        
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        OnCoinChanged?.Invoke(coins);
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


[System.Serializable]
public class EnemySpawn
{
    public string EventName;
    [Space]
    public EnemyData data;
    [Range(0f, 1f)] public float SpawnChance = 0.5f;

}

[System.Serializable]
public class EnemyGroup
{
    public string groupName;         
    public EnemySpawn[] enemies;    
}
