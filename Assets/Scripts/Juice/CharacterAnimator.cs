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
    public enum State { Idle, Run, Dead, Attack }

    [Header("Frames (drag sliced sprites here, in order)")]
    public Sprite[] idleFrames;
    public Sprite[] runFrames;
    public Sprite[] deathFrames;
    [Tooltip("Attack/cast/summon/windup frames. Trigger via PlayAttack() from the AI.")]
    public Sprite[] attackFrames;

    [Header("Timing")]
    public float fps = 10f;
    [Tooltip("Per-frame movement (world units) above which the character is 'running'.")]
    public float moveThreshold = 0.002f;

    private SpriteRenderer sr;
    private State state = State.Idle;
    private int frame;
    private float frameTimer;
    private Vector3 lastPos;
    private Sprite[] attackPlaying;
    private bool attackLoops;

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

    /// <summary>Play the assigned attackFrames once, then return to idle/run.</summary>
    public void PlayAttack() => PlayAttack(attackFrames, false);

    /// <summary>Play the assigned attackFrames on a loop until StopAttack() is called.</summary>
    public void PlayAttackLooping() => PlayAttack(attackFrames, true);

    /// <summary>Play a specific frame set as an attack (one-shot or looping).</summary>
    public void PlayAttack(Sprite[] frames, bool loop = false)
    {
        if (frames == null || frames.Length == 0) return;
        if (state == State.Dead) return; // death wins
        attackPlaying = frames;
        attackLoops = loop;
        state = State.Attack;
        frame = 0;
        frameTimer = 0f;
    }

    /// <summary>End a looping attack and fall back to idle/run.</summary>
    public void StopAttack()
    {
        if (state == State.Attack)
        {
            state = State.Idle;
            frame = 0;
            frameTimer = 0f;
        }
    }

    void LateUpdate()
    {
        if (sr == null) return;

        // Movement only decides Idle vs Run. Death and Attack hold until they finish.
        if (state == State.Idle || state == State.Run)
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
            {
                frame = Mathf.Min(frame, frames.Length - 1); // hold last frame
            }
            else if (state == State.Attack)
            {
                if (frame >= frames.Length)
                {
                    if (attackLoops)
                    {
                        frame = 0;
                    }
                    else
                    {
                        state = State.Idle; // one-shot done
                        frame = 0;
                        frames = CurrentFrames();
                        if (frames == null || frames.Length == 0) return;
                    }
                }
            }
            else
            {
                frame %= frames.Length; // idle/run loop
            }
        }

        frame = Mathf.Clamp(frame, 0, frames.Length - 1);
        sr.sprite = frames[frame];
    }

    private Sprite[] CurrentFrames()
    {
        switch (state)
        {
            case State.Run:    return (runFrames != null && runFrames.Length > 0) ? runFrames : idleFrames;
            case State.Dead:   return deathFrames;
            case State.Attack: return attackPlaying;
            default:           return idleFrames;
        }
    }
}
