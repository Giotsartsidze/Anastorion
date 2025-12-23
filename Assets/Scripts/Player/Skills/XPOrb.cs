using UnityEngine;

public class XPOrb : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float collectDistance = 0.5f;
    public float magnetRadius = 5f;
    private Transform player;

    void Start()
    {
        // ვეძებთ მოთამაშეს
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        // Null Check - თუ ფლეიერი ვერ მოიძებნა, არაფერი გააკეთო
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        
        // "მაგნიტის" ეფექტი
        if (distance < magnetRadius) 
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        }

        if (distance < collectDistance)
        {
            Collect();
        }
    }

    void Collect()
    {
        // ვპოულობთ XP მენეჯერს და ვამატებთ ქულას
        var manager = FindObjectOfType<ExperienceManager>();
        if (manager != null)
        {
            manager.AddExperience(10);
        }
        
        Destroy(gameObject);
    }
}