using UnityEngine;

/// <summary>
/// One-shot sprite VFX: grows slightly and fades out, then destroys itself.
/// Put on a prefab with a SpriteRenderer (e.g. the poison splat) and spawn it.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class FadeAndDie : MonoBehaviour
{
    public float life = 0.45f;
    public float growTo = 1.6f;

    private SpriteRenderer sr;
    private Vector3 baseScale;
    private Color baseCol;
    private float t;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        baseScale = transform.localScale;
        baseCol = sr.color;
    }

    void Update()
    {
        t += Time.deltaTime;
        float p = Mathf.Clamp01(t / life);
        transform.localScale = baseScale * Mathf.Lerp(1f, growTo, p);
        Color c = baseCol; c.a = baseCol.a * (1f - p); sr.color = c;
        if (t >= life) Destroy(gameObject);
    }
}
