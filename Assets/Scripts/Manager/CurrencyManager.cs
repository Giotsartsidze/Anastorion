using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;
    public int totalShards;
    public float shardMultiplier = 1f; // Greed meta-upgrade (set each run by MetaUpgradeApplier)
    private float shardAccum;          // carries fractional bonus so +10% actually pays out on 1-at-a-time drops

    void Awake()
    {
        Instance = this;
        LoadCurrency();
    }

    public void AddShards(int amount)
    {
        shardAccum += amount * shardMultiplier;
        int whole = Mathf.FloorToInt(shardAccum);
        if (whole > 0)
        {
            totalShards += whole;
            shardAccum -= whole;
            SaveCurrency();
        }
    }

    public bool SpendShards(int amount)
    {
        if (totalShards >= amount)
        {
            totalShards -= amount;
            SaveCurrency();
            return true;
        }
        return false;
    }

    public void SaveCurrency() => PlayerPrefs.SetInt("TotalShards", totalShards);
    private void LoadCurrency() => totalShards = PlayerPrefs.GetInt("TotalShards", 0);
}