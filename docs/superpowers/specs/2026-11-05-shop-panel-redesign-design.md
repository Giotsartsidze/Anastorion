# Shop Panel Redesign — Design

Date: 2026-11-05
Scene: `Assets/Scenes/MainMenu.unity` → `ShopPanel`

## Goal

Make the shop feel more **premium** without changing the existing gold-on-navy art
language. Replace the current 3-column flat grid of 9 upgrades with a **2-tab**
organized store and show upgrade progress as **gold level pips** instead of
`Lvl x/y` text.

## Decisions (from brainstorming)

- Style: keep existing gold-on-navy premium palette and 9-slice sprites.
- Layout: **2 tabs** — `Survival` / `Power`.
- Row detail: name + **gold level pips** + cost + BUY (no new per-upgrade icons).

### Tab groupings

- **Survival (4):** Max Health, Armor, Vitality, Revive
- **Power (5):** Power, Wisdom, Greed, Move Speed, Magnet Range

## Current hierarchy (relevant part)

```
ShopPanel            Image + VerticalLayoutGroup + ShopManager
  MainTitle
  currencyslot        ShardBalanceText, ShardIcon
  Box                 (3-column layout)
    vertical          UpgradeButton_MaxHealth, _Armor, _Greed
    Vertical1         UpgradeButton_Power, _Vitality, _Wisdom
    Vertical2         UpgradeButton_Revive, _MoveSpeed, _MagnetRange
  Close               Text (TMP)

UpgradeButton_* = RectTransform + Image + Button + ShopItemUI
  NameText, LevelText, CostText, costicon, Description
```

## Target hierarchy

```
ShopPanel
  MainTitle
  currencyslot
  TabBar                 (new, HorizontalLayoutGroup)
    TabButton_Survival   Image + Button + Text child
    TabButton_Power      Image + Button + Text child
  Box
    SurvivalGroup        (was "vertical")  MaxHealth, Armor, Vitality, Revive
    PowerGroup           (was "Vertical1") Power, Wisdom, Greed, MoveSpeed, MagnetRange
    (Vertical2 emptied + deactivated)
  Close

UpgradeButton_*
  NameText, CostText, costicon, Description
  LevelText              (deactivated — replaced by pips)
  PipBar                 (new) Image, type=Filled Horizontal, sprite=shop_pips
```

## Changes

### 1. Sprites (`Assets/Sprites/ui/_source/shop_gen.py`)
Add a **`shop_pips.png`**: a 5-cell horizontal bar, each cell a gold pip on the
dark/navy track with a thin gap, equal 20% cells so a `Filled` image at
`fillAmount = level/maxLevel` reveals exactly N filled pips. Import as Sprite,
single, no 9-slice. Optionally a `shop_tab.png` (or reuse `shop_button.png`).

### 2. Script: `ShopItemUI.cs`
- Add `public Image levelPips;` (the Filled pip bar). Keep `levelText` but make
  its use optional/null-safe (it will be deactivated in-scene).
- In `UpdateUI()`, set `levelPips.fillAmount = (float)level / data.maxLevel;`
  when `levelPips != null`. Keep existing cost/MAX/interactable logic.

### 3. New script: `ShopTabController.cs`
- Fields: `GameObject survivalGroup, powerGroup;` `Button survivalTab, powerTab;`
  and tab highlight colors (active gold / inactive navy).
- `ShowSurvival()` / `ShowPower()`: toggle the two group GameObjects active and
  recolor the two tab buttons. Default to Survival in `OnEnable`.
- Attached to `ShopPanel` (alongside `ShopManager`).

### 4. Scene (`MainMenu.unity`) — scripted transform (Python)
- Re-parent so `SurvivalGroup` = {MaxHealth, Armor, Vitality, Revive} and
  `PowerGroup` = {Power, Wisdom, Greed, MoveSpeed, MagnetRange}; empty + deactivate
  the third column.
- Add `TabBar` with two tab buttons (gold-on-navy, Button components).
- Per upgrade row: add a `PipBar` child Image (Filled) and deactivate `LevelText`.
- Add `ShopTabController` to `ShopPanel`, wire group + button references.
- All new objects get fresh unique fileIDs; every reference must resolve.

## Risks / verification

- Hand/script editing Unity scene YAML is delicate. Mitigation: do it with a
  Python transform using fresh fileIDs, then **validate**: scene still parses and
  every `{fileID: N}` reference resolves to an existing object (no dangling refs).
  Final visual confirmation happens when the user opens the scene in Unity.

## Out of scope

- Per-upgrade icon art.
- Changes to upgrade data, costs, or `ShopManager` logic (only reads items).
