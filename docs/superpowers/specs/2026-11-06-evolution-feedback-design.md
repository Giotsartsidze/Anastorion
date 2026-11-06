# Evolution Feedback — Design

Date: 2026-11-06
Area: in-run synergy/evolution system (gameplay, `SampleScene`)

## Goal

The game already has **6 working evolutions** in `UpgradeManager` (Supernova, Chain
Lightning, Comet, Meteor Storm, Thunderstorm, Titan's Wrath), each with real payoffs.
The problem: when one triggers, **nothing tells the player** — the skill silently gets
stronger. This feature adds a satisfying "it happened!" moment, plus cleanup of dead
duplicate code.

Discoverability is already covered: `StatsPanel` (Tab) shows recipes + live progress via
`UpgradeManager.GetSynergyReport()`. No new discoverability UI is needed.

## Decisions (from brainstorming)

- Direction: **feedback & discoverability** (make existing evolutions land), not new content or a refactor.
- Moment intensity: **banner + sound + player burst** (satisfying, not disruptive).
- Banner style: **framed pill** — gold-on-navy (matches shop/settings redesign) with an
  expanding gold ring burst behind the player. Wording: "⚡ EVOLVED" + evolution name.

## The 6 evolution trigger points (already in code)

In `UpgradeManager.CheckSynergies()` unless noted:
- Supernova — `wispLevel>=3 && radiusLevel>=2` (sets `isSupernovaUnlocked`)
- Chain Lightning — `dashLevel>=2 && speedLevel>=2` (sets `isLightningUnlocked`)
- Comet — `fireTrailLevel>=3 && speedLevel>=3` (`fireTrail.Evolve()`)
- Meteor Storm — `starfallLevel>=3 && radiusLevel>=3` (`starfall.Evolve()`)
- Thunderstorm — `smiteLevel>=3 && dashLevel>=3` (`heavenSmite.Evolve()`)
- Titan's Wrath — `chainLashLevel>=5` in `ApplyUpgrade()` (`chainLash.Evolve()`)

## Components

### 1. `EvolutionAnnouncer` (new MonoBehaviour, singleton)
Self-contained owner of the moment. Public API: `Announce(string evolutionName)`.
- **Banner UI**: a framed pill (navy fill + gold border, matching existing sprites) with
  a small gold "⚡ EVOLVED" label and a large name. Center-screen.
- **Animation**: scale-in 0.8→1.04→1, hold ~1.8s, fade out. **Uses `Time.unscaledDeltaTime`**
  because evolutions fire in `ApplyUpgrade()` while the level-up is still paused and
  `ResumeGame()` runs immediately after.
- **Sound**: plays `level_up_chime.wav` from its own `AudioSource`.
- **Burst**: triggers the player ring burst (below).
- **Queue**: a `Queue<string>`; if two evolutions pop from one pick, banners play
  sequentially rather than overlapping.
- Lives on the SampleScene canvas; `Instance` set in `Awake`.

### 2. Player ring burst (VFX)
A gold ring that scales up and fades at the player's position.
- Spawned in code (no prefab wiring): a `GameObject` with a `SpriteRenderer` using a small
  generated ring sprite, animated via a short coroutine (scale 0.4→1.6, alpha 0.9→0),
  auto-destroyed. Plus a light `CameraShake` kick if a shaker is present.
- New sprite asset: `Assets/Sprites/ui/evolve_ring.png` (gold ring, generated in the
  existing pixel-art style).

### 3. `UpgradeManager` hooks
One line at each of the 6 unlock points:
`EvolutionAnnouncer.Instance?.Announce("Comet");` etc. No other change to upgrade logic.

### 4. Cleanup
Delete `Assets/Scripts/EvolutionManager.cs` + `Assets/Scripts/SynergyData.cs` (+ `.meta`s).
Confirmed: nothing references them (not in any scene, `TrackUpgrade` never called).

### 5. Minor fix
`GetSynergyReport()` labels two recipes "Dash CD", but `dashLevel` is incremented on
**Dash Speed** picks (`ApplyUpgrade` `DashSpeed` case). Relabel to "Dash Speed" so the Tab
panel matches what the player actually upgrades.

### 6. Scene work
Add the banner UI object (pill image + label text + name text) to the `SampleScene` canvas,
inactive by default, with the `EvolutionAnnouncer` wired to its parts.

## Data flow

player picks upgrade → `UpgradeManager.ApplyUpgrade` → `CheckSynergies` crosses a threshold
→ sets flag / calls `skill.Evolve()` → calls `EvolutionAnnouncer.Instance.Announce(name)`
→ announcer enqueues → shows banner (unscaled), plays chime, spawns player ring burst →
`ResumeGame()` unpauses → banner finishes over live gameplay.

## Isolation

`EvolutionAnnouncer` owns UI + sound + VFX + queue; it knows nothing about upgrades.
`UpgradeManager` knows only `Announce(string)`. The ring burst is a private helper of the
announcer. Each unit is independently understandable and swappable.

## Testing / verification

Manual (UI/juice): set `debugUnlockSynergies` off and reach a recipe in play, or add a
temporary debug key to call `Announce("Test")`. Confirm: banner scales/holds/fades on
unscaled time, chime plays, ring bursts on the player, queue handles two-at-once, and the
Tab panel shows the corrected "Dash Speed" label.

## Out of scope

- New evolution recipes / content.
- Data-driven `SynergyData` refactor.
- Changes to evolution balance or thresholds.
