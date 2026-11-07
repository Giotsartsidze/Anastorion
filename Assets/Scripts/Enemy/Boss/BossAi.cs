using UnityEngine;
using System.Collections;

public class BossAI : MonoBehaviour
{
    [Header("Star-burst volley")]
    public GameObject projectilePrefab;
    public int projectileCount = 8; // how many bolts per ring
    public float attackInterval = 3f;
    public float projectileSpeed = 5f;

    [Header("Movement")]
    public float moveSpeed = 2f;

    [Header("Bite (melee)")]
    public Sprite[] biteFrames;     // gveleshapi_boss_attack_bite sheet
    [Tooltip("Measured CENTER-to-center. The boss is huge (~6-7 world units half-size), so this " +
             "must exceed its body radius or the player can never be 'in range' while touching it.")]
    public float meleeRange = 7.5f;
    public int biteDamage = 20;
    public float biteCooldown = 2.5f;

    [Header("SKILL 1 — Summon adds")]
    [Tooltip("Boss periodically summons minions around itself.")]
    public bool canSummon = true;
    [Tooltip("What to summon. Leave EMPTY to borrow random non-boss enemies from the WaveManager pool.")]
    public GameObject[] summonPrefabs;
    public int summonCount = 3;
    public float summonInterval = 10f;
    public float summonRadius = 4f;

    [Header("SKILL 2 — Enrage (second phase)")]
    [Tooltip("At this health fraction the boss speeds up, fires faster & wider, and turns red.")]
    public bool canEnrage = true;
    [Range(0f, 1f)] public float enrageHealthPct = 0.5f;

    [Header("SKILL 3 — Charge (telegraphed lunge)")]
    [Tooltip("Boss flashes, then dashes at your last position for heavy damage. Dodge it.")]
    public bool canCharge = true;
    public float chargeInterval = 7f;
    public float chargeTelegraph = 0.6f;
    public float chargeSpeed = 16f;
    public float chargeDuration = 0.5f;
    public int chargeDamage = 35;

    [Header("SKILL 4 — Shockwave ring")]
    [Tooltip("Boss slams out an expanding ring of bolts with ONE dodgeable gap.")]
    public bool canShockwave = true;
    public float shockwaveInterval = 9f;
    public int shockwaveProjectiles = 18;

    [Header("SKILL 5 — Aimed volley")]
    [Tooltip("A tight burst fired STRAIGHT at the player — dodge sideways.")]
    public bool canAimedVolley = true;
    public float aimedInterval = 5f;
    public int aimedShots = 3;
    public float aimedSpread = 10f; // degrees between bolts in the burst

    [Header("SKILL 6 — Spiral barrage")]
    [Tooltip("A spinning stream of bolts over a couple of seconds — weave through it.")]
    public bool canSpiral = true;
    public float spiralInterval = 12f;
    public float spiralDuration = 2.2f;
    public float spiralShotDelay = 0.07f; // time between bolts
    public float spiralStep = 24f;         // degrees the emitter turns per bolt

    private float biteTimer;
    private float patternOffset;
    private Transform player;
    private CharacterAnimator anim;
    private SpriteRenderer sr;
    private BossHealth health;
    private bool enraged;
    private bool busy; // a charge/telegraph currently owns movement — pause the normal chase

    void Start()
    {
        anim = GetComponent<CharacterAnimator>();
        sr = GetComponent<SpriteRenderer>();
        health = GetComponent<BossHealth>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        StartCoroutine(AttackRoutine());
        if (canSummon)      StartCoroutine(SummonRoutine());
        if (canCharge)      StartCoroutine(ChargeRoutine());
        if (canShockwave)   StartCoroutine(ShockwaveRoutine());
        if (canAimedVolley) StartCoroutine(AimedVolleyRoutine());
        if (canSpiral)      StartCoroutine(SpiralRoutine());
    }

    void Update()
    {
        if (player == null) return;

        if (canEnrage && !enraged && health != null && health.HealthPct <= enrageHealthPct)
            Enrage();

        if (busy) return; // charging — don't fight the lunge for control of transform

        float dist = Vector2.Distance(transform.position, player.position);

        // Chase the player — a moving threat, not a turret.
        if (dist > meleeRange)
            transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

        // Bite when close.
        biteTimer -= Time.deltaTime;
        if (dist <= meleeRange && biteTimer <= 0f)
        {
            Bite();
            biteTimer = biteCooldown;
        }
    }

    // ---- Phase 2 -------------------------------------------------------------
    void Enrage()
    {
        enraged = true;
        moveSpeed     *= 1.6f;
        attackInterval = Mathf.Max(0.6f, attackInterval * 0.6f);
        projectileCount += 4;
        chargeInterval = Mathf.Max(2.5f, chargeInterval * 0.7f);
        summonInterval = Mathf.Max(5f, summonInterval * 0.7f);
        aimedInterval  = Mathf.Max(2f, aimedInterval * 0.7f);
        aimedShots    += 2;
        spiralInterval = Mathf.Max(6f, spiralInterval * 0.7f);
        if (sr != null) sr.color = new Color(1f, 0.55f, 0.55f, 1f); // angrier tint
        EvolutionAnnouncer.Instance?.Announce("Gveleshapi Enrages");
    }

    // ---- Melee ---------------------------------------------------------------
    void Bite()
    {
        if (anim != null && biteFrames != null && biteFrames.Length > 0)
            anim.PlayAttack(biteFrames);
        if (player != null && Vector2.Distance(transform.position, player.position) <= meleeRange + 0.5f)
            player.GetComponent<PlayerHealth>()?.TakeDamage(biteDamage);
    }

    // ---- Star-burst volley ---------------------------------------------------
    IEnumerator AttackRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackInterval);
            if (!busy) ShootStarPattern();
        }
    }

    void ShootStarPattern()
    {
        if (projectilePrefab == null) return;
        if (anim != null) anim.PlayAttack();
        float angleStep = 360f / projectileCount;
        float angle = patternOffset;   // rotate each volley so the gaps shift
        patternOffset += 13f;

        for (int i = 0; i < projectileCount; i++)
        {
            float dirX = transform.position.x + Mathf.Sin((angle * Mathf.Deg2Rad));
            float dirY = transform.position.y + Mathf.Cos((angle * Mathf.Deg2Rad));
            Vector3 moveVec = new Vector3(dirX, dirY, 0);
            Vector2 dir = (moveVec - transform.position).normalized;

            GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            float rot = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            proj.transform.rotation = Quaternion.Euler(0, 0, rot);

            angle += angleStep;
        }
    }

    // ---- SKILL 1: Summon adds ------------------------------------------------
    IEnumerator SummonRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(summonInterval);
            Summon();
        }
    }

    void Summon()
    {
        int n = Mathf.Max(1, summonCount);
        for (int i = 0; i < n; i++)
        {
            GameObject prefab = PickSummon();
            if (prefab == null) continue;
            Vector2 off = Random.insideUnitCircle.normalized * summonRadius * Random.Range(0.6f, 1f);
            Instantiate(prefab, transform.position + (Vector3)off, Quaternion.identity);
        }
    }

    GameObject PickSummon()
    {
        if (summonPrefabs != null && summonPrefabs.Length > 0)
            return summonPrefabs[Random.Range(0, summonPrefabs.Length)];

        // Fallback: borrow a random NON-boss enemy straight from the WaveManager pool,
        // so this works with zero wiring.
        var wm = FindObjectOfType<WaveManager>();
        if (wm != null && wm.enemyPool != null && wm.enemyPool.Count > 0)
        {
            for (int tries = 0; tries < 6; tries++)
            {
                var e = wm.enemyPool[Random.Range(0, wm.enemyPool.Count)];
                if (e != null && e.prefab != null && e.prefab.GetComponent<BossHealth>() == null)
                    return e.prefab;
            }
        }
        return null;
    }

    // ---- SKILL 3: Telegraphed charge ----------------------------------------
    IEnumerator ChargeRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(chargeInterval);
            if (player != null && !busy) yield return StartCoroutine(Charge());
        }
    }

    IEnumerator Charge()
    {
        busy = true;

        // Telegraph: flash white so the player gets a beat to react.
        Color baseCol = sr != null ? sr.color : Color.white;
        float t = 0f;
        while (t < chargeTelegraph)
        {
            if (sr != null) sr.color = Color.Lerp(baseCol, Color.white, Mathf.PingPong(t * 10f, 1f));
            t += Time.deltaTime;
            yield return null;
        }
        if (sr != null) sr.color = baseCol;

        // Lock the direction NOW and lunge along it.
        Vector2 dir = ((Vector2)player.position - (Vector2)transform.position).normalized;
        if (anim != null) anim.PlayAttack();
        float d = 0f;
        bool hit = false;
        while (d < chargeDuration)
        {
            transform.position += (Vector3)(dir * chargeSpeed * Time.deltaTime);
            if (!hit && Vector2.Distance(transform.position, player.position) <= meleeRange)
            {
                player.GetComponent<PlayerHealth>()?.TakeDamage(chargeDamage);
                hit = true;
            }
            d += Time.deltaTime;
            yield return null;
        }
        busy = false;
    }

    // ---- SKILL 4: Shockwave ring --------------------------------------------
    IEnumerator ShockwaveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(shockwaveInterval);
            if (!busy) Shockwave();
        }
    }

    void Shockwave()
    {
        if (projectilePrefab == null) return;
        if (anim != null) anim.PlayAttack();

        int n = Mathf.Max(6, shockwaveProjectiles);
        int gap = Random.Range(0, n); // leave one dodgeable gap in the ring
        float step = 360f / n;

        for (int i = 0; i < n; i++)
        {
            if (i == gap || i == (gap + 1) % n) continue;
            float ang = i * step;
            GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.Euler(0, 0, ang));
        }
    }

    // ---- SKILL 5: Aimed volley ----------------------------------------------
    IEnumerator AimedVolleyRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(aimedInterval);
            if (!busy && player != null) AimedVolley();
        }
    }

    void AimedVolley()
    {
        if (projectilePrefab == null) return;
        if (anim != null) anim.PlayAttack();

        Vector2 dir = ((Vector2)player.position - (Vector2)transform.position).normalized;
        float baseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        int shots = Mathf.Max(1, aimedShots);
        float start = -aimedSpread * (shots - 1) * 0.5f; // center the fan on the player
        for (int i = 0; i < shots; i++)
        {
            float ang = baseAngle + start + i * aimedSpread;
            Instantiate(projectilePrefab, transform.position, Quaternion.Euler(0, 0, ang));
        }
    }

    // ---- SKILL 6: Spiral barrage --------------------------------------------
    IEnumerator SpiralRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spiralInterval);
            if (!busy) yield return StartCoroutine(Spiral());
        }
    }

    IEnumerator Spiral()
    {
        if (projectilePrefab == null) yield break;
        if (anim != null) anim.PlayAttack();

        float angle = Random.Range(0f, 360f);
        float elapsed = 0f;
        var wait = new WaitForSeconds(spiralShotDelay);
        while (elapsed < spiralDuration)
        {
            // two opposite arms so the spiral reads clearly
            Instantiate(projectilePrefab, transform.position, Quaternion.Euler(0, 0, angle));
            Instantiate(projectilePrefab, transform.position, Quaternion.Euler(0, 0, angle + 180f));
            angle += spiralStep;
            elapsed += spiralShotDelay;
            yield return wait;
        }
    }
}
