using UnityEngine;

/// <summary>
/// Infinite, seamless parallax background layer for top-down play.
/// Put on a GameObject with a SpriteRenderer set to Draw Mode = Tiled (and the
/// sprite's Wrap Mode = Repeat), sized larger than the camera view.
///
/// parallax = 0  → texture stays fixed in the world (scrolls past fastest = foreground)
/// parallax = 1  → layer locked to the camera (appears static = infinitely far)
/// So: nebula ~0.85, far stars ~0.6, near stars ~0.35 gives depth.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class ParallaxLayer : MonoBehaviour
{
    [Range(0f, 1f)] public float parallax = 0.5f;
    public Transform cameraTransform;

    private float tileX, tileY;

    void Start()
    {
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        // One tile's world size (before tiling repeats it).
        tileX = sr.sprite.rect.width / sr.sprite.pixelsPerUnit;
        tileY = sr.sprite.rect.height / sr.sprite.pixelsPerUnit;
    }

    void LateUpdate()
    {
        if (cameraTransform == null || tileX <= 0f || tileY <= 0f) return;

        Vector3 cam = cameraTransform.position;

        // Continuous parallax drift...
        float px = cam.x * parallax;
        float py = cam.y * parallax;

        // ...then re-center on the camera in whole-tile steps. Because the texture is
        // seamless, these jumps are invisible, but they guarantee the layer always
        // covers the view — infinite scroll with no visible edges.
        float x = px + Mathf.Round((cam.x - px) / tileX) * tileX;
        float y = py + Mathf.Round((cam.y - py) / tileY) * tileY;

        transform.position = new Vector3(x, y, transform.position.z);
    }
}
