using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Drop this on a UI Toggle and set its modifierKey (Horde/Giants/Fragile/Famine/Blackout).
/// It loads the saved state and writes any change back to RunModifiers — no extra wiring.
/// </summary>
[RequireComponent(typeof(Toggle))]
public class RunModifierToggle : MonoBehaviour
{
    public string modifierKey = "Horde";
    private Toggle toggle;

    void Awake() => toggle = GetComponent<Toggle>();

    void OnEnable()
    {
        if (toggle == null) toggle = GetComponent<Toggle>();
        toggle.isOn = RunModifiers.IsOn(modifierKey);
        toggle.onValueChanged.AddListener(OnChanged);
    }

    void OnDisable() => toggle.onValueChanged.RemoveListener(OnChanged);

    void OnChanged(bool on) => RunModifiers.Set(modifierKey, on);
}
