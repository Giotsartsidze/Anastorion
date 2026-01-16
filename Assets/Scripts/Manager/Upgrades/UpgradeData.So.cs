using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeData.So", menuName = "Scriptable Objects/UpgradeData.So")]
public class UpgradeData1 : ScriptableObject
{
    public string upgradeName;
    public int baseCost = 100;
    public float costMultiplier = 1.5f; // ფასი ყოველ ლეველზე იზრდება
    public int maxLevel = 5;

    public string GetPrefKey() => "Upgrade_" + upgradeName;

    public int GetCurrentLevel() => PlayerPrefs.GetInt(GetPrefKey(), 0);

    public int GetCurrentCost()
    {
        int level = GetCurrentLevel();
        // ფორმულა: Cost = Base * (Multiplier^Level)
        return Mathf.RoundToInt(baseCost * Mathf.Pow(costMultiplier, level));
    }

    public void Upgrade()
    {
        int level = GetCurrentLevel();
        if (level < maxLevel) PlayerPrefs.SetInt(GetPrefKey(), level + 1);
    }
}
