using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public enum DropType { XP, Health }

    [Header("Stats")]
    public int health = 1;
    public DropType dropType = DropType.XP;

    [Header("Drop Prefabs")]
    public GameObject coinPrefab;        // XP
    public GameObject healthPackPrefab;  // სიცოცხლე
    public GameObject shardPrefab;      // მუდმივი ქოინი (Energy Shard)
    public int dropCount = 1;
    [Range(0, 1)] public float shardDropChance = 0.2f; // 20% შანსი

    private bool isDying = false;

    void Start()
    {
        if (DifficultyManager.Instance == null)
            DifficultyManager.Instance = FindFirstObjectByType<DifficultyManager>();

        if (DifficultyManager.Instance != null)
        {
            float multiplier = DifficultyManager.Instance.GetDifficultyMultiplier();
            health = Mathf.RoundToInt(health * multiplier);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDying) return;
        health -= damage;
        if (health <= 0) StartCoroutine(DeathSequence());
    }

    IEnumerator DeathSequence()
    {
        isDying = true;
        yield return new WaitForSeconds(0.15f);

        // 1. ვაჩენთ XP-ს ან სიცოცხლეს
        GameObject prefabToSpawn = (dropType == DropType.Health) ? healthPackPrefab : coinPrefab;
        if (prefabToSpawn != null)
        {
            for (int i = 0; i < dropCount; i++)
            {
                Vector3 offset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
                Instantiate(prefabToSpawn, transform.position + offset, Quaternion.identity);
            }
        }

        // 2. ვაჩენთ მუდმივ ქოინს (Shard) შანსის მიხედვით
        if (shardPrefab != null && Random.value <= shardDropChance)
        {
            Instantiate(shardPrefab, transform.position, Quaternion.identity);
        }

        health = 1; // ან ბაზისური HP
    	isDying = false;
    	gameObject.SetActive(false);
    }
}