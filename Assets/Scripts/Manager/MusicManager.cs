using System.Collections;
using UnityEngine;

/// <summary>
/// Background music with crossfades. Survives scene loads (DontDestroyOnLoad) so the
/// menu→gameplay transition doesn't restart the track. Put ONE in the MainMenu scene
/// (and one in the gameplay scene for direct testing — the singleton drops duplicates).
/// Assign Default Music (bg) + Boss Music in the Inspector.
///   MusicManager.Instance?.PlayBoss();  / PlayDefault();
/// </summary>
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    public AudioClip defaultMusic;
    public AudioClip bossMusic;
    [Range(0f, 1f)] public float volume = 0.5f;
    public float fadeDuration = 1.5f;

    private AudioSource a, b, active;
    private Coroutine fadeRoutine;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        a = gameObject.AddComponent<AudioSource>();
        b = gameObject.AddComponent<AudioSource>();
        foreach (var s in new[] { a, b }) { s.loop = true; s.playOnAwake = false; s.volume = 0f; }

        active = a;
        if (defaultMusic != null)
        {
            active.clip = defaultMusic;
            active.volume = volume;
            active.Play();
        }
    }

    public void PlayDefault() => CrossfadeTo(defaultMusic);
    public void PlayBoss() => CrossfadeTo(bossMusic);

    /// <summary>Set music volume live (called by SettingsManager).</summary>
    public void SetVolume(float v)
    {
        volume = Mathf.Clamp01(v);
        if (active != null) active.volume = volume;
    }

    public void CrossfadeTo(AudioClip clip)
    {
        if (clip == null || (active != null && active.clip == clip && active.isPlaying)) return;
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(Crossfade(clip));
    }

    private IEnumerator Crossfade(AudioClip clip)
    {
        AudioSource from = active;
        AudioSource to = (active == a) ? b : a;
        to.clip = clip; to.volume = 0f; to.Play();

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime; // unscaled so fades work while paused
            float k = t / fadeDuration;
            to.volume = Mathf.Lerp(0f, volume, k);
            from.volume = Mathf.Lerp(volume, 0f, k);
            yield return null;
        }
        to.volume = volume;
        from.Stop();
        active = to;
    }
}
