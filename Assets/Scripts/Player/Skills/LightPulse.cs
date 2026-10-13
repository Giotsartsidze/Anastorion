using System.Collections;
using System.Collections.Generic; // ეს დაამატე HashSet-ისთვის
using UnityEngine;

public class LightPulse : MonoBehaviour
{
    public GameObject pulseVisual;
    public float maxRadius = 5f;
    public float pulseDuration = 0.5f;
    public float cooldown = 3f;
    public float knockbackForce = 2f;
    public LayerMask enemyLayer;
    [Tooltip("Starts OFF — unlocked when the Pulse upgrade card is picked.")]
    public bool active = false;

    private float timer;
    private SpriteRenderer pulseSr;

    void Start()
    {
        pulseVisual.SetActive(false);
        pulseSr = pulseVisual.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!active) return; // no longer fires from the start of the run

        timer += Time.deltaTime;
        if (timer >= cooldown)
        {
            StartCoroutine(PerformPulse());
            timer = 0;
        }
    }

    IEnumerator PerformPulse()
    {
        pulseVisual.SetActive(true);
        pulseVisual.transform.localScale = Vector3.zero;

        // --- სიახლე: ვქმნით სიას, ვინც უკვე "გავლახეთ" ამ პულსზე ---
        HashSet<Collider2D> hitEnemies = new HashSet<Collider2D>();

        float elapsed = 0;
        Color baseCol = pulseSr != null ? pulseSr.color : Color.white;

        while (elapsed < pulseDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / pulseDuration;

            // Ease-out burst: snaps out fast, then eases to full size (reads as a shockwave).
            float eased = 1f - Mathf.Pow(1f - progress, 3f);
            float currentSize = Mathf.Lerp(0, maxRadius * 2, eased);
            pulseVisual.transform.localScale = new Vector3(currentSize, currentSize, 1);

            // Fade the ring out as it expands so it dissipates instead of popping off.
            if (pulseSr != null)
            {
                Color c = baseCol;
                c.a = Mathf.Lerp(baseCol.a, 0f, progress);
                pulseSr.color = c;
            }

            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, currentSize / 2, enemyLayer);
            
            foreach (var enemy in enemies)
            {
                // თუ მტერს ჯერ არ მოხვედრია ამ პულსის დროს
                if (!hitEnemies.Contains(enemy))
                {
                    ApplyKnockback(enemy);
                    hitEnemies.Add(enemy); // ვამატებთ სიაში
                }
            }

            yield return null;
        }

        if (pulseSr != null) pulseSr.color = baseCol; // restore alpha for the next pulse
        pulseVisual.SetActive(false);
    }

    void ApplyKnockback(Collider2D enemy)
    {
        Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
        if (enemyRb != null)
        {
            Vector2 direction = (enemy.transform.position - transform.position).normalized;
            enemyRb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
        }

        // ზიანი ჩვეულებრივი მტრისთვის
        EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(4);
        }

        // სპეციალური შემოწმება ბოსისთვის (რომ პულსმა ბოსს ბევრი არ დააკლოს)
        BossHealth bossHealth = enemy.GetComponent<BossHealth>();
        if (bossHealth != null)
        {
            // ბოსს შეგიძლია სულ სხვა ზიანი მიაყენო, მაგალითად 2
            bossHealth.TakeBossDamage(2); 
        }
    }
}