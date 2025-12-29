using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int health = 1;
    public GameObject coinPrefab; // აქ ჩააგდებ XP Orb-ის პრეფაბს
	public int xpCount = 1;
    private bool isDying = false; // რომ ზედიზედ რამდენჯერმე არ მოკვდეს

    public void TakeDamage(int damage)
    {
        if (isDying) return;

        health -= damage;

        if (health <= 0)
        {
            StartCoroutine(DeathSequence());
        }
    }

    IEnumerator DeathSequence()
    {
        isDying = true;

        // 1. აქ მტერი უკვე მიფრინავს უკან (რადგან LightPulse-მა ძალა უკვე მისცა)
        // ვაცდით 0.15 წამს, რომ მოთამაშემ დაინახოს უკუგდება
        yield return new WaitForSeconds(0.15f);

        // 2. ვაჩენთ "კოინს" (სინათლის ნამსხვრევს)
      if (coinPrefab != null)
        {
            // ვაჩენთ იმდენ XP ორბს, რამდენიც xpCount-ში გვიწერია
            for (int i = 0; i < xpCount; i++)
            {
                // მცირე რანდომიზაცია პოზიციისთვის, რომ ორბები ერთმანეთს არ დაეფარონ
                Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
                Instantiate(coinPrefab, transform.position + randomOffset, Quaternion.identity);
            }
        }

        // 3. მტერი ქრება
        Destroy(gameObject);
    }
}