using UnityEngine;

public class VortexAI : MonoBehaviour
{
    public float pullForce = 5f;
    public float pullRadius = 8f;
    public float minPullDistance = 1.5f; // ახალი ცვლადი: ამაზე ახლოს აღარ შეიწოვს
    private Transform player;

    void Start() => player = GameObject.FindGameObjectWithTag("Player").transform;

    void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);
        
        // შეიწოვს მხოლოდ მაშინ, თუ რადიუსშია და ძალიან ახლოს არ არის
        if (dist <= pullRadius && dist > minPullDistance)
        {
            player.position = Vector2.MoveTowards(player.position, transform.position, pullForce * Time.deltaTime);
        }
        
        // თავად მტერი ნელა მოძრაობს
        transform.position = Vector2.MoveTowards(transform.position, player.position, 1f * Time.deltaTime);
    }
}