using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Starfall — periodically rains meteors onto enemies around the player (AoE on
/// impact), great for clearing the horde away from you. Put on the Player, assign
/// the meteor prefab. Starts inactive; unlocked/scaled by its upgrade card.
/// </summary>
public class Starfall : MonoBehaviour
{
    public GameObject meteorPrefab;   // prefab with a SpriteRenderer + Meteor
    public float interval = 3f;       // seconds between volleys
    public int meteorCount = 3;       // meteors per volley (raised by picks)
    public float range = 8f;          // target area around the player
    public int meteorDamage = 12;
    public float meteorRadius = 1.5f;
    public bool active = false;       // turned on when the Starfall card is picked
    public bool evolved = false;      // Meteor Storm evolution

    private float timer;

    /// <summary>Meteor Storm evolution: a relentless, bigger, deadlier rain.</summary>
    public void Evolve()
    {
        if (evolved) return;
        evolved = true;
        meteorCount += 4;
        interval = Mathf.Max(0.5f, interval * 0.5f);
        meteorRadius += 1f;
        meteorDamage += 10;
    }

    void Update()
    {
        if (!active || meteorPrefab == null) return;

        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer = 0f;
            CallMeteors();
        }
    }

    void CallMeteors()
    {
        // Prefer raining onto actual enemies; fall back to random points nearby.
        Collider2D[] near = Physics2D.OverlapCircleAll(transform.position, range);
        List<Vector3> targets = new List<Vector3>();
        foreach (var c in near)
            if (c.CompareTag("Enemy")) targets.Add(c.transform.position);

        for (int i = 0; i < meteorCount; i++)
        {
            Vector3 target = targets.Count > 0
                ? targets[Random.Range(0, targets.Count)]
                : transform.position + (Vector3)(Random.insideUnitCircle * range);

            Vector3 spawnPos = target + new Vector3(0f, 8f, 0f); // start "above"
            GameObject m = Instantiate(meteorPrefab, spawnPos, Quaternion.identity);
            Meteor meteor = m.GetComponent<Meteor>();
            if (meteor != null) meteor.Init(target, meteorDamage, meteorRadius);
        }
    }
}
