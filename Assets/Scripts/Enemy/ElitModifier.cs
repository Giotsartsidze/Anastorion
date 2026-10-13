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

    private bool isElite = false;
    private Vector3 originalScale;
    private SpriteRenderer sr;

    void Awake()
    {
        originalScale = transform.localScale;
        sr = GetComponent<SpriteRenderer>();
        // Awake-ში ვთიშავთ ვიზუალებს, რომ საწყისში არ გამოჩნდეს
        if (auraVisual != null) auraVisual.SetActive(false);
        if (eliteIcon != null) eliteIcon.SetActive(false);
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
        CancelInvoke(nameof(ApplyAuraBuff));
        if (auraVisual != null) auraVisual.SetActive(false);
        if (eliteIcon != null) eliteIcon.SetActive(false);
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
        // აურის "Pulse" ეფექტი
        if (isElite && auraVisual != null)
        {
            float pulse = 1f + Mathf.Sin(Time.time * 3f) * 0.1f;
            auraVisual.transform.localScale = new Vector3(pulse, pulse, 1);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, auraRadius);
    }
}