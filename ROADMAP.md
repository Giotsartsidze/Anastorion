# Anastorion → Steam Demo Roadmap

**Goal:** ship a tight, polished, ~10–15 minute demo that ends on a cliffhanger and drives Steam wishlists. NOT the full game — a vertical slice that proves the game is *fun and good-looking*.

**Guiding rule:** you already have enough *systems*. From here, almost every task is **polish, feel, art, and cutting** — not new features. The trap is building a 20th enemy instead of making the existing ones feel great. If a task isn't making the demo more fun or more beautiful, it waits until after launch.

---

## Phase 0 — Decide the demo's shape (1–2 days)
- [ ] Define the demo loop: e.g. survive 10 minutes → boss → win screen → "Wishlist to play the full game" call-to-action.
- [ ] Pick the **5–6 best enemies** for the demo. Disable the rest. Quality of encounters > quantity.
- [ ] Pick the **3–4 best skills + the 2 synergies** (Supernova, Chain Lightning). Everything else is post-demo.
- [ ] Write one sentence: *"Anastorion is a survivors-like where you ___, and it's special because ___."* This becomes your store pitch.

## Phase 1 — Clean the foundation (2–4 days)
- [ ] **Kill the dead code:** finish the data-driven `EvolutionManager` (it currently only `Debug.Log`s) OR delete it and keep the hardcoded synergy path in `UpgradeManager`. Do not ship both — pick one.
- [ ] Replace `FindObjectOfType` calls (esp. inside `ApplyUpgrade`, runs every level-up) with cached references set in `Start`.
- [ ] Decide on movement feel: the current physics `AddForce` + velocity-clamp is floaty. Playtest it against direct-velocity movement in dense swarms. Tight control usually wins for this genre.
- [ ] Make sure a full run can be played start→finish with zero console errors.

## Phase 2 — Meta-progression / "one more run" loop (4–7 days)
*This is what turns a tech demo into a game people replay.*
- [ ] Port the **save/load system** pattern from My-Game-Rpg (`ISaveManager`, `SaveManager`, `FileDataHandler`) into Anastorion.
- [ ] Port the **meta-currency + persistent unlock** pattern (`RunManager` gold→meta conversion, `MetaShopManager`) — your `CurrencyManager`/shards/shop are already the seed of this.
- [ ] Build a simple between-runs upgrade screen: spend earned currency on permanent boosts (max HP, starting damage, unlock a skill). 5–8 upgrades is plenty for a demo.
- [ ] This gives players a reason to replay even a short demo — critical for wishlist conversion.

## Phase 3 — Game feel / juice (1–2 weeks) ← biggest ROI
*Same content, 10x better to play. This is where wishlists are won.*
- [ ] Hit feedback: enemy flash-on-hit, brief hit-stop/freeze-frame on big hits, knockback.
- [ ] Screen shake tuned (you have `CameraShake`) — subtle on small hits, big on Supernova/boss death.
- [ ] Particles: muzzle/impact/death puffs, XP-orb pickup pop, level-up burst.
- [ ] Damage numbers floating up (port `PopUpTextFx` from the RPG project — it already exists there).
- [ ] Juicy level-up: pause, flash, satisfying sound, card animation (you have `AnimateIn`).
- [ ] Enemy death: dissolve/pop instead of just disappearing.
- [ ] **Sound is half of feel:** hit sounds, skill sounds, level-up jingle, boss music, ambient. Use free packs (Kenney, freesound) to start.

## Phase 4 — Art & cohesion (2–4 weeks, parallelizable)
*Weakest area today ("dumb sprites"). A clean, consistent style beats detailed-but-mismatched.*
- [ ] Lock ONE art style (e.g. clean neon-on-dark cosmic — fits the light-vs-shadow theme). Consistency > fidelity.
- [ ] Player sprite + animation (idle, move) that reads instantly.
- [ ] The 5–6 demo enemies: readable silhouettes, distinct colors, death anims.
- [ ] Skill VFX that look like the theme (light/stars/supernova).
- [ ] UI pass: health/XP bars, level-up cards, timer, main menu — consistent font & palette.
- [ ] A background/arena that looks intentional, not a flat color.
- [ ] If you can't art it yourself: budget for an asset pack or a contract artist for the demo's small set.

## Phase 5 — Steam setup (do this EARLY, in parallel — not last) (3–5 days)
- [ ] Pay Steam Direct ($100), set up Steamworks, tax form (W-8BEN — Georgia–US treaty reduces withholding).
- [ ] **Build the store page and make it live ASAP** — wishlists only accumulate while the page exists. Months of wishlists > a perfect page launched late.
- [ ] Assets needed: capsule images (the thumbnail is your #1 marketing asset — invest in it), 5+ screenshots, a 30–60s trailer, short + long description, tags, genre.
- [ ] Add a "Demo" depot. Plan the demo build around **Steam Next Fest** (huge free visibility — pick the next one and treat it as your deadline).

## Phase 6 — Polish, test, ship the demo (1 week)
- [ ] Playtest with 5–10 strangers (not friends). Watch them without helping. Fix what confuses them.
- [ ] Balance the difficulty curve so the 10-min run feels escalating but winnable.
- [ ] Settings: resolution, fullscreen, volume sliders, key rebinding (port `UI_Options`/`UI_VolumeSlider` from the RPG).
- [ ] Build for Windows (primary), test on a clean machine.
- [ ] End screen with a clear **"Wishlist on Steam"** button + link.

---

## What to explicitly NOT do for the demo
- ❌ New enemies / new skills beyond the chosen set.
- ❌ Multiple levels/biomes — one great arena is enough.
- ❌ Story/cutscenes/dialogue.
- ❌ Multiplayer, controller-perfect support, achievements, localization (all post-launch).
- ❌ Rewriting working systems "to be cleaner." Clean only what blocks the demo.

## Rough timeline
A focused solo dev: **~2–3 months** to a Next Fest-ready demo, if you hold the scope above. The schedule dies if scope creeps — guard it ruthlessly.

## Suggested order to actually work in
1. Phase 1 (clean) + start Phase 5 (Steam page live, even bare) in parallel.
2. Phase 3 (juice) — fastest visible improvement, keeps motivation high.
3. Phase 2 (meta loop).
4. Phase 4 (art) — ongoing throughout.
5. Phase 6 (test + ship) into the next Steam Next Fest.
