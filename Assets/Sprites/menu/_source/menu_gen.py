"""Main-menu art for Anastorion: a glowing Borjgali emblem + a cosmic backdrop.
borjgali.png (128, transparent) · menu_bg.png (512x288, opaque, vignette + center glow).
"""
from PIL import Image, ImageDraw
import math, random

GOLD=(236,176,56); GOLD_L=(255,228,140); CYAN=(96,222,244); WHITE=(246,244,230)
BLACK=(8,6,18); NAVY=(20,26,56); NAVY2=(34,44,92)
def lerp(a,b,t): return tuple(int(a[i]+(b[i]-a[i])*t) for i in range(3))

# ---------- BORJGALI EMBLEM ----------
S=128; C=S/2
im=Image.new('RGBA',(S,S),(0,0,0,0)); p=im.load()
for y in range(S):               # soft gold glow halo
    for x in range(S):
        d=math.hypot(x-C,y-C)
        if d<58: p[x,y]=(GOLD[0],GOLD[1],GOLD[2],int(70*(1-d/58)**2))
d=ImageDraw.Draw(im)
for k in range(7):               # seven curved spiralling wings
    a0=k*2*math.pi/7; prev=None
    for i in range(34):
        t=i/33; r=17+t*44; a=a0+t*0.95
        x=C+r*math.cos(a); y=C+r*math.sin(a)
        if prev:
            w=max(1,int(6*(1-t)+1))
            d.line([prev,(x,y)],fill=(GOLD if t<0.7 else GOLD_L)+(255,),width=w)
        prev=(x,y)
    d.ellipse([prev[0]-2,prev[1]-2,prev[0]+2,prev[1]+2],fill=CYAN+(255,))  # cyan tip
# central sun
d.ellipse([C-16,C-16,C+16,C+16],fill=(150,100,20,255))
d.ellipse([C-11,C-11,C+11,C+11],fill=GOLD+(255,))
d.ellipse([C-6,C-6,C+6,C+6],fill=GOLD_L+(255,))
d.ellipse([C-3,C-3,C+3,C+3],fill=WHITE+(255,))
im.save('borjgali.png')

# ---------- MENU BACKDROP ----------
W,H=512,288; bg=Image.new('RGBA',(W,H),(0,0,0,255)); pb=bg.load()
cx,cy=W/2,H*0.42; maxd=math.hypot(W/2,H/2)
random.seed(4)
# cosmic gradient: warm-ish glow at center, dark vignette at edges
for y in range(H):
    for x in range(W):
        d=math.hypot(x-cx,y-cy)/maxd
        c=lerp(NAVY, BLACK, min(1,d*1.25))
        g=max(0,1-d*1.6)                # central glow
        c=lerp(c, NAVY2, g*0.5)
        pb[x,y]=c+(255,)
# faint nebula blobs
for _ in range(10):
    bx,by=random.randint(0,W),random.randint(0,H); br=random.randint(30,80)
    tint=GOLD if random.random()<0.4 else CYAN
    for y in range(max(0,by-br),min(H,by+br)):
        for x in range(max(0,bx-br),min(W,bx+br)):
            dd=math.hypot(x-bx,y-by)
            if dd<br:
                a=0.06*(1-dd/br)**2
                pb[x,y]=lerp(pb[x,y][:3],tint,a)+(255,)
# stars
for _ in range(220):
    x,y=random.randrange(W),random.randrange(H)
    col=random.choice([WHITE,CYAN,GOLD_L]); b=random.uniform(0.4,1)
    pb[x,y]=tuple(int(ci*b) for ci in col)+(255,)
    if random.random()<0.15:
        for dx,dy in [(1,0),(-1,0),(0,1),(0,-1)]:
            xx,yy=x+dx,y+dy
            if 0<=xx<W and 0<=yy<H: pb[xx,yy]=tuple(int(ci*b*0.5) for ci in col)+(255,)
bg.save('menu_bg.png')
print('ok')
