using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public int currentLevel = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 100;

    [Header("UI References")]
    public Slider xpBar;
    public TextMeshProUGUI levelText;
    public GameObject levelUpPanel; // პანელი, სადაც აფგრეიდები გამოჩნდება

    public void AddXP(int amount)
    {
        currentXP += amount;
        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
        UpdateUI();
    }

    void LevelUp()
    {
        currentLevel++;
        currentXP -= xpToNextLevel;
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.5f); // სირთულის ზრდა
        
        // თამაშის დაპაუზება და აფგრეიდების მენიუს გამოჩენა
        Time.timeScale = 0f; 
        levelUpPanel.SetActive(true);
        
        // აქ უნდა გაეშვას აფგრეიდების გენერაციის ლოგიკა
        GetComponent<UpgradeManager>().ShowUpgrades();
    }

    void UpdateUI()
    {
        if (xpBar != null)
        {
            xpBar.maxValue = xpToNextLevel;
            xpBar.value = currentXP;
        }
        if (levelText != null) levelText.text = "LVL: " + currentLevel;
    }
}