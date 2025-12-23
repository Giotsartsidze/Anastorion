using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UpgradeManager : MonoBehaviour
{
    public List<UpgradeData> allUpgrades; // აქ ჩაყრი იმ 3 ფაილს
    public UpgradeUIElement[] cardUI;     // აქ ჩაყრი იმ 3 ღილაკს
    public GameObject levelUpPanel;

    public void ShowUpgrades()
    {
        levelUpPanel.SetActive(true);
        Time.timeScale = 0f; // თამაში ჩერდება

        // ვირჩევთ 3 შემთხვევითს
        var randomUpgrades = allUpgrades.OrderBy(x => Random.value).Take(3).ToList();

        // შეცვალე შენი For ციკლი ამით:
        for (int i = 0; i < randomUpgrades.Count; i++)
        {
            // ვამოწმებთ, რომ ინსპექტორში ნამდვილად ჩავაგდეთ ღილაკი ამ ინდექსზე
            if (i < cardUI.Length && cardUI[i] != null) 
            {
                cardUI[i].Setup(randomUpgrades[i], this);
            }
        }
    }

    public void ApplyUpgrade(UpgradeData data)
    {
        // აქ ვპოულობთ Player-ის კომპონენტებს და ვცვლით ციფრებს
        if (data.type == UpgradeData.UpgradeType.MoveSpeed)
            FindObjectOfType<PlayerMovement>().moveSpeed += data.valueModifier;
        
        if (data.type == UpgradeData.UpgradeType.LightRadius)
            FindObjectOfType<LightPulse>().maxRadius += data.valueModifier;

        ResumeGame();
    }

    void ResumeGame()
    {
        levelUpPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}