using UnityEngine;
using System.Collections;

public class LeaperAI : MonoBehaviour
{
    public float walkSpeed = 1.5f;
    public float jumpForce = 10f;
    public float jumpInterval = 3f;
    private bool isJumping = false;
    private Transform player;
    private Rigidbody2D rb;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(JumpRoutine());
    }

    void Update()
    {
        if (player == null || isJumping) return;
        // ჩვეულებრივი ნელი დევნა ნახტომებს შორის
        transform.position = Vector2.MoveTowards(transform.position, player.position, walkSpeed * Time.deltaTime);
    }

    IEnumerator JumpRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(jumpInterval);
            if (player != null)
            {
                isJumping = true;
                Vector2 jumpDir = (player.position - transform.position).normalized;
                rb.AddForce(jumpDir * jumpForce, ForceMode2D.Impulse);
                
                yield return new WaitForSeconds(1f); // ნახტომის ხანგრძლივობა
                rb.linearVelocity = Vector2.zero;
                isJumping = false;
            }
        }
    }
}