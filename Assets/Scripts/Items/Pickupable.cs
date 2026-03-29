using UnityEngine;

public class Pickupable : MonoBehaviour
{

    public float health = 5;
    public float speed = 2f;
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
            speed += Time.deltaTime*3;
            transform.position = Vector3.MoveTowards(transform.position,target.position, speed * Time.deltaTime);
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
            player.Heal(new HealContext(player, health));
            Destroy(gameObject);
        }
        else if (circle.IsTouching(other))
        {
            target = other.transform;
        }
    }
}
