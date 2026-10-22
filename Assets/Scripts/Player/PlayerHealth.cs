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

    [Header("Revive (Second Wind)")]
    [Range(0.1f, 1f)] public float reviveHealthFraction = 0.5f;
    public float reviveIFrames = 2.5f;     // longer safety window than a normal hit
    public float reviveClearRadius = 4.5f; // shove the horde back so you don't instantly re-die
    public float reviveKnockback = 16f;
    
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
                // Second Wind: spend a revive, come back with a cleared space + long i-frames.
                revives--;
                currentHealth = Mathf.Max(1, Mathf.RoundToInt(maxHealth * reviveHealthFraction));
                UpdateUI();
                if (shaker != null) shaker.TriggerShake();
                ClearSpaceOnRevive();
                StartCoroutine(BecomeInvincible(reviveIFrames));
            }
            else
            {
                Die();
            }
        }
        else
        {
            StartCoroutine(BecomeInvincible(iFrameDuration));
        }
    }

    // Shove every nearby enemy away so a revive isn't instantly undone by the horde.
    void ClearSpaceOnRevive()
    {
        Collider2D[] near = Physics2D.OverlapCircleAll(transform.position, reviveClearRadius);
        foreach (var c in near)
        {
            if (c == null || !c.CompareTag("Enemy")) continue;
            Rigidbody2D erb = c.GetComponent<Rigidbody2D>();
            if (erb != null)
            {
                Vector2 dir = ((Vector2)c.transform.position - (Vector2)transform.position).normalized;
                if (dir.sqrMagnitude < 0.0001f) dir = Vector2.up;
                erb.AddForce(dir * reviveKnockback, ForceMode2D.Impulse);
            }
        }
    }

    IEnumerator BecomeInvincible(float duration)
    {
        isInvincible = true;

        for (float i = 0; i < duration; i += 0.2f)
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