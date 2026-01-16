using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;
    public int totalShards;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadCurrency();
        }
        else { Destroy(gameObject); }
    }

    public void AddShards(int amount)
    {
        totalShards += amount;
        SaveCurrency();
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