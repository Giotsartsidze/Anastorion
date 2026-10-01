using UnityEngine;

/// <summary>
/// Frame-based sprite animator: plays idle / run / death sprite sequences.
/// "Idle vs run" is auto-detected from how far the object moved this frame, so it
/// works for every character regardless of how its AI drives movement — no Animator
/// state machine to wire.
///
/// Add this to each character PREFAB and assign that character's sheets in the
/// Inspector (frames must be serialized, so this can't be auto-attached at runtime).
/// Slice a sprite sheet via Sprite Editor → Slice → Grid By Cell Count, then drag the
/// resulting frames into the arrays below.
/// </summary>
[DisallowMultipleComponent]
public class CharacterAnimator : MonoBehaviour
{
    public enum State { Idle, Run, Dead }

    [Header("Frames (drag sliced sprites here, in order)")]
    public Sprite[] idleFrames;
    public Sprite[] runFrames;
    public Sprite[] deathFrames;

    [Header("Timing")]
    public float fps = 10f;
    [Tooltip("Per-frame movement (world units) above which the character is 'running'.")]
    public float moveThreshold = 0.002f;

    private SpriteRenderer sr;
    private State state = State.Idle;
    private int frame;
    private float frameTimer;
    private Vector3 lastPos;

    void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    // Pooling-safe: every respawn starts clean at idle frame 0.
    void OnEnable()
    {
        state = State.Idle;
        frame = 0;
        frameTimer = 0f;
        lastPos = transform.position;
    }

    public bool HasDeath => deathFrames != null && deathFrames.Length > 0;
    public float DeathDuration => HasDeath ? deathFrames.Length / Mathf.Max(1f, fps) : 0f;

    /// <summary>Switch to the death sequence (plays once, holds the last frame).</summary>
    public void PlayDeath()
    {
        if (!HasDeath) return;
        state = State.Dead;
        frame = 0;
        frameTimer = 0f;
    }

    void LateUpdate()
    {
        if (sr == null) return;

        // Death overrides everything until the object is disabled/respawned.
        if (state != State.Dead)
        {
            float movedSqr = (transform.position - lastPos).sqrMagnitude;
            state = movedSqr > moveThreshold * moveThreshold ? State.Run : State.Idle;
        }
        lastPos = transform.position;

        Sprite[] frames = CurrentFrames();
        if (frames == null || frames.Length == 0) return;

        frameTimer += Time.deltaTime;
        float frameDur = 1f / Mathf.Max(1f, fps);
        while (frameTimer >= frameDur)
        {
            frameTimer -= frameDur;
            frame++;
            if (state == State.Dead)
                frame = Mathf.Min(frame, frames.Length - 1); // hold last frame
            else
                frame %= frames.Length;                      // loop
        }

        frame = Mathf.Clamp(frame, 0, frames.Length - 1);
        sr.sprite = frames[frame];
    }

    private Sprite[] CurrentFrames()
    {
        switch (state)
        {
            case State.Run:  return (runFrames != null && runFrames.Length > 0) ? runFrames : idleFrames;
            case State.Dead: return deathFrames;
            default:         return idleFrames;
        }
    }
}
