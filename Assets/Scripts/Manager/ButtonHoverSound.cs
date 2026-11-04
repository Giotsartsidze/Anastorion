using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Plays a sound when the pointer hovers a UI button, and another on click.
/// Put it on each button (or a prefab) and assign the clips. Uses one shared
/// AudioSource so many buttons don't each need their own.
/// </summary>
public class ButtonHoverSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public AudioClip hoverClip;
    public AudioClip clickClip;
    [Range(0f, 1f)] public float volume = 0.6f;

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

    public void OnPointerEnter(PointerEventData e)
    {
        if (hoverClip != null) Src().PlayOneShot(hoverClip, volume);
    }

    public void OnPointerClick(PointerEventData e)
    {
        if (clickClip != null) Src().PlayOneShot(clickClip, volume);
    }
}
