"""Seamless top-down ancient rune-stone floor for Anastorion.
Dark flagstones + cracks + faint glowing Borjgali-style runes. Tiles seamlessly.
Kept dark/low-contrast so gameplay sprites still pop. Output: floor.png (opaque).
"""
from PIL import Image, ImageDraw
import math, random

SIZE = 512
STONE_D = (18, 20, 30)
STONE   = (34, 38, 52)
STONE_L = (50, 56, 76)
GROUT   = (10, 11, 18)
GOLD    = (236, 176, 56)
CYAN    = (96, 222, 244)

def lerp(a, b, t):
    return tuple(int(a[i] + (b[i] - a[i]) * t) for i in range(3))

def seamless_noise(size, octaves, seed):
    random.seed(seed)
    comps = [(2**o, random.uniform(0, 2*math.pi), random.uniform(0, 2*math.pi), 1.0/(2**o))
             for o in range(octaves)]
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

img = Image.new('RGBA', (SIZE, SIZE), STONE + (255,))
px = img.load()

# base mottled stone
n = seamless_noise(SIZE, 5, seed=5)
for y in range(SIZE):
    for x in range(SIZE):
        px[x, y] = lerp(STONE_D, STONE_L, n[y][x]) + (255,)

draw = ImageDraw.Draw(img)

# flagstone grid (aligned → seamless). 4x4 stones.
cell = SIZE // 4
for i in range(0, SIZE, cell):
    draw.line([(i, 0), (i, SIZE)], fill=GROUT + (255,), width=3)
    draw.line([(0, i), (SIZE, i)], fill=GROUT + (255,), width=3)
# soft bevel on the top/left inner edge of each stone
bevel = lerp(STONE, STONE_L, 0.6)
for gy in range(0, SIZE, cell):
    for gx in range(0, SIZE, cell):
        draw.line([(gx+3, gy+3), (gx+cell-4, gy+3)], fill=bevel + (255,), width=1)
        draw.line([(gx+3, gy+3), (gx+3, gy+cell-4)], fill=bevel + (255,), width=1)

def plot(x, y, col):
    px[x % SIZE, y % SIZE] = col + (255,)

# cracks (wrap at edges so they tile)
random.seed(11)
for _ in range(16):
    x = random.randrange(SIZE); y = random.randrange(SIZE)
    ang = random.uniform(0, 2*math.pi); length = random.randint(40, 110)
    for _ in range(length):
        ang += random.uniform(-0.35, 0.35)
        x += math.cos(ang); y += math.sin(ang)
        plot(int(x), int(y), STONE_D)
        if random.random() < 0.4:
            plot(int(x)+1, int(y), (6, 7, 12))

# faint glowing Borjgali-style runes (ring + radiating spokes)
def rune(cx, cy, col, r=12):
    # soft glow halo
    for gx in range(cx-r-6, cx+r+6):
        for gy in range(cy-r-6, cy+r+6):
            d = math.hypot(gx-cx, gy-cy)
            if d < r+6:
                base = px[gx % SIZE, gy % SIZE][:3]
                px[gx % SIZE, gy % SIZE] = lerp(base, col, max(0, 1 - d/(r+6)) * 0.22) + (255,)
    # ring
    for a in range(0, 360, 3):
        rad = math.radians(a)
        plot(int(cx + r*math.cos(rad)), int(cy + r*math.sin(rad)), col)
    # seven spokes (Borjgali)
    for k in range(7):
        a = math.radians(k * (360/7))
        for s in range(2, r-2):
            plot(int(cx + s*math.cos(a)), int(cy + s*math.sin(a)), lerp(col, STONE, 0.3))

random.seed(7)
for _ in range(3):
    cx = random.randint(50, SIZE-50); cy = random.randint(50, SIZE-50)
    rune(cx, cy, random.choice([GOLD, CYAN]))

img.save('floor.png')
print('ok')
