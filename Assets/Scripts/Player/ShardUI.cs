using UnityEngine;
using TMPro;

public class ShardUI : MonoBehaviour
{
    private TextMeshProUGUI text;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (CurrencyManager.Instance != null)
        {
            // ვაჩვენებთ საერთო ბალანსს
            text.text = "x " + CurrencyManager.Instance.totalShards.ToString();
        }
    }
}