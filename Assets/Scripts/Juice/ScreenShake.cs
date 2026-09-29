using UnityEngine;

/// <summary>
/// Direct camera screen-shake — no Cinemachine impulse/listener setup required.
/// Runs with a high execution order so its LateUpdate fires AFTER Cinemachine has
/// positioned the camera, then adds a decaying random offset on top. Cinemachine
/// resets the base position every frame, so the offset never drifts.
///
/// Put ONE of these anywhere in the scene. It shakes Camera.main by default.
/// Call: ScreenShake.Instance?.Shake(force)   (force ≈ world-unit magnitude)
/// </summary>
[DefaultExecutionOrder(10000)]
public class ScreenShake : MonoBehaviour
{
    public static ScreenShake Instance { get; private set; }

    [Tooltip("Minimum seconds between shakes. Prevents swarm-kill rumble.")]
    public float minInterval = 0.05f;

    [Tooltip("How long one shake takes to fully decay.")]
    public float duration = 0.15f;

    [Tooltip("Camera to shake. Leave empty to use Camera.main.")]
    public Transform cameraTransform;

    private float startMagnitude;
    private float timer;
    private bool shaking;
    private float nextAllowedTime;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void Shake(float force)
    {
        if (Time.unscaledTime < nextAllowedTime) return;
        nextAllowedTime = Time.unscaledTime + minInterval;

        // Blend with any shake still in progress, then take the stronger of the two.
        float remaining = shaking ? startMagnitude * (1f - timer / duration) : 0f;
        startMagnitude = Mathf.Max(remaining, force);
        timer = 0f;
        shaking = true;
    }

    void LateUpdate()
    {
        if (!shaking || cameraTransform == null) return;

        timer += Time.unscaledDeltaTime; // unscaled so it still shakes during hit-stop
        if (timer >= duration)
        {
            shaking = false;
            return;
        }

        float mag = startMagnitude * (1f - timer / duration);
        Vector3 offset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f) * mag;
        cameraTransform.position += offset;
    }
}
