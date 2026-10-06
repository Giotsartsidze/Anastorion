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
    [Range(0, 1)] public float shardDropChance = 0.08f; // 20% შანსი

    [Header("Juice")]
    [Tooltip("Optional particle/effect spawned at death (e.g. a pop/burst).")]
    public GameObject deathEffectPrefab;
    [Tooltip("Freeze-frame on death. Leave OFF for trash mobs; enable for elites/boss.")]
    public bool useHitStopOnDeath = false;
    public float hitStopDuration = 0.08f;
    [Tooltip("How hard hits shove this enemy back. 0 = no knockback.")]
    public float knockbackForce = 10f;
    [Tooltip("Screen-shake strength on death. Throttled globally so swarms don't rumble.")]
    public float deathShakeForce = 0.3f;
    [Tooltip("Length of the death pop animation (scale-punch + fade), seconds.")]
    public float deathPopDuration = 0.15f;

    private bool isDying = false;
    private HitFlash hitFlash;
    private Knockback knockback;
    private DeathPop deathPop;
    private CharacterAnimator characterAnimator;

    void Start()
    {
        if (DifficultyManager.Instance == null)
            DifficultyManager.Instance = FindFirstObjectByType<DifficultyManager>();

        if (DifficultyManager.Instance != null)
        {
            float multiplier = DifficultyManager.Instance.GetDifficultyMultiplier();
            health = Mathf.RoundToInt(health * multiplier);
        }

        // Auto-wire feel components so every enemy gets them with zero prefab setup.
        hitFlash = GetComponent<HitFlash>();
        if (hitFlash == null) hitFlash = gameObject.AddComponent<HitFlash>();
        knockback = GetComponent<Knockback>();
        if (knockback == null) knockback = gameObject.AddComponent<Knockback>();
        deathPop = GetComponent<DeathPop>();
        if (deathPop == null) deathPop = gameObject.AddComponent<DeathPop>();
        // NOT auto-added: frame sheets must be assigned per prefab in the Inspector.
        characterAnimator = GetComponent<CharacterAnimator>();
    }

    // Kept for callers that don't know the hit's origin (no knockback direction).
    public void TakeDamage(int damage)
    {
        TakeDamage(damage, transform.position);
    }

    public void TakeDamage(int damage, Vector2 hitSource)
    {
        if (isDying) return;
        health -= damage;

        if (SoundManager.Instance != null) SoundManager.Instance.PlayEnemyHit();
        if (hitFlash != null) hitFlash.Flash();
        if (DamagePopupSpawner.Instance != null)
            DamagePopupSpawner.Instance.Spawn(transform.position, damage);

        if (knockback != null && knockbackForce > 0f)
        {
            Vector2 dir = (Vector2)transform.position - hitSource;
            if (dir.sqrMagnitude > 0.0001f) knockback.Apply(dir, knockbackForce);
        }

        if (health <= 0) StartCoroutine(DeathSequence());
    }

    IEnumerator DeathSequence()
    {
        isDying = true;

        if (SoundManager.Instance != null) SoundManager.Instance.PlayEnemyDeath();
        if (deathEffectPrefab != null)
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        if (useHitStopOnDeath && HitStop.Instance != null)
            HitStop.Instance.Stop(hitStopDuration);
        if (deathShakeForce > 0f && ScreenShake.Instance != null)
            ScreenShake.Instance.Shake(deathShakeForce);

        // Death frames take priority; otherwise the scale-pop; otherwise a flat wait.
        if (characterAnimator != null && characterAnimator.HasDeath)
        {
            characterAnimator.PlayDeath();
            yield return new WaitForSeconds(characterAnimator.DeathDuration);
        }
        else if (deathPop != null)
            yield return StartCoroutine(deathPop.Play(deathPopDuration));
        else
            yield return new WaitForSeconds(deathPopDuration);

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

        if (RunStats.Instance != null) RunStats.Instance.RegisterKill();

        health = 1; // ან ბაზისური HP
        isDying = false;
        gameObject.SetActive(false); // must be last — coroutine stops here
    }
}