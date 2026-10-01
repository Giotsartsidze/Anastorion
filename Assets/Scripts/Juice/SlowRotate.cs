using UnityEngine;

/// <summary>
/// Spins the object slowly around Z. Uses unscaled time so it keeps turning on
/// menus/paused screens. Put on the Borjgali emblem for a living "spinning sun".
/// </summary>
public class SlowRotate : MonoBehaviour
{
    [Tooltip("Degrees per second (negative = clockwise).")]
    public float degreesPerSecond = -12f;

    void Update()
    {
        transform.Rotate(0f, 0f, degreesPerSecond * Time.unscaledDeltaTime);
    }
}
