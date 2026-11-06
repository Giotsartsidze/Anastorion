using UnityEngine;
using UnityEngine.UI;

// Drop ONE of these in a scene (e.g. on the EventSystem) and assign the hover /
// click clips. On start it finds every Button in the scene — including ones under
// inactive panels and the level-up upgrade cards — and gives each a UISound so the
// whole UI gets consistent click/hover feedback without hand-wiring each button.
//
// Buttons that already have a ButtonHoverSound (the main-menu buttons) are left
// alone, so nothing double-plays. Re-scans a few times after load to catch panels
// that get spawned slightly later.
public class UISoundInstaller : MonoBehaviour
{
    public AudioClip hoverClip;
    public AudioClip clickClip;
    [Range(0f, 1f)] public float volume = 0.6f;

    static AudioSource shared;
    static AudioSource Source()
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

    void Start()
    {
        Install();
        Invoke(nameof(Install), 1f);   // catch panels spawned shortly after load
        Invoke(nameof(Install), 3f);
    }

    public void Install()
    {
        var buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var b in buttons)
        {
            if (b.GetComponent<ButtonHoverSound>() != null) continue; // already has its own sound
            var s = b.GetComponent<UISound>();
            if (s == null) s = b.gameObject.AddComponent<UISound>();
            s.Bind(this);
        }
    }

    public void PlayHover() { if (hoverClip != null) Source().PlayOneShot(hoverClip, volume); }
    public void PlayClick() { if (clickClip != null) Source().PlayOneShot(clickClip, volume); }
}
