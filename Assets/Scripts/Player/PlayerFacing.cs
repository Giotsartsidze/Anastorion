using UnityEngine;

/// <summary>
/// Turns the player sprite to face where it matters: toward the nearest enemy
/// (so he reads as "aiming" his darts), and toward movement when no enemy is near.
/// Front-facing sprite, so we just mirror flipX — the standard survivors-like trick.
/// Put this on the Player; it auto-finds the SpriteRenderer and Rigidbody2D.
/// </summary>
public class PlayerFacing : MonoBehaviour
{
    public SpriteRenderer sr;
    [Tooltip("How far to look for an enemy to aim at.")]
    public float scanRange = 12f;
    [Tooltip("Ignore tiny nudges so the sprite doesn't jitter when nearly still.")]
    public float deadzone = 0.05f;
    [Tooltip("Seconds between enemy scans (cheap throttle).")]
    public float scanInterval = 0.1f;

    private Rigidbody2D rb;
    private float scanTimer;
    private Transform target;

    void Awake()
    {
        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        scanTimer -= Time.deltaTime;
        if (scanTimer <= 0f)
        {
            scanTimer = scanInterval;
            target = NearestEnemy();
        }

        float faceX;
        if (target != null)
            faceX = target.position.x - transform.position.x;   // aim at the enemy
        else
            faceX = rb != null ? rb.linearVelocity.x : 0f;       // else look where we move

        if (sr != null && Mathf.Abs(faceX) > deadzone)
            sr.flipX = faceX < 0f;
    }

    Transform NearestEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, scanRange);
        Transform best = null;
        float bestSqr = float.MaxValue;
        foreach (var c in hits)
        {
            if (c == null || !c.CompareTag("Enemy")) continue;
            float d = ((Vector2)c.transform.position - (Vector2)transform.position).sqrMagnitude;
            if (d < bestSqr) { bestSqr = d; best = c.transform; }
        }
        return best;
    }
}
