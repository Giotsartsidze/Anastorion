using UnityEngine;

/// <summary>
/// Scatters non-colliding decorative props (rocks, bones, dead trees, ruins...) across
/// the ground so the map stops looking empty. Pure visual — no colliders, sorted far
/// behind gameplay. Put ONE of these in the scene, drag the prop sprites into the list.
/// Deterministic per seed, so the layout is stable between runs (change the seed to reroll).
/// </summary>
public class BackgroundScatterer : MonoBehaviour
{
    [Header("Props")]
    public Sprite[] propSprites;

    [Header("Field")]
    [Tooltip("How many props to scatter.")]
    public int count = 250;
    [Tooltip("Size of the rectangular field, centered on this object.")]
    public Vector2 area = new Vector2(90f, 90f);
    [Tooltip("Keep this radius around the center clear (player spawn).")]
    public float clearRadius = 5f;
    public int seed = 12345;

    [Header("Look")]
    public Vector2 scaleRange = new Vector2(0.6f, 1.3f);
    [Tooltip("Random darkening so props vary (1 = full bright, lower = dimmer).")]
    public Vector2 brightnessRange = new Vector2(0.6f, 1f);
    public string sortingLayerName = "Default";
    [Tooltip("Very negative so props render behind the ground gameplay.")]
    public int sortingOrder = -100;

    void Start()
    {
        if (propSprites == null || propSprites.Length == 0) return;

        // Seed locally, then restore global RNG so we don't disturb gameplay randomness.
        Random.State prev = Random.state;
        Random.InitState(seed);

        Transform parent = new GameObject("BackgroundProps").transform;
        parent.SetParent(transform, false);

        Vector3 center = transform.position;
        float clearSqr = clearRadius * clearRadius;

        for (int i = 0; i < count; i++)
        {
            Vector2 offset = new Vector2(
                Random.Range(-area.x * 0.5f, area.x * 0.5f),
                Random.Range(-area.y * 0.5f, area.y * 0.5f));

            if (offset.sqrMagnitude < clearSqr) continue; // keep spawn area clean

            GameObject p = new GameObject("prop");
            p.transform.SetParent(parent, false);
            p.transform.position = center + (Vector3)offset;

            float s = Random.Range(scaleRange.x, scaleRange.y);
            p.transform.localScale = new Vector3(s, s, 1f);

            SpriteRenderer sr = p.AddComponent<SpriteRenderer>();
            sr.sprite = propSprites[Random.Range(0, propSprites.Length)];
            sr.flipX = Random.value < 0.5f;
            if (!string.IsNullOrEmpty(sortingLayerName)) sr.sortingLayerName = sortingLayerName;
            // spread props slightly in order so overlaps layer naturally
            sr.sortingOrder = sortingOrder + (i % 20);

            float b = Random.Range(brightnessRange.x, brightnessRange.y);
            sr.color = new Color(b, b, b, 1f);
        }

        Random.state = prev;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3(area.x, area.y, 0f));
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, clearRadius);
    }
}
