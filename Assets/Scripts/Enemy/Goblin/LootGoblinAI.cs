using UnityEngine;

public class LootGoblinAI : MonoBehaviour
{
    public float escapeSpeed = 5f;
    public float despawnTime = 10f;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        // თუ მოთამაშე 10 წამში ვერ მოკლავს, მტერი ქრება
        Destroy(gameObject, despawnTime);
    }

    void Update()
    {
        if (player == null) return;

        // მოთამაშისგან საპირისპირო მიმართულებით მოძრაობა
        Vector2 escapeDirection = (transform.position - player.position).normalized;
        transform.position += (Vector3)escapeDirection * escapeSpeed * Time.deltaTime;

        // Sprite Flip (რომ ყოველთვის წინ იყურებოდეს გაქცევისას)
        transform.localScale = new Vector3(escapeDirection.x < 0 ? -1 : 1, 1, 1);
    }
}