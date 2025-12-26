using UnityEngine;
using Unity.Cinemachine; // Unity 6-ისთვის ეს აუცილებელია!

public class ShakeSource : MonoBehaviour
{
    private CinemachineImpulseSource impulseSource;

    void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void TriggerShake()
    {
        // Unity 6-ში GenerateImpulse() ისევ მუშაობს
        if (impulseSource != null) impulseSource.GenerateImpulse();
    }
}