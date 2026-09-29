using System.Collections;
using UnityEngine;

/// <summary>
/// Brief time-freeze for impact ("hit-stop"). Use sparingly — big hits, elite/boss
/// deaths, player death. NOT on every trash-mob kill (that turns into a stutter fest
/// in a bullet-heaven).
/// Put ONE of these in the scene, then call: HitStop.Instance?.Stop(0.08f)
/// </summary>
public class HitStop : MonoBehaviour
{
    public static HitStop Instance { get; private set; }

    private Coroutine routine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
            Time.timeScale = 1f; // never leave the game frozen
        }
    }

    /// <param name="durationSeconds">Real-time length of the freeze.</param>
    /// <param name="timeScaleDuringStop">0 = full freeze; try 0.05 for slow-mo instead.</param>
    public void Stop(float durationSeconds, float timeScaleDuringStop = 0f)
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(DoStop(durationSeconds, timeScaleDuringStop));
    }

    private IEnumerator DoStop(float duration, float scale)
    {
        Time.timeScale = scale;
        yield return new WaitForSecondsRealtime(duration); // realtime — unaffected by timeScale
        Time.timeScale = 1f;
        routine = null;
    }
}
