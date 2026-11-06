using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Shows the "EVOLVED" moment when a synergy unlocks: a framed gold-on-navy banner
// (scale-in, hold, fade), a chime, and a gold ring burst on the player. Self-contained —
// builds its own overlay UI in code; UpgradeManager just calls Announce(name).
// Everything animates on UNSCALED time because evolutions fire while the level-up is paused.
public class EvolutionAnnouncer : MonoBehaviour
{
    public static EvolutionAnnouncer Instance { get; private set; }

    [Header("Assets")]
    public Sprite pillSprite;    // navy+gold panel (e.g. shop_panel)
    public TMP_FontAsset font;   // Cinzel SDF
    public AudioClip chime;      // level_up_chime
    public Sprite ringSprite;    // evolve_ring burst

    [Header("Banner layout  (edit here, then re-enter Play to apply)")]
    public Vector2 bannerSize = new Vector2(700f, 150f);
    public float bannerTopOffset = 120f;   // distance below the top edge
    public float labelSize = 34f;
    public float nameMaxSize = 62f;

    [Header("Timing (unscaled seconds)")]
    public float inTime = 0.18f;
    public float hold = 1.8f;
    public float outTime = 0.45f;

    [Header("Player ring burst")]
    public float burstStartSize = 120f;    // UI pixels at the start
    public float burstEndScale = 3.5f;     // how far it expands
    public float burstDuration = 0.55f;
    public Color burstColor = new Color(1f, 0.86f, 0.35f, 1f);

    [Header("Colors")]
    public Color goldLabel = new Color(1f, 0.886f, 0.541f); // #FFE28A
    public Color nameColor = new Color(1f, 0.925f, 0.745f); // #FFECBE

    Canvas bannerCanvas;
    RectTransform banner;
    CanvasGroup group;
    TextMeshProUGUI nameText;
    AudioSource audioSrc;
    readonly Queue<string> queue = new Queue<string>();
    bool running;

    void Awake()
    {
        Instance = this;
        audioSrc = gameObject.AddComponent<AudioSource>();
        audioSrc.playOnAwake = false;
        audioSrc.spatialBlend = 0f;
        BuildUI();
    }

    void OnDestroy() { if (Instance == this) Instance = null; }

    // ----- TEMP DEBUG: press P to preview each banner. Delete this whole block when done. -----
    static readonly string[] debugNames = { "Comet", "Supernova", "Meteor Storm", "Thunderstorm", "Chain Lightning", "Titan's Wrath" };
    int debugIdx;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            Announce(debugNames[debugIdx++ % debugNames.Length]);
    }
    // ----- end TEMP DEBUG -----

    void BuildUI()
    {
        var canvasGO = new GameObject("EvolutionBannerCanvas");
        canvasGO.transform.SetParent(transform, false);
        bannerCanvas = canvasGO.AddComponent<Canvas>();
        bannerCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        bannerCanvas.sortingOrder = 1000;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        var rootGO = new GameObject("Banner");
        rootGO.transform.SetParent(canvasGO.transform, false);
        banner = rootGO.AddComponent<RectTransform>();
        banner.anchorMin = banner.anchorMax = new Vector2(0.5f, 1f); // top-center: never clipped off the top
        banner.pivot = new Vector2(0.5f, 0.5f);
        banner.anchoredPosition = new Vector2(0f, -bannerTopOffset);
        banner.sizeDelta = bannerSize;
        group = rootGO.AddComponent<CanvasGroup>();
        group.alpha = 0f;

        var pillGO = new GameObject("Pill");
        pillGO.transform.SetParent(rootGO.transform, false);
        var pillRT = pillGO.AddComponent<RectTransform>();
        Stretch(pillRT);
        var pill = pillGO.AddComponent<Image>();
        pill.sprite = pillSprite;
        pill.type = Image.Type.Sliced;
        pill.raycastTarget = false;

        var label = MakeText("Label", rootGO.transform, "⚡ EVOLVED", labelSize, goldLabel);
        var lrt = label.rectTransform;
        lrt.anchorMin = new Vector2(0f, 1f); lrt.anchorMax = new Vector2(1f, 1f); lrt.pivot = new Vector2(0.5f, 1f);
        lrt.offsetMin = new Vector2(10f, -58f); lrt.offsetMax = new Vector2(-10f, -14f);

        nameText = MakeText("Name", rootGO.transform, "", nameMaxSize, nameColor);
        nameText.enableAutoSizing = true;   // long names (e.g. "Chain Lightning") shrink to fit the pill
        nameText.fontSizeMin = 28f;
        nameText.fontSizeMax = nameMaxSize;
        var nrt = nameText.rectTransform;
        nrt.anchorMin = new Vector2(0f, 0f); nrt.anchorMax = new Vector2(1f, 1f);
        nrt.offsetMin = new Vector2(24f, 14f); nrt.offsetMax = new Vector2(-24f, -52f);

        rootGO.SetActive(false);
    }

    TextMeshProUGUI MakeText(string n, Transform parent, string s, float size, Color c)
    {
        var go = new GameObject(n);
        go.transform.SetParent(parent, false);
        var t = go.AddComponent<TextMeshProUGUI>();
        if (font != null) t.font = font;
        t.text = s;
        t.fontSize = size;
        t.color = c;
        t.alignment = TextAlignmentOptions.Center;
        t.raycastTarget = false;
        return t;
    }

    static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
    }

    public void Announce(string evolutionName)
    {
        queue.Enqueue(evolutionName);
        if (!running) StartCoroutine(RunQueue());
    }

    IEnumerator RunQueue()
    {
        running = true;
        while (queue.Count > 0)
        {
            yield return ShowOne(queue.Dequeue());
            yield return WaitUnscaled(0.15f);
        }
        running = false;
    }

    IEnumerator ShowOne(string name)
    {
        nameText.text = name;
        banner.gameObject.SetActive(true);
        if (chime != null) audioSrc.PlayOneShot(chime);
        SpawnBurst();
        Shake();

        float t = 0f;
        while (t < inTime)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / inTime);
            banner.localScale = Vector3.one * Mathf.Lerp(0.8f, 1.05f, k);
            group.alpha = k;
            yield return null;
        }
        t = 0f; const float settle = 0.08f;
        while (t < settle)
        {
            t += Time.unscaledDeltaTime;
            banner.localScale = Vector3.one * Mathf.Lerp(1.05f, 1f, t / settle);
            yield return null;
        }
        banner.localScale = Vector3.one;
        group.alpha = 1f;

        yield return WaitUnscaled(hold);

        t = 0f;
        while (t < outTime)
        {
            t += Time.unscaledDeltaTime;
            group.alpha = 1f - Mathf.Clamp01(t / outTime);
            yield return null;
        }
        group.alpha = 0f;
        banner.gameObject.SetActive(false);
    }

    IEnumerator WaitUnscaled(float s)
    {
        float t = 0f;
        while (t < s) { t += Time.unscaledDeltaTime; yield return null; }
    }

    // Burst is a UI ring on the top overlay canvas, placed at the player's screen position,
    // so the gameplay darkness/vision overlay can never hide it.
    void SpawnBurst()
    {
        if (ringSprite == null || bannerCanvas == null) return;
        var player = GameObject.FindWithTag("Player");
        Vector3 screenPos = (player != null && Camera.main != null)
            ? Camera.main.WorldToScreenPoint(player.transform.position)
            : new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);

        var go = new GameObject("EvolveBurst");
        go.transform.SetParent(bannerCanvas.transform, false);
        var img = go.AddComponent<Image>();
        img.sprite = ringSprite;
        img.raycastTarget = false;
        img.color = burstColor;
        img.rectTransform.sizeDelta = new Vector2(burstStartSize, burstStartSize);
        img.rectTransform.position = screenPos; // overlay canvas: world units == screen pixels
        StartCoroutine(BurstRoutine(go, img));
    }

    IEnumerator BurstRoutine(GameObject go, Image img)
    {
        float t = 0f;
        while (go != null && t < burstDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = t / burstDuration;
            go.transform.localScale = Vector3.one * Mathf.Lerp(0.5f, burstEndScale, k);
            var c = img.color; c.a = burstColor.a * (1f - k); img.color = c;
            yield return null;
        }
        if (go != null) Destroy(go);
    }

    void Shake()
    {
        var cs = FindObjectOfType<CameraShake>();
        if (cs != null) cs.StartCoroutine(cs.Shake(0.15f, 0.12f));
    }
}
