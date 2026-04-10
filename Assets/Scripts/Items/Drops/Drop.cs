using System.Xml.XPath;
using UnityEngine;

public class Drop : MonoBehaviour
{

    public float health = 5;
    public float dropMoveSpeed = 2f;
    protected float expAmount = 1f;
    protected int coinAmount = 1;

    Transform target;
    BoxCollider2D box;
    CircleCollider2D circle;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        box = GetComponent<BoxCollider2D>();
        circle = GetComponent<CircleCollider2D>();
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
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (box.IsTouching(other))
        {
            Player player = other.GetComponent<Player>();
            OnGrab(player);
            Destroy(gameObject);
        }
        else if (circle.IsTouching(other))
        {
            target = other.transform;
        }
    }

    public virtual void OnGrab(Player player)
    {
        
    }

    public virtual void Initialize(float xp)
    {
        expAmount = xp;
    }
}
