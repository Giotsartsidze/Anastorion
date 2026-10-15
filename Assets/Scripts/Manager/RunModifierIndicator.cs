using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// In-game HUD label listing the active run modifiers (e.g. "CURSED: Horde · Giants").
/// Put on a TMP_Text in the gameplay HUD. Hides itself when no modifiers are active.
/// </summary>
[RequireComponent(typeof(TMP_Text))]
public class RunModifierIndicator : MonoBehaviour
{
    public string prefix = "CURSED: ";
    private TMP_Text label;

    void Awake() => label = GetComponent<TMP_Text>();

    void Start()
    {
        List<string> active = new List<string>();
        foreach (var k in RunModifiers.Keys)
            if (RunModifiers.IsOn(k)) active.Add(k);

        if (active.Count == 0)
        {
            gameObject.SetActive(false); // no cursed run → no clutter
            return;
        }

        label.text = prefix + string.Join("  ·  ", active);
    }
}
