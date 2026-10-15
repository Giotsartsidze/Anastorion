"""Health pickup — a glowing green healing potion flask (clearly not a red projectile)."""
from PIL import Image
import math, os
os.makedirs('out', exist_ok=True)
S=32
OUT=(14,12,20)
GLASS=(198,222,232); GL_L=(242,250,252)
HEAL_D=(32,126,64); HEAL=(66,198,104); HEAL_L=(156,242,176)
CORK_D=(96,66,40); CORK=(142,102,64)
WHITE=(240,255,245)

g=[[None]*S for _ in range(S)]
def px(x,y,c):
    if 0<=x<S and 0<=y<S and c: g[y][x]=c
def disc(cx,cy,r,c,cond=None):
    for y in range(S):
        for x in range(S):
            if (x-cx)**2+(y-cy)**2<=r*r and (cond is None or cond(x,y)): px(x,y,c)

cx,cy,r=16,20,8
disc(cx,cy,r,GLASS)                                   # bulb glass
disc(cx,cy,r,HEAL, cond=lambda x,y: y>=17)            # liquid
for x in range(cx-7,cx+8):                            # meniscus
    if (x-cx)**2+(17-cy)**2<=r*r: px(x,17,HEAL_L)
disc(cx,cy,r,HEAL_D, cond=lambda x,y: y>=24)          # liquid bottom shade

for y in range(9,14):                                 # neck
    for x in range(14,19): px(x,y,GLASS)
for y in range(12,14):                                # liquid in neck
    for x in range(14,19): px(x,y,HEAL)

for y in range(5,9):                                  # cork
    for x in range(13,20): px(x,y, CORK_D if y==5 else CORK)
px(13,8,CORK_D); px(19,8,CORK_D)

for x in range(cx-2,cx+3): px(x,20,WHITE)             # heal cross
for y in range(18,23): px(cx,y,WHITE)

for (sx,sy) in [(12,16),(12,17),(13,15),(11,18),(14,10),(14,11)]: px(sx,sy,GL_L)  # glass shine

# dark outline
add=[]
for y in range(S):
    for x in range(S):
        if g[y][x] is None:
            for a,b in ((1,0),(-1,0),(0,1),(0,-1),(1,1),(-1,-1),(1,-1),(-1,1)):
                nx,ny=x+a,y+b
                if 0<=nx<S and 0<=ny<S and g[ny][nx] not in (None,OUT): add.append((x,y)); break
for x,y in add: px(x,y,OUT)

im=Image.new('RGBA',(S,S),(0,0,0,0))
for y in range(S):                                    # soft green glow behind
    for x in range(S):
        d=math.hypot(x-cx,y-(cy-1))
        if d<=14: im.putpixel((x,y),(90,230,130,int(72*(1-d/14)**2)))
for y in range(S):
    for x in range(S):
        if g[y][x]: im.putpixel((x,y), g[y][x]+(255,))
im.save('out/health_potion.png')
im.resize((S*8,S*8),Image.NEAREST).save('potion_preview.png')
print('ok')
