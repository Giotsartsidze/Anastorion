using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public enum DropType { XP, Health } // ჩამონათვალი სხვადასხვა დროპისთვის

    [Header("Stats")]
    public int health = 1;
    public DropType dropType = DropType.XP; // დეფოლტად XP

    [Header("Drop Prefabs")]
    public GameObject coinPrefab; // XP ორბი
    public GameObject healthPackPrefab; // სიცოცხლის აღსადგენი პაკეტი
    public int dropCount = 1;

    private bool isDying = false;

    public void TakeDamage(int damage)
    {
        if (isDying) return;
        health -= damage;

        if (health <= 0) StartCoroutine(DeathSequence());
    }

    IEnumerator DeathSequence()
    {
        isDying = true;
        yield return new WaitForSeconds(0.15f);

        // ვირჩევთ რომელ პრეფაბს ვაჩენთ არჩეული DropType-ის მიხედვით
        GameObject prefabToSpawn = (dropType == DropType.Health) ? healthPackPrefab : coinPrefab;

        if (prefabToSpawn != null)
        {
            for (int i = 0; i < dropCount; i++)
            {
                Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
                Instantiate(prefabToSpawn, transform.position + randomOffset, Quaternion.identity);
            }
        }

        Destroy(gameObject);
    }
}