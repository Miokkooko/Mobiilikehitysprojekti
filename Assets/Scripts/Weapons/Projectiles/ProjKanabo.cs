using System.Collections.Generic;
using UnityEngine;

public class MeleeSwing : Projectile
{
    [Header("Melee Settings")]
    public float swingArc = 180f;
    public float weaponRange = 1.5f; // Kuinka pitkälle melee yltää

    private float startAngle;
    private float elapsed = 0;

    // Lista, jolla pidetään kirjaa kehen on jo osuttu tämän heilautuksen aikana
    private List<IDamageable> alreadyHit = new List<IDamageable>();

    public override void OnEnable()
    {
        base.OnEnable();
        elapsed = 0;
        alreadyHit.Clear(); // Tyhjennä lista uutta lyöntiä varten
    }

    public override void Initialize(WeaponInstance w, Unit p, Vector3 dir)
    {
        base.Initialize(w, p, dir);

        // Lasketaan keskikulma suunnasta
        float centerAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // Asetetaan alkuperäinen kulma kaaren alkupäähän
        startAngle = centerAngle - (swingArc / 2f);

        elapsed = 0;
        UpdateSwing(0); // Asetetaan ase heti alkupaikalleen
    }

    public override void Move()
    {


        elapsed += Time.deltaTime;
        float progress = elapsed / projectileLifetime;

        if (progress <= 1f)
        {
            UpdateSwing(progress);
        }
        else
        {
            Disable();
        }
    }

    private void UpdateSwing(float progress)
    {
        // Lasketaan kulma lerpillä kaaren läpi
        float currentAngle = Mathf.Lerp(startAngle, startAngle + swingArc, progress);

        // Päivitetään rotaatio
        transform.rotation = Quaternion.Euler(0, 0, currentAngle);

        if (owner != null)
        {

            float offsetDistance = 0.5f;
            Vector3 offset = direction * offsetDistance;

            transform.position = owner.transform.position;
        }
    }

    // Ylikirjoitetaan osumislogiikka, jotta ei osu samaan viholliseen monta kertaa
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) return;

        if (collision.TryGetComponent<IDamageable>(out var d))
        {
            // Jos tähän viholliseen ei ole vielä osuttu tämän lyönnin aikana
            if (!alreadyHit.Contains(d))
            {
                Unit.DealDamage(new DamageContext(owner, d, damage));
                alreadyHit.Add(d); // Lisätään listalle

                OnHitParticles();

                // Knockback-logiikka (kopioitu KnockBackistä)
                if (collision.CompareTag("Enemy"))
                {
                    Enemy enemy = collision.GetComponent<Enemy>();
                    if (OnHitEffects != null)
                    {
                        foreach (var effect in OnHitEffects)
                        {
                            if (effect is KnockBack kb)
                            {
                                Vector2 dir = (enemy.transform.position - owner.transform.position).normalized;
                                enemy.ApplyKnockback(dir, kb.force, kb.duration);
                            }
                            else
                            {
                                Unit.ApplyStatusEffect(effect, enemy);
                            }
                        }
                    }
                }
            }
        }
    }
}