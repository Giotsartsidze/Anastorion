using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float speed = 2f;
    private Transform player;
    public GameObject xpOrbPrefab;

    void Start()
    {
        // ვპოულობთ მოთამაშეს Tag-ით (დარწმუნდი, რომ Player-ს აქვს Tag "Player")
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player != null)
        {
            // მტერი მუდმივად მიდის მოთამაშისკენ
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }
    }
    
    public void Die()
    {
        Instantiate(xpOrbPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}