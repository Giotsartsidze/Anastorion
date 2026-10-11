using UnityEngine;

/// <summary>
/// Heaven's Smite — periodically calls down a lightning bolt on the biggest threat
/// in range (boss first, else the highest-HP enemy), with a small splash. A focused,
/// bursty "elite/boss killer". Put on the Player, assign the bolt VFX prefab.
/// Starts inactive; unlocked/scaled by its upgrade card.
/// </summary>
public class HeavenSmite : MonoBehaviour
{
    public GameObject boltVfxPrefab;  // lightning_strike VFX (shown briefly at the target)
    public float interval = 2.5f;
    public float range = 10f;
    public int damage = 25;           // big single-target hit
    public float splashRadius = 1.5f; // small AoE around the strike
    public int splashDamage = 8;
    public float vfxLifetime = 0.3f;
    public bool active = false;       // turned on when the Heaven's Smite card is picked

    private float timer;

    void Update()
    {
        if (!active) return;

        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer = 0f;
            Strike();
        }
    }

    void Strike()
    {
        Collider2D[] near = Physics2D.OverlapCircleAll(transform.position, range);

        Transform best = null;
        int bestHp = -1;
        bool bestIsBoss = false;

        foreach (var c in near)
        {
            if (!c.CompareTag("Enemy")) continue;

            if (c.GetComponent<BossHealth>() != null)   // boss always wins priority
            {
                best = c.transform; bestIsBoss = true; break;
            }
            var eh = c.GetComponent<EnemyHealth>();
            if (!bestIsBoss && eh != null && eh.health > bestHp)
            {
                bestHp = eh.health; best = c.transform;
            }
        }

        if (best == null) return;
        Vector3 pos = best.position;

        if (boltVfxPrefab != null)
        {
            GameObject fx = Instantiate(boltVfxPrefab, pos, Quaternion.identity);
            Destroy(fx, vfxLifetime);
        }

        // main hit
        best.GetComponent<EnemyHealth>()?.TakeDamage(damage, pos);
        best.GetComponent<BossHealth>()?.TakeBossDamage(damage);

        // splash
        Collider2D[] splash = Physics2D.OverlapCircleAll(pos, splashRadius);
        foreach (var c in splash)
        {
            if (!c.CompareTag("Enemy") || c.transform == best) continue;
            c.GetComponent<EnemyHealth>()?.TakeDamage(splashDamage, pos);
        }
    }
}
