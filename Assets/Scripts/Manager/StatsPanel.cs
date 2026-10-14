using UnityEngine;
using TMPro;

/// <summary>
/// Toggleable panel (default: Tab) that shows the player's current stats and the
/// synergy/evolution recipes with progress. Pauses the game while open.
/// Build a panel with two TMP_Text fields, drag them in, and put this anywhere.
/// </summary>
public class StatsPanel : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text statsText;     // left column: live stats
    public TMP_Text synergyText;   // right column: evolution recipes + progress
    public KeyCode toggleKey = KeyCode.Tab;

    private bool open;

    void Start()
    {
        if (panel != null) panel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey)) Toggle();
    }

    public void Toggle()
    {
        open = !open;
        if (panel != null) panel.SetActive(open);

        if (open)
        {
            Refresh();
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    void Refresh()
    {
        if (statsText != null) statsText.text = BuildStats();
        if (synergyText != null)
        {
            var um = FindObjectOfType<UpgradeManager>();
            synergyText.text = um != null ? um.GetSynergyReport() : "";
        }
    }

    string BuildStats()
    {
        var sb = new System.Text.StringBuilder();
        var xp = FindObjectOfType<ExperienceManager>();
        var hp = FindObjectOfType<PlayerHealth>();
        var mv = FindObjectOfType<PlayerMovement>();
        var dash = FindObjectOfType<PlayerDash>();
        var darts = FindObjectOfType<StellarDarts>();
        var wisp = FindObjectOfType<OrbitingWisps>();
        var pulse = FindObjectOfType<LightPulse>();
        var fire = FindObjectOfType<FireTrail>();
        var star = FindObjectOfType<Starfall>();
        var smite = FindObjectOfType<HeavenSmite>();
        var chain = FindObjectOfType<ChainLash>();

        if (xp != null)   sb.AppendLine($"Level: {xp.currentLevel}");
        if (hp != null)   sb.AppendLine($"Health: {hp.maxHealth}");
        if (mv != null)   sb.AppendLine($"Move Speed: {mv.maxSpeed:0.#}");
        if (dash != null) sb.AppendLine($"Dash: {dash.dashSpeed:0.#} (CD {dash.dashCooldown:0.#}s)");
        if (darts != null)sb.AppendLine($"Darts: x{darts.dartCount} (rate {darts.fireRate:0.##}s)");
        if (wisp != null) sb.AppendLine($"Wisps: {wisp.count}");
        if (pulse != null)sb.AppendLine($"Pulse: {(pulse.active ? "ON" : "locked")} (r {pulse.maxRadius:0.#})");

        sb.AppendLine();
        sb.AppendLine("<b>Active skills</b>");
        if (fire != null && fire.active)   sb.AppendLine("• Ember Path");
        if (star != null && star.active)   sb.AppendLine("• Starfall");
        if (smite != null && smite.active) sb.AppendLine("• Heaven's Smite");
        if (chain != null && chain.active) sb.AppendLine("• Chain Lash");

        return sb.ToString();
    }
}
