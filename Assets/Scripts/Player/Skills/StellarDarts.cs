using UnityEngine;

public class StellarDarts : MonoBehaviour
{
    public GameObject dartPrefab;
    public float fireRate = 1.5f;
    public float range = 10f;
    public LayerMask enemyLayer;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= fireRate)
        {
            ShootNearestEnemy();
            timer = 0;
        }
    }

    void ShootNearestEnemy()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, range, enemyLayer);
        
        Transform closestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (var enemy in enemies)
        {
            float dist = Vector2.Distance(transform.position, enemy.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closestEnemy = enemy.transform;
            }
        }

        if (closestEnemy != null)
        {
            // ვქმნით ისარს და ვატრიალებთ მტრისკენ
            Vector2 direction = (closestEnemy.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Instantiate(dartPrefab, transform.position, Quaternion.Euler(0, 0, angle));
        }
    }
}