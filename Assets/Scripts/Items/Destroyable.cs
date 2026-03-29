using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Destroyable : Unit
{
    public ItemData drop;
    public Sprite sprite;
    float random;

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


    public override void TakeDamage(DamageContext context)
    {
        base.TakeDamage(context);

        if(Health<=0)
        {
            OnDestroy();
        }
    }

    public void OnDestroy()
    {
        FireDropEvent();
        
        Destroy(gameObject);
    }
    
   private void FireDropEvent()
    {

        float totalChance = 0f;
        foreach(DropEvents dropEvents in _dropEvents)
        {
            totalChance += dropEvents.DropChance;
        }

        float rand = Random.Range(0f, totalChance);
        float cumulaticeChance = 0f;

        foreach(DropEvents dropEvents in _dropEvents)
        {
            cumulaticeChance += dropEvents.DropChance;

            if(rand <= cumulaticeChance)
            {
                Debug.Log("Here!");
                dropEvents.DropEvent.Invoke();
                return;
            }
        }

    }


    public void SpawnHeart()
    {
        Debug.Log("Heart Spawned!");
        Instantiate(Resources.Load<GameObject>("Drops/healthdrop"), transform.position, Quaternion.identity);
    }

    public void SpawnEnemy()
    {
        Debug.Log("Enemy Spawned!");
        Instantiate(Resources.Load<GameObject>("Drops/Enemy"), transform.position, Quaternion.identity);
    }
}


[System.Serializable]

public class DropEvents
{
    public string EventName;
    [Space]
    [Space]
    [Range(0f, 1f)] public float DropChance = 0.5f;
    public UnityEvent DropEvent;
}