using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;

public class BossHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    private ShakeSource shaker;
    private Slider bossSlider;
    public GameObject deathEffectPrefab;
    public GameObject skillLootPrefab;

    void Start()
    {
        currentHealth = maxHealth;
        WaveManager waveManager = FindObjectOfType<WaveManager>();
        shaker = GetComponent<ShakeSource>();
        
        if (waveManager != null && waveManager.bossHealthSlider != null)
        {
            bossSlider = waveManager.bossHealthSlider;
            bossSlider.gameObject.SetActive(true);
            bossSlider.maxValue = maxHealth;
            bossSlider.value = currentHealth;
        }
    }

    public void TakeBossDamage(int amount)
    {
        if (currentHealth <= 0) return; // რომ ბევრჯერ არ მოკვდეს ერთდროულად

        currentHealth -= amount;
        if (bossSlider != null) bossSlider.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die(); // ერთადერთი ადგილი სადაც სიკვდილი წყდება
        }
    }

    void Die()
    {
        Debug.Log("BOSS DIE FUNCTION STARTED");
        
        if (shaker != null) shaker.TriggerShake();

        if (bossSlider != null) bossSlider.gameObject.SetActive(false);

        // ეფექტის გაჩენა
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }
        
        // ლუტის გაჩენა
        if (skillLootPrefab != null)
        {
            Instantiate(skillLootPrefab, transform.position, Quaternion.identity);
        }
        
        Destroy(gameObject); // მხოლოდ აქ ვშლით ობიექტს საბოლოოდ
    }

    void OnDestroy()
    {
        if (gameObject.scene.isLoaded)
        {
            Debug.Log("BOSS OBJECT REMOVED FROM MEMORY");
        }
    }
}