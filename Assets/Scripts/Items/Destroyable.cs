using UnityEngine;

public class Destroyable : MonoBehaviour, IDamageable
{
    public Sprite sprite;

    public float Health;

    [Header("Drops")]
    [SerializeField] private DropEvents[] _dropEvents;


    void Start()
    {
        if (sprite != null)
        {
            GetComponent<SpriteRenderer>().sprite = sprite;
            GetComponent<SpriteRenderer>().color = Color.white;
        }
    }


    public void TakeDamage(DamageContext context)
    {
        Health -= context.Amount;

        if (Health <= 0)
        {
            FireDropEvent();
            Destroy(gameObject);
        }
    }

    private void FireDropEvent()
    {

        float totalChance = 0f;
        foreach (DropEvents dropEvents in _dropEvents)
        {
            totalChance += dropEvents.DropChance;
        }

        float rand = Random.Range(0f, totalChance);
        float cumulaticeChance = 0f;

        foreach (DropEvents dropEvents in _dropEvents)
        {
            cumulaticeChance += dropEvents.DropChance;

            if (rand <= cumulaticeChance)
            {
                PoolManager manager = PoolManager.Instance;
                manager.SpawnDrop(dropEvents.dropType, transform.position, dropEvents.dropValue);
                return;
            }
        }
    }

    public void Heal(HealContext context, bool showPopUp)
    {
        throw new System.NotImplementedException();
    }
}


[System.Serializable]

public class DropEvents
{
    public DropType dropType;
    public float dropValue;
    [Range(0f, 1f)] public float DropChance = 0.5f;
}