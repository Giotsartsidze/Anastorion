using UnityEngine;

public class VortexAI : MonoBehaviour
{
    public float pullForce = 5f;
    public float pullRadius = 8f;
    public float minPullDistance = 1.5f; // ახალი ცვლადი: ამაზე ახლოს აღარ შეიწოვს
    private Transform player;
    private Rigidbody2D playerRb;
    private CharacterAnimator anim;
    private bool wasPulling = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player != null) playerRb = player.GetComponent<Rigidbody2D>();
        anim = GetComponent<CharacterAnimator>();
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        // შეიწოვს მხოლოდ მაშინ, თუ რადიუსშია და ძალიან ახლოს არ არის
        bool pulling = dist <= pullRadius && dist > minPullDistance;
        if (pulling)
        {
            Vector2 pullDir = ((Vector2)transform.position - (Vector2)player.position).normalized;
            if (playerRb != null)
                playerRb.AddForce(pullDir * pullForce, ForceMode2D.Force);
            else
                player.position = Vector2.MoveTowards(player.position, transform.position, pullForce * Time.deltaTime);
        }

        // Drive the channel animation on engage/disengage.
        if (pulling && !wasPulling && anim != null) anim.PlayAttackLooping();
        else if (!pulling && wasPulling && anim != null) anim.StopAttack();
        wasPulling = pulling;

        // თავად მტერი ნელა მოძრაობს
        transform.position = Vector2.MoveTowards(transform.position, player.position, 1f * Time.deltaTime);
    }
}