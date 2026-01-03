using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // ტაიმერისთვის
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{
    public Wave[] waves; // აქ ჩავყრით ტალღებს ინსპექტორიდან
    public Transform[] spawnPoints; // სად გაჩნდნენ მტრები (Player-ის გარშემო)
    
    private Wave currentWave;
    private int currentWaveIndex = 0;
    private Transform player;
    
    private bool canSpawn = true;
    private float nextSpawnTime;

    public TextMeshProUGUI timerText; // ეკრანზე დროის საჩვენებლად
    private float gameTime = 0;

	public GameObject bossPrefab; // ბოსის პრეფაბის ჩასაგდები ველი
	private bool bossSpawned = false;
	public GameObject bossWarningUI;
	public Slider bossHealthSlider;
    
    [Header("Swarm Settings")]
    public int swarmSize = 10; // რამდენი გაჩნდეს ერთად
    [Header("Archetype Prefabs")]
    public GameObject tankEnemyPrefab;
    public GameObject rangedEnemyPrefab;
    public GameObject swarmEnemyPrefab;

    [Header("Spawn Chances (0-100)")]
    public float tankChance = 10f;
    public float rangedChance = 20f;
    public float swarmChance = 15f;

    [Header("Timing Settings")]
    public float startTankAt = 45f;   // ტანკები 45-ე წამიდან
    public float startRangedAt = 60f; // რეინჯერები 1 წუთიდან
    public float startSwarmAt = 90f;  // სვორმები 1.5 წუთიდან

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentWave = waves[currentWaveIndex];
    }

    void Update()
    {
        if (player == null) return;
        
        // --- DEBUG GILAKI TESTIREBISTVIS ---
        if (Input.GetKeyDown(KeyCode.B)) 
        {
            Debug.Log("Debug: Manual Boss Spawn!");
            SpawnBoss();
        }

float timeDiminisher = Mathf.Clamp(gameTime / 300f, 0, 0.5f); // მაქსიმუმ 50%-ით აჩქარდეს
    float effectiveRate = currentWave.rate * (1 - timeDiminisher);

    if (canSpawn && Time.time >= nextSpawnTime)
    {
        SpawnEnemy();
        nextSpawnTime = Time.time + (bossSpawned ? effectiveRate * 3f : effectiveRate);
    }

        // 1. ტაიმერის ლოგიკა
        gameTime += Time.deltaTime;
        UpdateTimerUI();

if (!bossSpawned && gameTime >= 120f) // 120 წამი = 2 წუთი
    {
        SpawnBoss();
    }

        // 2. ტალღების გადართვის ლოგიკა
        if (currentWaveIndex + 1 < waves.Length && gameTime >= (currentWaveIndex + 1) * 30f) // ყოველ 30 წამში ახალი ტალღა
        {
            currentWaveIndex++;
            currentWave = waves[currentWaveIndex];
            Debug.Log("Next Wave: " + currentWave.waveName);
        }

        // 3. სპაუნინგის ლოგიკა
float currentSpawnRate = bossSpawned ? currentWave.rate * 3f : currentWave.rate;
        if (canSpawn && Time.time >= nextSpawnTime)
{
    SpawnEnemy();
    nextSpawnTime = Time.time + currentSpawnRate;
}
    }

    void SpawnEnemy()
    {
        // 1. ვირჩევთ სპაუნის წერტილს (ეკრანის გარეთ)
        Vector2 spawnDir = Random.insideUnitCircle.normalized * 24f;
        Vector3 spawnPos = player.position + (Vector3)spawnDir;

        // 2. ვირჩევთ რომელ არქეტიპს ვაჩენთ (Weighted Random)
        float roll = Random.Range(0, 100);

        // SWARM - გუნდური სპაუნინგი
        if (gameTime >= startSwarmAt && roll < swarmChance)
        {
            StartCoroutine(SpawnSwarmGroup(spawnPos));
        }
        // TANK - მძიმე მტერი
        else if (gameTime >= startTankAt && roll < (swarmChance + tankChance))
        {
            Instantiate(tankEnemyPrefab, spawnPos, Quaternion.identity);
        }
        // RANGER - მსროლელი
        else if (gameTime >= startRangedAt && roll < (swarmChance + tankChance + rangedChance))
        {
            Instantiate(rangedEnemyPrefab, spawnPos, Quaternion.identity);
        }
        // DEFAULT - ჩვეულებრივი მტერი
        else
        {
            Instantiate(currentWave.enemyPrefab, spawnPos, Quaternion.identity);
        }
    }
    
    IEnumerator SpawnSwarmGroup(Vector3 centerPos)
    {
        for (int i = 0; i < swarmSize; i++)
        {
            // გუნდის წევრები ერთმანეთთან ახლოს
            Vector3 offset = Random.insideUnitSphere * 2f;
            offset.z = 0;
            Instantiate(swarmEnemyPrefab, centerPos + offset, Quaternion.identity);
            yield return new WaitForSeconds(0.05f); 
        }
    }

void SpawnBoss()
{
   bossSpawned = true;
    bossWarningUI.SetActive(true);
    Invoke("HideWarning", 3f); // 3 წამში გავაქროთ
    Vector3 spawnPos = player.position + new Vector3(0, 15, 0); // ჩნდება მოთამაშის ზემოთ
    Instantiate(bossPrefab, spawnPos, Quaternion.identity);
    Debug.Log("The Shadow Colossus has arrived!");
}

void HideWarning() => bossWarningUI.SetActive(false);

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(gameTime / 60);
        int seconds = Mathf.FloorToInt(gameTime % 60);
        if(timerText != null) timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}