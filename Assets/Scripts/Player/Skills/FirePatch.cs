using System.Collections;
using UnityEngine;

/// <summary>
/// A single lingering fire patch: damages enemies in its radius on a tick, fades
/// out over its lifetime, then destroys itself. Spawned by FireTrail.
/// Put on the fire-patch prefab (with a SpriteRenderer using fire_patch.png).
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class FirePatch : MonoBehaviour
{
    public int damagePerTick = 2;
    public float radius = 1.2f;
    public float duration = 3f;
    public float tickInterval = 0.5f;

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        transform.localScale = Vector3.one * (radius * 2f); // visual roughly matches hit radius
        StartCoroutine(Burn());
    }

    IEnumerator Burn()
    {
        float elapsed = 0f;
        float tick = tickInterval; // tick immediately on spawn
        Color baseColor = sr.color;

        while (elapsed < duration)
        {
            float dt = Time.deltaTime;
            elapsed += dt;
            tick += dt;

            if (tick >= tickInterval)
            {
                tick = 0f;
                Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
                foreach (var c in hits)
                {
                    if (!c.CompareTag("Enemy")) continue;
                    c.GetComponent<EnemyHealth>()?.TakeDamage(damagePerTick, transform.position);
                    c.GetComponent<BossHealth>()?.TakeBossDamage(damagePerTick);
                }
            }

            Color col = baseColor;
            col.a = Mathf.Lerp(baseColor.a, 0f, elapsed / duration);
            sr.color = col;
            yield return null;
        }

        Destroy(gameObject);
    }
}
