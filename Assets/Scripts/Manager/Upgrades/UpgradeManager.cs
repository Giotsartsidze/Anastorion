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

        var randomUpgrades = activeUpgrades.OrderBy(x => Random.value).Take(3).ToList();

        for (int i = 0; i < randomUpgrades.Count; i++)
        {
            if (i < cardUI.Length && cardUI[i] != null) 
            {
                cardUI[i].Setup(randomUpgrades[i], this);
                cardUI[i].AnimateIn(i * 0.15f); // შენი ანიმაცია
            }
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
        }

        CheckSynergies(); // ყოველი აფგრეიდის შემდეგ ვამოწმებთ სინერგიას
        ResumeGame();
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