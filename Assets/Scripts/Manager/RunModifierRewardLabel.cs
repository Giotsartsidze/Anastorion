using UnityEngine;
using TMPro;

/// <summary>
/// Menu label that shows the total shard reward from the currently-toggled modifiers.
/// Put on a TMP_Text next to the modifier toggles. Updates live as toggles change.
/// Keep percentPerModifier in sync with RunModifierManager.shardRewardPerModifier.
/// </summary>
[RequireComponent(typeof(TMP_Text))]
public class RunModifierRewardLabel : MonoBehaviour
{
    public float percentPerModifier = 25f;
    private TMP_Text label;

    void Awake() => label = GetComponent<TMP_Text>();

    void Update()
    {
        int pct = Mathf.RoundToInt(RunModifiers.ActiveCount() * percentPerModifier);
        label.text = pct > 0 ? $"Reward: +{pct}% shards" : "Reward: —";
    }
}
