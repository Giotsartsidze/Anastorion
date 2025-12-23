using UnityEngine;
using UnityEngine.UI;

public class ExperienceManager : MonoBehaviour
{
    public Slider xpSlider;
    public int currentXP = 0;
    public int targetXP = 100;
    public int currentLevel = 1;
	public GameObject levelUpPanel;

    void Start()
    {
        xpSlider.maxValue = targetXP;
        xpSlider.value = currentXP;
    }

    public void AddExperience(int amount)
    {
        currentXP += amount;
        if (currentXP >= targetXP)
        {
            LevelUp();
        }
        UpdateUI();
    }

    void LevelUp()
    {
        currentLevel++;
        currentXP = 0;
        targetXP = Mathf.RoundToInt(targetXP * 1.5f); // ექსპონენციალური ზრდა
        xpSlider.maxValue = targetXP;

		PauseGame();
        FindObjectOfType<UpgradeManager>().ShowUpgrades();
        Debug.Log("Anastorion evolved to Level " + currentLevel);
        // აქ მოგვიანებით დავამატებთ პაუზას და სკილის არჩევის მენიუს
    }

    void UpdateUI()
    {
        xpSlider.value = currentXP;
    }

void PauseGame()
{
    levelUpPanel.SetActive(true);
    Time.timeScale = 0f; // თამაში სრულიად ჩერდება (JS-ში ამის ანალოგი არ გვაქვს, ეს Unity-ს ფუფუნებაა)
}

public void ResumeGame()
{
    levelUpPanel.SetActive(false);
    Time.timeScale = 1f; // თამაში გრძელდება
}


}