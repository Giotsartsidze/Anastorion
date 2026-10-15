using UnityEngine;

/// <summary>
/// Applies the chosen run modifiers at the start of a run. Put ONE in the gameplay
/// scene (SampleScene). Reads the toggles from RunModifiers and wires each to its
/// system; stacks a shard reward for every active modifier.
/// </summary>
public class RunModifierManager : MonoBehaviour
{
    public static float EnemyHealthMult = 1f; // Giants (read by EnemyHealth)
    public static bool NoHealthDrops = false; // Famine  (read by EnemyHealth)

    [Header("Modifier strengths")]
    public float hordeSpawnBonus = 0.5f;        // +50% spawn rate
    public float giantsHpBonus = 0.5f;          // +50% enemy HP
    public float fragileDamageBonus = 0.5f;     // +50% damage taken
    public float blackoutVisionCut = 0.35f;     // -35% vision
    public float shardRewardPerModifier = 0.25f;// +25% shards per active modifier

    void Awake()
    {
        // reset the statics each run (scene reload re-runs this)
        EnemyHealthMult = 1f;
        NoHealthDrops = false;

        bool horde    = RunModifiers.IsOn("Horde");
        bool giants   = RunModifiers.IsOn("Giants");
        bool fragile  = RunModifiers.IsOn("Fragile");
        bool famine   = RunModifiers.IsOn("Famine");
        bool blackout = RunModifiers.IsOn("Blackout");

        if (giants) EnemyHealthMult = 1f + giantsHpBonus;
        if (famine) NoHealthDrops = true;

        if (horde)
        {
            var wm = FindObjectOfType<WaveManager>();
            if (wm != null) wm.spawnRateMultiplier *= 1f + hordeSpawnBonus;
        }
        if (fragile)
        {
            var ph = FindObjectOfType<PlayerHealth>();
            if (ph != null) ph.damageTakenMultiplier *= 1f + fragileDamageBonus;
        }
        if (blackout)
        {
            var pv = FindObjectOfType<PlayerVision>();
            if (pv != null)
            {
                pv.baseRadius *= 1f - blackoutVisionCut;
                pv.maxRadius  *= 1f - blackoutVisionCut;
            }
        }

        // reward: more shards for every active modifier (separate from Greed, so they stack)
        int active = RunModifiers.ActiveCount();
        var cm = FindObjectOfType<CurrencyManager>();
        if (cm != null) cm.runRewardMultiplier = 1f + active * shardRewardPerModifier;
    }
}
