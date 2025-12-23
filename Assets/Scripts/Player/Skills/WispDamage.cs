using UnityEngine;

public class WispDamage : MonoBehaviour
{
    public int damage = 2; // ბურთულების ზიანი

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // ჩვეულებრივი მტერი
            collision.GetComponent<EnemyHealth>()?.TakeDamage(damage);
            // ბოსი
            collision.GetComponent<BossHealth>()?.TakeBossDamage(damage);
            
            // აქ არ ვწერთ Destroy(gameObject)! ბურთულა რჩება.
        }
    }
}