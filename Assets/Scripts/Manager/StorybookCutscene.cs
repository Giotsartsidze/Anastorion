using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[Serializable]
public class CutscenePage
{
    public Sprite illustration;
    [TextArea] public string narration;
    public float duration = 4f;
}

// Reusable storybook cutscene: an open book (left page illustration, right page narration)
// over a dark backdrop. Auto-advances each page after its duration, cross-fading between
// them, and is skippable (Esc / Space / click) at any time. Builds its own overlay UI in
// code. Call Play(onComplete). Used for both the intro and the death/wake outro.
public class StorybookCutscene : MonoBehaviour
{
    [Header("Content")]
    public CutscenePage[] pages;

    [Header("Assets")]
    public TMP_FontAsset font;
    public Sprite pageSprite;   // parchment page background (9-sliced)

    [Header("Feel")]
    public float fadeTime = 0.6f;
    public bool skippable = true;
    public Color backdropColor = new Color(0.02f, 0.02f, 0.05f, 0.96f);
    public Color inkColor = new Color(0.227f, 0.173f, 0.078f); // #3A2C14 on parchment

    Canvas canvas;
    CanvasGroup rootGroup;      // whole cutscene fade
    CanvasGroup contentGroup;   // per-page cross-fade (illustration + narration)
    Image illImage;
    TextMeshProUGUI narrationText;
    readonly System.Collections.Generic.Dictionary<string, string> tokens = new System.Collections.Generic.Dictionary<string, string>();
    Action onComplete;
    bool skipRequested;
    bool playing;

    // Replace a placeholder (e.g. "{shards}") in page narration at runtime. Set before Play().
    public void SetToken(string key, string value) { tokens[key] = value; }

    void Awake()
    {
        BuildUI();
        canvas.gameObject.SetActive(false);
    }

    public void Play(Action onDone)
    {
        if (playing) return;
        onComplete = onDone;
        skipRequested = false;
        canvas.gameObject.SetActive(true);
        StartCoroutine(Run());
    }

    void Update()
    {
        if (playing && skippable &&
            (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
            skipRequested = true;
    }

    IEnumerator Run()
    {
        playing = true;
        rootGroup.alpha = 0f;
        contentGroup.alpha = 0f;
        yield return Fade(rootGroup, 0f, 1f, fadeTime);

        foreach (var p in pages)
        {
            if (skipRequested) break;
            illImage.sprite = p.illustration;
            illImage.enabled = p.illustration != null;
            string txt = p.narration;
            foreach (var kv in tokens) txt = txt.Replace(kv.Key, kv.Value);
            narrationText.text = txt;
            yield return Fade(contentGroup, 0f, 1f, fadeTime * 0.6f);

            float t = 0f;
            while (t < p.duration && !skipRequested)
            {
                t += Time.unscaledDeltaTime;
                yield return null;
            }
            yield return Fade(contentGroup, 1f, 0f, fadeTime * 0.5f);
        }

        // Do NOT fade the cutscene out here — fading rootGroup to 0 would reveal the menu
        // behind it for a moment before the next scene loads (the "menu flashes, then the
        // game starts" bug). onComplete loads a scene, which replaces the menu while this
        // opaque cutscene is still covering the screen.
        playing = false;
        onComplete?.Invoke();
    }

    IEnumerator Fade(CanvasGroup g, float from, float to, float time)
    {
        float t = 0f;
        while (t < time)
        {
            t += Time.unscaledDeltaTime;
            g.alpha = Mathf.Lerp(from, to, t / time);
            yield return null;
        }
        g.alpha = to;
    }

    // ---------------- UI construction ----------------
    void BuildUI()
    {
        var canvasGO = new GameObject("StorybookCanvas");
        canvasGO.transform.SetParent(transform, false);
        canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 2000;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();

        rootGroup = canvasGO.AddComponent<CanvasGroup>();

        // full-screen backdrop (blocks the menu behind)
        var bg = NewImage("Backdrop", canvasGO.transform);
        Stretch(bg.rectTransform);
        // force opaque: during page cross-fades the illustration drops out, and a
        // translucent backdrop would briefly reveal the menu behind it.
        bg.color = new Color(backdropColor.r, backdropColor.g, backdropColor.b, 1f);

        // per-page content (illustration + text fade together)
        var content = NewRect("Content", canvasGO.transform);
        Stretch(content);
        contentGroup = content.gameObject.AddComponent<CanvasGroup>();

        // full-screen illustration
        illImage = NewImage("Illustration", content);
        Stretch(illImage.rectTransform);
        illImage.preserveAspect = false; // fill the whole screen

        // dark scrim along the bottom so narration always reads over the art
        var scrim = NewImage("Scrim", content);
        SetRect(scrim.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.5f, 0f),
                new Vector2(0f, 0f), new Vector2(0f, 340f));
        scrim.color = new Color(0f, 0f, 0f, 0.66f);

        // narration in the bottom bar
        narrationText = NewText("Narration", content, 50f, new Color(1f, 0.925f, 0.745f));
        narrationText.enableWordWrapping = true;
        narrationText.alignment = TextAlignmentOptions.Center;
        SetRect(narrationText.rectTransform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f),
                new Vector2(0f, 60f), new Vector2(1600f, 240f));

        // skip hint
        var hint = NewText("SkipHint", canvasGO.transform, 28f, new Color(1f, 0.886f, 0.541f, 0.8f));
        hint.text = "Esc · Space · click  —  skip";
        hint.alignment = TextAlignmentOptions.BottomRight;
        SetRect(hint.rectTransform, new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(1f, 0f),
                new Vector2(-40f, 36f), new Vector2(560f, 40f));
    }

    RectTransform NewRect(string n, Transform parent)
    {
        var go = new GameObject(n); go.transform.SetParent(parent, false);
        return go.AddComponent<RectTransform>();
    }
    Image NewImage(string n, Transform parent)
    {
        var rt = NewRect(n, parent);
        var img = rt.gameObject.AddComponent<Image>();
        img.raycastTarget = false;
        return img;
    }
    TextMeshProUGUI NewText(string n, Transform parent, float size, Color c)
    {
        var rt = NewRect(n, parent);
        var t = rt.gameObject.AddComponent<TextMeshProUGUI>();
        if (font != null) t.font = font;
        t.fontSize = size; t.color = c; t.raycastTarget = false;
        t.alignment = TextAlignmentOptions.Center;
        return t;
    }
    static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
    }
    static void SetRect(RectTransform rt, Vector2 aMin, Vector2 aMax, Vector2 piv, Vector2 pos, Vector2 size)
    {
        rt.anchorMin = aMin; rt.anchorMax = aMax; rt.pivot = piv;
        rt.anchoredPosition = pos; rt.sizeDelta = size;
    }
}
