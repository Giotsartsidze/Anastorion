using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeUIElement : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText; 
    public Image iconImage;                
    public Button button;                   

    public void Setup(UpgradeData data, UpgradeManager manager)
    {
        titleText.text = data.upgradeName;
        descriptionText.text = data.description;
        
        // NULL CHECK: თუ ინსპექტორში IconImage არ ჩააგდე, ერორი რომ არ ამოაგდოს
        if (iconImage != null && data.icon != null) 
        {
            iconImage.sprite = data.icon;
        }

        // ბარათის ფერის შეცვლა იშვიათობის მიხედვით
        if (GetComponent<Image>() != null)
            GetComponent<Image>().color = data.GetRarityColor();

        // ღილაკის ლოგიკა
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => manager.ApplyUpgrade(data));
    }
}