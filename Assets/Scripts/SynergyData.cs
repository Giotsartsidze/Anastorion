using UnityEngine;

[CreateAssetMenu(fileName = "NewSynergy", menuName = "Stats/Synergy")]
public class SynergyData : ScriptableObject
{
    public string synergyName;
    public UpgradeData.UpgradeType requiredType1;
    public int requiredLevel1;
    public UpgradeData.UpgradeType requiredType2;
    public int requiredLevel2;
    
    public GameObject specialEffectPrefab; // მაგ: Supernova-ს ვიზუალი
}
