using System.Runtime.InteropServices;
using System.Xml.XPath;
using UnityEngine;


public enum DropType { Coin, Exp, Heart, Reward }
public class Drop : MonoBehaviour
{

    public float dropMoveSpeed = 2f;

    protected float dropValue = 1f;

    Transform target;
    BoxCollider2D box;
    CircleCollider2D circle;

    public DropType type;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        box = GetComponent<BoxCollider2D>();
        circle = GetComponent<CircleCollider2D>();
    }

    private void OnEnable()
    {
        target = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            dropMoveSpeed += Time.deltaTime * 3;
            transform.position = Vector3.MoveTowards(transform.position, target.position, dropMoveSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || !gameObject.activeInHierarchy)
        {
            return;
        }

        if (box.IsTouching(other))
        {
            if(other.GetComponent<Player>() is Player p)
            {
                OnGrab(p);
                PoolManager manager = PoolManager.Instance;
                manager.DisableObject(PoolType.Drop, gameObject);
            }
        }
        else if (circle.IsTouching(other))
        {
            target = other.transform;
        }
    }

    public void OnGrab(Player player)
    {
        switch (type)
        {
            case DropType.Coin:
                Debug.Log("Coin got!");
                break;
            case DropType.Exp:
                player.IncreaseExp(dropValue);
                break;
            case DropType.Heart:
                player.Heal(new HealContext(player, dropValue));
                break;
            case DropType.Reward:
                GameManager.Instance.TriggerReward();
                break;
            default:
                break;
        }
    }

    public virtual void Initialize(DropType t, float value)
    {
        type = t;

        dropValue = value;
    }
}
