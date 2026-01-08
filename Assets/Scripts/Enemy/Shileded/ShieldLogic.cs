using UnityEngine;
using System.Collections;

public class ShieldLogic : MonoBehaviour
{
    [Header("Shield Stats")]
    public int maxShieldHealth = 3; // ფარის გამძლეობა
    private int currentShieldHealth;

    private SpriteRenderer sprite;
    private Color originalColor;

    void Start()
    {
        currentShieldHealth = maxShieldHealth;
        sprite = GetComponent<SpriteRenderer>();
        if (sprite != null) originalColor = sprite.color;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ვამოწმებთ ტყვიასთან შეხებას
        if (collision.CompareTag("Projectile"))
        {
            // 1. ვანადგურებთ ტყვიას
            Destroy(collision.gameObject);

            // 2. ვაკლებთ ფარს სიცოცხლეს
            TakeShieldDamage(1);
        }
    }

    void TakeShieldDamage(int damage)
    {
        currentShieldHealth -= damage;

        // ვიზუალური ეფექტი: ფარი წითლდება დარტყმისას
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(FlashEffect());
        }

        if (currentShieldHealth <= 0)
        {
            BreakShield();
        }
    }

    IEnumerator FlashEffect()
    {
        if (sprite == null) yield break;
        
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sprite.color = originalColor;
    }

    void BreakShield()
    {
        Debug.Log("Shield Destroyed!");
        // აქ შეგიძლია დაამატო ხმა ან ნამსხვრევების ეფექტი
        // ჩვენ უბრალოდ ვთიშავთ ფარს
        gameObject.SetActive(false);
    }
}