using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public enum DropType { XP, Health }

    [Header("Stats")]
    public int health = 1;
    [Tooltip("Damage this enemy deals to the player on contact (tanks/big foes should hurt more).")]
    public int contactDamage = 8;
    public DropType dropType = DropType.XP;

    [Header("Drop Prefabs")]
    public GameObject coinPrefab;        // XP
    public GameObject healthPackPrefab;  // სიცოცხლე
    public GameObject shardPrefab;      // მუდმივი ქოინი (Energy Shard)
    public int dropCount = 1;
    [Range(0, 1)] public float shardDropChance = 0.08f; // 20% შანსი
    [Tooltip("Global chance for ANY enemy to also drop a health pack. Needs healthPackPrefab assigned.")]
    [Range(0, 1)] public float healthDropChance = 0.04f;

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
    public bool IsDying => isDying; // so AIs can stop acting (e.g. firing) while dying
    private HitFlash hitFlash;
    private Knockback knockback;
    private DeathPop deathPop;
    private CharacterAnimator characterAnimator;
    private Collider2D[] bodyColliders;
    private int baseHealth;          // inspector HP, before difficulty/elite scaling
    private EliteModifier eliteModifier;
    private BossHealth bossHealth;   // if present, it owns HP + death; EnemyHealth is feedback-only
    [HideInInspector] public GameObject overrideDropPrefab; // mega-elite drops this (a Relic) instead of XP

    void Awake()
    {
        baseHealth = health; // snapshot the designed HP once, before anything scales it

        // Auto-wire feel components so every enemy gets them with zero prefab setup.
        hitFlash = GetComponent<HitFlash>();
        if (hitFlash == null) hitFlash = gameObject.AddComponent<HitFlash>();
        knockback = GetComponent<Knockback>();
        if (knockback == null) knockback = gameObject.AddComponent<Knockback>();
        deathPop = GetComponent<DeathPop>();
        if (deathPop == null) deathPop = gameObject.AddComponent<DeathPop>();
        // NOT auto-added: frame sheets must be assigned per prefab in the Inspector.
        characterAnimator = GetComponent<CharacterAnimator>();
        bodyColliders = GetComponentsInChildren<Collider2D>();
        eliteModifier = GetComponent<EliteModifier>();
        bossHealth = GetComponent<BossHealth>();
    }

    // Pooled respawn: come back fully alive and interactive, at full HP.
    void OnEnable()
    {
        isDying = false;
        overrideDropPrefab = null; // clear any mega-drop from a previous life
        if (bodyColliders != null)
            foreach (var c in bodyColliders) if (c != null) c.enabled = true;
        ApplySpawnHealth();
    }

    // Recompute HP from the designed base + current difficulty, then let the elite
    // roll (if any) multiply on top. Runs on every spawn so reused enemies aren't
    // stuck at the leftover 1 HP from their previous death.
    void ApplySpawnHealth()
    {
        if (DifficultyManager.Instance == null)
            DifficultyManager.Instance = FindFirstObjectByType<DifficultyManager>();

        float multiplier = DifficultyManager.Instance != null
            ? DifficultyManager.Instance.GetDifficultyMultiplier() : 1f;

        health = Mathf.Max(1, Mathf.RoundToInt(baseHealth * multiplier * RunModifierManager.EnemyHealthMult));

        if (eliteModifier != null) eliteModifier.RollOnSpawn(); // stacks its multiplier after this
    }

    // Kept for callers that don't know the hit's origin (no knockback direction).
    public void TakeDamage(int damage)
    {
        TakeDamage(damage, transform.position);
    }

    public void TakeDamage(int damage, Vector2 hitSource)
    {
        if (isDying) return;

        // Feedback always: hit sound, flash, damage number.
        if (SoundManager.Instance != null) SoundManager.Instance.PlayEnemyHit();
        if (hitFlash != null) hitFlash.Flash();
        if (DamagePopupSpawner.Instance != null)
            DamagePopupSpawner.Instance.Spawn(transform.position, damage);

        // Boss: BossHealth owns the 1000-HP pool and the death. Don't kill it via this
        // 1-HP EnemyHealth, and don't knock a boss around.
        if (bossHealth != null) return;

        health -= damage;

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
        // Go inert immediately so the dying enemy stops hurting/blocking the player
        // while its death animation plays out.
        if (bodyColliders != null)
            foreach (var c in bodyColliders) if (c != null) c.enabled = false;

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

        if (overrideDropPrefab != null)
        {
            // Mega-elite: drop a single Relic instead of the usual XP/shard spray.
            Instantiate(overrideDropPrefab, transform.position, Quaternion.identity);
        }
        else
        {
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

            // 3. small global chance for an XP enemy to also drop a heal, so you can
            //    actually survive to the boss (health-type enemies already drop 100%).
            if (healthPackPrefab != null && !RunModifierManager.NoHealthDrops && dropType != DropType.Health && Random.value <= healthDropChance)
            {
                Instantiate(healthPackPrefab, transform.position, Quaternion.identity);
            }
        }

        if (RunStats.Instance != null) RunStats.Instance.RegisterKill();

        health = 1; // ან ბაზისური HP
        isDying = false;
        gameObject.SetActive(false); // must be last — coroutine stops here
    }
}