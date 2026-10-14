from PIL import Image
import math

S = 32
OUT = (14, 12, 22)
GOLD_D = (168, 106, 30); GOLD = (236, 176, 56); GOLD_L = (255, 228, 140)
GEM_D = (92, 44, 150); GEM = (150, 86, 230); GEM_L = (206, 160, 255)

g = [[None] * S for _ in range(S)]
def px(x, y, c):
    if 0 <= x < S and 0 <= y < S and c: g[y][x] = c

cx = cy = 15.5
# gold medallion ring
for y in range(S):
    for x in range(S):
        d = math.hypot(x - cx, y - cy)
        if 9.5 <= d <= 12.5:
            px(x, y, GOLD_L if (x + y) % 5 == 0 else (GOLD if d < 11.3 else GOLD_D))
# little studs around the ring
for a in range(0, 360, 45):
    rx = int(round(cx + math.cos(math.radians(a)) * 11))
    ry = int(round(cy + math.sin(math.radians(a)) * 11))
    px(rx, ry, GOLD_L)
# central gem (diamond)
for y in range(S):
    for x in range(S):
        dx = abs(x - cx); dy = abs(y - cy)
        if dx + dy <= 7:
            if (x - cx) + (y - cy) < -2: c = GEM_L
            elif dx + dy > 5: c = GEM_D
            else: c = GEM
            px(x, y, c)
px(13, 12, GEM_L); px(14, 11, (255, 255, 255))  # sparkle

# outline (dark) around filled, excluding glow we add after
add = []
for y in range(S):
    for x in range(S):
        if g[y][x] is None:
            for a, b in ((1,0),(-1,0),(0,1),(0,-1),(1,1),(-1,-1),(1,-1),(-1,1)):
                nx, ny = x+a, y+b
                if 0 <= nx < S and 0 <= ny < S and g[ny][nx] is not None:
                    add.append((x, y)); break
for x, y in add: px(x, y, OUT)

im = Image.new('RGBA', (S, S), (0, 0, 0, 0))
# soft violet glow behind everything
for y in range(S):
    for x in range(S):
        d = math.hypot(x - cx, y - cy)
        if d <= 15:
            a = max(0, 1 - d / 15) ** 2
            im.putpixel((x, y), (150, 90, 230, int(90 * a)))
for y in range(S):
    for x in range(S):
        if g[y][x]:
            im.putpixel((x, y), g[y][x] + (255,))

im.save('out/relic.png')
im.resize((S * 8, S * 8), Image.NEAREST).save('relic_preview.png')
print('ok')
