using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EvolutionManager : MonoBehaviour
{
    public List<SynergyData> allSynergies;
    private Dictionary<UpgradeData.UpgradeType, int> playerStats = new Dictionary<UpgradeData.UpgradeType, int>();

    public void TrackUpgrade(UpgradeData.UpgradeType type)
    {
        if (playerStats.ContainsKey(type)) playerStats[type]++;
        else playerStats[type] = 1;

        CheckForEvolutions();
    }

    void CheckForEvolutions()
    {
        foreach (var synergy in allSynergies)
        {
            if (playerStats.GetValueOrDefault(synergy.requiredType1) >= synergy.requiredLevel1 &&
                playerStats.GetValueOrDefault(synergy.requiredType2) >= synergy.requiredLevel2)
            {
                ActivateUltimate(synergy);
            }
        }
    }

    void ActivateUltimate(SynergyData synergy)
    {
        Debug.Log("ULTIMATE ACTIVATED: " + synergy.synergyName);
        // აქ ჩავრთავთ სპეციალურ ეფექტს ან ახალ სკრიპტს
    }
}
