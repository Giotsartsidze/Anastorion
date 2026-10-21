using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 5f; // slower than before (7) so it's actually dodgeable
    public int damage = 6;
    [Tooltip("Enlarges the VISUAL so the shot is easy to see. The hitbox is kept the same size.")]
    public float visualScale = 1.8f;
    [Tooltip("Spawned where the projectile hits the player (e.g. a poison splat).")]
    public GameObject hitVfxPrefab;
    private Vector2 targetDirection;

    void Start()
    {
        // Make the shot read clearly without making it unfair: scale up the sprite,
        // then shrink the circle collider back so the real hitbox is unchanged.
        if (visualScale > 1f)
        {
            transform.localScale *= visualScale;
            CircleCollider2D cc = GetComponent<CircleCollider2D>();
            if (cc != null) cc.radius /= visualScale;
        }

        GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO != null)
        {
            // ითვლის მიმართულებას მხოლოდ გაჩენის მომენტში (არ მიჰყვება ფლეიერს)
            targetDirection = (playerGO.transform.position - transform.position).normalized;
        }
        Destroy(gameObject, 5f); // 5 წამში გაქრეს, რომ მეხსიერება არ გაავსოს
    }

    void Update()
    {
        transform.Translate(targetDirection * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            if (hitVfxPrefab != null)
                Instantiate(hitVfxPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}