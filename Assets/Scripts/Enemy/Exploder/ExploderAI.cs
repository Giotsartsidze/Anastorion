using UnityEngine;
using System.Collections;

public class ExploderAI : MonoBehaviour
{
    public float speed = 4f;
    public float explosionRange = 2.5f;
    public float explosionDamage = 30f;
    public float countdownTime = 1.2f;
    public GameObject explosionEffect; // პატარა Particle პრეფაბი

    private Transform player;
    private bool isPrimed = false;
    private SpriteRenderer sprite;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (player == null || isPrimed) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= explosionRange)
        {
            StartCoroutine(StartDetonation());
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }
    }

    IEnumerator StartDetonation()
    {
        isPrimed = true;
        float elapsed = 0;

        // ციმციმის ლოგიკა
        while (elapsed < countdownTime)
        {
            sprite.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            sprite.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.2f;
        }

        Explode();
    }

    void Explode()
    {
        // ვამოწმებთ არის თუ არა ფლეიერი რადიუსში აფეთქების მომენტში
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= explosionRange + 1f)
        {
            player.GetComponent<PlayerHealth>()?.TakeDamage((int)explosionDamage);
        }

        // ვიზუალური ეფექტი
        if (explosionEffect != null) Instantiate(explosionEffect, transform.position, Quaternion.identity);
        
        // მტერი ქრება აფეთქებისას
        Destroy(gameObject);
    }

    // რადიუსის დახატვა ედიტორში
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}