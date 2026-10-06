using UnityEngine;

public class SupernovaEffect : MonoBehaviour
{
    public int damage = 25;
    public float radius = 4.5f;        // blast radius — tune to match the visual size
    public float knockbackForce = 10f;
    public float duration = 0.5f;      // how long the burst is visible

    void Start()
    {
        // Instant AoE on spawn: hit EVERYTHING in radius right now (OnTriggerEnter
        // missed enemies already inside the blast — this is reliable).
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (var c in hits)
        {
            if (!c.CompareTag("Enemy")) continue;

            c.GetComponent<EnemyHealth>()?.TakeDamage(damage, transform.position);
            c.GetComponent<BossHealth>()?.TakeBossDamage(damage);

            Rigidbody2D rb = c.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 dir = ((Vector2)c.transform.position - (Vector2)transform.position).normalized;
                rb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);
            }
        }

        Destroy(gameObject, duration);
    }
}
