"""Cosmic skill/projectile VFX for Anastorion (replace sprites broken by 32rogues removal).
Glowing light-vs-shadow style matching the hero/enemies. Transparent PNGs.
Outputs: stellar_dart, wisp, enemy_bolt, boss_fire, skill_pickup, supernova.
"""
from PIL import Image, ImageDraw
import math, random

GOLD   = (236, 176, 56);  GOLD_L = (255, 228, 140)
CYAN   = (96, 222, 244)
WHITE  = (246, 244, 230)
VIOLET = (150, 90, 220)
SHADOW = (20, 12, 34)

def glow(size, cx, cy, r, col, strength=170, power=2):
    im = Image.new('RGBA', (size, size), (0, 0, 0, 0))
    p = im.load()
    for y in range(size):
        for x in range(size):
            d = math.hypot(x - cx + 0.5, y - cy + 0.5)
            if d < r:
                p[x, y] = (col[0], col[1], col[2], int(strength * (1 - d/r) ** power))
    return im

# STELLAR DART — main star projectile, points right
S = 24; cy = S // 2
im = glow(S, S/2, S/2, 9, CYAN, 130); d = ImageDraw.Draw(im)
d.polygon([(4, cy), (14, cy-4), (20, cy), (14, cy+4)], fill=GOLD + (255,))
d.polygon([(7, cy), (14, cy-2), (19, cy), (14, cy+2)], fill=GOLD_L + (255,))
d.point([(19, cy), (18, cy)], fill=WHITE + (255,))
d.line([(2, cy), (6, cy)], fill=CYAN + (200,), width=1)   # tail spark
im.save('stellar_dart.png')

# WISP — small orbiting spirit
S = 16; c = S // 2
im = glow(S, S/2, S/2, 8, CYAN, 190); d = ImageDraw.Draw(im)
d.ellipse([c-3, c-3, c+2, c+2], fill=CYAN + (255,))
d.ellipse([c-2, c-2, c, c], fill=WHITE + (255,))
im.save('wisp.png')

# ENEMY BOLT — cursed witch-fire orb
S = 16; c = S // 2
im = glow(S, S/2, S/2, 8, VIOLET, 180); d = ImageDraw.Draw(im)
d.ellipse([c-3, c-3, c+2, c+2], fill=VIOLET + (255,))
d.ellipse([c-2, c-2, c, c], fill=CYAN + (255,))
im.save('enemy_bolt.png')

# BOSS FIRE — eclipsed shadow sphere with gold rim
S = 24; c = S // 2
im = glow(S, S/2, S/2, 11, GOLD, 120); d = ImageDraw.Draw(im)
d.ellipse([c-6, c-6, c+5, c+5], fill=GOLD + (255,))
d.ellipse([c-5, c-5, c+4, c+4], fill=SHADOW + (255,))
d.ellipse([c-2, c-3, c, c-1], fill=CYAN + (255,))
im.save('boss_fire.png')

# SKILL PICKUP — glowing rune
S = 24; c = S // 2
im = glow(S, S/2, S/2, 11, GOLD_L, 150); d = ImageDraw.Draw(im)
d.polygon([(c, c-7), (c+7, c), (c, c+7), (c-7, c)], fill=(60, 40, 80, 255))
d.polygon([(c, c-5), (c+5, c), (c, c+5), (c-5, c)], fill=GOLD + (255,))
d.line([(c, c-5), (c, c+5)], fill=WHITE + (255,))
d.line([(c-5, c), (c+5, c)], fill=WHITE + (255,))
im.save('skill_pickup.png')

# SUPERNOVA — radial burst (code can scale/fade it)
S = 64; c = S // 2
im = glow(S, c, c, 30, GOLD, 150); d = ImageDraw.Draw(im)
for a in range(0, 360, 30):
    rad = math.radians(a)
    d.line([(c, c), (c + 28*math.cos(rad), c + 28*math.sin(rad))], fill=GOLD_L + (210,), width=2)
for r, col in [(16, CYAN), (11, GOLD), (7, GOLD_L), (4, WHITE)]:
    d.ellipse([c-r, c-r, c+r, c+r], fill=col + (255,))
im.save('supernova.png')
print('ok')
