using System;
using System.Collections.Generic;
using UnityEngine;

public enum PoolType { Enemy, Projectile, Other }
public enum EnemyPoolType
{
    None,
    GenericEnemy,
    BulletMiniBoss,
}
public enum ProjectilePoolType
{
    None,
    Projectile_Axe,
    Projectile_Fireball,
    Projectile_Mine,
    Projectile_Generic,
    Projectile_Sword,
    Projectile_Scythe,
    Projectile_Lightning,
    Projectile_Enemy,
}
public enum OtherPoolType
{
    None,
    Drop,
    DmgPopUp,
}

[Serializable]
public class PoolData
{
    public GameObject prefab;
    public int poolAmount;
}
[Serializable]
public class OtherPoolData : PoolData { public OtherPoolType type; }
[Serializable]
public class ProjectilePoolData : PoolData { public ProjectilePoolType type; }
[Serializable]
public class EnemyPoolData : PoolData { public EnemyPoolType type; }

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    public Transform ProjectileParent;
    public Transform EnemyParent;
    public Transform OtherParent;

    public Dictionary<EnemyPoolType, Queue<GameObject>> DisabledEnemyPools = new Dictionary<EnemyPoolType, Queue<GameObject>>();
    public Dictionary<EnemyPoolType, List<GameObject>> EnabledEnemyPools = new Dictionary<EnemyPoolType, List<GameObject>>();

    public Dictionary<ProjectilePoolType, Queue<GameObject>> DisabledProjectilePools = new Dictionary<ProjectilePoolType, Queue<GameObject>>();
    public Dictionary<ProjectilePoolType, List<GameObject>> EnabledProjectilePools = new Dictionary<ProjectilePoolType, List<GameObject>>();

    public Dictionary<OtherPoolType, Queue<GameObject>> DisabledPools = new Dictionary<OtherPoolType, Queue<GameObject>>();
    public Dictionary<OtherPoolType, List<GameObject>> EnabledPools = new Dictionary<OtherPoolType, List<GameObject>>();

    public List<OtherPoolData> otherPoolData;
    public List<ProjectilePoolData> projectilePoolData;
    public List<EnemyPoolData> enemyPoolData;

    private void Awake()
    {
        Instance = this;

        InitializePools();
    }

    void InitializePools()
    {
        GameObject prefab;
    

        foreach (var pool in otherPoolData)
        {
            DisabledPools.Add(pool.type, new Queue<GameObject>());
            EnabledPools.Add(pool.type, new List<GameObject>());

            prefab = pool.prefab;

            for (int i = 0; i < pool.poolAmount; i++)
            {
                InstantiateDisabledPool(prefab, OtherParent, DisabledPools, pool.type);
            }
        }

        foreach (var pool in enemyPoolData)
        {
            DisabledEnemyPools.Add(pool.type, new Queue<GameObject>());
            EnabledEnemyPools.Add(pool.type, new List<GameObject>());

            prefab = pool.prefab;

            for (int i = 0; i < pool.poolAmount; i++)
            {
                InstantiateDisabledPool(prefab, EnemyParent, DisabledEnemyPools, pool.type);
            }
        }

        foreach (var pool in projectilePoolData)
        {
            DisabledProjectilePools.Add(pool.type, new Queue<GameObject>());
            EnabledProjectilePools.Add(pool.type, new List<GameObject>());

            prefab = pool.prefab;

            for (int i = 0; i < pool.poolAmount; i++)
            {
                InstantiateDisabledPool(prefab, ProjectileParent, DisabledProjectilePools, pool.type);
            }
        }
    }

    GameObject InstantiateDisabledPool<T>(GameObject prefab, Transform parent, Dictionary<T, Queue<GameObject>> pools, T type) where T : Enum
    {
        prefab.SetActive(false);
        GameObject g = Instantiate(prefab, parent);
        prefab.SetActive(true);

        pools[type].Enqueue(g);

        return g;
    }

    GameObject InstantiateEnabledPool<T>(GameObject prefab, Transform parent, Dictionary<T, List<GameObject>> pools, T type) where T : Enum
    {
        prefab.SetActive(false);
        GameObject g = Instantiate(prefab, parent);
        prefab.SetActive(true);
        pools[type].Add(g);

        return g;
    }

    GameObject GetPrefabFromType<T>(PoolType baseType, T type) where T : Enum
    {
        switch (baseType)
        {
            case PoolType.Enemy:
                foreach (var pool in enemyPoolData)
                {
                    if (pool.type.Equals(type))
                        return pool.prefab;
                }
                break;
            case PoolType.Projectile:
                foreach (var pool in projectilePoolData)
                {
                    if (pool.type.Equals(type))
                        return pool.prefab;
                }
                break;
            case PoolType.Other:
                foreach (var pool in otherPoolData)
                {
                    if (pool.type.Equals(type))
                        return pool.prefab;
                }
                break;
        }

        return null;
    }

    public GameObject SpawnEnemy(EnemyData data, Vector2 position)
    {
        GameObject g;
        try
        {
            g = DisabledEnemyPools[data.poolType].Dequeue();
            EnabledEnemyPools[data.poolType].Add(g);
        }
        catch (Exception)
        {
            g = GetPrefabFromType(PoolType.Enemy, data.poolType);

            if (g == null)
                return null;

            g = InstantiateEnabledPool(g, EnemyParent, EnabledEnemyPools, data.poolType);
        }

        if (g.GetComponent<Enemy>() is Enemy enemy)
        {
            g.transform.position = position;
            enemy.InitializeUnit(data);
            g.SetActive(true);
            return g;
        }

        return null;
    }

    public GameObject SpawnProjectile(ProjectilePoolType projectileType, Vector2 position)
    {
        GameObject g;
        try
        {
            g = DisabledProjectilePools[projectileType].Dequeue();
            EnabledProjectilePools[projectileType].Add(g);
        }
        catch (Exception)
        {
            g = GetPrefabFromType(PoolType.Projectile, projectileType);

            if (g == null)
                return null;

            g = InstantiateEnabledPool(g, ProjectileParent, EnabledProjectilePools, projectileType);
        }

        g.transform.position = position;

        return g;
    }

    public GameObject SpawnDrop(DropType type, Vector2 position, float amount = 0)
    {
        GameObject g;
        try
        {
            g = DisabledPools[OtherPoolType.Drop].Dequeue();
            EnabledPools[OtherPoolType.Drop].Add(g);
        }
        catch (Exception)
        {
            g = GetPrefabFromType(PoolType.Other, OtherPoolType.Drop);

            if (g == null)
                return null;

            g = InstantiateEnabledPool(g, OtherParent, EnabledPools, OtherPoolType.Drop);
        }

        g.transform.position = position;

        if (g.GetComponent<Drop>() is Drop d)
        {
            d.Initialize(type, amount);
        }
        g.SetActive(true);

        return g;
    }

    public GameObject SpawnPopUp(Vector2 position)
    {
        GameObject g;
        try
        {
            g = DisabledPools[OtherPoolType.DmgPopUp].Dequeue();
            EnabledPools[OtherPoolType.DmgPopUp].Add(g);
        }
        catch (Exception)
        {
            g = GetPrefabFromType(PoolType.Other, OtherPoolType.DmgPopUp);

            if (g == null)
                return null;

            g = InstantiateEnabledPool(g, OtherParent, EnabledPools, OtherPoolType.DmgPopUp);
        }

        g.transform.position = position;
        g.SetActive(true);

        return g;
    }

    public void DisableProjectile(ProjectilePoolType type, GameObject g)
    {
        g.SetActive(false);
        EnabledProjectilePools[type].Remove(g);
        DisabledProjectilePools[type].Enqueue(g);
    }

    public void DisableEnemy(EnemyPoolType type, GameObject g)
    {
        g.SetActive(false);
        EnabledEnemyPools[type].Remove(g);
        DisabledEnemyPools[type].Enqueue(g);
    }

    public void DisableOther(OtherPoolType type, GameObject g)
    {
        g.SetActive(false);
        EnabledPools[type].Remove(g);
        DisabledPools[type].Enqueue(g);
    }
}