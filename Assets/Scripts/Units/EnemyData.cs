using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Units/EnemyData")]
public class EnemyData : UnitData
{
    public int xpValue;
    public EnemyPoolType poolType = EnemyPoolType.GenericEnemy;
} 