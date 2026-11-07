using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class UpgradeManager : MonoBehaviour
{
    public List<UpgradeData> allUpgrades;
    public UpgradeUIElement[] cardUI;
    public GameObject levelUpPanel;
    public List<UpgradeData> activeUpgrades;
    public List<UpgradeData> lockedUpgrades;

    [Header("Level-up panel chrome")]
    public TMP_FontAsset uiFont;     // Cinzel
    public TMP_FontAsset pixelFont;  // Pixelify
    // Rerolls are NOT free — they come from the "Reroll" meta-shop upgrade.
    // Owned level (PlayerPrefs "Upgrade_Reroll") = rerolls granted each level-up.
    const string RerollPrefKey = "Upgrade_Reroll";
    int rerollsOwned;   // how many the player bought (per level-up)
    int rerollsLeft;    // remaining this level-up
    bool chromeBuilt;
    TMP_Text rerollLabel, levelLabel;
    Button rerollButton, skipButton;

    [Header("Supernova Synergy")]
    public GameObject supernovaPrefab;
    public float supernovaCooldown = 5f;
    private float supernovaTimer;
    private bool isSupernovaUnlocked = false;

	[Header("Chain Lightning Synergy")]
	public ChainLightning chainLightningScript; // ჩააგდე სკრიპტი აქ
	private bool isLightningUnlocked = false;
	private int dashLevel = 0;
	private int speedLevel = 0;

    [Header("Debug — show all evolutions (for testing / trailer)")]
    [Tooltip("ON = Supernova + Chain Lightning active from the start. Turn OFF for real play.")]
    public bool debugUnlockSynergies = false;

    // დონეების მთვლელები
    private int wispLevel = 0;
    private int radiusLevel = 0;

    // New-skill pick counts + evolution flags
    private int fireTrailLevel = 0;
    private int starfallLevel = 0;
    private int smiteLevel = 0;
    private int chainLashLevel = 0;
    private bool isCometUnlocked = false;
    private bool isMeteorStormUnlocked = false;
    private bool isThunderstormUnlocked = false;

    [Header("Rarity gating by level")]
    [Tooltip("Rare cards start appearing at this level; Legendary at the next. Below, only Common.")]
    public int rareUnlockLevel = 4;
    public int legendaryUnlockLevel = 8;
    public float commonWeight = 100f;
    public float rareWeight = 30f;
    public float legendaryWeight = 8f;

    // Cached component references (no per-upgrade FindObjectOfType anymore)
    private PlayerMovement playerMovement;
    private PlayerDash playerDash;
    private LightPulse lightPulse;
    private FireTrail fireTrail;
    private Starfall starfall;
    private HeavenSmite heavenSmite;
    private ChainLash chainLash;
    private OrbitingWisps wisps;
    private StellarDarts stellarDarts;
    private ExperienceManager xpManager;

    void Start()
    {
        MenuGuard.AnyOpen = false; // fresh run — nothing open (clears any stale guard)
        SimplePool.Reset();        // clear pooled projectiles/VFX from the previous run
        playerMovement = FindObjectOfType<PlayerMovement>();
        playerDash = FindObjectOfType<PlayerDash>();
        lightPulse = FindObjectOfType<LightPulse>();
        fireTrail = FindObjectOfType<FireTrail>();
        starfall = FindObjectOfType<Starfall>();
        heavenSmite = FindObjectOfType<HeavenSmite>();
        chainLash = FindObjectOfType<ChainLash>();
        wisps = FindObjectOfType<OrbitingWisps>();
        stellarDarts = FindObjectOfType<StellarDarts>();
        xpManager = FindObjectOfType<ExperienceManager>();

        if (debugUnlockSynergies)
        {
            isSupernovaUnlocked = true;
            isLightningUnlocked = true;
        }
    }

    public void UnlockWispUpgrades()
    {
        foreach (var upgrade in lockedUpgrades)
        {
            if (!activeUpgrades.Contains(upgrade)) activeUpgrades.Add(upgrade);
        }
    }

    public void ShowUpgrades()
    {
        levelUpPanel.SetActive(true);
        Time.timeScale = 0f;
        MenuGuard.AnyOpen = true;
        EnsureChrome();
        rerollsOwned = PlayerPrefs.GetInt(RerollPrefKey, 0);
        rerollsLeft = rerollsOwned;
        RollUpgrades();
        UpdateChrome();
    }

    // Picks + fills the three cards (rarity-gated, rarity-weighted). Called on open and reroll.
    void RollUpgrades()
    {
        int level = xpManager != null ? xpManager.currentLevel : 1;
        int maxRarity = (level >= legendaryUnlockLevel) ? 2 : (level >= rareUnlockLevel ? 1 : 0);

        var pool = activeUpgrades.Where(IsEligible).Where(u => (int)u.rarity <= maxRarity).ToList();
        if (pool.Count == 0) pool = activeUpgrades.Where(IsEligible).ToList(); // never show an empty screen

        var picks = WeightedPick(pool, 3); // rarity-weighted so Common shows more than Legendary

        for (int i = 0; i < cardUI.Length; i++)
        {
            if (cardUI[i] == null) continue;
            if (i < picks.Count)
            {
                cardUI[i].gameObject.SetActive(true);
                cardUI[i].Setup(picks[i], this);
                cardUI[i].AnimateIn(i * 0.12f);
            }
            else cardUI[i].gameObject.SetActive(false);
        }
    }

    public void Reroll()
    {
        if (rerollsLeft <= 0) return;
        rerollsLeft--;
        RollUpgrades();
        UpdateChrome();
    }

    public void Skip() { ResumeGame(); }

    void UpdateChrome()
    {
        // No reroll owned → hide the button entirely and center Skip.
        bool owns = rerollsOwned > 0;
        if (rerollButton != null) rerollButton.gameObject.SetActive(owns);
        if (skipButton != null)
            ((RectTransform)skipButton.transform).anchoredPosition = owns ? new Vector2(108, -196) : new Vector2(0, -196);

        if (owns && rerollLabel != null) rerollLabel.text = rerollsLeft > 0 ? "REROLL (" + rerollsLeft + ")" : "NO REROLLS";
        if (rerollButton != null) rerollButton.interactable = rerollsLeft > 0;
        if (levelLabel != null && xpManager != null) levelLabel.text = "LEVEL " + xpManager.currentLevel;
    }

    // Builds the title / subtitle / level label / reroll+skip buttons once, centers the
    // three cards, and drops the shared gold frame behind everything.
    void EnsureChrome()
    {
        if (chromeBuilt || levelUpPanel == null) return;
        chromeBuilt = true;

        // This panel ships with a HorizontalLayoutGroup that force-lays-out every child
        // (squeezing cards + our chrome into thin columns). Turn off any layout driver so
        // our explicit anchoredPosition / sizeDelta actually take effect.
        foreach (var lg in levelUpPanel.GetComponents<LayoutGroup>()) lg.enabled = false;
        var csf = levelUpPanel.GetComponent<ContentSizeFitter>(); if (csf != null) csf.enabled = false;

        if (uiFont == null) uiFont = FindFont("cinzel");
        if (pixelFont == null) pixelFont = FindFont("pixelify");
        var panel = (RectTransform)levelUpPanel.transform;

        // Scalable content wrapper: lets us size the whole composition (title +
        // cards + buttons + gold frame) as ONE unit with margin, while the panel's
        // own navy fill stays behind it as the dim backdrop.
        var wrap = new GameObject("LevelUpContent", typeof(RectTransform)).GetComponent<RectTransform>();
        wrap.SetParent(panel, false);
        wrap.anchorMin = Vector2.zero; wrap.anchorMax = Vector2.one;
        wrap.offsetMin = wrap.offsetMax = Vector2.zero;
        wrap.pivot = new Vector2(0.5f, 0.5f);
        wrap.localScale = Vector3.one * 0.82f; // shrink the whole panel for breathing room
        Transform host = wrap;

        Color gold = new Color(0.93f, 0.78f, 0.42f);
        Color slate = new Color(0.52f, 0.57f, 0.68f);

        float[] xs = { -236f, 0f, 236f };
        for (int i = 0; i < cardUI.Length && i < 3; i++)
        {
            if (cardUI[i] == null) continue;
            var rt = (RectTransform)cardUI[i].transform;
            rt.SetParent(host, false);
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f); rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(206, 290);
            rt.anchoredPosition = new Vector2(xs[i], -20f);
        }

        // Narrow text rects so they don't inflate the auto-fit frame width.
        MakeText(host, "LEVEL UP", new Vector2(0, 196), new Vector2(560, 46), 34, gold, true, 7f, TextAlignmentOptions.Center);
        MakeText(host, "· choose your boon ·", new Vector2(0, 168), new Vector2(560, 24), 15, slate, false, 6f, TextAlignmentOptions.Center);
        levelLabel = MakeText(host, "LEVEL 1", new Vector2(0, 148), new Vector2(560, 20), 12, gold, false, 4f, TextAlignmentOptions.Center);

        rerollButton = MakeButton(host, "REROLL", new Vector2(-108, -196), new Vector2(188, 50), Reroll, out rerollLabel);
        skipButton = MakeButton(host, "SKIP", new Vector2(108, -196), new Vector2(188, 50), Skip, out _);

        UiSkin.Frame(wrap.gameObject, Vector2.zero); // shared gold frame, auto-fit to content
    }

    static TMP_FontAsset FindFont(string contains)
    {
        foreach (var f in Resources.FindObjectsOfTypeAll<TMP_FontAsset>())
            if (f != null && f.name.ToLowerInvariant().Contains(contains)) return f;
        return null;
    }

    TMP_Text MakeText(Transform parent, string text, Vector2 pos, Vector2 size, float fs, Color c, bool bold, float spacing, TextAlignmentOptions align)
    {
        var go = new GameObject("Chrome_" + text, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var rt = (RectTransform)go.transform;
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f); rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos; rt.sizeDelta = size;
        var t = go.AddComponent<TextMeshProUGUI>();
        if (uiFont != null) t.font = uiFont;
        t.text = text; t.fontSize = fs; t.color = c; t.alignment = align; t.characterSpacing = spacing;
        if (bold) t.fontStyle = FontStyles.Bold;
        t.enableWordWrapping = false; t.raycastTarget = false;
        return t;
    }

    Button MakeButton(Transform parent, string label, Vector2 pos, Vector2 size, UnityEngine.Events.UnityAction action, out TMP_Text labelOut)
    {
        var go = new GameObject("ChromeBtn_" + label, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var rt = (RectTransform)go.transform;
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f); rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos; rt.sizeDelta = size;
        var img = go.AddComponent<Image>(); img.sprite = UiSkin.ButtonSprite(); img.type = Image.Type.Sliced; img.color = Color.white;
        var b = go.AddComponent<Button>(); b.targetGraphic = img;
        var colors = b.colors; colors.highlightedColor = new Color(1.08f, 1.08f, 1.08f, 1f);
        colors.disabledColor = new Color(0.55f, 0.52f, 0.45f, 1f); b.colors = colors;
        b.onClick.AddListener(action);
        var t = new GameObject("Label", typeof(RectTransform)).AddComponent<TextMeshProUGUI>();
        t.transform.SetParent(go.transform, false);
        var lrt = t.rectTransform; lrt.anchorMin = Vector2.zero; lrt.anchorMax = Vector2.one; lrt.offsetMin = lrt.offsetMax = Vector2.zero;
        if (uiFont != null) t.font = uiFont;
        t.text = label; t.fontSize = 19; t.color = new Color(0.16f, 0.10f, 0.02f);
        t.alignment = TextAlignmentOptions.Center; t.fontStyle = FontStyles.Bold; t.characterSpacing = 3f;
        t.enableWordWrapping = false; t.raycastTarget = false;
        labelOut = t;
        return b;
    }

    // Pick `count` distinct upgrades, weighted by rarity (Common most likely).
    List<UpgradeData> WeightedPick(List<UpgradeData> source, int count)
    {
        var result = new List<UpgradeData>();
        var work = new List<UpgradeData>(source);
        for (int n = 0; n < count && work.Count > 0; n++)
        {
            float total = 0f;
            foreach (var u in work) total += RarityWeight(u.rarity);
            float r = Random.value * total;
            UpgradeData chosen = work[work.Count - 1];
            foreach (var u in work)
            {
                r -= RarityWeight(u.rarity);
                if (r <= 0f) { chosen = u; break; }
            }
            result.Add(chosen);
            work.Remove(chosen);
        }
        return result;
    }

    float RarityWeight(UpgradeData.Rarity rar)
    {
        switch (rar)
        {
            case UpgradeData.Rarity.Rare: return rareWeight;
            case UpgradeData.Rarity.Legendary: return legendaryWeight;
            default: return commonWeight;
        }
    }

    // Relic reward (from a mega-elite): same panel, but biased to the best cards.
    public void ShowRelicReward()
    {
        if (levelUpPanel == null) return;
        levelUpPanel.SetActive(true);
        Time.timeScale = 0f;
        MenuGuard.AnyOpen = true;
        EnsureChrome();
        rerollsOwned = 0; rerollsLeft = 0; // relic rewards aren't rerollable

        var picks = activeUpgrades
            .Where(IsEligible)
            .OrderByDescending(u => (int)u.rarity)
            .ThenBy(_ => Random.value)
            .Take(3).ToList();

        for (int i = 0; i < cardUI.Length; i++)
        {
            if (cardUI[i] == null) continue;
            if (i < picks.Count)
            {
                cardUI[i].gameObject.SetActive(true);
                cardUI[i].Setup(picks[i], this);
                cardUI[i].AnimateIn(i * 0.12f);
            }
            else cardUI[i].gameObject.SetActive(false);
        }
        UpdateChrome();
    }

    // True on the FIRST pick of a skill (before it's active), so the card can show
    // an "unlock" description instead of the "upgrade" one.
    public bool IsFirstPick(UpgradeData data)
    {
        switch (data.type)
        {
            case UpgradeData.UpgradeType.Pulse:       return lightPulse != null && !lightPulse.active;
            case UpgradeData.UpgradeType.FireTrail:   { var f = fireTrail;   return f != null && !f.active; }
            case UpgradeData.UpgradeType.Starfall:    { var s = starfall;     return s != null && !s.active; }
            case UpgradeData.UpgradeType.HeavenSmite: { var h = heavenSmite;  return h != null && !h.active; }
            case UpgradeData.UpgradeType.ChainLash:   { var c = chainLash;    return c != null && !c.active; }
            case UpgradeData.UpgradeType.WispCount:   return wispLevel == 0;
            default: return false;
        }
    }

    // Decides whether a card should even be offered, so you never see a one-time
    // unlock you already own, or a stat card for a skill you don't have yet.
    bool IsEligible(UpgradeData u)
    {
        switch (u.type)
        {
            case UpgradeData.UpgradeType.Pulse:          // one-time unlock: hide once learned
                return lightPulse == null || !lightPulse.active;

            case UpgradeData.UpgradeType.PulseCooldown:  // only useful once Pulse exists
            case UpgradeData.UpgradeType.LightRadius:    // only grows the Pulse — gate it too
                return lightPulse != null && lightPulse.active;

            case UpgradeData.UpgradeType.WispSpeed:      // pointless without wisps
                return wispLevel > 0;

            default:
                return true;
        }
    }

    public void ApplyUpgrade(UpgradeData data)
    {
        switch (data.type)
        {
            case UpgradeData.UpgradeType.MoveSpeed:
                if (playerMovement != null)
                {
                    playerMovement.moveSpeed += data.valueModifier;
                    playerMovement.maxSpeed += data.valueModifier; // raise the cap so it's actually felt
                }
                speedLevel++;
                break;

            case UpgradeData.UpgradeType.DashSpeed:
                if (playerDash != null) playerDash.dashSpeed += data.valueModifier;
                dashLevel++;
                break;

            case UpgradeData.UpgradeType.LightRadius:
                if (lightPulse != null) lightPulse.maxRadius += data.valueModifier;
                radiusLevel++;
                break;

            case UpgradeData.UpgradeType.WispCount:
                if (wisps != null) {
                    wisps.count++;
                    wisps.ActivateSkill();
                }
                wispLevel++;
                break;

            case UpgradeData.UpgradeType.WispSpeed:
                if (wisps != null) wisps.orbitSpeed += data.valueModifier;
                break;

            case UpgradeData.UpgradeType.Pulse:
                if (lightPulse != null)
                {
                    if (!lightPulse.active) lightPulse.active = true;            // first pick: unlock the skill
                    else lightPulse.maxRadius += Mathf.Max(0.5f, data.valueModifier); // later: bigger blast
                }
                break;

            case UpgradeData.UpgradeType.PulseCooldown:
                if (lightPulse != null) lightPulse.cooldown = Mathf.Max(0.5f, lightPulse.cooldown - data.valueModifier);
                break;

            case UpgradeData.UpgradeType.DashCooldown:
                if (playerDash != null) playerDash.dashCooldown -= data.valueModifier;
                break;

            case UpgradeData.UpgradeType.DartCount:
                var dartsC = stellarDarts;
                if (dartsC != null) dartsC.dartCount += Mathf.Max(1, Mathf.RoundToInt(data.valueModifier));
                break;

            case UpgradeData.UpgradeType.DartFireRate:
                var dartsF = stellarDarts;
                if (dartsF != null) dartsF.fireRate = Mathf.Max(0.15f, dartsF.fireRate - data.valueModifier);
                break;

            case UpgradeData.UpgradeType.FireTrail:
                var fire = fireTrail;
                if (fire != null)
                {
                    if (!fire.active) fire.active = true;                 // first pick: unlock
                    else fire.patchDamage += Mathf.Max(1, Mathf.RoundToInt(data.valueModifier)); // later: stronger
                }
                fireTrailLevel++;
                break;

            case UpgradeData.UpgradeType.Starfall:
                var star = starfall;
                if (star != null)
                {
                    if (!star.active) star.active = true;                 // first pick: unlock
                    else star.meteorCount += Mathf.Max(1, Mathf.RoundToInt(data.valueModifier)); // later: more meteors
                }
                starfallLevel++;
                break;

            case UpgradeData.UpgradeType.HeavenSmite:
                var smite = heavenSmite;
                if (smite != null)
                {
                    if (!smite.active) smite.active = true;               // first pick: unlock
                    else smite.damage += Mathf.Max(1, Mathf.RoundToInt(data.valueModifier)); // later: harder strikes
                }
                smiteLevel++;
                break;

            case UpgradeData.UpgradeType.ChainLash:
                var chain = chainLash;
                if (chain != null)
                {
                    if (!chain.active) chain.active = true;               // first pick: unlock
                    else { chain.lashCount += 1; chain.damage += Mathf.Max(1, Mathf.RoundToInt(data.valueModifier)); }
                }
                chainLashLevel++;
                if (chainLashLevel == 5)                              // exactly at the threshold, once
                {
                    chainLash?.Evolve(); // Titan's Wrath
                    EvolutionAnnouncer.Instance?.Announce("Titan's Wrath");
                }
                break;
        }

        CheckSynergies(); // ყოველი აფგრეიდის შემდეგ ვამოწმებთ სინერგიას
        ResumeGame();
    }

    // Read-only accessors so the stats panel can draw synergy progress graphically.
    public int WispLevel => wispLevel;
    public int RadiusLevel => radiusLevel;
    public int SpeedLevel => speedLevel;
    public int DashLevel => dashLevel;
    public int FireTrailLevel => fireTrailLevel;
    public int StarfallLevel => starfallLevel;
    public int SmiteLevel => smiteLevel;
    public int ChainLashLevel => chainLashLevel;
    public bool SupernovaUnlocked => isSupernovaUnlocked;
    public bool LightningUnlocked => isLightningUnlocked;
    public bool CometUnlocked => isCometUnlocked;
    public bool MeteorStormUnlocked => isMeteorStormUnlocked;
    public bool ThunderstormUnlocked => isThunderstormUnlocked;

    // Human-readable evolution recipes + progress, for the stats/synergy panel.
    public string GetSynergyReport()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine(SynLine("Supernova", "Wisps", wispLevel, 3, "Light Radius", radiusLevel, 2, isSupernovaUnlocked));
        sb.AppendLine(SynLine("Chain Lightning", "Dash Speed", dashLevel, 2, "Move Speed", speedLevel, 2, isLightningUnlocked));
        sb.AppendLine(SynLine("Comet", "Ember Path", fireTrailLevel, 3, "Move Speed", speedLevel, 3, isCometUnlocked));
        sb.AppendLine(SynLine("Meteor Storm", "Starfall", starfallLevel, 3, "Light Radius", radiusLevel, 3, isMeteorStormUnlocked));
        sb.AppendLine(SynLine("Thunderstorm", "Heaven's Smite", smiteLevel, 3, "Dash Speed", dashLevel, 3, isThunderstormUnlocked));

        bool tw = chainLashLevel >= 5;
        string mark = tw ? "<color=#ECB038>✓</color>" : "•";
        sb.AppendLine($"{mark} Titan's Wrath: Chain Lash {Mathf.Min(chainLashLevel, 5)}/5");
        return sb.ToString();
    }

    string SynLine(string name, string s1, int l1, int r1, string s2, int l2, int r2, bool done)
    {
        string mark = done ? "<color=#ECB038>✓</color>" : "•";
        return $"{mark} {name}: {s1} {Mathf.Min(l1, r1)}/{r1}, {s2} {Mathf.Min(l2, r2)}/{r2}";
    }

    void CheckSynergies()
    {
        // თუ პირობა სრულდება (მაგ: Wisp დონე 5 და Radius დონე 3)
        if (!isSupernovaUnlocked && wispLevel >= 3 && radiusLevel >= 2)
        {
            isSupernovaUnlocked = true;
            EvolutionAnnouncer.Instance?.Announce("Supernova");
        }

        // New-skill evolutions: max the skill + its paired stat.
        if (!isCometUnlocked && fireTrailLevel >= 3 && speedLevel >= 3)
        {
            isCometUnlocked = true;
            fireTrail?.Evolve();   // Comet
            EvolutionAnnouncer.Instance?.Announce("Comet");
        }
        if (!isMeteorStormUnlocked && starfallLevel >= 3 && radiusLevel >= 3)
        {
            isMeteorStormUnlocked = true;
            starfall?.Evolve();    // Meteor Storm
            EvolutionAnnouncer.Instance?.Announce("Meteor Storm");
        }
        if (!isThunderstormUnlocked && smiteLevel >= 3 && dashLevel >= 3)
        {
            isThunderstormUnlocked = true;
            heavenSmite?.Evolve(); // Thunderstorm
            EvolutionAnnouncer.Instance?.Announce("Thunderstorm");
        }
		if (!isLightningUnlocked && dashLevel >= 2 && speedLevel >= 2)
    	{
        	isLightningUnlocked = true;
            EvolutionAnnouncer.Instance?.Announce("Chain Lightning");
    	}
    }

    void Update()
    {
        if (isSupernovaUnlocked)
        {
            supernovaTimer += Time.deltaTime;
            if (supernovaTimer >= supernovaCooldown)
            {
                // spawn on the PLAYER (this manager lives on the Canvas, so transform.position is useless)
                Vector3 at = playerMovement != null ? playerMovement.transform.position : transform.position;
                Instantiate(supernovaPrefab, at, Quaternion.identity);
                supernovaTimer = 0;
            }
        }
    }

    void ResumeGame()
    {
        levelUpPanel.SetActive(false);
        Time.timeScale = 1f;
        MenuGuard.AnyOpen = false;
    }

public void TryTriggerLightning()
{
    if (!isLightningUnlocked) return;

    // search around the PLAYER (manager is on the Canvas, so transform.position is useless)
    Vector3 at = playerMovement != null ? playerMovement.transform.position : transform.position;
    Collider2D firstEnemy = Physics2D.OverlapCircle(at, 6f, LayerMask.GetMask("Enemies"));
    if (firstEnemy != null)
    {
        chainLightningScript.TriggerChain(firstEnemy.transform);
    }
}
}