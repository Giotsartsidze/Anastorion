"""Boss health bar: ornate gold frame + crimson fill (+ dark inner), 9-sliceable."""
from PIL import Image
import os
os.makedirs('out', exist_ok=True)
OUT=(14,12,20)
GOLD_D=(150,96,26); GOLD=(224,168,52); GOLD_L=(255,226,138)
RED_D=(110,18,24); RED=(188,38,42); RED_L=(238,96,78); RED_SHEEN=(255,180,150)
DARK=(26,18,24)

def save(g,w,h,name):
    im=Image.new('RGBA',(w,h),(0,0,0,0))
    for y in range(h):
        for x in range(w):
            if g[y][x]: im.putpixel((x,y),g[y][x]+(255,))
    im.save('out/'+name)
    return im

# ---- FRAME 9-slice: 48x20, border 7, transparent center ----
W,H=48,20; g=[[None]*W for _ in range(H)]
def rect(x0,y0,x1,y1,c):
    for y in range(y0,y1+1):
        for x in range(x0,x1+1):
            if 0<=x<W and 0<=y<H: g[y][x]=c
# outer dark outline
rect(0,0,W-1,H-1,OUT)
# gold bevel band (thickness ~5)
rect(1,1,W-2,H-2,GOLD_D)
rect(2,2,W-3,H-3,GOLD)
rect(2,2,W-3,2,GOLD_L); rect(2,2,2,H-3,GOLD_L)      # top/left highlight
rect(W-3,3,W-3,H-3,GOLD_D); rect(3,H-3,W-3,H-3,GOLD_D) # bottom/right shade
rect(3,3,W-4,H-4,GOLD_D)
# inner dark lip
rect(5,5,W-6,H-6,OUT)
# hollow center
for y in range(6,H-6):
    for x in range(6,W-6): g[y][x]=None
# little corner studs
for (cx,cy) in ((3,3),(W-4,3),(3,H-4),(W-4,H-4)): g[cy][cx]=GOLD_L
save(g,W,H,'boss_bar_frame.png')   # 9-slice border = 7

# ---- FILL 9-slice: 16x12, border 4 ----
W,H=16,12; g=[[None]*W for _ in range(H)]
for y in range(H):
    t=y/(H-1)
    if y==1: col=RED_SHEEN
    elif y==0: col=RED_L
    elif t<0.5: col=RED
    else: col=RED_D
    for x in range(W): g[y][x]=col
# vertical edge shading
for y in range(H): g[y][0]=RED_D; g[y][W-1]=RED_D
save(g,W,H,'boss_bar_fill.png')    # 9-slice border = 4

# ---- BG inner: 8x8 near-black ----
W,H=8,8; g=[[DARK]*W for _ in range(H)]
save(g,W,H,'boss_bar_bg.png')

# ---- preview: assemble a bar at ~70% ----
fw,fh=360,40
frame=Image.open('out/boss_bar_frame.png').resize((fw,fh),Image.NEAREST)
bg=Image.open('out/boss_bar_bg.png').resize((fw-28,fh-28),Image.NEAREST)
fillw=int((fw-28)*0.7)
fill=Image.open('out/boss_bar_fill.png').resize((max(1,fillw),fh-28),Image.NEAREST)
canvas=Image.new('RGBA',(fw,fh+10),(24,20,28,255))
canvas.alpha_composite(bg,(14,14))
canvas.alpha_composite(fill,(14,14))
canvas.alpha_composite(frame,(0,5))
canvas.save('bossbar_preview.png')
print('ok')
