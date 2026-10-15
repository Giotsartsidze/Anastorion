using UnityEngine;

/// <summary>
/// Applies the permanent meta-shop upgrades (bought between runs) to the player at
/// the start of a run. The shop only SAVES the purchased level (PlayerPrefs); this is
/// what actually makes those purchases do something in gameplay.
///
/// Put ONE of these in the gameplay scene (SampleScene). Runs in Awake so the boosts
/// are in place before PlayerHealth/PlayerMovement initialize.
/// Pref keys match the shop assets' upgradeName: "Upgrade_MaxHealth", etc.
/// </summary>
public class MetaUpgradeApplier : MonoBehaviour
{
    [Header("Bonus per purchased level")]
    public int healthPerLevel = 20;      // +HP per MaxHealth level
    public float moveSpeedPerLevel = 1f; // +move speed per MoveSpeed level
    public float magnetPerLevel = 1.5f;  // +magnet radius per MagnetRange level

    [Header("New upgrades — bonus per level")]
    public int armorPerLevel = 1;          // Armor: -damage taken
    public float regenPerLevel = 0.3f;     // Vitality: +HP/sec
    public float xpPercentPerLevel = 0.08f;// Wisdom: +8% XP
    public float shardPercentPerLevel = 0.10f; // Greed: +10% shards
    public int dartsPerLevel = 1;          // Power: +starting darts
    public int revivesPerLevel = 1;        // Revive: +free second-chances

    void Awake()
    {
        int hpLvl  = PlayerPrefs.GetInt("Upgrade_MaxHealth", 0);
        int spLvl  = PlayerPrefs.GetInt("Upgrade_MoveSpeed", 0);
        int mgLvl  = PlayerPrefs.GetInt("Upgrade_MagnetRange", 0);
        int armLvl = PlayerPrefs.GetInt("Upgrade_Armor", 0);
        int vitLvl = PlayerPrefs.GetInt("Upgrade_Vitality", 0);
        int wisLvl = PlayerPrefs.GetInt("Upgrade_Wisdom", 0);
        int grdLvl = PlayerPrefs.GetInt("Upgrade_Greed", 0);
        int powLvl = PlayerPrefs.GetInt("Upgrade_Power", 0);
        int revLvl = PlayerPrefs.GetInt("Upgrade_Revive", 0);

        // Max Health — raise before PlayerHealth.Start sets currentHealth = maxHealth.
        var ph = FindObjectOfType<PlayerHealth>();
        if (ph != null)
        {
            ph.maxHealth += hpLvl * healthPerLevel;
            ph.armor += armLvl * armorPerLevel;              // Armor
            ph.regenPerSecond += vitLvl * regenPerLevel;     // Vitality
            ph.revives += revLvl * revivesPerLevel;          // Revive
        }

        // Move Speed — raise both the force and the cap so top speed actually rises.
        var pm = FindObjectOfType<PlayerMovement>();
        if (pm != null)
        {
            pm.moveSpeed += spLvl * moveSpeedPerLevel;
            pm.maxSpeed += spLvl * moveSpeedPerLevel;
        }

        // Magnet Range — global bonus read by every XP orb.
        XPOrb.BonusMagnetRadius = mgLvl * magnetPerLevel;

        // Wisdom — more XP per orb (static, reset each run).
        ExperienceManager.XPMultiplier = 1f + wisLvl * xpPercentPerLevel;

        // Greed — more shards earned this run.
        var cm = FindObjectOfType<CurrencyManager>();
        if (cm != null) cm.shardMultiplier = 1f + grdLvl * shardPercentPerLevel;

        // Power — start the run with extra darts.
        var darts = FindObjectOfType<StellarDarts>();
        if (darts != null) darts.dartCount += powLvl * dartsPerLevel;
    }
}
