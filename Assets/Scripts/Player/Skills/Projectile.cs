using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 1;

    void Update()
    {
        // ისარი მუდმივად მიფრინავს წინ (მარჯვნივ თავისი ლოკალური ღერძით)
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // თუ მოთამაშეს მოხვდა (ბოსის ნასროლი)
        if (collision.CompareTag("Player"))
        {
            PlayerHealth player = collision.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(10); // დავაკლოთ 10 HP
                Destroy(gameObject);   // ისარი ქრება
            }
        }

        // თუ მტერს მოხვდა (მოთამაშის ნასროლი)
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<EnemyHealth>()?.TakeDamage(damage);
            collision.GetComponent<BossHealth>()?.TakeBossDamage(damage);
            Destroy(gameObject);
        }
    }
}