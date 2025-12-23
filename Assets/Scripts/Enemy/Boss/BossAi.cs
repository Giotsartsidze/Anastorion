using UnityEngine;
using System.Collections;

public class BossAI : MonoBehaviour
{
    public GameObject projectilePrefab;
    public int projectileCount = 8; // რამდენი ისარი გავარდეს ერთდროულად
    public float attackInterval = 3f; // ყოველ 3 წამში
    public float projectileSpeed = 5f;

    void Start()
    {
        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackInterval);
            ShootStarPattern();
        }
    }

    void ShootStarPattern()
    {
        float angleStep = 360f / projectileCount;
        float angle = 0f;

        for (int i = 0; i < projectileCount; i++)
        {
            // ვიანგარიშებთ მიმართულებას წრეზე
            float dirX = transform.position.x + Mathf.Sin((angle * Mathf.Deg2Rad));
            float dirY = transform.position.y + Mathf.Cos((angle * Mathf.Deg2Rad));

            Vector3 projectileMoveVector = new Vector3(dirX, dirY, 0);
            Vector2 projectileDir = (projectileMoveVector - transform.position).normalized;

            // ვქმნით ტყვიას
            GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            
            // ვაძლევთ ტყვიას მიმართულებას და კუთხეს
            float rotationAngle = Mathf.Atan2(projectileDir.y, projectileDir.x) * Mathf.Rad2Deg;
            proj.transform.rotation = Quaternion.Euler(0, 0, rotationAngle);

            angle += angleStep;
        }
    }
}