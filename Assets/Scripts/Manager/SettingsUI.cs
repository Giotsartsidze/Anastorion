using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Wires the options panel's controls to SettingsManager. Put on the Settings panel.
/// Sliders should have Min 0 / Max 1. Initializes from saved values and writes back on change.
/// </summary>
public class SettingsUI : MonoBehaviour
{
    public Slider masterSlider, musicSlider, sfxSlider;
    public Toggle fullscreenToggle;
    public TMP_Dropdown resolutionDropdown;

    // Refresh the controls to the current saved values each time the panel opens.
    void OnEnable()
    {
        var s = SettingsManager.Instance;
        if (s == null) return;

        if (masterSlider) masterSlider.SetValueWithoutNotify(s.Master);
        if (musicSlider) musicSlider.SetValueWithoutNotify(s.Music);
        if (sfxSlider) sfxSlider.SetValueWithoutNotify(s.SFX);
        if (fullscreenToggle) fullscreenToggle.SetIsOnWithoutNotify(s.Fullscreen);

        if (resolutionDropdown)
        {
            resolutionDropdown.ClearOptions();
            var opts = new List<string>();
            foreach (var r in s.Resolutions) opts.Add(r.width + " x " + r.height);
            resolutionDropdown.AddOptions(opts);
            resolutionDropdown.SetValueWithoutNotify(s.ResolutionIndex);
            resolutionDropdown.RefreshShownValue();
        }
    }

    void Start()
    {
        if (masterSlider) masterSlider.onValueChanged.AddListener(v => SettingsManager.Instance.SetMaster(v));
        if (musicSlider) musicSlider.onValueChanged.AddListener(v => SettingsManager.Instance.SetMusic(v));
        if (sfxSlider) sfxSlider.onValueChanged.AddListener(v => SettingsManager.Instance.SetSFX(v));
        if (fullscreenToggle) fullscreenToggle.onValueChanged.AddListener(f => SettingsManager.Instance.SetFullscreen(f));
        if (resolutionDropdown) resolutionDropdown.onValueChanged.AddListener(i => SettingsManager.Instance.SetResolution(i));
    }
}
