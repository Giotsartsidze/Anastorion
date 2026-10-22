using UnityEngine;

/// <summary>
/// Hides the gameplay HUD (XP bar, coin/shard icon + text, kills, level, etc.) while
/// any menu panel is open, so panels aren't cluttered by the HUD behind them.
/// Put this anywhere in the gameplay scene and drag every HUD element into the list.
/// </summary>
public class HudVisibility : MonoBehaviour
{
    [Tooltip("HUD elements to hide while a panel is open — XP bar, coin icon, coin text, etc.")]
    public GameObject[] hudElements;

    void Awake()
    {
        MenuGuard.AnyOpen = false; // fresh scene = nothing open (clear any stale static)
    }

    void Update()
    {
        bool show = !MenuGuard.AnyOpen;
        foreach (var e in hudElements)
            if (e != null && e.activeSelf != show)
                e.SetActive(show);
    }
}
