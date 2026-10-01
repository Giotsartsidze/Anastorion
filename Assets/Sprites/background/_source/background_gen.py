"""Seamless, tileable cosmic background layers for Anastorion.
Palette matches the Amirani/enemy sprites (dark midnight + gold/cyan/starlight).
Outputs: nebula.png (opaque), stars_far.png, stars_near.png (transparent).
All tiles are seamless on every edge so they repeat infinitely under parallax.
"""
from PIL import Image
import math, random

SIZE = 384  # tile resolution (seamless)

BLACK = (8, 6, 18)
NAVY  = (20, 26, 56)
NAVY2 = (34, 44, 92)
GOLD  = (236, 176, 56)
CYAN  = (96, 222, 244)
WHITE = (246, 244, 230)

def lerp(a, b, t):
    return tuple(int(a[i] + (b[i] - a[i]) * t) for i in range(3))

def seamless_noise(size, octaves, seed):
    """Tileable value noise from integer-frequency sine octaves (wraps perfectly)."""
    random.seed(seed)
    comps = []
    for o in range(octaves):
        f = 2 ** o
        comps.append((f, random.uniform(0, 2*math.pi), random.uniform(0, 2*math.pi), 1.0 / f))
    field = [[0.0]*size for _ in range(size)]
    for y in range(size):
        for x in range(size):
            v = 0.0
            for f, px, py, amp in comps:
                v += amp * math.sin(2*math.pi*f*x/size + px) * math.sin(2*math.pi*f*y/size + py)
            field[y][x] = v
    lo = min(min(r) for r in field); hi = max(max(r) for r in field)
    rng = (hi - lo) or 1e-9
    return [[(field[y][x]-lo)/rng for x in range(size)] for y in range(size)]

# ---- NEBULA (opaque, dark, subtle so gameplay still reads) ----
neb = Image.new('RGBA', (SIZE, SIZE), BLACK + (255,))
cloud = seamless_noise(SIZE, 5, seed=3)
wisp  = seamless_noise(SIZE, 4, seed=9)
pn = neb.load()
for y in range(SIZE):
    for x in range(SIZE):
        n = cloud[y][x]
        c = lerp(BLACK, NAVY, n * 0.8)
        c = lerp(c, NAVY2, max(0.0, n - 0.6) * 1.1)
        w = wisp[y][x]
        if w > 0.80:                       # faint colored gas, kept subtle
            tint = GOLD if (x + y) % 2 else CYAN
            c = lerp(c, tint, (w - 0.80) * 0.5)
        pn[x, y] = c + (255,)
neb.save('nebula.png')

# ---- STAR LAYERS (transparent, wrap-safe dots) ----
def star_layer(count, sizes, seed, fname, colors):
    random.seed(seed)
    im = Image.new('RGBA', (SIZE, SIZE), (0, 0, 0, 0))
    p = im.load()
    for _ in range(count):
        x, y = random.randrange(SIZE), random.randrange(SIZE)
        base = random.choice(colors)
        b = random.uniform(0.5, 1.0)
        col = tuple(int(ci * b) for ci in base)
        s = random.choice(sizes)
        for dx in range(-s, s+1):
            for dy in range(-s, s+1):
                if dx*dx + dy*dy <= s*s:
                    a = 255 if (dx == 0 and dy == 0) else 150
                    p[(x+dx) % SIZE, (y+dy) % SIZE] = col + (a,)
    im.save(fname)

star_layer(300, [0, 0, 0, 1], seed=1, fname='stars_far.png',  colors=[WHITE, CYAN, (180, 190, 210)])
star_layer(90,  [0, 1, 1, 2], seed=2, fname='stars_near.png', colors=[WHITE, GOLD, CYAN])
print('ok')
