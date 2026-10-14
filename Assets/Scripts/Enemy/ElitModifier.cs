using UnityEngine;
using System.Collections.Generic;

public class EliteModifier : MonoBehaviour
{
    [Header("Elite Settings")]
    public float eliteChance = 10f;
    public float healthMultiplier = 3f;
    public float scaleMultiplier = 1.3f;
    public GameObject auraVisual; 

    [Header("Aura Buff Settings")]
    public float auraRadius = 5f;
    public float speedMultiplier = 1.25f; 
    public LayerMask enemyLayer;

    [Header("Visual Identifier")]
    public GameObject eliteIcon;

    [Header("Mega Elite")]
    [Tooltip("Dropped when a mega-elite dies (a Relic pickup) instead of XP.")]
    public GameObject relicPrefab;
    public float megaHealthMultiplier = 8f;
    public float megaScaleMultiplier = 1.8f;

    private bool isElite = false;
    private bool isMega = false;
    private Vector3 originalScale;
    private SpriteRenderer sr;
    private GameObject glow;        // runtime silhouette-glow behind the enemy
    private SpriteRenderer glowSr;

    void Awake()
    {
        originalScale = transform.localScale;
        sr = GetComponentInChildren<SpriteRenderer>();
        // Awake-ში ვთიშავთ ვიზუალებს, რომ საწყისში არ გამოჩნდეს
        if (auraVisual != null) auraVisual.SetActive(false);
        if (eliteIcon != null) eliteIcon.SetActive(false);
    }

    // A tinted copy of the enemy's sprite, slightly larger and behind it, pulsing.
    // Reads unmistakably as "elite" and survives HitFlash (separate renderer).
    void EnsureGlow()
    {
        if (glow != null || sr == null) return;
        glow = new GameObject("EliteGlow");
        glow.transform.SetParent(sr.transform, false);
        glow.transform.localPosition = Vector3.zero;
        glowSr = glow.AddComponent<SpriteRenderer>();
        glowSr.sortingLayerID = sr.sortingLayerID;
        glowSr.sortingOrder = sr.sortingOrder - 1; // sits behind the body
    }

    // Called by EnemyHealth every (re)spawn, AFTER base health is set, so the
    // multiplier stacks on the right number and each spawn re-rolls its elite status.
    public void RollOnSpawn()
    {
        ResetEliteStatus();
        if (Random.Range(0f, 100f) <= eliteChance) MakeElite();
    }

    public void ResetEliteStatus()
    {
        isElite = false;
        isMega = false;
        CancelInvoke(nameof(ApplyAuraBuff));
        if (auraVisual != null) auraVisual.SetActive(false);
        if (eliteIcon != null) eliteIcon.SetActive(false);
        if (glow != null) glow.SetActive(false);
        // restore base size + color (critical now that enemies are pooled/reused)
        transform.localScale = originalScale;
        if (sr != null) sr.color = Color.white;
    }

    public void MakeElite()
    {
		if (isElite) return;
        isElite = true;

        // 1. ტრანსფორმაცია (from the cached base scale, so reuse never compounds)
        transform.localScale = originalScale * scaleMultiplier;
        if (sr != null) sr.color = new Color(1, 0.5f, 0, 1); // ნარინჯისფერი

        // 2. ჯანმრთელობა — ყველა ელიტარი იღებს სრულ მულტიპლაიერს (აღარ კვდება სწრაფად)
        EnemyHealth healthScript = GetComponent<EnemyHealth>();
        if (healthScript != null)
        {
            healthScript.health = Mathf.RoundToInt(healthScript.health * healthMultiplier);
            healthScript.dropCount = (healthScript.dropType == EnemyHealth.DropType.Health) ? 3 : 5;
        }

        // 3. ვიზუალების ჩართვა
        EnsureGlow();
        if (glow != null) glow.SetActive(true);
        if (auraVisual != null) auraVisual.SetActive(true);
        if (eliteIcon != null)
        {
            eliteIcon.SetActive(true);
            var isr = eliteIcon.GetComponent<SpriteRenderer>();
            if (isr != null) isr.color = Color.yellow;
        }

        // 4. აურის ლოგიკის გააქტიურება
        InvokeRepeating(nameof(ApplyAuraBuff), 0.5f, 0.5f);
    }

    /// <summary>
    /// Promote this spawn to a mega-elite: much tankier + larger, violet glow, and
    /// it drops a Relic instead of XP. Called by WaveManager on the mega timer.
    /// </summary>
    public void MakeMega()
    {
        MakeElite();          // base elite setup (guarded so it won't double-apply)
        isMega = true;
        transform.localScale = originalScale * megaScaleMultiplier;
        if (sr != null) sr.color = new Color(0.72f, 0.42f, 1f, 1f); // violet body tint

        EnemyHealth hp = GetComponent<EnemyHealth>();
        if (hp != null)
        {
            // MakeElite already applied healthMultiplier; scale up to the net mega amount.
            float extra = Mathf.Max(1f, megaHealthMultiplier / Mathf.Max(1f, healthMultiplier));
            hp.health = Mathf.RoundToInt(hp.health * extra);
            hp.overrideDropPrefab = relicPrefab;
            hp.dropCount = 1;
        }
    }

    void ApplyAuraBuff()
    {
        if (!isElite) return;

        // ვპოულობთ მტრებს რადიუსში
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, auraRadius, enemyLayer);
        
        foreach (var enemy in hitEnemies)
        {
            if (enemy.gameObject == this.gameObject) continue;

            // სენიორული მიდგომა: ვიყენებთ SendMessage-ს, რომ ყველა ტიპის AI-მ გაიგოს
            // ეს იმუშავებს Blinker-ზეც, Summoner-ზეც და ჩვეულებრივზეც
            enemy.SendMessage("ApplySpeedBuff", speedMultiplier, SendMessageOptions.DontRequireReceiver);
        }
    }

    void Update()
    {
        if (!isElite) return;

        // optional assigned aura visual
        if (auraVisual != null)
        {
            float p = 1f + Mathf.Sin(Time.time * 3f) * 0.1f;
            auraVisual.transform.localScale = new Vector3(p, p, 1);
        }

        // runtime silhouette-glow: keep it synced to the animated sprite + pulse it
        if (glowSr != null && sr != null)
        {
            glowSr.sprite = sr.sprite;
            glowSr.flipX = sr.flipX;
            float beat = Mathf.Sin(Time.time * 6f);
            float scale = (isMega ? 1.24f : 1.18f) + beat * 0.06f;
            glow.transform.localScale = new Vector3(scale, scale, 1f);
            glowSr.color = isMega
                ? new Color(0.75f, 0.35f, 1f, 0.6f + beat * 0.18f)   // violet mega halo
                : new Color(1f, 0.45f, 0.05f, 0.55f + beat * 0.15f); // orange elite halo
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, auraRadius);
    }
}