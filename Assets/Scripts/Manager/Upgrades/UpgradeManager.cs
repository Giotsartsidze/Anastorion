using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UpgradeManager : MonoBehaviour
{
    public List<UpgradeData> allUpgrades; 
    public UpgradeUIElement[] cardUI;     
    public GameObject levelUpPanel;

    public void ShowUpgrades()
    {
        levelUpPanel.SetActive(true);
        Time.timeScale = 0f; 

        // ვირჩევთ 3 შემთხვევითს
        var randomUpgrades = allUpgrades.OrderBy(x => Random.value).Take(3).ToList();

        for (int i = 0; i < randomUpgrades.Count; i++)
        {
            if (i < cardUI.Length && cardUI[i] != null) 
            {
                cardUI[i].Setup(randomUpgrades[i], this);
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
                    wisps.wispCount++;
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
        }

        ResumeGame();
    }

    void ResumeGame()
    {
        levelUpPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}