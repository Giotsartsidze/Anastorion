using UnityEngine;

/// <summary>
/// Slow "Ken Burns" drift + breathe on a full-screen UI background image, so static
/// key art feels alive (like a gentle looping gif). Put on the menu's background Image.
/// Keep the image a touch larger than the screen (the overscan below handles that) so
/// the motion never reveals an edge.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class MenuBackgroundMotion : MonoBehaviour
{
    [Tooltip("Base zoom so drift never shows the image edges. 1.06 = 6% overscan.")]
    public float overscan = 1.06f;
    [Tooltip("How much it gently breathes in/out on top of the overscan.")]
    public float zoomAmount = 0.04f;
    [Tooltip("Pixels of slow drift.")]
    public float driftAmount = 18f;
    [Tooltip("Seconds for one full loop. Bigger = slower/calmer.")]
    public float cycleSeconds = 26f;

    private RectTransform rt;
    private Vector2 startPos;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        startPos = rt.anchoredPosition;
    }

    void Update()
    {
        float t = (Time.unscaledTime / cycleSeconds) * Mathf.PI * 2f;
        float zoom = overscan + zoomAmount * (0.5f + 0.5f * Mathf.Sin(t));
        rt.localScale = new Vector3(zoom, zoom, 1f);
        rt.anchoredPosition = startPos + new Vector2(Mathf.Sin(t * 0.6f), Mathf.Cos(t)) * driftAmount;
    }
}
