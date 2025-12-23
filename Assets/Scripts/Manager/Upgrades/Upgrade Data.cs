using UnityEngine;

// ეს ხაზი საშუალებას მოგვცემს Right Click-ით შევქმნათ აფგრეიდის ფაილები
[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Anastorion/Upgrade")]
public class UpgradeData : ScriptableObject
{
    public string upgradeName;     // აფგრეიდის სახელი
    [TextArea] 
    public string description;     // მოკლე აღწერა
    public Sprite icon;            // სურათი (თუ გვექნება)

    public enum UpgradeType { MoveSpeed, LightRadius, FireRate }
    public UpgradeType type;       // რას ცვლის ეს აფგრეიდი

    public float valueModifier;    // რა ციფრით ცვლის (მაგ: +2 ან -0.5)
}