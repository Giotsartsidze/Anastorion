using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float speed = 2f;
    private Transform player;
    public GameObject xpOrbPrefab;
	[HideInInspector] public bool isBuffed = false;
	private float originalSpeed;

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

public void ApplySpeedBuff(float multiplier)
{
    if (isBuffed) return;
    
    isBuffed = true;
    originalSpeed = speed; // ვინახავთ საწყისს
    speed *= multiplier;
    
    // ვიზუალური ინდიკატორი აჩქარებულ მტერზე
    GetComponent<SpriteRenderer>().color = Color.yellow;
    
    // 3 წამში სიჩქარე დაუბრუნდეს ნორმას
    Invoke(nameof(ResetSpeed), 3f);
}

void ResetSpeed()
{
    speed = originalSpeed;
    isBuffed = false;
    GetComponent<SpriteRenderer>().color = Color.white;
}
}