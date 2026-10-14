using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Grows the player's light as they level up, so the dark closes in less and less
/// the stronger you get. Put on the Player and assign its point Light2D (or it
/// auto-finds one in its children). Smoothly lerps the lit radius toward the
/// level-based target each frame.
/// </summary>
public class PlayerVision : MonoBehaviour
{
    public Light2D visionLight;         // the player's point Light2D (the one that lights the area)
    public float baseRadius = 9f;       // outer radius at level 1
    public float growthPerLevel = 0.7f; // extra radius gained per level
    public float maxRadius = 18f;       // cap so the whole map never just lights up
    public float lerpSpeed = 2f;        // how fast it eases to the new size
    [Range(0f, 0.9f)] public float innerRatio = 0.35f;

    private ExperienceManager xp;

    void Start()
    {
        if (visionLight == null) visionLight = GetComponentInChildren<Light2D>();
        xp = FindObjectOfType<ExperienceManager>();
    }

    void Update()
    {
        if (visionLight == null) return;

        int level = xp != null ? xp.currentLevel : 1;
        float target = Mathf.Min(maxRadius, baseRadius + (level - 1) * growthPerLevel);

        float r = Mathf.Lerp(visionLight.pointLightOuterRadius, target, Time.deltaTime * lerpSpeed);
        visionLight.pointLightOuterRadius = r;
        visionLight.pointLightInnerRadius = r * innerRatio;
    }
}
