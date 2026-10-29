using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 1;
    [Tooltip("Return to the pool after this long if it hasn't hit anything (so strays don't pile up).")]
    public float life = 3f;

    private float timer;

    void OnEnable() => timer = 0f; // pooling-safe reset

    void Update()
    {
        // ისარი მუდმივად მიფრინავს წინ (მარჯვნივ თავისი ლოკალური ღერძით)
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        timer += Time.deltaTime;
        if (timer >= life) SimplePool.Despawn(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // თუ მოთამაშეს მოხვდა (ბოსის ნასროლი)
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerHealth>()?.TakeDamage(10);
            SimplePool.Despawn(gameObject);
            return;
        }

        // თუ მტერს მოხვდა (მოთამაშის ნასროლი)
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<EnemyHealth>()?.TakeDamage(damage, transform.position);
            collision.GetComponent<BossHealth>()?.TakeBossDamage(damage);
            SimplePool.Despawn(gameObject);
        }
    }
}
