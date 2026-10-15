using UnityEngine;

/// <summary>
/// Persistent toggle state for run modifiers ("curses"), chosen on the main menu and
/// read by RunModifierManager when a run starts. Stored in PlayerPrefs ("Mod_<key>").
/// Harder run = more shards.
/// </summary>
public static class RunModifiers
{
    public static readonly string[] Keys = { "Horde", "Giants", "Fragile", "Famine", "Blackout" };

    public static bool IsOn(string key) => PlayerPrefs.GetInt("Mod_" + key, 0) == 1;

    public static void Set(string key, bool on)
    {
        PlayerPrefs.SetInt("Mod_" + key, on ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static int ActiveCount()
    {
        int n = 0;
        foreach (var k in Keys) if (IsOn(k)) n++;
        return n;
    }
}
