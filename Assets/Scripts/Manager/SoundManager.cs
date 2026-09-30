using UnityEngine;

/// <summary>
/// One-stop SFX player. Put ONE of these in the scene and drag the clips into the
/// Inspector slots. Anything can trigger a sound with one line, e.g.
///   SoundManager.Instance?.PlayEnemyHit();
///
/// Uses a small pool of AudioSources with slight random pitch so rapid-fire sounds
/// (enemy hits in a swarm) don't sound like one flat machine-gun sample, and throttles
/// the spammy ones so a bullet-heaven doesn't turn into noise.
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Clips")]
    public AudioClip enemyHit;
    public AudioClip enemyDeath;
    public AudioClip playerHurt;
    public AudioClip levelUp;
    public AudioClip shoot;

    [Header("Mix")]
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Tooltip("How many sounds can overlap at once.")]
    public int voices = 10;
    [Tooltip("Random pitch range per play for variety.")]
    public Vector2 pitchRange = new Vector2(0.94f, 1.06f);

    [Header("Throttle (min seconds between plays; 0 = none)")]
    public float enemyHitMinInterval = 0.04f;
    public float shootMinInterval = 0f;

    private AudioSource[] pool;
    private int next;
    private float nextHitTime;
    private float nextShootTime;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        pool = new AudioSource[Mathf.Max(1, voices)];
        for (int i = 0; i < pool.Length; i++)
        {
            AudioSource a = gameObject.AddComponent<AudioSource>();
            a.playOnAwake = false;
            a.spatialBlend = 0f; // 2D — full volume regardless of position
            pool[i] = a;
        }
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void Play(AudioClip clip, float volumeScale)
    {
        if (clip == null) return;
        AudioSource a = pool[next];
        next = (next + 1) % pool.Length;
        a.pitch = Random.Range(pitchRange.x, pitchRange.y);
        a.PlayOneShot(clip, sfxVolume * volumeScale);
    }

    public void PlayEnemyHit()
    {
        if (Time.unscaledTime < nextHitTime) return;
        nextHitTime = Time.unscaledTime + enemyHitMinInterval;
        Play(enemyHit, 0.7f);
    }

    public void PlayShoot()
    {
        if (Time.unscaledTime < nextShootTime) return;
        nextShootTime = Time.unscaledTime + shootMinInterval;
        Play(shoot, 0.5f);
    }

    public void PlayEnemyDeath() => Play(enemyDeath, 0.9f);
    public void PlayPlayerHurt() => Play(playerHurt, 1f);
    public void PlayLevelUp() => Play(levelUp, 1f);
}
