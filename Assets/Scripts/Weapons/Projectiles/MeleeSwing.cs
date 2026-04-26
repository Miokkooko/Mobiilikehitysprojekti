using System.Collections.Generic;
using UnityEngine;

public class MeleeSwing : Projectile
{
    [Header("Melee Settings")]
    public float swingArc = 180f;
    public float weaponRange = 1.5f; // Kuinka pitkälle melee yltää

    private float startAngle;
    private float elapsed = 0;
    private bool hasCleared = false;

    // Lista, jolla pidetään kirjaa kehen on jo osuttu tämän heilautuksen aikana
    private List<IDamageable> alreadyHit = new List<IDamageable>();

    public override void OnEnable()
    {
        base.OnEnable();
        elapsed = 0;
        alreadyHit.Clear(); // Tyhjennä lista uutta lyöntiä varten

    } // OnEnable

    public override void Initialize(WeaponInstance w, Unit p, Vector3 dir)
    {
        base.Initialize(w, p, dir);


        float finalScale = weaponRange * aoeRadius;
        transform.localScale = new Vector3(finalScale, finalScale, 1f);

        // Lasketaan keskikulma suunnasta
        float centerAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // Asetetaan alkuperäinen kulma kaaren alkupäähän
        startAngle = centerAngle - (swingArc / 2f);

        elapsed = 0;
        UpdateSwing(0); // Asetetaan ase heti alkupaikalleen

    } // Intitialize

    public override void Rotate()
    {
        // Jätä tämä tyhjäksi! 
        // Emme halua Projectile-luokan kääntävän asetta, 
        // koska meillä on oma UpdateSwing-logiikka.
    }

    public override void Move()
    {


        elapsed += Time.deltaTime;
        float progress = elapsed / projectileLifetime;

        if (progress >= 0.5f && !hasCleared)
        {
            alreadyHit.Clear(); // Nollataan osumat, jotta voi osua uudestaan
            hasCleared = true;
            Debug.Log("Melee re-hit enabled!");
        }

        if (progress <= 1f)
        {
            UpdateSwing(progress);
        }
        else
        {
            Disable();
        }
    } // Move

    private void UpdateSwing(float progress)
    {
        if (swingArc < 360f)
        {
            float currentAngle = Mathf.Lerp(startAngle, startAngle + swingArc, progress);
            transform.rotation = Quaternion.Euler(0, 0, currentAngle);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, startAngle + (swingArc / 2f));
        }

        if (owner != null)
        {
            transform.position = owner.transform.position;
        }
    } // UpdateSwing

    // Ylikirjoitetaan osumislogiikka, jotta ei osu samaan viholliseen monta kertaa
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) return;

        if (collision.TryGetComponent<IDamageable>(out var d))
        {
            // Jos tähän viholliseen ei ole vielä osuttu tämän lyönnin aikana
            if (!alreadyHit.Contains(d))
            {
                Unit.DealDamage(new DamageContext(owner, d, damage, true, (Vector2)direction));
                alreadyHit.Add(d); // Lisätään listalle

                OnHit();

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
    } // OnCollisionEnter
} // Class MeleeSwing