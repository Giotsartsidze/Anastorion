using UnityEngine;

public class RangedEnemyAI : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 3f;
    public float stoppingDistance = 5f; // რა მანძილზე გაჩერდეს
    public float retreatDistance = 3f;  // როდის დაიწყოს უკან დახევა

    [Header("Combat")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 2f;
    private float nextFireTime;

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // 1. მოძრაობის ლოგიკა
        if (distance > stoppingDistance)
        {
            // ძალიან შორსაა - მიუახლოვდეს
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }
        else if (distance < stoppingDistance && distance > retreatDistance)
        {
            // იდეალურ მანძილზეა - გაჩერდეს
            transform.position = transform.position;
        }
        else if (distance < retreatDistance)
        {
            // ძალიან ახლოსაა - დაიხიოს უკან
            transform.position = Vector2.MoveTowards(transform.position, player.position, -speed * Time.deltaTime);
        }

        // 2. სროლის ლოგიკა
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }

        // მუდამ ფლეიერისკენ იყურებოდეს (Flip Sprite)
        if (player.position.x < transform.position.x)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);
    }

    void Shoot()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        }
    }
}