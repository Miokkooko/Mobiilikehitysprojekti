using UnityEngine;
public class ProjLightning : Projectile
{
    Enemy target;
    Animator animator;
    
    public override void OnEnable()
    {
        base.OnEnable();

        if (animator == null)
            animator = GetComponent<Animator>();

        animator.Play("LightningAnimation");
        target = GetRandomEnemy();
        if(target == null)
        {
            Disable(); 
            //Destroy(gameObject);
            return;
        }

        projectilePiercing = 99f;
        transform.position = target.transform.position;
        transform.localScale = new Vector2(aoeRadius, aoeRadius);
    }

    public override void Move()
    {

    }
}
