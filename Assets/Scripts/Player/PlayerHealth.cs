using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI References")]
    public Slider healthSlider;

    [Header("I-Frame Settings")]
    public float iFrameDuration = 1f;
    private bool isInvincible = false;
    
    [Header("Juice Effects")]
    public ParticleSystem damageParticles; // ეს ხაზი აკლდა
    private CameraShake cameraShake;

    private SpriteRenderer sprite;

    void Start()
    {
        currentHealth = maxHealth;
        sprite = GetComponent<SpriteRenderer>();

        // UI-ს ინიციალიზაცია
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    // --- ფიზიკური შეხების დაფიქსირება ---
    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckDamage(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        CheckDamage(other.gameObject);
    }

    void CheckDamage(GameObject go)
    {
        if (go.CompareTag("Enemy"))
        {
            TakeDamage(10);
        }
    }

    public void TakeDamage(int amount)
    {
        if (isInvincible) return;

        currentHealth -= amount;
    
        if(damageParticles != null) damageParticles.Play();
    
        if(cameraShake != null) StartCoroutine(cameraShake.Shake(0.15f, 0.2f));

        UpdateUI();

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(BecomeInvincible());
        }
    }

    IEnumerator BecomeInvincible()
    {
        isInvincible = true;
        
        for (float i = 0; i < iFrameDuration; i += 0.2f)
        {
            sprite.color = new Color(1, 1, 1, 0.2f);
            yield return new WaitForSeconds(0.1f);
            sprite.color = new Color(1, 1, 1, 1f);
            yield return new WaitForSeconds(0.1f);
        }

        isInvincible = false;
    }

    void UpdateUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }

    void Die()
    {
        Debug.Log("Anastorion has faded...");
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}