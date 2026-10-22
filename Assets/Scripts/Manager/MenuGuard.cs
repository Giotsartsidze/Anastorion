/// <summary>
/// Tiny shared flag: true while ANY blocking panel (level-up, pause, stats) is open.
/// Panels set it so only one can be open at a time, and the HUD hides while it's set.
/// Reset to false on scene load (HudVisibility.Awake) so a stale value can't linger.
/// </summary>
public static class MenuGuard
{
    public static bool AnyOpen = false;
}
