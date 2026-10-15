using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 100;
    public int currentHealth;
    [Tooltip("Flat damage reduction (Armor meta-upgrade). Incoming hits are floored to 1.")]
    public int armor = 0;
    [Tooltip("Multiplier on damage taken (Fragile run modifier). 1 = normal.")]
    public float damageTakenMultiplier = 1f;
    [Tooltip("Passive HP regen per second (Vitality meta-upgrade).")]
    public float regenPerSecond = 0f;
    private float regenAccum;
    [Tooltip("Free second-chances this run (Revive meta-upgrade). Revives at half HP instead of dying.")]
    public int revives = 0;

    [Header("UI References")]
    public Slider healthSlider;
    public TMP_Text healthText; // optional "80 / 100" overlay on the health bar

    [Header("I-Frame Settings")]
    public float iFrameDuration = 1f;
    private bool isInvincible = false;
    
    [Header("Juice Effects")]
    public ParticleSystem damageParticles; // ეს ხაზი აკლდა

    private SpriteRenderer sprite;
    private ShakeSource shaker;

    void Start()
    {
        currentHealth = maxHealth;
        sprite = GetComponent<SpriteRenderer>();
        shaker = GetComponent<ShakeSource>();

        // UI-ს ინიციალიზაცია
        if (healthSlider != null) healthSlider.maxValue = maxHealth;
        UpdateUI();
    }

    void Update()
    {
        // Vitality: slow passive regen while alive and hurt.
        if (regenPerSecond > 0f && currentHealth > 0 && currentHealth < maxHealth)
        {
            regenAccum += regenPerSecond * Time.deltaTime;
            if (regenAccum >= 1f)
            {
                int heal = Mathf.FloorToInt(regenAccum);
                regenAccum -= heal;
                currentHealth = Mathf.Min(currentHealth + heal, maxHealth);
                UpdateUI();
            }
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
            TakeDamage(8);
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateUI(); // refresh both the bar AND the number
    }

    public void TakeDamage(int amount)
    {
        if (isInvincible) return;

        int dmg = Mathf.Max(1, Mathf.RoundToInt(Mathf.Max(1, amount - armor) * damageTakenMultiplier)); // Armor then Fragile
        currentHealth -= dmg;
        if (SoundManager.Instance != null) SoundManager.Instance.PlayPlayerHurt();
        if (shaker != null) shaker.TriggerShake();
        if(damageParticles != null) damageParticles.Play();
    

        UpdateUI();

        if (currentHealth <= 0)
        {
            if (revives > 0)
            {
                // Second wind: spend a revive, come back at half health with i-frames.
                revives--;
                currentHealth = Mathf.Max(1, Mathf.RoundToInt(maxHealth * 0.5f));
                UpdateUI();
                if (shaker != null) shaker.TriggerShake();
                StartCoroutine(BecomeInvincible());
            }
            else
            {
                Die();
            }
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
        if (healthSlider != null) healthSlider.value = currentHealth;
        if (healthText != null) healthText.text = Mathf.Max(0, currentHealth) + " / " + maxHealth;
    }

    void Die()
    {
        if (GameOverManager.Instance != null)
            GameOverManager.Instance.ShowGameOver();
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}