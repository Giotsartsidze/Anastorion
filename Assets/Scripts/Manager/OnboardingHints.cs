using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// First-run new-player hints: shows a short sequence of fading tips at the start
/// of a run (controls + goal), then gets out of the way. Put on a GameObject and
/// assign a TMP text (centered, upper area of the screen).
/// </summary>
public class OnboardingHints : MonoBehaviour
{
    public TMP_Text hintText;

    [TextArea]
    public string[] messages = new[]
    {
        "Move with WASD / Arrow Keys",
        "Press SPACE to Dash away from danger",
        "Survive the horde of the dream...",
        "Collect XP to Level Up — then pick a powerful upgrade",
    };

    public float secondsPerMessage = 4f;
    public float fadeTime = 0.5f;

    [Tooltip("Show only the first time on this machine. Off = show every run (good for Next Fest kiosks).")]
    public bool onlyFirstTime = false;

    private bool sequenceActive;

    void Start()
    {
        if (hintText == null) return;

        if (onlyFirstTime && PlayerPrefs.GetInt("seen_tutorial", 0) == 1)
        {
            hintText.gameObject.SetActive(false);
            return;
        }
        PlayerPrefs.SetInt("seen_tutorial", 1);
        sequenceActive = true;
        StartCoroutine(Run());
    }

    void Update()
    {
        // Never overlap a paused menu — pause/settings set timeScale to 0.
        if (hintText != null && sequenceActive)
            hintText.enabled = Time.timeScale > 0f;
    }

    IEnumerator Run()
    {
        foreach (string msg in messages)
        {
            hintText.text = msg;
            yield return Fade(0f, 1f);
            yield return new WaitForSecondsRealtime(secondsPerMessage);
            yield return Fade(1f, 0f);
        }
        sequenceActive = false;
        hintText.gameObject.SetActive(false);
    }

    IEnumerator Fade(float from, float to)
    {
        float t = 0f;
        Color c = hintText.color;
        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime; // realtime, so it finishes even if paused
            c.a = Mathf.Lerp(from, to, t / fadeTime);
            hintText.color = c;
            yield return null;
        }
        c.a = to;
        hintText.color = c;
    }
}
