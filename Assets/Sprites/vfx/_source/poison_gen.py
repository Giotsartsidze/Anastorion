"""Poison flask projectile + chemical splat hit-VFX (original game art)."""
from PIL import Image
import math, os
os.makedirs('out', exist_ok=True)
OUT=(14,16,12)
GLASS=(196,216,200); GL_L=(238,250,240)
TOX_D=(30,104,40); TOX=(92,196,74); TOX_L=(176,244,128)
CORK_D=(96,66,40); CORK=(142,102,64)
SKULL=(226,240,210)

# ---------- POISON FLASK (32) ----------
S=32; g=[[None]*S for _ in range(S)]
def px(x,y,c):
    if 0<=x<S and 0<=y<S and c: g[y][x]=c
def disc(cx,cy,r,c,cond=None):
    for y in range(S):
        for x in range(S):
            if (x-cx)**2+(y-cy)**2<=r*r and (cond is None or cond(x,y)): px(x,y,c)
cx,cy,r=16,20,8
disc(cx,cy,r,GLASS)
disc(cx,cy,r,TOX, cond=lambda x,y:y>=16)
for x in range(cx-7,cx+8):
    if (x-cx)**2+(16-cy)**2<=r*r: px(x,16,TOX_L)
disc(cx,cy,r,TOX_D, cond=lambda x,y:y>=24)
for y in range(9,14):
    for x in range(14,19): px(x,y,GLASS)
for y in range(12,14):
    for x in range(14,19): px(x,y,TOX)
for y in range(5,9):
    for x in range(13,20): px(x,y, CORK_D if y==5 else CORK)
# rising bubbles
for bx,by in [(14,20),(18,22),(16,18),(19,19),(13,23)]: px(bx,by,TOX_L)
# tiny skull mark
px(15,21,SKULL); px(17,21,SKULL); px(16,23,SKULL); px(14,21,SKULL); px(18,21,SKULL)
for (sx,sy) in [(12,16),(12,17),(13,15),(14,10)]: px(sx,sy,GL_L)
# outline
add=[]
for y in range(S):
    for x in range(S):
        if g[y][x] is None:
            for a,b in ((1,0),(-1,0),(0,1),(0,-1),(1,1),(-1,-1),(1,-1),(-1,1)):
                nx,ny=x+a,y+b
                if 0<=nx<S and 0<=ny<S and g[ny][nx] not in (None,OUT): add.append((x,y)); break
for x,y in add: px(x,y,OUT)
im=Image.new('RGBA',(S,S),(0,0,0,0))
for y in range(S):
    for x in range(S):
        d=math.hypot(x-cx,y-(cy-1))
        if d<=14: im.putpixel((x,y),(90,220,90,int(70*(1-d/14)**2)))
for y in range(S):
    for x in range(S):
        if g[y][x]: im.putpixel((x,y),g[y][x]+(255,))
im.save('out/poison_flask.png')
im.resize((S*8,S*8),Image.NEAREST).save('poison_flask_preview.png')

# ---------- CHEMICAL SPLAT (48) ----------
T=48; sp=Image.new('RGBA',(T,T),(0,0,0,0)); p=sp.load()
cx=cy=T/2
import random; random.seed(5)
# irregular central blob
for y in range(T):
    for x in range(T):
        ang=math.atan2(y-cy,x-cx); d=math.hypot(x-cx,y-cy)
        wob=12 + math.sin(ang*5)*3 + math.sin(ang*3+1)*2
        if d<=wob:
            t=d/wob
            if t<0.4: c=(176,244,128)
            elif t<0.8: c=(92,196,74)
            else: c=(30,104,40)
            p[x,y]=c+(235,)
# droplets flung out
for _ in range(16):
    a=random.uniform(0,2*math.pi); dist=random.uniform(14,22)
    dx=int(cx+math.cos(a)*dist); dy=int(cy+math.sin(a)*dist)
    rr=random.randint(1,3)
    for yy in range(dy-rr,dy+rr+1):
        for xx in range(dx-rr,dx+rr+1):
            if 0<=xx<T and 0<=yy<T and (xx-dx)**2+(yy-dy)**2<=rr*rr:
                p[xx,yy]=(92,196,74,230)
sp.save('out/poison_splat.png')
sp.resize((T*6,T*6),Image.NEAREST).save('poison_splat_preview.png')
print('ok')
