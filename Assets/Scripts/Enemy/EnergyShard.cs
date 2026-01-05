using UnityEngine;

public class EnergyShard : MonoBehaviour
{
    public float magnetRange = 5f;
    public float moveSpeed = 10f;
    private Transform player;
    private bool isFollowing = false;

    void Start()
    {
        // ვპოულობთ ფლეიერს თეგის მიხედვით
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // თუ ფლეიერი რადიუსშია, ვიწყებთ მიდევნებას
        if (distance <= magnetRange) isFollowing = true;

        if (isFollowing)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.AddShards(1);
                Debug.Log("Shard Collected! Total: " + CurrencyManager.Instance.totalShards);
            }
            Destroy(gameObject);
        }
    }
}