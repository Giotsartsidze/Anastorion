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

    // დონეების მთვლელები
    private int wispLevel = 0;
    private int radiusLevel = 0;

    // Cached component references
    private PlayerMovement playerMovement;
    private PlayerDash playerDash;
    private LightPulse lightPulse;

    void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        playerDash = FindObjectOfType<PlayerDash>();
        lightPulse = FindObjectOfType<LightPulse>();
    }

    public void UnlockWispUpgrades()
    {
        foreach (var upgrade in lockedUpgrades)
        {
            if (!activeUpgrades.Contains(upgrade)) activeUpgrades.Add(upgrade);
        }
        Debug.Log("WISP UPGRADES UNLOCKED!");
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
                if (playerMovement != null) playerMovement.moveSpeed += data.valueModifier;
                speedLevel++;
                break;

            case UpgradeData.UpgradeType.DashSpeed:
                if (playerDash != null) playerDash.dashSpeed += 5f;
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

            case UpgradeData.UpgradeType.PulseCooldown:
                if (lightPulse != null) lightPulse.cooldown -= data.valueModifier;
                break;

            case UpgradeData.UpgradeType.DashCooldown:
                if (playerDash != null) playerDash.dashCooldown -= 0.3f;
                break;
        }

        CheckSynergies(); // ყოველი აფგრეიდის შემდეგ ვამოწმებთ სინერგიას
        ResumeGame();
    }

    void CheckSynergies()
    {
        // თუ პირობა სრულდება (მაგ: Wisp დონე 5 და Radius დონე 3)
        if (!isSupernovaUnlocked && wispLevel >= 5 && radiusLevel >= 3)
        {
            isSupernovaUnlocked = true;
            Debug.Log("SUPERNOVA EVOLUTION UNLOCKED!");
        }
		if (!isLightningUnlocked && dashLevel >= 3 && speedLevel >= 3)
    	{
        	isLightningUnlocked = true;
        	Debug.Log("SYNERGY UNLOCKED: CHAIN LIGHTNING!");
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