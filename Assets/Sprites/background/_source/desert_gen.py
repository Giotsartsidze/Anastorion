"""Chain Lash icon (32), enemy projectile (32), mythical-desert props (32) + sand ground tile (128)."""
from PIL import Image, ImageDraw
import math, random, os

os.makedirs('out', exist_ok=True)
GOLD=(236,176,56); GOLD_L=(255,228,140); CYAN=(96,222,244); WHITE=(246,244,230)
STEEL=(150,158,185); STEEL_L=(205,212,235); STEEL_D=(96,102,128)

# ---------- helpers shared with the card-icon look ----------
def canvas(s=32): return Image.new('RGBA',(s,s),(0,0,0,0))
def glow(im,col,r=14,strength=95,s=32):
    C=s/2; base=Image.new('RGBA',(s,s),(0,0,0,0)); p=base.load()
    for y in range(s):
        for x in range(s):
            d=math.hypot(x-C+0.5,y-C+0.5)
            if d<r: p[x,y]=(col[0],col[1],col[2],int(strength*(1-d/r)**2))
    base.alpha_composite(im); return base

# ---------- Chain Lash card icon (32) : broken shackle + lashing chain ----------
im=canvas(); d=ImageDraw.Draw(im); C=16
# three chain links on a diagonal (the lash)
links=[(8,24),(14,18),(20,12)]
for i,(x,y) in enumerate(links):
    col=STEEL_L if i%2 else STEEL
    d.ellipse([x-3,y-4,x+3,y+4], outline=col+(255,), width=2)
# open shackle cuff at the top end (gold)
d.arc([19,5,29,15], start=120, end=400, fill=GOLD+(255,), width=3)
d.ellipse([26,4,30,8], fill=GOLD_L+(255,))
# impact spark at the bottom end
for a in range(0,360,45):
    ex=8+math.cos(math.radians(a))*5; ey=24+math.sin(math.radians(a))*5
    d.line([(8,24),(ex,ey)], fill=GOLD_L+(220,), width=1)
glow(im, GOLD).save('out/chain_lash.png')

# ---------- Enemy projectile : hostile glowing orb (32) ----------
S=32; C=S/2
im=Image.new('RGBA',(S,S),(0,0,0,0)); p=im.load()
for y in range(S):
    for x in range(S):
        d0=math.hypot(x-C+0.5,y-C+0.5)
        if d0<=5.5:                       # white-hot core
            p[x,y]=(255,250,230,255)
        elif d0<=8.5:                     # orange body
            t=(d0-5.5)/3.0
            p[x,y]=(255,int(200-80*t),int(90-50*t),255)
        elif d0<=15:                      # red outer glow
            a=int(200*(1-(d0-8.5)/6.5)**2)
            p[x,y]=(230,70,40,max(0,a))
d=ImageDraw.Draw(im)
# four flame spikes so it reads as a hostile bolt, not an xp orb
for a in range(0,360,90):
    x1=C+math.cos(math.radians(a))*7; y1=C+math.sin(math.radians(a))*7
    x2=C+math.cos(math.radians(a))*13; y2=C+math.sin(math.radians(a))*13
    d.line([(x1,y1),(x2,y2)], fill=(255,150,60,230), width=2)
d.ellipse([C-3,C-4,C+1,C], fill=(255,255,255,255))  # highlight
im.save('out/enemy_projectile.png')

# ================= MYTHICAL DESERT PROPS (32) =================
OUT=(42,28,16)
SAND_D=(150,120,78); SAND=(196,166,112); SAND_L=(226,200,150)
BONE_D=(150,138,108); BONE=(214,204,176); BONE_L=(240,232,208)
STONE_D=(120,96,66); STONE=(170,140,98); STONE_L=(208,182,138)
WOOD_D=(78,54,34); WOOD=(116,84,52)
CACT_D=(58,92,62); CACT=(92,130,86); CACT_L=(130,170,120)

class P:
    def __init__(s,w=32,h=32): s.w,s.h=w,h; s.g=[[None]*w for _ in range(h)]
    def px(s,x,y,c):
        if 0<=x<s.w and 0<=y<s.h and c: s.g[y][x]=c
    def rect(s,x0,y0,x1,y1,c):
        for y in range(y0,y1+1):
            for x in range(x0,x1+1): s.px(x,y,c)
    def outline(s):
        add=[]
        for y in range(s.h):
            for x in range(s.w):
                if s.g[y][x] is None:
                    for a,b in ((1,0),(-1,0),(0,1),(0,-1),(1,1),(-1,-1),(1,-1),(-1,1)):
                        nx,ny=x+a,y+b
                        if 0<=nx<s.w and 0<=ny<s.h and s.g[ny][nx] not in (None,OUT): add.append((x,y)); break
        for x,y in add: s.px(x,y,OUT)
    def img(s):
        im=Image.new('RGBA',(s.w,s.h),(0,0,0,0))
        for y in range(s.h):
            for x in range(s.w):
                if s.g[y][x]: im.putpixel((x,y),s.g[y][x]+(255,))
        return im

def dome(p,cx,cy,rx,ry,dk,md,lt):
    for y in range(p.h):
        for x in range(p.w):
            nx=(x-cx)/rx; ny=(y-cy)/ry
            if nx*nx+ny*ny<=1 and y>=cy-ry:
                c=lt if (nx+ny)<-0.5 else (dk if nx*nx+ny*ny>0.55 else md)
                p.px(x,y,c)

def dune():
    p=P(); dome(p,16,24,13,9,SAND_D,SAND,SAND_L)
    for x in range(6,27,3): p.px(x,22,SAND_L)   # wind ripple
    p.outline(); return p.img()

def cracked():
    p=P(); dome(p,16,25,13,6,SAND_D,SAND,SAND_L)
    for (x0,y0,x1,y1) in ((16,21,15,27),(16,22,22,26),(16,22,9,27),(16,23,20,28)):
        steps=max(abs(x1-x0),abs(y1-y0))
        for i in range(steps+1):
            p.px(round(x0+(x1-x0)*i/steps),round(y0+(y1-y0)*i/steps),STONE_D)
    p.outline(); return p.img()

def skull():
    p=P(); dome(p,16,15,8,8,BONE_D,BONE,BONE_L); p.rect(13,22,19,27,BONE)
    p.rect(12,15,14,18,(26,20,16)); p.rect(18,15,20,18,(26,20,16)); p.rect(15,20,16,22,(26,20,16))
    for x in range(13,20,2): p.px(x,26,(26,20,16))
    p.outline(); return p.img()

def bones():
    p=P(); M,L,Dk=BONE,BONE_L,BONE_D
    def bone(x0,y0,x1,y1):
        st=int(max(abs(x1-x0),abs(y1-y0)))+1
        for i in range(st):
            x=round(x0+(x1-x0)*i/st); y=round(y0+(y1-y0)*i/st); p.px(x,y,M); p.px(x,y-1,L)
        for ex,ey in ((x0,y0),(x1,y1)):
            p.px(ex-1,ey,L); p.px(ex+1,ey,M); p.px(ex,ey+1,Dk)
    bone(8,16,24,21); bone(10,24,22,15)
    p.outline(); return p.img()

def obelisk():
    p=P(); p.rect(12,5,19,27,STONE); p.rect(12,5,13,27,STONE_L); p.rect(18,5,19,27,STONE_D)
    # tapered top
    p.px(14,3,STONE); p.px(15,2,STONE_L); p.px(16,2,STONE); p.px(17,3,STONE_D); p.px(15,4,STONE_L); p.px(16,4,STONE)
    # broken chunk out of the side + carvings
    p.px(19,14,None); p.px(18,15,None); p.px(19,15,None)
    for y in range(9,24,4): p.px(15,y,STONE_D); p.px(16,y+1,STONE_D)
    p.rect(9,27,22,29,STONE_D); p.rect(9,27,22,27,STONE)  # sand-buried base
    p.outline(); return p.img()

def dead_shrub():
    p=P(); cx=16
    for (x,y,dx,dy,n) in ((16,28,0,-1,8),(16,24,-1,-1,6),(16,23,1,-1,7),(16,21,-1,-1,4),(16,20,1,-1,5)):
        px_,py=x,y
        for i in range(n):
            px_+=dx; py+=dy; p.px(px_,py,WOOD); p.px(px_,py,WOOD_L if (i%2) else WOOD)
    p.outline(); return p.img()

def cactus():
    p=P(); p.rect(14,10,18,29,CACT); p.rect(14,10,14,29,CACT_L); p.rect(18,10,18,29,CACT_D)
    p.rect(10,16,13,18,CACT); p.rect(10,12,11,18,CACT); p.rect(10,12,10,18,CACT_L)   # left arm
    p.rect(19,19,22,21,CACT); p.rect(21,14,22,21,CACT); p.rect(22,14,22,21,CACT_D)   # right arm
    for y in range(12,28,3): p.px(16,y,CACT_D)
    for y in range(12,28,4): p.px(13,y,CACT_L); p.px(19,y,CACT_L)   # spine dots
    p.outline(); return p.img()

def rock():
    p=P(); dome(p,15,23,8,6,STONE_D,STONE,STONE_L); dome(p,22,25,5,4,STONE_D,STONE,STONE_L)
    p.outline(); return p.img()

WOOD_L=(150,112,72)
props={'desert_dune':dune(),'desert_cracked':cracked(),'desert_skull':skull(),'desert_bones':bones(),
       'desert_obelisk':obelisk(),'desert_shrub':dead_shrub(),'desert_cactus':cactus(),'desert_rock':rock()}
for n,im in props.items(): im.save(f'out/{n}.png')

# ================= SEAMLESS SAND GROUND TILE (128) =================
T=128; ground=Image.new('RGBA',(T,T),(0,0,0,0)); gp=ground.load()
random.seed(3)
base=(150,120,80)
for y in range(T):
    for x in range(T):
        # wind ripples: sin periods that divide 128 -> seamless
        rip=math.sin((x+ y*0.5)/128*2*math.pi*4)*6 + math.sin(y/128*2*math.pi*8)*3
        n=random.randint(-10,10)+int(rip)
        r=max(120,min(205,base[0]+n)); g=max(95,min(178,base[1]+n)); b=max(60,min(130,base[2]+int(n*0.7)))
        gp[x,y]=(r,g,b,255)
# a few darker pebbles / speckle (kept away from edges so tiling stays clean)
for _ in range(90):
    x=random.randint(4,T-5); y=random.randint(4,T-5)
    c=random.choice([(120,96,64),(170,142,98),(205,182,138)])
    gp[x,y]=c+(255,)
    if random.random()<0.4: gp[x+1,y]=c+(255,)
ground.save('out/ground_sand.png')

# ---------- previews ----------
cols=4; pad=6; cell=32
sheet=Image.new('RGBA',(cols*(cell+pad)+pad, 2*(cell+pad)+pad+40),(30,22,16,255))
items=list(props.items())
for i,(n,im) in enumerate(items):
    r,c=divmod(i,cols); sheet.alpha_composite(im,(pad+c*(cell+pad), 34+pad+r*(cell+pad)))
sheet.resize((sheet.width*5,sheet.height*5),Image.NEAREST).save('props_preview.png')

mini=Image.new('RGBA',(170,70),(30,22,16,255))
mini.alpha_composite(glow(Image.open('out/chain_lash.png').convert('RGBA'),GOLD).resize((64,64),Image.NEAREST),(5,3))
mini.alpha_composite(Image.open('out/enemy_projectile.png').resize((64,64),Image.NEAREST),(95,3))
mini.resize((mini.width*3,mini.height*3),Image.NEAREST).save('icon_proj_preview.png')

# ground tiled 2x2 preview
g=Image.open('out/ground_sand.png'); tiled=Image.new('RGBA',(T*2,T*2))
for a in range(2):
    for b in range(2): tiled.alpha_composite(g,(a*T,b*T))
tiled.save('ground_preview.png')
print('ok')
