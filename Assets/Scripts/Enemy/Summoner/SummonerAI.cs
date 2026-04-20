using UnityEngine;
using System.Collections;

public class SummonerAI : MonoBehaviour
{
    public float speed = 1.5f;
    public float stoppingDistance = 7f;
    public GameObject enemyToSpawn; // აქ ჩააგდე Swarm პრეფაბი
    public float spawnInterval = 4f;
    public int spawnAmount = 3;

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        // ვიწყებთ სპაუნინგის ციკლს
        InvokeRepeating(nameof(SpawnMinions), spawnInterval, spawnInterval);
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // მომხმობელი ცდილობს დისტანცია დაიჭიროს
        if (distance > stoppingDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }
        else if (distance < stoppingDistance - 2f)
        {
            // თუ ფლეიერი ძალიან მოუახლოვდა, უკან იხევს
            transform.position = Vector2.MoveTowards(transform.position, player.position, -speed * Time.deltaTime);
        }

        // Sprite Flip
        transform.localScale = new Vector3(player.position.x < transform.position.x ? -1 : 1, 1, 1);
    }

    void OnDisable() => CancelInvoke(nameof(SpawnMinions));
    void OnDestroy() => CancelInvoke(nameof(SpawnMinions));

    void SpawnMinions()
    {
        if (enemyToSpawn == null) return;
        for (int i = 0; i < spawnAmount; i++)
        {
            // მინიონებს ვაჩენთ მომხმობლის გარშემო მცირე რადიუსში
            Vector3 randomPos = transform.position + (Vector3)Random.insideUnitCircle * 2f;
            Instantiate(enemyToSpawn, randomPos, Quaternion.identity);
        }
    }
}