using UnityEngine;

public class ParticleBurst : MonoBehaviour
{

    public ParticleSystem particles;
    public SpriteRenderer sr;
    public bool once = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        particles = GetComponent<ParticleSystem>();

        var em = particles.emission;
        var dur = particles.duration;

        em.enabled = true;
        particles.Play();

        Destroy(sr);
        Invoke(nameof(DestroyObj), dur);

    }

    void DestroyObj()
    {
        Destroy(gameObject);
    }
}
