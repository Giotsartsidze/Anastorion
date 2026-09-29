using System.Collections;
using UnityEngine;

/// <summary>
/// Code-only death "pop": punches the sprite scale up, then shrinks it to nothing
/// while fading out. No art or particles required.
///
/// Pooling-safe: captures the resting scale/colors and restores them on every enable,
/// so a pooled enemy that "died" small and transparent comes back looking normal.
/// </summary>
[DisallowMultipleComponent]
public class DeathPop : MonoBehaviour
{
    [Tooltip("Scale multiplier at the very start of the pop (the 'punch').")]
    public float punchScale = 1.4f;

    private Vector3 originalScale;
    private SpriteRenderer[] renderers;
    private Color[] originalColors;
    private bool captured;

    void Awake()
    {
        originalScale = transform.localScale;
        renderers = GetComponentsInChildren<SpriteRenderer>(true);
        originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
            originalColors[i] = renderers[i].color;
        captured = true;
    }

    // Every pooled respawn: put the look back to normal.
    void OnEnable()
    {
        if (captured) Restore();
    }

    private void Restore()
    {
        transform.localScale = originalScale;
        for (int i = 0; i < renderers.Length; i++)
            if (renderers[i] != null) renderers[i].color = originalColors[i];
    }

    public IEnumerator Play(float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / duration);

            transform.localScale = originalScale * Mathf.Lerp(punchScale, 0f, p);

            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] == null) continue;
                Color c = originalColors[i];
                c.a = Mathf.Lerp(originalColors[i].a, 0f, p);
                renderers[i].color = c;
            }
            yield return null;
        }
    }
}
