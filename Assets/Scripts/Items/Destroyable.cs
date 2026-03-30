using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Destroyable : Unit
{
    public Sprite sprite;
    private bool destroyed=false;

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
        if (!destroyed)
        {
            destroyed = true;
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
                    Instantiate(dropEvents.dropPrefab, transform.position, Quaternion.identity);
                    return;
                }
            }
        }

    }



}


[System.Serializable]

public class DropEvents
{
    public string EventName;
    [Space]
    [Space]
    [Range(0f, 1f)] public float DropChance = 0.5f;
    public GameObject dropPrefab;
}