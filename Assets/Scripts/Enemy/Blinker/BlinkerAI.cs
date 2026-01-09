using UnityEngine;
using System.Collections;

public class BlinkerAI : MonoBehaviour
{
    public float speed = 2f;
    public float blinkDistance = 4f;
    public float blinkCooldown = 3f;
    private Transform player;
    private SpriteRenderer sprite;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        sprite = GetComponent<SpriteRenderer>();
        StartCoroutine(BlinkRoutine());
    }

    void Update()
    {
        if (player == null) return;
        transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
    }

    IEnumerator BlinkRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(blinkCooldown);
            
            // ვიზუალური გაფრთხილება (ციმციმი)
            sprite.color = new Color(1, 1, 1, 0.3f);
            yield return new WaitForSeconds(0.5f);
            
            // ტელეპორტაცია მოთამაშისკენ
            Vector2 blinkDir = (player.position - transform.position).normalized;
            transform.position += (Vector3)blinkDir * blinkDistance;
            
            sprite.color = Color.white;
        }
    }
}