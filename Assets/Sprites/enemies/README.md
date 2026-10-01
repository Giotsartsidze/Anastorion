# Anastorion — enemies

All strips: horizontal, transparent PNG, 1 row. Enemies 48x48/frame, boss 96x96/frame.
Palette/pixel scale matches Amirani (dark outline, cyan/gold rim-light).

| Folder | Strips (frames) |
|---|---|
| devi_grunt | idle 4, walk 6, death 6 |
| ali | idle 4, walk 6 (crouch→leap→land), death 6 |
| kaji | idle 4, walk 6, death 6 (alpha flicker baked in) |
| paskunji | idle 4, walk 6 (flap cycle), death 6 |
| veshapi_hatchling | idle 4, walk 6 (undulating swim), death 6 — faces right |
| armored_devi | idle 4, walk 6, death 6 + armored_devi_shield.png (32x32) + shield_glow strip (4) |
| chinka | idle 4, walk 6 (= run/scamper), death 6 (drops sack, gold burst) |
| kudiani | idle 4, walk 6, attack 5, death 6 |
| cinder_whelp | idle 4, walk 6, attack 4 (swell→flash), death 6 |
| tkash_mapa | idle 4, walk 6, summon 5, death 6 |
| rokapi | idle 4, walk 6, channel 5 (loop), death 6 |
| kva_devi | idle 4, walk 6, cast 5, death 6 (crumble) |
| ochokochi | idle 4, walk 6, windup 4, death 6 |
| gveleshapi_boss | idle 4, move 6, attack_bite 6, attack_nova 6, death 8 (96x96) |

Unity import: Filter Point, Compression None, Sprite Mode Multiple, Slice Grid By Cell Size (48x48 or 96x96), Pivot Bottom Center.
PPU 48 for everything → boss automatically renders 2x size. Want it even bigger → PPU 32 on boss only.

_previews/: GIF of every animation + _contact_sheet.png. _source/: Python generators (python build.py).
