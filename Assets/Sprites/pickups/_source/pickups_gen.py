"""Glowing cosmic pickup sprites for Anastorion (replace the deleted 32rogues ones).
Bright with a soft glow halo so they read clearly on the dark rune-stone floor.
Outputs 24x24 transparent PNGs: xp_orb, coin, shard, healthpack.
"""
from PIL import Image, ImageDraw
import math

S = 24
GOLD   = (236, 176, 56);  GOLD_L = (255, 228, 140)
CYAN   = (96, 222, 244)
WHITE  = (246, 244, 230)
RED    = (232, 64, 64);   RED_L  = (255, 150, 150)
cx = cy = S // 2

def radial_glow(col, r, strength=170):
    im = Image.new('RGBA', (S, S), (0, 0, 0, 0))
    p = im.load()
    for y in range(S):
        for x in range(S):
            d = math.hypot(x - cx + 0.5, y - cy + 0.5)
            if d < r:
                p[x, y] = (col[0], col[1], col[2], int(strength * (1 - d/r) ** 2))
    return im

def disc(d, rad, col):     d.ellipse([cx-rad, cy-rad, cx+rad, cy+rad], fill=col + (255,))
def diamond(d, rad, col):  d.polygon([(cx, cy-rad), (cx+rad, cy), (cx, cy+rad), (cx-rad, cy)], fill=col + (255,))

# XP orb — cyan gem
im = radial_glow(CYAN, 11); d = ImageDraw.Draw(im)
diamond(d, 6, (40, 120, 160)); diamond(d, 5, CYAN)
d.point([(cx-1, cy-2), (cx, cy-3)], fill=WHITE + (255,))
im.save('xp_orb.png')

# coin — gold
im = radial_glow(GOLD, 11); d = ImageDraw.Draw(im)
disc(d, 6, (150, 100, 20)); disc(d, 5, GOLD)
d.ellipse([cx-3, cy-3, cx, cy], fill=GOLD_L + (255,))
im.save('coin.png')

# shard — gold crystal (meta currency)
im = radial_glow(GOLD_L, 11, 150); d = ImageDraw.Draw(im)
d.polygon([(cx, cy-7), (cx+4, cy-1), (cx+2, cy+7), (cx-2, cy+7), (cx-4, cy-1)], fill=(150, 100, 20, 255))
d.polygon([(cx, cy-6), (cx+3, cy-1), (cx+1, cy+6), (cx-1, cy+6), (cx-3, cy-1)], fill=GOLD + (255,))
d.line([(cx, cy-6), (cx, cy+6)], fill=GOLD_L + (255,))
im.save('shard.png')

# health — red cross
im = radial_glow(RED, 11); d = ImageDraw.Draw(im)
d.rectangle([cx-2, cy-5, cx+1, cy+5], fill=RED + (255,))
d.rectangle([cx-5, cy-2, cx+5, cy+1], fill=RED + (255,))
d.rectangle([cx-1, cy-4, cx, cy+3], fill=RED_L + (255,))
im.save('healthpack.png')
print('ok')
