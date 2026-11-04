using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Plays a sound when the pointer hovers a UI button, and another on click.
/// Put it on each button (or a prefab) and assign the clips. Uses one shared
/// AudioSource so many buttons don't each need their own.
/// </summary>
public class ButtonHoverSound : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public AudioClip hoverClip;
    public AudioClip clickClip;
    [Range(0f, 1f)] public float volume = 0.6f;

    [Header("Hover grow")]
    public float hoverScale = 1.05f;   // how much the button grows on hover
    public float scaleSpeed = 14f;     // how snappy the grow/shrink is

    private Vector3 baseScale;
    private float target = 1f;
    private static AudioSource shared;

    private static AudioSource Src()
    {
        if (shared == null)
        {
            var go = new GameObject("UI_SFX_Source");
            shared = go.AddComponent<AudioSource>();
            shared.playOnAwake = false;
            shared.spatialBlend = 0f; // 2D
        }
        return shared;
    }

    void Awake()
    {
        baseScale = transform.localScale;
    }

    void OnDisable()
    {
        transform.localScale = baseScale; // don't get stuck scaled if hidden mid-hover
        target = 1f;
    }

    void Update()
    {
        Vector3 want = baseScale * target;
        if ((transform.localScale - want).sqrMagnitude > 0.0000001f)
            transform.localScale = Vector3.Lerp(transform.localScale, want, Time.unscaledDeltaTime * scaleSpeed);
    }

    public void OnPointerEnter(PointerEventData e)
    {
        target = hoverScale;
        if (hoverClip != null) Src().PlayOneShot(hoverClip, volume);
    }

    public void OnPointerExit(PointerEventData e)
    {
        target = 1f;
    }

    public void OnPointerClick(PointerEventData e)
    {
        if (clickClip != null) Src().PlayOneShot(clickClip, volume);
    }
}
