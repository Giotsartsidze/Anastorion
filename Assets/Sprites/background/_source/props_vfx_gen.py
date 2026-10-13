from PIL import Image
import math, os

OUT = (14, 12, 22)  # shared dark outline

# ---------------- pulse shockwave ring (64x64) ----------------
def pulse_ring(size=64):
    im = Image.new('RGBA', (size, size), (0, 0, 0, 0))
    cx = cy = (size - 1) / 2
    R = size * 0.46          # ring radius
    thick = size * 0.10      # ring half-thickness
    for y in range(size):
        for x in range(size):
            d = math.hypot(x - cx, y - cy)
            t = abs(d - R) / thick
            if t < 1.0:
                a = (1 - t) ** 1.6
                # white core -> cyan edge
                r = int(240 * a + 150 * (1 - a))
                g = int(250 * a + 220 * (1 - a))
                b = int(255)
                im.putpixel((x, y), (r, g, b, int(235 * a)))
    return im

# ---------------- pixel-prop helpers ----------------
class P:
    def __init__(s, w=32, h=32):
        s.w, s.h = w, h
        s.g = [[None] * w for _ in range(h)]
    def px(s, x, y, c):
        if 0 <= x < s.w and 0 <= y < s.h and c:
            s.g[y][x] = c
    def rect(s, x0, y0, x1, y1, c):
        for y in range(y0, y1 + 1):
            for x in range(x0, x1 + 1):
                s.px(x, y, c)
    def outline(s):
        add = []
        for y in range(s.h):
            for x in range(s.w):
                if s.g[y][x] is None:
                    for a, b in ((1,0),(-1,0),(0,1),(0,-1),(1,1),(-1,-1),(1,-1),(-1,1)):
                        nx, ny = x+a, y+b
                        if 0 <= nx < s.w and 0 <= ny < s.h and s.g[ny][nx] is not None and s.g[ny][nx] != OUT:
                            add.append((x, y)); break
        for x, y in add: s.g[y][x] = OUT
    def img(s):
        im = Image.new('RGBA', (s.w, s.h), (0, 0, 0, 0))
        for y in range(s.h):
            for x in range(s.w):
                if s.g[y][x]: im.putpixel((x, y), s.g[y][x] + (255,))
        return im

def blob(p, cx, cy, rx, ry, dark, mid, light):
    for y in range(p.h):
        for x in range(p.w):
            dx = (x - cx) / rx; dy = (y - cy) / ry
            d = dx*dx + dy*dy
            if d <= 1.0:
                if dx + dy < -0.5: c = light
                elif d > 0.55: c = dark
                else: c = mid
                p.px(x, y, c)

def rock():
    p = P()
    blob(p, 16, 22, 11, 7, (40,42,52), (62,64,76), (92,94,108))
    p.outline(); return p.img()

def boulders():
    p = P()
    blob(p, 11, 24, 7, 5, (38,40,50), (58,60,72), (88,90,104))
    blob(p, 21, 21, 8, 7, (44,46,56), (66,68,80), (96,98,112))
    p.outline(); return p.img()

def dead_tree():
    p = P()
    D,M,L = (36,26,20),(70,50,36),(98,72,50)
    p.rect(14,10,17,29, M)            # trunk
    p.rect(14,10,14,29, L); p.rect(17,10,17,29, D)
    # bare branches
    for (x,y,dx,dy,n) in ((14,14,-1,-1,5),(17,12,1,-1,6),(15,18,-1,-1,4),(16,16,1,-1,3)):
        cx,cy=x,y
        for i in range(n):
            cx+=dx; cy+=dy; p.px(cx,cy,M); p.px(cx,cy-1,L)
    p.outline(); return p.img()

def pillar():
    p = P()
    D,M,L = (48,50,60),(78,80,92),(112,114,128)
    p.rect(12,6,19,26, M)
    p.rect(12,6,13,26, L); p.rect(18,6,19,26, D)
    # broken top (jagged)
    for x,yy in ((12,6),(13,5),(16,7),(17,5),(19,6)): p.px(x,yy,OUT)
    p.px(14,6,None); p.px(15,5,None)
    # cracks
    for y in range(10,24,3): p.px(15,y,D); p.px(16,y+1,D)
    # rubble base
    p.rect(9,27,22,29, M); p.rect(9,27,22,27, L)
    p.outline(); return p.img()

def mushrooms():
    p = P()
    cap_d,cap,cap_l = (96,44,56),(150,70,84),(190,104,118)
    stem_d,stem = (120,112,96),(176,168,146)
    # big
    p.rect(13,20,16,28, stem); p.rect(13,20,13,28, stem)  # stem
    p.px(16,20,stem_d)
    blob(p, 15, 17, 7, 4, cap_d, cap, cap_l)
    for sx,sy in ((12,16),(17,18),(14,15)): p.px(sx,sy,cap_l)  # spots
    # small
    p.rect(20,23,22,28, stem)
    blob(p, 21, 21, 4, 3, cap_d, cap, cap_l)
    p.outline(); return p.img()

def grass():
    p = P()
    D,M,L = (40,60,40),(62,90,58),(92,124,80)
    blades = ((12,29,-1,8),(15,29,0,11),(18,29,1,9),(21,29,0,7),(9,29,1,6))
    for x,y,dx,n in blades:
        cx=x
        for i in range(n):
            col = L if i==n-1 else (M if i%2 else D)
            p.px(cx, y-i, col)
            cx += dx if (i% (3)==2) else 0
    p.outline(); return p.img()

def skull():
    p = P()
    D,M,L = (120,114,98),(186,180,160),(222,218,200)
    blob(p, 16, 16, 8, 8, D, M, L)
    p.rect(13,24,19,28, M)          # jaw
    p.px(13,18,OUT); p.rect(12,16,14,18, (20,18,28)); p.rect(18,16,20,18,(20,18,28)) # eye sockets
    p.rect(15,20,16,22, (20,18,28)) # nose
    for x in range(13,20,2): p.px(x,27,OUT)  # teeth
    p.outline(); return p.img()

def bones():
    p = P()
    D,M,L=(120,114,98),(190,184,164),(226,222,204)
    def bone(x0,y0,x1,y1):
        # shaft
        import math as _m
        steps=int(max(abs(x1-x0),abs(y1-y0)))+1
        for i in range(steps):
            x=round(x0+(x1-x0)*i/steps); y=round(y0+(y1-y0)*i/steps)
            p.px(x,y,M); p.px(x,y-1,L)
        for (ex,ey) in ((x0,y0),(x1,y1)):
            p.px(ex-1,ey,L); p.px(ex+1,ey,M); p.px(ex,ey-1,L); p.px(ex,ey+1,D)
    bone(9,14,24,20)
    bone(10,22,23,17)
    p.outline(); return p.img()

props = {
    'prop_rock': rock(), 'prop_boulders': boulders(), 'prop_dead_tree': dead_tree(),
    'prop_pillar': pillar(), 'prop_mushrooms': mushrooms(), 'prop_grass': grass(),
    'prop_skull': skull(), 'prop_bones': bones(),
}

os.makedirs('out', exist_ok=True)
pulse_ring().save('out/pulse_ring.png')
for name, im in props.items():
    im.save(f'out/{name}.png')

# contact sheet preview (dark bg, scaled)
cols = 4
rows = (len(props) + cols - 1) // cols
pad = 6; cell = 32
sheet = Image.new('RGBA', (cols*(cell+pad)+pad, rows*(cell+pad)+pad + 48), (22,24,34,255))
# pulse at top
pr = pulse_ring()
sheet.alpha_composite(pr.resize((48,48), Image.NEAREST), (pad, pad))
items = list(props.items())
for i,(n,im) in enumerate(items):
    r,c = divmod(i, cols)
    x = pad + c*(cell+pad); y = 56 + pad + r*(cell+pad)
    sheet.alpha_composite(im, (x, y))
sheet.resize((sheet.width*5, sheet.height*5), Image.NEAREST).save('preview_contact.png')
print('ok', list(props.keys()))
