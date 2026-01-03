using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChainLightning : MonoBehaviour
{
    public int damage = 10;
    public int maxJumps = 4;
    public float jumpRadius = 5f;
    public LayerMask enemyLayer;

    // ვიზუალიზაციისთვის შეგვიძლია გამოვიყენოთ LineRenderer
    public GameObject lightningLinePrefab; 

    public void TriggerChain(Transform startTarget)
    {
        StartCoroutine(LightningRoutine(startTarget));
    }

    IEnumerator LightningRoutine(Transform startTransform)
    {
        Transform currentTarget = startTransform;
        List<Transform> hitEnemies = new List<Transform>();

        for (int i = 0; i < maxJumps; i++)
        {
            if (currentTarget == null) break;

            // ზიანის მიყენება
            currentTarget.GetComponent<EnemyHealth>()?.TakeDamage(damage);
            hitEnemies.Add(currentTarget);

            // ვეძებთ შემდეგ მსხვერპლს
            Transform nextTarget = FindNextTarget(currentTarget, hitEnemies);
            
            if (nextTarget != null)
            {
                // ვიზუალური ეფექტი ორ მტერს შორის
                CreateLightningLine(currentTarget.position, nextTarget.position);
                
                currentTarget = nextTarget;
                yield return new WaitForSeconds(0.05f); // მცირე დაყოვნება "ჯაჭვის" ეფექტისთვის
            }
            else break;
        }
    }

    Transform FindNextTarget(Transform current, List<Transform> hitEnemies)
    {
        Collider2D[] potentialTargets = Physics2D.OverlapCircleAll(current.position, jumpRadius, enemyLayer);
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (var col in potentialTargets)
        {
            if (hitEnemies.Contains(col.transform)) continue; // რომ ერთს ორჯერ არ დაეცეს

            float dist = Vector2.Distance(current.position, col.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = col.transform;
            }
        }
        return closest;
    }

    void CreateLightningLine(Vector3 start, Vector3 end)
    {
        if (lightningLinePrefab != null)
        {
            GameObject line = Instantiate(lightningLinePrefab, Vector3.zero, Quaternion.identity);
            LineRenderer lr = line.GetComponent<LineRenderer>();
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
            Destroy(line, 0.2f); // ხაზი მალევე ქრება
        }
    }
}