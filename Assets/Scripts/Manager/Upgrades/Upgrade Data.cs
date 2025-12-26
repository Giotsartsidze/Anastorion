using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Anastorion/Upgrade")]
public class UpgradeData : ScriptableObject
{
    public string upgradeName;
    [TextArea] 
    public string description;
    public Sprite icon;

    // დავამატეთ ყველა საჭირო ტიპი
    public enum UpgradeType { 
        MoveSpeed, 
        LightRadius, 
        WispCount, 
        WispSpeed, 
        PulseCooldown ,
        DashCooldown,
        DashSpeed
    }
    
    // იშვიათობის სისტემა
    public enum Rarity { Common, Rare, Legendary }

    public UpgradeType type;
    public Rarity rarity;
    public float valueModifier;

    // დამხმარე ფუნქცია ფერებისთვის
    public Color GetRarityColor()
    {
        return rarity switch
        {
            Rarity.Common => Color.white,
            Rarity.Rare => new Color(0.2f, 0.6f, 1f), // ლურჯი
            Rarity.Legendary => new Color(1f, 0.8f, 0f), // ოქროსფერი
            _ => Color.white
        };
    }
}