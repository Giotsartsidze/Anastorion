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
    public GameObject bossWarningUI;
    public Slider bossHealthSlider;
    private bool bossSpawned = false;

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
        if (!bossSpawned && currentTime >= 120f) SpawnBoss();

        // სპაუნინგის ლოგიკა
        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy(currentTime);
            // სირთულის მიხედვით სპაუნინგის აჩქარება
            float spawnRate = 1.5f / DifficultyManager.Instance.GetDifficultyMultiplier();
            nextSpawnTime = Time.time + spawnRate;
        }
    }

    void SpawnEnemy(float currentTime)
    {
        // 1. ვფილტრავთ მტრებს, რომლებიც უკვე "გახსნილია" დროის მიხედვით
        List<EnemyArchetype> availableEnemies = new List<EnemyArchetype>();
        foreach (var enemy in enemyPool)
        {
            if (currentTime >= enemy.startTime) availableEnemies.Add(enemy);
        }

        if (availableEnemies.Count == 0) return;

        // 2. Weighted Random (ვირჩევთ ერთ-ერთს შანსების მიხედვით)
        EnemyArchetype selectedEnemy = GetRandomEnemy(availableEnemies);

        // 3. სპაუნის პოზიცია
        Vector2 spawnDir = Random.insideUnitCircle.normalized * 22f;
        Vector3 spawnPos = player.position + (Vector3)spawnDir;

        if (selectedEnemy.isSwarm)
            StartCoroutine(SpawnSwarmGroup(selectedEnemy.prefab, spawnPos));
        else
            ObjectPooler.Instance.SpawnFromPool(selectedEnemy.name, spawnPos, Quaternion.identity);
    }

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
        bossWarningUI.SetActive(true);
        Invoke(nameof(HideWarning), 3f);
        Instantiate(bossPrefab, player.position + new Vector3(0, 15, 0), Quaternion.identity);
    }

    void HideWarning() => bossWarningUI.SetActive(false);

    void UpdateTimerUI(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        if (timerText != null) timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}