using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Cinemachine;

public class BossHealth : MonoBehaviour
{
    public int maxHealth = 1000;
    [Tooltip("Name shown above the boss health bar.")]
    public string bossName = "GVELESHAPI";
    [Tooltip("Shards awarded when the boss dies (on top of the loot drop).")]
    public int shardReward = 50;
    private int currentHealth;
    /// <summary>Remaining health as 0..1, for the AI's enrage / phase logic.</summary>
    public float HealthPct => maxHealth > 0 ? Mathf.Clamp01((float)currentHealth / maxHealth) : 0f;
    private ShakeSource shaker;
    private bool useHud;             // drive the gold HUD boss bar instead of the old slider
    private Slider bossSlider;       // legacy scene bar (fallback only)
    private TMP_Text bossText;
    private GameObject bossBarRoot;
    public GameObject deathEffectPrefab;
    [Tooltip("Dropped on death — assign the Relic prefab for a guaranteed upgrade reward.")]
    public GameObject skillLootPrefab;

    void Start()
    {
        currentHealth = maxHealth;
        shaker = GetComponent<ShakeSource>();
        useHud = GameHud.Instance != null;

        // Primary: the code-driven gold HUD boss bar.
        if (useHud) GameHud.Instance.ShowBoss(bossName, currentHealth, maxHealth);

        // The old plain scene bar: hide it when the HUD is present, otherwise use it as a fallback.
        WaveManager waveManager = FindObjectOfType<WaveManager>();
        if (waveManager != null)
        {
            bossBarRoot = waveManager.bossBarRoot;
            bossSlider  = waveManager.bossHealthSlider;
            bossText    = waveManager.bossHealthText;

            if (useHud)
            {
                if (bossBarRoot != null) bossBarRoot.SetActive(false);
                if (bossSlider  != null) bossSlider.gameObject.SetActive(false);
                if (bossText    != null) bossText.gameObject.SetActive(false);
            }
            else
            {
                if (bossBarRoot != null) bossBarRoot.SetActive(true);
                if (bossSlider != null)
                {
                    bossSlider.gameObject.SetActive(true);
                    bossSlider.maxValue = maxHealth;
                    bossSlider.value = currentHealth;
                }
                if (bossText != null)
                {
                    bossText.gameObject.SetActive(true);
                    bossText.text = currentHealth + " / " + maxHealth;
                }
            }
        }
    }

    public void TakeBossDamage(int amount)
    {
        if (currentHealth <= 0) return; // რომ ბევრჯერ არ მოკვდეს ერთდროულად

        currentHealth -= amount;

        if (useHud && GameHud.Instance != null)
            GameHud.Instance.UpdateBoss(currentHealth, maxHealth);
        else
        {
            if (bossSlider != null) bossSlider.value = currentHealth;
            if (bossText != null) bossText.text = Mathf.Max(0, currentHealth) + " / " + maxHealth;
        }

        if (currentHealth <= 0)
        {
            Die(); // ერთადერთი ადგილი სადაც სიკვდილი წყდება
        }
    }

    void Die()
    {

        if (MusicManager.Instance != null) MusicManager.Instance.PlayDefault();
        if (shaker != null) shaker.TriggerShake();

        // The boss is the demo's win condition — defeating it triggers Victory.
        // Robust path: works even if the scene's VictoryManager is disabled/unwired.
        VictoryManager.ReportBossDefeated();

        // let outside waves resume (safety — Victory normally freezes the game anyway)
        var wm = FindObjectOfType<WaveManager>();
        if (wm != null) wm.NotifyBossDefeated();

        // hide the boss bar (HUD primary, scene bar fallback)
        if (GameHud.Instance != null) GameHud.Instance.HideBoss();
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