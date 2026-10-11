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

    void Awake()
    {
        int hpLvl = PlayerPrefs.GetInt("Upgrade_MaxHealth", 0);
        int spLvl = PlayerPrefs.GetInt("Upgrade_MoveSpeed", 0);
        int mgLvl = PlayerPrefs.GetInt("Upgrade_MagnetRange", 0);

        // Max Health — raise before PlayerHealth.Start sets currentHealth = maxHealth.
        var ph = FindObjectOfType<PlayerHealth>();
        if (ph != null) ph.maxHealth += hpLvl * healthPerLevel;

        // Move Speed — raise both the force and the cap so top speed actually rises.
        var pm = FindObjectOfType<PlayerMovement>();
        if (pm != null)
        {
            pm.moveSpeed += spLvl * moveSpeedPerLevel;
            pm.maxSpeed += spLvl * moveSpeedPerLevel;
        }

        // Magnet Range — global bonus read by every XP orb.
        XPOrb.BonusMagnetRadius = mgLvl * magnetPerLevel;
    }
}
