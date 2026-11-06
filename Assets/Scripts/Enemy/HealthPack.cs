using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healAmount = 20;
    public float moveSpeed = 8f;
    private Transform player;
    private bool isFollowing = false;

    void Start()
    {
        GlowFx.Attach(transform, new Color(0.45f, 1f, 0.55f, 0.75f), 1.4f); // green health glow
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Magnet ეფექტი: თუ მოთამაშე ახლოსაა, გული მასთან მიფრინავს
        if (distance < 5f) isFollowing = true;

        if (isFollowing)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth ph = collision.GetComponent<PlayerHealth>();
            if (ph != null) ph.Heal(healAmount); // updates health + bar + number together

            Destroy(gameObject); // გული ქრება აღებისას
        }
    }
}