using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;
    
    // საერთო ქოინები, რომელიც მოთამაშეს აქამდე დაუგროვდა
    public int totalShards;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadCurrency(); // თამაშის ჩართვისას ვკითხულობთ დანაზოგს
            DontDestroyOnLoad(gameObject); // რომ მენიუშიც გადაყვეს ეს ინფორმაცია
        }
        else Destroy(gameObject);
    }

    // ქოინების დამატება (ამას ვიძახებთ თამაშის დროს)
    public void AddShards(int amount)
    {
        totalShards += amount;
        SaveCurrency();
    }

    // შენახვა PlayerPrefs-ში
    public void SaveCurrency()
    {
        PlayerPrefs.SetInt("TotalShards", totalShards);
        PlayerPrefs.Save();
    }

    // წაკითხვა
    public void LoadCurrency()
    {
        totalShards = PlayerPrefs.GetInt("TotalShards", 0);
    }
}