using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class EnemyArchetype
{
    public string name;
    public GameObject prefab;
    [Range(0, 100)] public float spawnChance;
    public float startTime; // რა წუთიდან/წამიდან შემოვიდეს თამაშში
    public bool isSwarm = false;
}

public class WaveManager : MonoBehaviour
{
    [Header("Enemy Pool")]
    public List<EnemyArchetype> enemyPool; // აქ ჩაამატებ 12-ივე მტერს ინსპექტორიდან
    
    public Transform[] spawnPoints;
    public TextMeshProUGUI timerText;

    [Header("Boss Settings")]
    public GameObject bossPrefab;
    [Tooltip("Seconds into the run when the single boss spawns. Set LOW (e.g. 10) to test.")]
    public float bossSpawnTime = 420f;
    [Tooltip("While the boss is alive, stop spawning outside enemies/elites so the fight is 1v1 (plus the boss's own summons).")]
    public bool stopSpawnsDuringBoss = true;
    private bool bossActive;
    public GameObject bossWarningUI;
    public Slider bossHealthSlider;
    public TMP_Text bossHealthText;   // optional "X / Y" overlay on the boss bar
    [Tooltip("The whole boss-bar UI container (slider + frame + text). Hidden when the boss dies.")]
    public GameObject bossBarRoot;
    private bool bossSpawned = false;

[Header("Elite Event Settings")]
    public float eventDuration = 15f;    // რამდენი ხანი გრძელდება ელიტური ტალღა
    public GameObject eliteEventUI;     // UI ტექსტი: "ELITE WAVE!"
    private bool isEliteEventActive = false;
    private float lastEventMinute = 0;

    [Header("Mega Elite Settings")]
    [Tooltip("A mega-elite (tanky, drops a Relic) spawns this often, in seconds.")]
    public float megaEliteInterval = 90f;
    public GameObject megaEliteUI;      // optional "MEGA ELITE!" banner
    [Tooltip("Assign the Relic prefab ONCE here — every mega-elite will drop it, regardless of enemy.")]
    public GameObject megaRelicPrefab;
    private float nextMegaTime = 90f;

    [Header("Debug — see every enemy quickly")]
    [Tooltip("Ignores time-gates and cycles through ALL enemies one by one. Turn OFF for real play.")]
    public bool debugSpawnAll = false;
    public float debugSpawnInterval = 1f;
    private int debugIndex = 0;

    [HideInInspector] public float spawnRateMultiplier = 1f; // raised by the Horde run modifier
    private float nextSpawnTime;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        float currentTime = DifficultyManager.Instance.gameTime;
        UpdateTimerUI(currentTime);

        // ბოსის სპაუნინგი
        if (!bossSpawned && currentTime >= bossSpawnTime) SpawnBoss(); // the one scripted boss climax

        // While the boss is up, the arena belongs to it (plus its own summons): no outside
        // waves, elite events, or mega-elites. Keeps the climax a clean duel.
        bool bossLockdown = bossActive && stopSpawnsDuringBoss;

		float currentMinute = Mathf.Floor(currentTime / 120f); // elite event every 2 minutes
        if (!bossLockdown && currentMinute > lastEventMinute && currentMinute > 0)
        {
            lastEventMinute = currentMinute;
            StartCoroutine(TriggerEliteEvent());
        }

        // mega-elite on its own timer (tanky, drops a Relic)
        if (!bossLockdown && currentTime >= nextMegaTime)
        {
            nextMegaTime += megaEliteInterval;
            SpawnMegaElite(currentTime);
        }

        // სპაუნინგის ლოგიკა
        if (!bossLockdown && Time.time >= nextSpawnTime)
        {
            SpawnEnemy(currentTime);
            // სირთულის მიხედვით სპაუნინგის აჩქარება
            float spawnRate = debugSpawnAll
                ? debugSpawnInterval
                : (1.2f / DifficultyManager.Instance.GetDifficultyMultiplier()) / Mathf.Max(0.1f, spawnRateMultiplier);
            nextSpawnTime = Time.time + spawnRate;
        }
    }
IEnumerator TriggerEliteEvent()
    {
        isEliteEventActive = true;
        if (eliteEventUI != null) eliteEventUI.SetActive(true);
        
        
        yield return new WaitForSeconds(eventDuration);

        isEliteEventActive = false;
        if (eliteEventUI != null) eliteEventUI.SetActive(false);
    }

    void SpawnMegaElite(float currentTime)
    {
        if (ObjectPooler.Instance == null || player == null) return;

        // Only archetypes that can actually become a mega (have EliteModifier) and
        // aren't the boss or a swarm unit.
        List<EnemyArchetype> available = new List<EnemyArchetype>();
        foreach (var e in enemyPool)
        {
            if (e.isSwarm || currentTime < e.startTime || e.prefab == null) continue;
            if (e.prefab.GetComponent<EliteModifier>() == null) continue;
            if (e.prefab.GetComponent<BossHealth>() != null) continue;
            available.Add(e);
        }
        if (available.Count == 0) return;

        EnemyArchetype pick = available[Random.Range(0, available.Count)];
        Vector2 dir = Random.insideUnitCircle.normalized * 22f;
        Vector3 pos = player.position + (Vector3)dir;

        GameObject mega = ObjectPooler.Instance.SpawnFromPool(pick.name, pos, Quaternion.identity);
        if (mega != null)
        {
            EliteModifier em = mega.GetComponent<EliteModifier>();
            if (em != null)
            {
                if (megaRelicPrefab != null) em.relicPrefab = megaRelicPrefab; // so EVERY mega drops a relic
                em.MakeMega();
            }
        }
        if (megaEliteUI != null) StartCoroutine(FlashMegaUI());
    }

    IEnumerator FlashMegaUI()
    {
        megaEliteUI.SetActive(true);
        yield return new WaitForSeconds(3f);
        if (megaEliteUI != null) megaEliteUI.SetActive(false);
    }

    void SpawnEnemy(float currentTime)
    {
        if (debugSpawnAll) { SpawnDebugCycle(); return; }

        // 1. ვფილტრავთ მტრებს, რომლებიც უკვე "გახსნილია" დროის მიხედვით
        List<EnemyArchetype> availableEnemies = new List<EnemyArchetype>();
        foreach (var enemy in enemyPool)
        {
            if (IsBoss(enemy)) continue; // the boss only ever comes from SpawnBoss(), never the pool
            if (currentTime >= enemy.startTime) availableEnemies.Add(enemy);
        }

        if (availableEnemies.Count == 0) return;

        // 2. Weighted Random (ვირჩევთ ერთ-ერთს შანსების მიხედვით)
        EnemyArchetype selectedEnemy = GetRandomEnemy(availableEnemies);

        // 3. სპაუნის პოზიცია
        Vector2 spawnDir = Random.insideUnitCircle.normalized * 22f;
        Vector3 spawnPos = player.position + (Vector3)spawnDir;

        GameObject spawnedEnemy;
        if (selectedEnemy.isSwarm)
        {
            // Swarm-ის შემთხვევაში კოდი იგივე დარჩა, თუმცა Elite ლოგიკა მასზეც იმოქმედებს ქვემოთ
            StartCoroutine(SpawnSwarmGroup(selectedEnemy.prefab, spawnPos));
            return; // Swarm-ს ცალკე ვამუშავებთ იტერაციაში
        }
        else
        {
            if (ObjectPooler.Instance == null) return;
            spawnedEnemy = ObjectPooler.Instance.SpawnFromPool(selectedEnemy.name, spawnPos, Quaternion.identity);
        }

        // --- ELITE OVERRIDE ---
        if (isEliteEventActive && spawnedEnemy != null)
        {
            EliteModifier elite = spawnedEnemy.GetComponent<EliteModifier>();
            if (elite != null)
            {
                // პირდაპირ ვიძახებთ MakeElite-ს, რომ 100%-ით გააქტიურდეს
                elite.MakeElite(); 
            }
        }
    }

    // Cycles through every enemy in enemyPool, ignoring time-gates & chances,
    // so you can watch each one (and its animations) in order.
    void SpawnDebugCycle()
    {
        if (enemyPool == null || enemyPool.Count == 0) return;

        EnemyArchetype e = enemyPool[debugIndex % enemyPool.Count];
        debugIndex++;
        if (IsBoss(e)) return; // never cycle the boss in as a regular debug spawn

        Vector2 spawnDir = Random.insideUnitCircle.normalized * 22f;
        Vector3 spawnPos = player.position + (Vector3)spawnDir;

        if (e.isSwarm)
            StartCoroutine(SpawnSwarmGroup(e.prefab, spawnPos));
        else if (ObjectPooler.Instance != null)
            ObjectPooler.Instance.SpawnFromPool(e.name, spawnPos, Quaternion.identity);
    }

    // The boss is a scripted, one-time climax (SpawnBoss). If it's accidentally left in
    // the enemyPool it must never spawn as a regular/swarm enemy, or you get a horde of bosses.
    static bool IsBoss(EnemyArchetype e) => e != null && e.prefab != null && e.prefab.GetComponent<BossHealth>() != null;

    EnemyArchetype GetRandomEnemy(List<EnemyArchetype> pool)
    {
        float totalChance = 0;
        foreach (var e in pool) totalChance += e.spawnChance;

        float roll = Random.Range(0, totalChance);
        float cumulative = 0;

        foreach (var e in pool)
        {
            cumulative += e.spawnChance;
            if (roll <= cumulative) return e;
        }
        return pool[0];
    }

    IEnumerator SpawnSwarmGroup(GameObject prefab, Vector3 center)
    {
        for (int i = 0; i < 8; i++)
        {
            Instantiate(prefab, center + (Vector3)Random.insideUnitCircle * 2f, Quaternion.identity);
            yield return new WaitForSeconds(0.05f);
        }
    }

    void SpawnBoss()
    {
        bossSpawned = true;
        bossActive = true; // lock down outside spawns for the duel
        if (bossPrefab == null) { Debug.LogError("WaveManager: bossPrefab is not assigned — no boss to spawn."); return; }
        if (MusicManager.Instance != null) MusicManager.Instance.PlayBoss();
        if (bossWarningUI != null) { bossWarningUI.SetActive(true); Invoke(nameof(HideWarning), 3f); }
        // Spawn just inside the top of the view (camera half-height is ~10) so the boss
        // is visible immediately and descends toward the player, instead of lurking
        // off-screen. Raise this for a longer "approaching from the dark" entrance.
        Vector3 at = (player != null ? player.position : Vector3.zero) + new Vector3(0, 8f, 0);
        GameObject boss = Instantiate(bossPrefab, at, Quaternion.identity);
        Debug.Log("WaveManager: boss spawned at " + at + " (name: " + boss.name + ")");
    }

    void HideWarning() { if (bossWarningUI != null) bossWarningUI.SetActive(false); }

    // Called by BossHealth on boss death — lifts the spawn lockdown.
    public void NotifyBossDefeated() { bossActive = false; }

    void UpdateTimerUI(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        if (timerText != null) timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}