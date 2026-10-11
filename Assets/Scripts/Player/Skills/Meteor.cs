using UnityEngine;

/// <summary>
/// A single falling meteor: streaks down to its target point, then deals AoE damage
/// on impact and destroys itself. Spawned by Starfall.
/// Put on the meteor prefab (SpriteRenderer using meteor.png).
/// </summary>
public class Meteor : MonoBehaviour
{
    public float fallSpeed = 22f;
    public GameObject impactVfxPrefab; // optional burst at impact (e.g. supernova sprite)

    private Vector3 target;
    private int damage;
    private float radius;
    private bool ready;

    public void Init(Vector3 target, int damage, float radius)
    {
        this.target = target;
        this.damage = damage;
        this.radius = radius;
        ready = true;
    }

    void Update()
    {
        if (!ready) return;

        transform.position = Vector3.MoveTowards(transform.position, target, fallSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, target) < 0.05f)
            Impact();
    }

    void Impact()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(target, radius);
        foreach (var c in hits)
        {
            if (!c.CompareTag("Enemy")) continue;
            c.GetComponent<EnemyHealth>()?.TakeDamage(damage, target);
            c.GetComponent<BossHealth>()?.TakeBossDamage(damage);
        }

        if (impactVfxPrefab != null) Instantiate(impactVfxPrefab, target, Quaternion.identity);
        Destroy(gameObject);
    }
}
