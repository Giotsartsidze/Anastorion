using UnityEngine;

/// <summary>
/// Dropped by a mega-elite. Walk into it to claim a guaranteed high-rarity upgrade
/// (opens the upgrade screen biased toward the best cards). Bobs + pulses so it
/// reads as a special reward. Needs a trigger Collider2D and the "Player" tag check.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class RelicPickup : MonoBehaviour
{
    public float bobAmplitude = 0.2f;
    public float bobSpeed = 2f;

    private Vector3 basePos;
    private Vector3 baseScale;
    private bool claimed;

    void Start()
    {
        basePos = transform.position;
        baseScale = transform.localScale;
    }

    void Update()
    {
        transform.position = basePos + Vector3.up * Mathf.Sin(Time.time * bobSpeed) * bobAmplitude;
        float s = 1f + Mathf.Sin(Time.time * 3f) * 0.08f;
        transform.localScale = baseScale * s;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (claimed || !other.CompareTag("Player")) return;
        claimed = true;
        FindObjectOfType<UpgradeManager>()?.ShowRelicReward();
        Destroy(gameObject);
    }
}
