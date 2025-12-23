using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement; // რესტარტისთვის

public class VictoryManager : MonoBehaviour
{
    public CanvasGroup canvasGroup; // დაამატე Canvas Group კომპონენტი პანელს
    public float fadeDuration = 2f;

    public void ShowVictory()
    {
        gameObject.SetActive(true);
        StartCoroutine(VictoryRoutine());
    }

    IEnumerator VictoryRoutine()
    {
        // 1. გამოვაჩინოთ ტექსტი
        yield return new WaitForSeconds(3f);
    
        // 2. გავაძლიეროთ მტრები მომავალი ტალღებისთვის
        WaveManager waveManager = FindObjectOfType<WaveManager>();
        if (waveManager != null)
        {
            // ყოველი ბოსის მოკვლის მერე მტრები 20%-ით ჩქარდებიან
            // (დაამატე ეს ლოგიკა WaveManager-ში)
            Debug.Log("ENEMIES ARE NOW STRONGER!");
        }

        // 3. უბრალოდ გავაქროთ პანელი და გავაგრძელოთ თამაში
        gameObject.SetActive(false);
    }
    
}