using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 7f;
    public int damage = 10;
    private Vector2 targetDirection;

    void Start()
    {
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player != null)
        {
            // ითვლის მიმართულებას მხოლოდ გაჩენის მომენტში (არ მიჰყვება ფლეიერს)
            targetDirection = (player.position - transform.position).normalized;
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
            Destroy(gameObject);
        }
    }
}