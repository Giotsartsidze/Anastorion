using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Full-screen dark overlay that automatically appears whenever the game is paused
/// (any menu open = Time.timeScale 0) and hides during play. Dims the gameplay behind
/// every menu — pause, settings, shop, level-up — with zero per-menu wiring.
///
/// Setup: put this on a full-screen UI Image (black, ~70% alpha, Raycast Target ON),
/// placed as the FIRST child of the Canvas so it sits behind the HUD and menu panels.
/// </summary>
[RequireComponent(typeof(Image))]
public class PauseDimmer : MonoBehaviour
{
    private Image img;

    void Awake()
    {
        img = GetComponent<Image>();
        img.enabled = false;
    }

    void Update()
    {
        bool paused = Time.timeScale == 0f;
        if (img.enabled != paused) img.enabled = paused;
    }
}
