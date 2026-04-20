using UnityEngine;
using System.Collections;

public class ChargerEnemyAI : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float chargeSpeed = 15f;
    public float chargeRange = 6f;
    public float prepareTime = 1.2f;
    public float cooldownTime = 2f;

    private Transform player;
    private bool isCharging = false;
    private bool isPreparing = false;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (player == null || isCharging || isPreparing) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= chargeRange)
        {
            StartCoroutine(PerformCharge());
        }
        else
        {
            // ჩვეულებრივი ნელი დევნა
            transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        }
    }

    IEnumerator PerformCharge()
    {
        isPreparing = true;
        
        // ვიზუალური გაფრთხილება: გაწითლდეს მომზადებისას
        Color originalColor = sprite.color;
        sprite.color = Color.red;

        // გაჩერდეს მომზადების დროს
        Vector2 targetDir = (player.position - transform.position).normalized;
        yield return new WaitForSeconds(prepareTime);

        sprite.color = originalColor;
        isPreparing = false;
        isCharging = true;

        // რეალური შეტევა (Charge)
        rb.linearVelocity = targetDir * chargeSpeed;
        
        // ეკრანის მცირე ზანზარი შეჯახებისას (თუ ShakeSource გაქვს)
        GetComponent<ShakeSource>()?.TriggerShake();

        yield return new WaitForSeconds(0.5f); // შეტევის ხანგრძლივობა

        rb.linearVelocity = Vector2.zero;
        isCharging = false;

        // დასვენების ფაზა (Cooldown) — isPreparing prevents a new charge from starting
        isPreparing = true;
        yield return new WaitForSeconds(cooldownTime);
        isPreparing = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isCharging)
        {
            collision.GetComponent<PlayerHealth>()?.TakeDamage(20); // მეტი ზიანი ვიდრე ჩვეულებრივ მტერს
        }
    }
}