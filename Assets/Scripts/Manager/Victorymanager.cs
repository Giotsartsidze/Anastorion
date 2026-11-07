using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Victory is the demo's climax: it fires when the BOSS is defeated (not on a timer).
/// The results panel is built in code to match the shared gold-on-navy UI
/// (title, subtitle, run stats, Play Again / Main Menu).
/// </summary>
public class VictoryManager : MonoBehaviour
{
    public static VictoryManager Instance;

    [Header("UI")]
    public GameObject panel;

    [Tooltip("Seconds (realtime) to let the boss-death effects play before the panel appears.")]
    public float victoryDelay = 1.2f;

    static readonly Color Gold      = new Color(0.93f, 0.78f, 0.42f);
    static readonly Color GoldBright= new Color(0.96f, 0.86f, 0.52f);
    static readonly Color Slate     = new Color(0.55f, 0.60f, 0.72f);
    static readonly Color Ink       = new Color(0.16f, 0.10f, 0.02f);

    TMP_FontAsset uiFont, pixelFont;
    TMP_Text timeText, killsText, levelText;
    bool victoryShown;
    bool built;

    void Awake()
    {
        Instance = this; // newest wins — survives scene reload without vanishing
        if (panel != null)
        {
            BuildUI();
            UiSkin.Frame(panel, new Vector2(620, 440)); // shared gold frame
            panel.SetActive(false);
        }
    }

    // Called by BossHealth when the boss dies — THIS is the win condition.
    public void OnBossDefeated()
    {
        if (victoryShown) return;
        StartCoroutine(VictoryAfterDelay());
    }

    IEnumerator VictoryAfterDelay()
    {
        yield return new WaitForSecondsRealtime(victoryDelay);
        ShowVictory();
    }

    public void ShowVictory()
    {
        if (victoryShown) return;
        victoryShown = true;

        Time.timeScale = 0f;
        if (panel != null) panel.SetActive(true);
        MenuGuard.AnyOpen = true;

        if (RunStats.Instance != null)
        {
            if (timeText  != null) timeText.text  = Stat("Time Survived",   RunStats.Instance.GetFormattedTime());
            if (killsText != null) killsText.text = Stat("Enemies Slain",    RunStats.Instance.killCount.ToString());
            if (levelText != null) levelText.text = Stat("Level Reached",    RunStats.Instance.GetLevel().ToString());
        }
    }

    static string Stat(string label, string value) =>
        $"<color=#8C93A6>{label}</color>    <color=#F2D692>{value}</color>";

    // ---- UI construction -------------------------------------------------

    void BuildUI()
    {
        if (built) return; built = true;
        uiFont    = FindFont("cinzel");
        pixelFont = FindFont("pixelify");
        var panelRT = (RectTransform)panel.transform;

        // dim backdrop so the card pops over the frozen battlefield. Oversized +
        // center-anchored so it covers the whole screen even if the panel rect is small.
        var scrim = NewImage("scrim", panelRT);
        scrim.color = new Color(0f, 0f, 0f, 0.72f);
        var srt = scrim.rectTransform;
        srt.anchorMin = srt.anchorMax = new Vector2(0.5f, 0.5f); srt.pivot = new Vector2(0.5f, 0.5f);
        srt.sizeDelta = new Vector2(4000, 4000); srt.anchoredPosition = Vector2.zero;

        MakeText("VICTORY", new Vector2(0, 150), new Vector2(560, 60), 46, GoldBright, uiFont, 8f, FontStyles.Normal);
        MakeText("· the dream endures ·", new Vector2(0, 110), new Vector2(560, 28), 18, Slate, uiFont, 6f, FontStyles.Normal);

        // thin gold divider
        var rule = NewImage("Rule", panelRT);
        rule.color = new Color(Gold.r, Gold.g, Gold.b, 0.45f);
        var rrt = rule.rectTransform;
        rrt.anchorMin = rrt.anchorMax = new Vector2(0.5f, 0.5f); rrt.pivot = new Vector2(0.5f, 0.5f);
        rrt.sizeDelta = new Vector2(360, 2); rrt.anchoredPosition = new Vector2(0, 84);

        timeText  = MakeText("", new Vector2(0,  44), new Vector2(520, 34), 24, Color.white, pixelFont, 0f, FontStyles.Normal);
        killsText = MakeText("", new Vector2(0,   2), new Vector2(520, 34), 24, Color.white, pixelFont, 0f, FontStyles.Normal);
        levelText = MakeText("", new Vector2(0, -40), new Vector2(520, 34), 24, Color.white, pixelFont, 0f, FontStyles.Normal);

        MakeButton("PLAY AGAIN", new Vector2(-108, -148), new Vector2(196, 56), Restart);
        MakeButton("MAIN MENU",  new Vector2( 108, -148), new Vector2(196, 56), ToMenu);
    }

    TMP_Text MakeText(string text, Vector2 pos, Vector2 size, float fs, Color c, TMP_FontAsset font, float spacing, FontStyles style)
    {
        var go = new GameObject("Text", typeof(RectTransform));
        go.transform.SetParent(panel.transform, false);
        var rt = (RectTransform)go.transform;
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f); rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos; rt.sizeDelta = size;
        var t = go.AddComponent<TextMeshProUGUI>();
        if (font != null) t.font = font;
        t.text = text; t.fontSize = fs; t.color = c; t.characterSpacing = spacing; t.fontStyle = style;
        t.alignment = TextAlignmentOptions.Center; t.richText = true;
        t.enableWordWrapping = false; t.raycastTarget = false;
        return t;
    }

    Button MakeButton(string label, Vector2 pos, Vector2 size, UnityEngine.Events.UnityAction action)
    {
        var go = new GameObject("Button_" + label, typeof(RectTransform));
        go.transform.SetParent(panel.transform, false);
        var rt = (RectTransform)go.transform;
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f); rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos; rt.sizeDelta = size;
        var img = go.AddComponent<Image>(); img.sprite = UiSkin.ButtonSprite(); img.type = Image.Type.Sliced; img.color = Color.white;
        var b = go.AddComponent<Button>();
        var colors = b.colors; colors.highlightedColor = new Color(1.08f, 1.08f, 1.08f, 1f); b.colors = colors;
        b.onClick.AddListener(action);
        var t = new GameObject("Label", typeof(RectTransform)).AddComponent<TextMeshProUGUI>();
        t.transform.SetParent(go.transform, false);
        var lrt = t.rectTransform; lrt.anchorMin = Vector2.zero; lrt.anchorMax = Vector2.one; lrt.offsetMin = lrt.offsetMax = Vector2.zero;
        if (uiFont != null) t.font = uiFont;
        t.text = label; t.fontSize = 20; t.color = Ink; t.fontStyle = FontStyles.Bold; t.characterSpacing = 3f;
        t.alignment = TextAlignmentOptions.Center; t.enableWordWrapping = false; t.raycastTarget = false;
        return b;
    }

    static Image NewImage(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go.AddComponent<Image>();
    }

    static TMP_FontAsset FindFont(string contains)
    {
        foreach (var f in Resources.FindObjectsOfTypeAll<TMP_FontAsset>())
            if (f != null && f.name.ToLower().Contains(contains)) return f;
        return null;
    }

    // ---- buttons ---------------------------------------------------------

    public void Restart()
    {
        Time.timeScale = 1f;
        MenuGuard.AnyOpen = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ToMenu()
    {
        Time.timeScale = 1f;
        MenuGuard.AnyOpen = false;
        SceneManager.LoadScene("MainMenu");
    }
}
