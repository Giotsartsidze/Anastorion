using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UpgradeManager : MonoBehaviour
{
    public List<UpgradeData> allUpgrades; 
    public UpgradeUIElement[] cardUI;     
    public GameObject levelUpPanel;
	public List<UpgradeData> activeUpgrades; // აქ მხოლოდ საწყისი აფგრეიდები (Speed, Radius)
    public List<UpgradeData> lockedUpgrades;

	public void UnlockWispUpgrades()
{
    // გადაგვაქვს ყველა დალოქილი აფგრეიდი აქტიურებში
    foreach (var upgrade in lockedUpgrades)
    {
        if (!activeUpgrades.Contains(upgrade))
        {
            activeUpgrades.Add(upgrade);
        }
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
            
            // სიახლე: თითოეულ ბარათს ვაძლევთ 0.1 წამით მეტ დაყოვნებას (Cascade effect)
            cardUI[i].AnimateIn(i * 0.15f);
        }
    }
}

    public void ApplyUpgrade(UpgradeData data)
    {
        // ვიყენებთ switch-ს მეტი სიცხადისთვის
        switch (data.type)
        {
            case UpgradeData.UpgradeType.MoveSpeed:
                FindObjectOfType<PlayerMovement>().moveSpeed += data.valueModifier;
                break;

            case UpgradeData.UpgradeType.LightRadius:
                FindObjectOfType<LightPulse>().maxRadius += data.valueModifier;
                break;

            case UpgradeData.UpgradeType.WispCount:
                // ვამატებთ ბურთულების რაოდენობას
                var wisps = FindObjectOfType<OrbitingWisps>();
                if (wisps != null) {
                    wisps.count++;
                    wisps.ActivateSkill(); // ხელახლა ვრთავთ ახალი რაოდენობით
                }
                break;

            case UpgradeData.UpgradeType.WispSpeed:
                var wispScript = FindObjectOfType<OrbitingWisps>();
                if (wispScript != null) wispScript.orbitSpeed += data.valueModifier;
                break;

            case UpgradeData.UpgradeType.PulseCooldown:
                // ვამცირებთ დალოდების დროს
                FindObjectOfType<LightPulse>().cooldown -= data.valueModifier;
                break;
            case UpgradeData.UpgradeType.DashCooldown:
                FindObjectOfType<PlayerDash>().dashCooldown -= 0.3f; // ამცირებს დალოდებას
                break;

            case UpgradeData.UpgradeType.DashSpeed:
                FindObjectOfType<PlayerDash>().dashSpeed += 5f; // ზრდის მანძილს
                break;
        }

        ResumeGame();
    }

    void ResumeGame()
    {
        levelUpPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}