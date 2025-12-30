using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems; // აუცილებელია ინტერფეისებისთვის
using System.Collections;

public class UpgradeUIElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText; 
    public Image iconImage;                
    public Button button;

    [Header("Hover Settings")]
    public float hoverScale = 1.1f;    // რამდენად გაიზარდოს
    public float scaleSpeed = 10f;     // ზრდის სისწრაფე
    private Vector3 originalScale = Vector3.one;
    private Coroutine scaleCoroutine;

    public void Setup(UpgradeData data, UpgradeManager manager)
    {
        titleText.text = data.upgradeName;
        descriptionText.text = data.description;
        
        if (iconImage != null && data.icon != null) 
            iconImage.sprite = data.icon;

        if (GetComponent<Image>() != null)
            GetComponent<Image>().color = data.GetRarityColor();

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => manager.ApplyUpgrade(data));
        
        // საწყისი ზომის შენახვა
        originalScale = Vector3.one;
        transform.localScale = originalScale;
    }

    // როცა მაუსს მივიტანთ
    public void OnPointerEnter(PointerEventData eventData)
    {
        StopAndStartScale(originalScale * hoverScale);
    }

    // როცა მაუსს მოვაცილებთ
    public void OnPointerExit(PointerEventData eventData)
    {
        StopAndStartScale(originalScale);
    }

    private void StopAndStartScale(Vector3 target)
    {
        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
        scaleCoroutine = StartCoroutine(ScaleTo(target));
    }

    private IEnumerator ScaleTo(Vector3 target)
    {
        // რადგან თამაში დაპაუზებულია, აქაც Realtime გვჭირდება
        while (Vector3.Distance(transform.localScale, target) > 0.001f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, target, Time.unscaledDeltaTime * scaleSpeed);
            yield return null;
        }
        transform.localScale = target;
    }
    

public void AnimateIn(float delay)
{
    // 1. საწყის ზომას ვაყენებთ ნულზე, რომ არ ჩანდეს
    transform.localScale = Vector3.zero;
    // 2. ვუშვებთ კორუტინას დაყოვნებით
    StartCoroutine(PopInRoutine(delay));
}

private IEnumerator PopInRoutine(float delay)
{
    // ვიყენებთ Realtime-ს, რადგან თამაში გაჩერებულია
    yield return new WaitForSecondsRealtime(delay);
    
    float duration = 0.3f; // ანიმაციის ხანგრძლივობა
    float elapsed = 0;

    while (elapsed < duration)
    {
        elapsed += Time.unscaledDeltaTime;
        float progress = elapsed / duration;
        
        // "Overshoot" ეფექტისთვის (ოდნავ მეტზე იზრდება და მერე დგება 1-ზე)
        float curve = Mathf.Sin(progress * Mathf.PI * 0.75f) * 1.15f;
        transform.localScale = Vector3.one * curve;
        
        yield return null;
    }

    transform.localScale = Vector3.one; // ბოლოს ვაფიქსირებთ 1-ზე
}
}