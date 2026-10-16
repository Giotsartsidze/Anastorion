using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Cinemachine;

public class BossHealth : MonoBehaviour
{
    public int maxHealth = 1000;
    [Tooltip("Shards awarded when the boss dies (on top of the loot drop).")]
    public int shardReward = 50;
    private int currentHealth;
    private ShakeSource shaker;
    private Slider bossSlider;
    private TMP_Text bossText;
    private GameObject bossBarRoot;
    public GameObject deathEffectPrefab;
    [Tooltip("Dropped on death — assign the Relic prefab for a guaranteed upgrade reward.")]
    public GameObject skillLootPrefab;

    void Start()
    {
        currentHealth = maxHealth;
        WaveManager waveManager = FindObjectOfType<WaveManager>();
        shaker = GetComponent<ShakeSource>();

        if (waveManager != null)
        {
            bossBarRoot = waveManager.bossBarRoot;
            if (bossBarRoot != null) bossBarRoot.SetActive(true);

            if (waveManager.bossHealthSlider != null)
            {
                bossSlider = waveManager.bossHealthSlider;
                bossSlider.gameObject.SetActive(true);
                bossSlider.maxValue = maxHealth;
                bossSlider.value = currentHealth;
            }

            bossText = waveManager.bossHealthText;
            if (bossText != null)
            {
                bossText.gameObject.SetActive(true);
                bossText.text = currentHealth + " / " + maxHealth;
            }
        }
    }

    public void TakeBossDamage(int amount)
    {
        if (currentHealth <= 0) return; // რომ ბევრჯერ არ მოკვდეს ერთდროულად

        currentHealth -= amount;
        if (bossSlider != null) bossSlider.value = currentHealth;
        if (bossText != null) bossText.text = Mathf.Max(0, currentHealth) + " / " + maxHealth;

        if (currentHealth <= 0)
        {
            Die(); // ერთადერთი ადგილი სადაც სიკვდილი წყდება
        }
    }

    void Die()
    {

        if (MusicManager.Instance != null) MusicManager.Instance.PlayDefault();
        if (shaker != null) shaker.TriggerShake();

        // hide the whole bar (root covers slider + frame + text); fall back to parts
        if (bossBarRoot != null) bossBarRoot.SetActive(false);
        if (bossSlider != null) bossSlider.gameObject.SetActive(false);
        if (bossText != null) bossText.gameObject.SetActive(false);

        // ეფექტის გაჩენა
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        // meaningful reward: the loot (assign a Relic) + a shard payout
        if (skillLootPrefab != null)
        {
            Instantiate(skillLootPrefab, transform.position, Quaternion.identity);
        }
        if (shardReward > 0 && CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.AddShards(shardReward);
        }

        Destroy(gameObject); // მხოლოდ აქ ვშლით ობიექტს საბოლოოდ
    }

    void OnDestroy()
    {
        if (gameObject.scene.isLoaded)
        {
        }
    }
}