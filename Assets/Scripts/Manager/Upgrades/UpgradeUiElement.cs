using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeUIElement : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descText;
    
    private UpgradeData currentData;
    private UpgradeManager manager;

    public void Setup(UpgradeData data, UpgradeManager mngr)
    {
        currentData = data;
        manager = mngr;
        titleText.text = data.upgradeName;
        descText.text = data.description;
    }

    public void OnClick() // ამას მივაბამთ ღილაკს
    {
        manager.ApplyUpgrade(currentData);
    }
}