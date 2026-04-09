public class ProjLightning : Projectile
{
    Enemy target;
    
    public override void OnEnable()
    {
        base.OnEnable();
        target = GetRandomEnemy();
        if(target == null)
        {
            Disable(); 
            //Destroy(gameObject);
            return;
        }

        projectilePiercing = 99f;
        transform.position = target.transform.position;
    }

    public override void Move()
    {

    }
}
