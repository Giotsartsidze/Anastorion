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
            FindObjectOfType<PlayerMovement>().moveSpeed += data.valueModifier;
            speedLevel++;
            break;

        case UpgradeData.UpgradeType.DashSpeed:
            FindObjectOfType<PlayerDash>().dashSpeed += 5f;
            dashLevel++;
            break;

            case UpgradeData.UpgradeType.LightRadius:
                FindObjectOfType<LightPulse>().maxRadius += data.valueModifier;
                radiusLevel++; // ვუმატებთ დონეს
                break;

            case UpgradeData.UpgradeType.WispCount:
                var wisps = FindObjectOfType<OrbitingWisps>();
                if (wisps != null) {
                    wisps.count++;
                    wisps.ActivateSkill();
                }
                wispLevel++; // ვუმატებთ დონეს
                break;

            case UpgradeData.UpgradeType.WispSpeed:
                var wispScript = FindObjectOfType<OrbitingWisps>();
                if (wispScript != null) wispScript.orbitSpeed += data.valueModifier;
                break;

            case UpgradeData.UpgradeType.PulseCooldown:
                FindObjectOfType<LightPulse>().cooldown -= data.valueModifier;
                break;
                
            case UpgradeData.UpgradeType.DashCooldown:
                FindObjectOfType<PlayerDash>().dashCooldown -= 0.3f;
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