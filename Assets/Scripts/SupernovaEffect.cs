using UnityEngine;

public class SupernovaEffect : MonoBehaviour
{
    public int damage = 15;
    public float knockbackForce = 10f;
    public float duration = 0.5f; // რამდენ ხანს ჩანდეს აფეთქება

    void Start()
    {
        // აფეთქება ჩნდება და 0.5 წამში ქრება
        Destroy(gameObject, duration);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // ზიანის მიყენება
            collision.GetComponent<EnemyHealth>()?.TakeDamage(damage);
            collision.GetComponent<BossHealth>()?.TakeBossDamage(damage);

            // ძლიერი Knockback
            Rigidbody2D enemyRb = collision.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                Vector2 dir = (collision.transform.position - transform.position).normalized;
                enemyRb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);
            }
        }
    }
}