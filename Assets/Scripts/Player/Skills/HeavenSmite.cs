using System.Collections.Generic;
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
    public int strikeCount = 1;       // how many enemies struck per interval
    public bool active = false;       // turned on when the Heaven's Smite card is picked
    public bool evolved = false;      // Thunderstorm evolution

    private float timer;

    /// <summary>Thunderstorm evolution: strikes several enemies at once, faster.</summary>
    public void Evolve()
    {
        if (evolved) return;
        evolved = true;
        strikeCount = 3;
        interval = Mathf.Max(0.8f, interval * 0.7f);
        splashRadius += 0.5f;
        splashDamage += 10;
    }

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
        List<Transform> struck = new List<Transform>();
        int count = Mathf.Max(1, strikeCount);

        for (int n = 0; n < count; n++)
        {
            Transform best = null;
            int bestHp = -1;
            foreach (var c in near)
            {
                if (c == null || !c.CompareTag("Enemy")) continue;
                if (struck.Contains(c.transform)) continue;

                int hp;
                if (c.GetComponent<BossHealth>() != null) hp = int.MaxValue; // boss priority
                else { var eh = c.GetComponent<EnemyHealth>(); if (eh == null) continue; hp = eh.health; }

                if (hp > bestHp) { bestHp = hp; best = c.transform; }
            }
            if (best == null) break;
            struck.Add(best);
            StrikeTarget(best);
        }
    }

    void StrikeTarget(Transform target)
    {
        Vector3 pos = target.position;

        if (boltVfxPrefab != null)
        {
            GameObject fx = Instantiate(boltVfxPrefab, pos, Quaternion.identity);
            Destroy(fx, vfxLifetime);
        }

        target.GetComponent<EnemyHealth>()?.TakeDamage(damage, pos);
        target.GetComponent<BossHealth>()?.TakeBossDamage(damage);

        Collider2D[] splash = Physics2D.OverlapCircleAll(pos, splashRadius);
        foreach (var c in splash)
        {
            if (!c.CompareTag("Enemy") || c.transform == target) continue;
            c.GetComponent<EnemyHealth>()?.TakeDamage(splashDamage, pos);
        }
    }
}
