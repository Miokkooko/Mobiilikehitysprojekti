using System;
using System.Collections.Generic;
using UnityEngine;

public enum PoolType
{
    Enemy,
    Drop,
    Projectile_Axe,
    Projectile_Fireball,
    Projectile_Mine,
    Projectile_Generic,
    Projectile_Sword,
    Projectile_Scythe,
    Projectile_Lightning,
    Projectile_Enemy,
}

[Serializable]
public class PoolData
{
    public GameObject prefab;
    public int poolAmount;
    public PoolType type;
}

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    public Transform ProjectileParent;
    public Transform EnemyParent;
    public Transform DropParent;

    public Dictionary<PoolType, Queue<GameObject>> DisabledPools = new Dictionary<PoolType, Queue<GameObject>>();
    public Dictionary<PoolType, List<GameObject>> EnabledPools = new Dictionary<PoolType, List<GameObject>>();

    public List<PoolData> prefabPoolData;

    private void Awake()
    {
        Instance = this;

        InitializePools();
    }

    void InitializePools()
    {
        GameObject prefab;
        Transform parent = null;

        foreach (var pool in prefabPoolData)
        {
            DisabledPools.Add(pool.type, new Queue<GameObject>());
            EnabledPools.Add(pool.type, new List<GameObject>());

            if (pool.type == PoolType.Enemy)
                parent = EnemyParent;
            else if (pool.type == PoolType.Drop)
                parent = DropParent;
            else
                parent = ProjectileParent;

            prefab = pool.prefab;

            for (int i = 0; i < pool.poolAmount; i++)
            {
                InstantiateDisabledPool(prefab, parent, pool.type);
            }
        }
    }

    GameObject InstantiateDisabledPool(GameObject prefab, Transform parent, PoolType type)
    {
        GameObject g = Instantiate(prefab, parent);
        g.SetActive(false);
        DisabledPools[type].Enqueue(g);
        return g;
    }

    GameObject InstantiateEnabledPool(GameObject prefab, Transform parent, PoolType type)
    {
        GameObject g = Instantiate(prefab, parent);
        EnabledPools[type].Add(g);
        return g;
    }

    GameObject GetPrefabFromType(PoolType type)
    {
        foreach (var pool in prefabPoolData)
        {
            if (pool.type == type)
                return pool.prefab;
        }

        return null;
    }

    public GameObject SpawnGenericEnemy(UnitData data, Vector2 position)
    {
        GameObject g;
        try
        {
            g = DisabledPools[PoolType.Enemy].Dequeue();
            EnabledPools[PoolType.Enemy].Add(g);
        }
        catch (Exception)
        {
            g = GetPrefabFromType(PoolType.Enemy);

            if (g == null)
                return null;

            g = InstantiateEnabledPool(g, EnemyParent, PoolType.Enemy);
        }

        if (g.GetComponent<Enemy>() is Enemy enemy)
        {
            Debug.Log("wow");
            g.transform.position = position;
            g.SetActive(true);
            enemy.InitializeUnit(data);
            return g;
        }
       
        return null;
    }

    public GameObject SpawnProjectile(PoolType projectileType, Vector2 position)
    {
        GameObject g;
        try
        {
            g = DisabledPools[projectileType].Dequeue();
            EnabledPools[projectileType].Add(g);
        }
        catch (Exception)
        {
            g = GetPrefabFromType(projectileType);

            if (g == null)
                return null;

            g = InstantiateEnabledPool(g, ProjectileParent, projectileType);
        }

        g.transform.position = position;
        g.SetActive(true);

        return g;
    }

    public GameObject SpawnDrop(DropType type, Vector2 position, float amount = 0)
    {
        GameObject g;
        try
        {
            g = DisabledPools[PoolType.Drop].Dequeue();
            EnabledPools[PoolType.Drop].Add(g);
        }
        catch (Exception)
        {
            g = GetPrefabFromType(PoolType.Drop);

            if (g == null)
                return null;

            g = InstantiateEnabledPool(g, DropParent, PoolType.Drop);
        }

        g.transform.position = position;

        if (g.GetComponent<Drop>() is Drop d)
        {
            d.Initialize(type, amount);
        }
        g.SetActive(true);

        return g;
    }

    public void DisableObject(PoolType type, GameObject g)
    {
        Debug.Log((EnabledPools[type] == null) + " | " + (DisabledPools[type] == null) + " | " + (g == null));
        g.SetActive(false);
        EnabledPools[type].Remove(g);
        DisabledPools[type].Enqueue(g);
    }
}