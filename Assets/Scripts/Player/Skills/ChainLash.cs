using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Titan's Chains — periodically whips the broken shackles at the nearest enemies,
/// dealing damage and YANKING them toward the player (drags the horde into your
/// other skills). On-theme for the chained fire-titan. Put on the Player; starts
/// inactive, unlocked/scaled by the Chain Lash upgrade card. Draws its own chain
/// line at runtime, so no prefab is required (assign one later if you want prettier).
/// </summary>
public class ChainLash : MonoBehaviour
{
    public float interval = 2f;
    public float range = 7f;
    public int damage = 10;
    public int lashCount = 1;          // enemies hit per swing (raised by picks)
    public float pullForce = 6f;       // yank toward the player
    public float vfxLifetime = 0.15f;
    public Color chainColor = new Color(0.78f, 0.80f, 0.90f, 1f);
    public bool active = false;        // turned on when the Chain Lash card is picked
    public bool evolved = false;       // Titan's Wrath evolution

    private float timer;
    private static Material lineMat;

    /// <summary>Titan's Wrath: lashes a whole crowd at once, faster and farther.</summary>
    public void Evolve()
    {
        if (evolved) return;
        evolved = true;
        lashCount += 3;
        damage += 12;
        interval = Mathf.Max(0.5f, interval * 0.6f);
        range += 2f;
        pullForce += 4f;
    }

    void Update()
    {
        if (!active) return;
        timer += Time.deltaTime;
        if (timer >= interval) { timer = 0f; Lash(); }
    }

    void Lash()
    {
        Collider2D[] near = Physics2D.OverlapCircleAll(transform.position, range);
        List<Transform> chosen = new List<Transform>();
        int want = Mathf.Max(1, lashCount);

        for (int n = 0; n < want; n++)
        {
            Transform best = null;
            float bestSqr = float.MaxValue;
            foreach (var c in near)
            {
                if (c == null || !c.CompareTag("Enemy") || chosen.Contains(c.transform)) continue;
                float d = ((Vector2)c.transform.position - (Vector2)transform.position).sqrMagnitude;
                if (d < bestSqr) { bestSqr = d; best = c.transform; }
            }
            if (best == null) break;
            chosen.Add(best);
            LashTarget(best);
        }
    }

    void LashTarget(Transform target)
    {
        target.GetComponent<EnemyHealth>()?.TakeDamage(damage, transform.position);
        target.GetComponent<BossHealth>()?.TakeBossDamage(damage);

        Rigidbody2D rb = target.GetComponent<Rigidbody2D>();
        if (rb != null && pullForce > 0f)
        {
            Vector2 dir = ((Vector2)transform.position - (Vector2)target.position).normalized;
            rb.AddForce(dir * pullForce, ForceMode2D.Impulse); // yank it in
        }

        SpawnChainVfx(transform.position, target.position);
    }

    void SpawnChainVfx(Vector3 a, Vector3 b)
    {
        GameObject go = new GameObject("ChainLashVfx");
        LineRenderer lr = go.AddComponent<LineRenderer>();
        if (lineMat == null)
        {
            Shader sh = Shader.Find("Sprites/Default");
            if (sh != null) lineMat = new Material(sh);
        }
        if (lineMat != null) lr.material = lineMat;
        lr.positionCount = 2;
        lr.SetPosition(0, a);
        lr.SetPosition(1, b);
        lr.startWidth = 0.18f;
        lr.endWidth = 0.10f;
        lr.numCapVertices = 2;
        lr.startColor = chainColor;
        lr.endColor = chainColor;
        lr.sortingOrder = 50;
        Destroy(go, vfxLifetime);
    }
}
