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

	[Header("Ranged Enemy Settings")]
	public GameObject rangedEnemyPrefab;
	[Range(0, 100)]
	public float rangedSpawnChance = 20f; // 20% შანსი, რომ Ranger გაჩნდეს
	public float startRangedAt = 60f;

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
    Vector2 spawnDir = Random.insideUnitCircle.normalized * 24f;
    Vector3 spawnPos = player.position + (Vector3)spawnDir;

    // ლოგიკა: თუ 1 წუთი გავიდა და რენდომმა "გაამართლა", ვაჩენთ Ranger-ს
    if (gameTime >= startRangedAt && Random.Range(0, 100) <= rangedSpawnChance)
    {
        Instantiate(rangedEnemyPrefab, spawnPos, Quaternion.identity);
    }
    else
    {
        // წინააღმდეგ შემთხვევაში ჩვეულებრივი მტერი
        Instantiate(currentWave.enemyPrefab, spawnPos, Quaternion.identity);
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