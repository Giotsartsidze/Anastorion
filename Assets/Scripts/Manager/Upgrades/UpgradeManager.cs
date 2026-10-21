using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UpgradeManager : MonoBehaviour
{
    public List<UpgradeData> allUpgrades; 
    public UpgradeUIElement[] cardUI;     
    public GameObject levelUpPanel;
    public List<UpgradeData> activeUpgrades;
    public List<UpgradeData> lockedUpgrades;

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

    // Cached component references
    private PlayerMovement playerMovement;
    private PlayerDash playerDash;
    private LightPulse lightPulse;

    void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        playerDash = FindObjectOfType<PlayerDash>();
        lightPulse = FindObjectOfType<LightPulse>();

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

        var randomUpgrades = activeUpgrades.Where(IsEligible).OrderBy(x => Random.value).Take(3).ToList();

        for (int i = 0; i < randomUpgrades.Count; i++)
        {
            if (i < cardUI.Length && cardUI[i] != null) 
            {
                cardUI[i].Setup(randomUpgrades[i], this);
                cardUI[i].AnimateIn(i * 0.15f); // შენი ანიმაცია
            }
        }
    }

    // Relic reward (from a mega-elite): same panel, but biased to the best cards.
    public void ShowRelicReward()
    {
        if (levelUpPanel == null) return;
        levelUpPanel.SetActive(true);
        Time.timeScale = 0f;

        var picks = activeUpgrades
            .Where(IsEligible)
            .OrderByDescending(u => (int)u.rarity)
            .ThenBy(_ => Random.value)
            .Take(3).ToList();

        for (int i = 0; i < picks.Count; i++)
        {
            if (i < cardUI.Length && cardUI[i] != null)
            {
                cardUI[i].Setup(picks[i], this);
                cardUI[i].AnimateIn(i * 0.15f);
            }
        }
    }

    // True on the FIRST pick of a skill (before it's active), so the card can show
    // an "unlock" description instead of the "upgrade" one.
    public bool IsFirstPick(UpgradeData data)
    {
        switch (data.type)
        {
            case UpgradeData.UpgradeType.Pulse:       return lightPulse != null && !lightPulse.active;
            case UpgradeData.UpgradeType.FireTrail:   { var f = FindObjectOfType<FireTrail>();   return f != null && !f.active; }
            case UpgradeData.UpgradeType.Starfall:    { var s = FindObjectOfType<Starfall>();     return s != null && !s.active; }
            case UpgradeData.UpgradeType.HeavenSmite: { var h = FindObjectOfType<HeavenSmite>();  return h != null && !h.active; }
            case UpgradeData.UpgradeType.ChainLash:   { var c = FindObjectOfType<ChainLash>();    return c != null && !c.active; }
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
                var wisps = FindObjectOfType<OrbitingWisps>();
                if (wisps != null) {
                    wisps.count++;
                    wisps.ActivateSkill();
                }
                wispLevel++;
                break;

            case UpgradeData.UpgradeType.WispSpeed:
                var wispScript = FindObjectOfType<OrbitingWisps>();
                if (wispScript != null) wispScript.orbitSpeed += data.valueModifier;
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
                var dartsC = FindObjectOfType<StellarDarts>();
                if (dartsC != null) dartsC.dartCount += Mathf.Max(1, Mathf.RoundToInt(data.valueModifier));
                break;

            case UpgradeData.UpgradeType.DartFireRate:
                var dartsF = FindObjectOfType<StellarDarts>();
                if (dartsF != null) dartsF.fireRate = Mathf.Max(0.15f, dartsF.fireRate - data.valueModifier);
                break;

            case UpgradeData.UpgradeType.FireTrail:
                var fire = FindObjectOfType<FireTrail>();
                if (fire != null)
                {
                    if (!fire.active) fire.active = true;                 // first pick: unlock
                    else fire.patchDamage += Mathf.Max(1, Mathf.RoundToInt(data.valueModifier)); // later: stronger
                }
                fireTrailLevel++;
                break;

            case UpgradeData.UpgradeType.Starfall:
                var star = FindObjectOfType<Starfall>();
                if (star != null)
                {
                    if (!star.active) star.active = true;                 // first pick: unlock
                    else star.meteorCount += Mathf.Max(1, Mathf.RoundToInt(data.valueModifier)); // later: more meteors
                }
                starfallLevel++;
                break;

            case UpgradeData.UpgradeType.HeavenSmite:
                var smite = FindObjectOfType<HeavenSmite>();
                if (smite != null)
                {
                    if (!smite.active) smite.active = true;               // first pick: unlock
                    else smite.damage += Mathf.Max(1, Mathf.RoundToInt(data.valueModifier)); // later: harder strikes
                }
                smiteLevel++;
                break;

            case UpgradeData.UpgradeType.ChainLash:
                var chain = FindObjectOfType<ChainLash>();
                if (chain != null)
                {
                    if (!chain.active) chain.active = true;               // first pick: unlock
                    else { chain.lashCount += 1; chain.damage += Mathf.Max(1, Mathf.RoundToInt(data.valueModifier)); }
                }
                chainLashLevel++;
                if (chainLashLevel >= 5) FindObjectOfType<ChainLash>()?.Evolve(); // Titan's Wrath
                break;
        }

        CheckSynergies(); // ყოველი აფგრეიდის შემდეგ ვამოწმებთ სინერგიას
        ResumeGame();
    }

    // Human-readable evolution recipes + progress, for the stats/synergy panel.
    public string GetSynergyReport()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine(SynLine("Supernova", "Wisps", wispLevel, 3, "Light Radius", radiusLevel, 2, isSupernovaUnlocked));
        sb.AppendLine(SynLine("Chain Lightning", "Dash CD", dashLevel, 2, "Move Speed", speedLevel, 2, isLightningUnlocked));
        sb.AppendLine(SynLine("Comet", "Ember Path", fireTrailLevel, 3, "Move Speed", speedLevel, 3, isCometUnlocked));
        sb.AppendLine(SynLine("Meteor Storm", "Starfall", starfallLevel, 3, "Light Radius", radiusLevel, 3, isMeteorStormUnlocked));
        sb.AppendLine(SynLine("Thunderstorm", "Heaven's Smite", smiteLevel, 3, "Dash CD", dashLevel, 3, isThunderstormUnlocked));

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
        }

        // New-skill evolutions: max the skill + its paired stat.
        if (!isCometUnlocked && fireTrailLevel >= 3 && speedLevel >= 3)
        {
            isCometUnlocked = true;
            FindObjectOfType<FireTrail>()?.Evolve();   // Comet
        }
        if (!isMeteorStormUnlocked && starfallLevel >= 3 && radiusLevel >= 3)
        {
            isMeteorStormUnlocked = true;
            FindObjectOfType<Starfall>()?.Evolve();    // Meteor Storm
        }
        if (!isThunderstormUnlocked && smiteLevel >= 3 && dashLevel >= 3)
        {
            isThunderstormUnlocked = true;
            FindObjectOfType<HeavenSmite>()?.Evolve(); // Thunderstorm
        }
		if (!isLightningUnlocked && dashLevel >= 2 && speedLevel >= 2)
    	{
        	isLightningUnlocked = true;
    	}	
    }

    void Update()
    {
        if (isSupernovaUnlocked)
        {
            supernovaTimer += Time.deltaTime;
            if (supernovaTimer >= supernovaCooldown)
            {
                // ვაჩენთ სუპერნოვას მოთამაშის პოზიციაზე
                Instantiate(supernovaPrefab, transform.position, Quaternion.identity, transform);
                supernovaTimer = 0;
            }
        }
    }

    void ResumeGame()
    {
        levelUpPanel.SetActive(false);
        Time.timeScale = 1f;
    }

public void TryTriggerLightning()
{
    if (!isLightningUnlocked) return;

    // ვეძებთ უახლოეს მტერს, რომ დავიწყოთ ჯაჭვი
    Collider2D firstEnemy = Physics2D.OverlapCircle(transform.position, 6f, LayerMask.GetMask("Enemies"));
    if (firstEnemy != null)
    {
        chainLightningScript.TriggerChain(firstEnemy.transform);
    }
}
}