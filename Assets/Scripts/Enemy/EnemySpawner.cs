using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float baseSpawnRate = 2f; // საწყისი სისწრაფე
    public float minSpawnRate = 0.5f; // მაქსიმალური სისწრაფე (რამდენად ხშირად)
    public float difficultyFactor = 0.05f; // რამდენად სწრაფად გათრთულდეს

    private float currentSpawnRate;
    private float timer;
    private float timeElapsed;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentSpawnRate = baseSpawnRate;
    }

    void Update()
    {
        if (player == null) return;

        timeElapsed += Time.deltaTime;
        
        // სირთულის ზრდა: ყოველ წამს მცირდება spawnRate
        currentSpawnRate = Mathf.Max(minSpawnRate, baseSpawnRate - (timeElapsed * difficultyFactor));

        timer += Time.deltaTime;
        if (timer >= currentSpawnRate)
        {
            SpawnEnemy();
            timer = 0;
        }
    }

    void SpawnEnemy()
    {
        Vector2 spawnDirection = Random.insideUnitCircle.normalized;
        Vector3 spawnPosition = player.position + (Vector3)(spawnDirection * 12f);
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}