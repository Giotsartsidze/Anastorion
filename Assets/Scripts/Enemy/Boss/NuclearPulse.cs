using UnityEngine;
using System.Collections;

public class NuclearPulse : MonoBehaviour
{
    public float expandSpeed = 10f;  // გაზრდის სისწრაფე
    public float maxSize = 30f;      // მაქსიმალური ზომა (მთელ ეკრანზე)
    public LayerMask enemyLayer;     // მტრების ლეიერი

    void Start()
    {
        StartCoroutine(ExpandAndKill());
    }

    IEnumerator ExpandAndKill()
    {
        float currentSize = 0f;

        while (currentSize < maxSize)
        {
            // 1. ზომის გაზრდა
            currentSize += expandSpeed * Time.deltaTime;
            transform.localScale = new Vector3(currentSize, currentSize, 1f);

            // 2. მტრების პოვნა ამ რადიუსში
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, currentSize / 2, enemyLayer);
            foreach (var hit in hits)
            {
                // თუ ეს ჩვეულებრივი მტერია (და არა ბოსი, რომელიც უკვე კვდება)
                if (hit.CompareTag("Enemy") && hit.GetComponent<BossHealth>() == null)
                {
                    // მყისიერი სიკვდილი ეფექტით
                    Destroy(hit.gameObject);
                    // აქ შეგიძლია დაამატო პატარა აფეთქების VFX თითოეულ მტერზე
                }
            }
            
            // ფერის გაქრობა (Fading)
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            Color color = sr.color;
            color.a = 1f - (currentSize / maxSize); // რაც იზრდება, მით უფრო გამჭვირვალე ხდება
            sr.color = color;

            yield return null;
        }

        Destroy(gameObject); // ბოლოს ვშლით თვითონ ტალღას
    }
}