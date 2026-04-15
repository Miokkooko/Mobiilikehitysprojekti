using UnityEngine;

public class ProjBloodyTear : Projectile
{
    ParticleSystem particles;
    public override void Start()
    {
        particles = gameObject.GetComponent<ParticleSystem>();
    }

    

    public override void Rotate()
    {
        base.Rotate();
        particles.transform.rotation = Quaternion.Euler(0, 0, angle - 180);
    }
}
