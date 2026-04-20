using UnityEngine;
using System.Collections;
public class SineWaveAI : MonoBehaviour
{
    public float speed = 3f;
    public float frequency = 5f; // ტალღის სიხშირე
    public float magnitude = 2f; // ტალღის სიგანე
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        // მოძრაობა ფლეიერისკენ
        Vector2 direction = (player.position - transform.position).normalized;
        Vector2 sideways = new Vector2(-direction.y, direction.x); // პერპენდიკულარული ვექტორი

        // ძირითადი მოძრაობა + სინუსოიდური გადახრა
        transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        transform.position += (Vector3)sideways * Mathf.Sin(Time.time * frequency) * magnitude * Time.deltaTime;
    }
}