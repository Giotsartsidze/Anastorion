using UnityEngine;
using TMPro;

/// <summary>
/// A single floating damage number. Rises, scales down, and fades out, then
/// destroys itself. Spawn these through <see cref="DamagePopupSpawner"/>.
/// Requires a TextMeshPro (world-space, NOT the UGUI one) component on the same object.
/// </summary>
[RequireComponent(typeof(TextMeshPro))]
public class DamagePopup : MonoBehaviour
{
    [Tooltip("Seconds before the number disappears.")]
    public float lifetime = 0.6f;

    [Tooltip("Upward drift speed in world units/sec.")]
    public float riseSpeed = 1.2f;

    [Tooltip("Random spawn offset so stacked hits don't overlap perfectly.")]
    public Vector2 randomSpread = new Vector2(0.3f, 0.15f);

    [Tooltip("Sorting order for the text mesh. High so numbers draw ON TOP of enemies/background.")]
    public int sortingOrder = 1000;

    private TextMeshPro tmp;
    private float timer;
    private Color startColor;

    void Awake()
    {
        tmp = GetComponent<TextMeshPro>();
        // Force the number to render above sprites, which otherwise hide it.
        Renderer r = GetComponent<Renderer>();
        if (r != null) r.sortingOrder = sortingOrder;
    }

    public void Setup(int amount)
    {
        if (tmp == null) tmp = GetComponent<TextMeshPro>();
        tmp.text = amount.ToString();

        // Snap to the camera plane (z=0) so an off-plane enemy z can't hide it.
        Vector3 p = transform.position;
        p.z = 0f;
        transform.position = p;

        transform.position += new Vector3(
            Random.Range(-randomSpread.x, randomSpread.x),
            Random.Range(0f, randomSpread.y),
            0f);

        startColor = tmp.color;
        startColor.a = 1f;
        tmp.color = startColor;
        timer = 0f;
        transform.localScale = Vector3.one * 1.2f;
    }

    void Update()
    {
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / lifetime);

        transform.position += Vector3.up * riseSpeed * Time.deltaTime;
        transform.localScale = Vector3.one * Mathf.Lerp(1.2f, 0.9f, t);

        Color c = startColor;
        c.a = Mathf.Lerp(1f, 0f, t);
        tmp.color = c;

        if (timer >= lifetime) Destroy(gameObject);
    }
}
