using System.Collections;
using UnityEngine;

/// <summary>
/// Drop-in white-flash on hit. Auto-collects every SpriteRenderer under this object,
/// so it works for single-sprite and multi-part enemies alike.
/// Pooling-safe: restores original colors if the object is disabled mid-flash.
/// </summary>
[DisallowMultipleComponent]
public class HitFlash : MonoBehaviour
{
    [Tooltip("Color to flash to on hit (usually white).")]
    public Color flashColor = Color.white;

    [Tooltip("How long the flash lasts, in seconds.")]
    public float flashDuration = 0.06f;

    private SpriteRenderer[] renderers;
    private Color[] originalColors;
    private Coroutine flashRoutine;

    void Awake()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>(true);
        originalColors = new Color[renderers.Length];
    }

    public void Flash()
    {
        if (renderers == null || renderers.Length == 0) return;
        // Ignore re-triggers while already flashing so we never overwrite the
        // captured "original" colors with the flash color.
        if (flashRoutine != null) return;
        flashRoutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] == null) continue;
            originalColors[i] = renderers[i].color; // capture live color (respects elite tints)
            renderers[i].color = flashColor;
        }

        yield return new WaitForSeconds(flashDuration);

        RestoreColors();
        flashRoutine = null;
    }

    private void RestoreColors()
    {
        for (int i = 0; i < renderers.Length; i++)
            if (renderers[i] != null) renderers[i].color = originalColors[i];
    }

    // Pooled enemies get SetActive(false) on death — make sure they never come
    // back frozen mid-flash with the wrong color.
    void OnDisable()
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
            flashRoutine = null;
            RestoreColors();
        }
    }
}
