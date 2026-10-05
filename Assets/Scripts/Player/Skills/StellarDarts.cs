using UnityEngine;

public class StellarDarts : MonoBehaviour
{
    public GameObject dartPrefab;
    public float fireRate = 1.5f;
    public float range = 10f;
    public int dartCount = 1;   // how many darts per volley (upgradeable)
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
        if (enemies.Length == 0) return;

        // nearest first
        System.Array.Sort(enemies, (a, b) =>
            Vector2.Distance(transform.position, a.transform.position)
            .CompareTo(Vector2.Distance(transform.position, b.transform.position)));

        int shots = Mathf.Max(1, dartCount);
        for (int i = 0; i < shots; i++)
        {
            // target distinct nearest enemies; extra darts fan out from the nearest
            Transform target = enemies[Mathf.Min(i, enemies.Length - 1)].transform;
            Vector2 direction = ((Vector2)target.position - (Vector2)transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            if (i >= enemies.Length)
            {
                int extra = i - enemies.Length + 1;
                angle += extra * 12f * (extra % 2 == 0 ? 1 : -1); // alternating spread
            }
            Instantiate(dartPrefab, transform.position, Quaternion.Euler(0, 0, angle));
        }

        if (SoundManager.Instance != null) SoundManager.Instance.PlayShoot();
    }
}