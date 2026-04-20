using UnityEngine;

public class EnemySpider : Enemy
{
    public override void Update()
    {
        base.Update();
        Rotate();
        Debug.Log("Toimii rotation");
    }
    public void Rotate()
    {
        Vector2 direction = player.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
