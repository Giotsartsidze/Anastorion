using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Single source of truth for options (volumes, fullscreen, resolution).
/// Persists to PlayerPrefs and applies live. Survives scene loads so settings
/// stick between menu and gameplay. Drives audio via AudioListener (master),
/// MusicManager (music), and SoundManager (sfx).
/// </summary>
public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    public float Master { get; private set; } = 1f;
    public float Music { get; private set; } = 0.5f;
    public float SFX { get; private set; } = 1f;
    public bool Fullscreen { get; private set; } = true;

    public List<Resolution> Resolutions { get; private set; } = new List<Resolution>();
    public int ResolutionIndex { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Master = PlayerPrefs.GetFloat("set_master", 1f);
        Music = PlayerPrefs.GetFloat("set_music", 0.5f);
        SFX = PlayerPrefs.GetFloat("set_sfx", 1f);
        Fullscreen = PlayerPrefs.GetInt("set_fullscreen", 1) == 1;

        var seen = new HashSet<string>();
        foreach (var r in Screen.resolutions)
        {
            string key = r.width + "x" + r.height;
            if (seen.Add(key)) Resolutions.Add(r);
        }
        ResolutionIndex = PlayerPrefs.GetInt("set_res", -1);
        if (ResolutionIndex < 0 || ResolutionIndex >= Resolutions.Count)
            ResolutionIndex = CurrentResolutionIndex();
    }

    void Start() { ApplyAll(); }

    int CurrentResolutionIndex()
    {
        for (int i = 0; i < Resolutions.Count; i++)
            if (Resolutions[i].width == Screen.width && Resolutions[i].height == Screen.height) return i;
        return Mathf.Max(0, Resolutions.Count - 1);
    }

    public void ApplyAll()
    {
        AudioListener.volume = Master;
        if (MusicManager.Instance != null) MusicManager.Instance.SetVolume(Music);
        if (SoundManager.Instance != null) SoundManager.Instance.sfxVolume = SFX;
        Screen.fullScreen = Fullscreen;
    }

    public void SetMaster(float v)
    {
        Master = v; AudioListener.volume = v; PlayerPrefs.SetFloat("set_master", v);
    }
    public void SetMusic(float v)
    {
        Music = v; if (MusicManager.Instance != null) MusicManager.Instance.SetVolume(v);
        PlayerPrefs.SetFloat("set_music", v);
    }
    public void SetSFX(float v)
    {
        SFX = v; if (SoundManager.Instance != null) SoundManager.Instance.sfxVolume = v;
        PlayerPrefs.SetFloat("set_sfx", v);
    }
    public void SetFullscreen(bool f)
    {
        Fullscreen = f; Screen.fullScreen = f; PlayerPrefs.SetInt("set_fullscreen", f ? 1 : 0);
    }
    public void SetResolution(int index)
    {
        if (index < 0 || index >= Resolutions.Count) return;
        ResolutionIndex = index;
        Resolution r = Resolutions[index];
        Screen.SetResolution(r.width, r.height, Fullscreen);
        PlayerPrefs.SetInt("set_res", index);
    }
}
