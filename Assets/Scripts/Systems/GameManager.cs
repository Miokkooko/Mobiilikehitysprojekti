using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using UnityEngine.Rendering.Universal;
using Unity.VisualScripting;

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

    public int bossState = 0;
    private float bossStateTimer = 0;

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
    public GameObject mainMap;
    public GameObject bossMap;
    public Light2D globalLight;

    public GameObject finalBoss;
    public GameObject sceneHand;

    [Header("Spawn intervals")]
    public float interval = 2;
    public float spawnDecreaseTime;
    public float spawnDecreaseAmount;
    public float miniBossInterval = 60;
    public float spawnDecreaseTimeBoss;
    public float spawnDecreaseAmountBoss;

    [Header("Spawn distances")]
    public float enemySpawnDistance = 5;

    private float lastContainerSpawnTime;
    private float containerInterval = 15f;

    [System.Serializable]
    private class SingleGameData
    {
        public string playerName;
        public int kills;
        public float time;
    }

    private string savePath;

    public enum GameState
    {
        Normal,
        SceneChange,
        SpawnBoss,
        BossFight
    }

    public GameState _currentState = GameState.Normal;


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

        currentEnemyList = enemyGroups[listNumber].enemies;
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

        switch (_currentState)
        {
            case GameState.Normal:
                NormalGameplay();
                break;
            case GameState.SceneChange:
                ChangingScenes();
                break;
            case GameState.SpawnBoss:
                SpawnBoss();
                break;
            case GameState.BossFight:
                BossFightState();
                break;


        }
    }

    public void NormalGameplay()
    {
        if (gameTimer > lastEnemySpawnTime + interval)
        {
            CalculateEnemy(currentEnemyList);
            lastEnemySpawnTime = gameTimer;
        }

        if (gameTimer > lastMiniBossSpawnTime + miniBossInterval)
        {
            CalculateEnemy(currentMiniBossList);
            lastMiniBossSpawnTime = gameTimer;
        }

        if (gameTimer > lastContainerSpawnTime + containerInterval)
        {
            SpawnContainer();
            lastContainerSpawnTime = gameTimer;
        }
        UpdateSpawnInterval();
        ChangeLists();

        if(gameTimer > 60)
        {
            sceneHand.SetActive(true);
        }
    }

    public void ChangingScenes()
    {
        bossStateTimer += Time.deltaTime;

        PoolManager.Instance.DisableAllEnemies(EnemyPoolType.GenericEnemy);
        PoolManager.Instance.DisableAllEnemies(EnemyPoolType.HandMiniBoss);
        PoolManager.Instance.DisableAllEnemies(EnemyPoolType.SkeletonMiniBoss);
        PoolManager.Instance.DisableAllEnemies(EnemyPoolType.BulletMiniBoss);

        if (bossStateTimer > 6)
        {
            SpawnCultistCircle();
            bossStateTimer = 0;
            _currentState = GameState.SpawnBoss;
        }
    }

    public void BossFightState()
    {
        currentEnemyList = enemyGroups[5].enemies;
        if (gameTimer > lastEnemySpawnTime + interval)
        {
            CalculateEnemy(currentEnemyList);
            lastEnemySpawnTime = gameTimer;
        }
    }

    public void SpawnBoss()
    {
        bossStateTimer += Time.deltaTime;

        if (bossStateTimer > 6)
        {
            finalBoss.transform.position = new Vector2(player.transform.position.x, player.transform.position.y + 10);
            finalBoss.SetActive(true);

            _currentState = GameState.BossFight;
        }
    }

    public Vector2 GetSpawnCoordinates()
    {
        for (int i = 0; i < 50; i++)
        {
            float x = player.transform.position.x + UnityEngine.Random.Range(-20, 20);
            float y = player.transform.position.y + UnityEngine.Random.Range(-20, 20);

            Vector2 pos = new Vector2(x, y);

            bool invalidGround = !IsValidGround(pos);
            bool tooCloseToPlayer = TooCloseToPlayer(x, y);

            if (!tooCloseToPlayer && !invalidGround)
                return pos;
        }

        Debug.LogWarning("Failed to find spawn position");
        return player.transform.position;
    }

    #region spawnChecks
    bool TooCloseToPlayer(float xCordinate, float yCordinate)
    {
        return Math.Abs(player.transform.position.x - xCordinate) < enemySpawnDistance &&
                Math.Abs(player.transform.position.y - yCordinate) < enemySpawnDistance;
    }
    bool IsValidGround(Vector2 position)
    {
        Vector3Int cell;
        if (mainMap.activeInHierarchy == true)
        {
            Transform groundChild = mainMap.transform.Find("Ground");
            Tilemap tilemap = groundChild.GetComponent<Tilemap>();
            cell = tilemap.WorldToCell(position);
            return tilemap.HasTile(cell);
        }else if(bossMap.activeInHierarchy == true)
        {
            Transform groundChild = bossMap.transform.Find("Ground");
            Tilemap tilemap = groundChild.GetComponent<Tilemap>();
            cell = tilemap.WorldToCell(position);
            return tilemap.HasTile(cell);
        } else return false;

        //Vector3Int cell = groundTilemap.WorldToCell(position);
        //return groundTilemap.HasTile(cell);
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


    #region bossfight things

    public void SpawnCultistCircle()
    {



        float radius = 8f;

        EnemyData data = Resources.Load<EnemyData>("UnitData/Enemies/CultistData");

        int count = 12;
        

        for (int i = 0; i < count; i++)
        {
            float angle = i * Mathf.PI * 2f / count;
            Vector3 dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);

            Vector3 spawnPos = player.transform.position + dir * radius;

            PoolManager.Instance.SpawnEnemy(data, spawnPos);
        }
    }

    #endregion



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
            if (listNumber < enemyGroups.Length && enemyGroups[listNumber] != null)
            {
                currentEnemyList = enemyGroups[listNumber].enemies;
                globalLight.intensity += 0.01f;
            }
            lastListChange = gameTimer;
        }
        
    }

    private void SpawnContainer()
    {
        PoolManager manager = PoolManager.Instance;
        manager.SpawnOther(OtherPoolType.Container, GetSpawnCoordinates());
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

    public void SetBossSettings()
    {
        // tänne valo asetukset kun teleportataan bossiin.
        mainMap.SetActive(false);
        bossMap.SetActive(true);

        globalLight.intensity = 0;
    }

    public void SetNormalSettings()
    {
        // tänne valo asetukset kun teleportataan bossiin.
        mainMap.SetActive(true);
        bossMap.SetActive(false);

        globalLight.intensity = 0.05f;
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
