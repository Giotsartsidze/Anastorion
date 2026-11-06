# Dream Framing (storybook cutscenes + bedroom menu) — Design

Date: 2026-11-06
Area: MainMenu + SampleScene flow, new cutscene system, new art

## Concept

"Survive the dream" told literally: a boy reads a book of Georgian myth before bed,
falls asleep, and **dreams the game**. Dying = **waking up** — thrilled, not defeated —
and every night he re-reads the same book to return to the same dream. The roguelite's
Play → die → Play loop *is* the story.

## Decisions (from brainstorming)

- **Scope:** full framing — bedroom menu + intro cutscene + death/wake outro.
- **Cutscene style:** **storybook** — book spreads, left page illustration, right page narration.
- **Art:** none exists; **generate procedural pixel art** (atmospheric gold-on-navy, silhouettes/
  starfields), as placeholders to be refined later.
- **Intro:** 4 page-turns, **auto-advance**, **skippable** (Esc / on-screen Skip). Then → game.
- **Bedroom menu:** **diegetic** — the room is the menu. Glowing book = **Play** ("read & sleep").
  Side chips: **Nightmares** (run modifiers), **Keepsakes** (shop), **Settings**.
- **Relabels:** Modifiers → Nightmares, Shop → Keepsakes (kept).
- **Outro:** death → fade to black → 2 wake pages (boy wakes thrilled; **run shards folded into
  the wake page**) → returns to the bedroom menu.
- Narration tone: Georgian myth (Amirani, the devs). Placeholder name "Giorgi". Wording is editable.

## Current flow (to change)

- Play: `MainMenuManager.PlayGame()` → `SceneManager.LoadScene("SampleScene")`.
- Death: `GameOverManager.ShowGameOver()` panel → Restart → `LoadScene("MainMenu")`
  (also `PlayerHealth.cs:174` reloads the active scene — to reconcile).

## Architecture

### 1. `StorybookCutscene` (new, reusable MonoBehaviour)
The engine for both intro and outro. Self-contained; builds/owns an overlay Canvas with a
book: left **illustration** `Image`, right **narration** `TMP_Text`, a page-turn/cross-fade,
and a **Skip** control.
- Data: `CutscenePage[] pages`, each `{ Sprite illustration; [TextArea] string narration; float duration; }`.
- Plays pages in order, **auto-advancing** after each `duration` (unscaled time), with a
  page-turn transition; **Esc or Skip** jumps to the end immediately.
- `Play(System.Action onComplete)` — runs the sequence, then invokes the callback.
- Optional `string extraLine` (used by the outro to inject "◆ Shards gathered: N").

### 2. `CutscenePage` (serializable data)
Plain serializable class (above). Intro and outro are each a `CutscenePage[]` assigned in the
Inspector on their scene's cutscene object. (Kept as inline arrays, not ScriptableObjects —
only two sequences, no reuse pressure.)

### 3. Flow glue
- **Intro:** the book (Play) calls a new `MainMenuManager` path that, instead of loading
  directly, runs the intro `StorybookCutscene`; on complete → `LoadScene("SampleScene")`.
  A short "fade to sleep" precedes the pages.
- **Outro:** on death, a `StorybookCutscene` in SampleScene plays the wake sequence, fed the
  run's shard total (from `CurrencyManager`/`RunStats`) for the folded reward line; on complete
  → `LoadScene("MainMenu")`. This replaces the straight-to-menu/game-over jump; the existing
  results/shop lives in the bedroom (Keepsakes) rather than a separate panel.

### 4. Bedroom menu reframe (MainMenu scene)
Rebuild the menu UI as the boy's room: background (window + stars + moon + bed + desk), a
**glowing book** as the central Play control, and three side chips wired to existing
`MainMenuManager` methods:
- Book → `PlayGame()` (now routed through the intro cutscene)
- Nightmares → `OpenModifiers()`
- Keepsakes → `OpenShop()`
- Settings → existing settings open
Reuse the opaque gold-on-navy panels already built for the sub-screens.

## Art to generate (procedural pixel art, placeholders)

- **Intro pages:** (1) desk + candle + open book; (2) blurring letters / dying ember;
  (3) boy asleep falling into stars; (4a) hero wreathed in light vs devs; (4b) "survive the
  dream" title card.
- **Outro pages:** (o1) dark closes over the hero; (o2) boy bolt upright, moonlight;
  (o3) pulls the book close, candle relit.
- **Bedroom menu:** room background, glowing book (Play), boy-at-desk silhouette (optional).

All gold-on-navy, generated like the existing UI sprites, with proper `.meta`s.

## Build sequence (full framing is large — ship in stages)

1. **Cutscene engine + intro:** `StorybookCutscene`, intro pages/art, route Play → intro → game, skip.
2. **Death/wake outro:** reuse the engine, generate wake art, fold shards, route death → outro → menu.
3. **Bedroom menu reframe:** room art + rewire MainMenu to the diegetic layout + relabels.

Each stage is independently testable and leaves the game playable.

## Data flow

Book(Play) → fade → intro StorybookCutscene.Play → onComplete → LoadScene(SampleScene).
Death → outro StorybookCutscene.Play(shards) → onComplete → LoadScene(MainMenu / bedroom).

## Isolation

`StorybookCutscene` knows only its pages + a completion callback — no scene/shop/menu
coupling. Flow glue (one method each in MainMenuManager and the death path) wires it to scene
loads. Art is data assigned in the Inspector. Each piece is swappable.

## Testing / verification

Manual: Play → intro auto-advances and is Esc-skippable → game loads. Die → wake pages show the
run's shard count → bedroom menu loads. Bedroom chips open Nightmares/Keepsakes/Settings.
Verify scene YAML parses and references resolve after each edit (as with prior scene work).

## Out of scope (this spec)

- Hand-drawn / final art (placeholders now; refine later).
- Voiceover / music changes.
- Localizing narration (English placeholder text).
- Per-run branching narration (same pages every time).
