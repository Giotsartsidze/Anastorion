from PIL import Image, ImageDraw
import math, random

OUT=(10,8,22)
SHD=(16,20,44); NAVY=(28,38,84); NAVY_L=(52,70,132)
GOLD_D=(168,106,30); GOLD=(236,176,56); GOLD_L=(255,228,140)
WHITE=(252,250,232); CYAN=(96,222,244); CYAN_D=(34,120,176)
STEEL=(96,102,128); STEEL_L=(170,178,200)
BONE=(214,204,176); BONE_D=(150,140,118)
DEVI=(34,44,92); DEVI_D=(20,26,58); DEVI_L=(58,74,138)
IRON=(42,46,64); IRON_D=(26,28,42); IRON_L=(84,92,122)
STONE=(70,72,86); STONE_D=(44,46,58); STONE_L=(104,106,120); MOSS=(58,96,56); MOSS_L=(86,130,70)
FUR=(46,36,34); FUR_D=(28,22,22); FUR_L=(78,62,52)
IMP=(66,52,78); IMP_D=(42,32,54); IMP_L=(96,80,110)
ROBE=(36,26,48); ROBE_D=(22,16,30); ROBE_L=(60,44,76)
PALE=(232,228,242); HAIRW=(244,244,255); HAIRW_D=(170,180,214)
HAG=(112,122,102); HAG_D=(76,84,70)
WOOD=(92,62,38); WOOD_D=(62,40,24); STRAW=(196,160,80)
ROK=(52,50,62); ROK_D=(30,28,38); ROK_L=(88,86,102); VOID=(6,4,12); CHAIN=(58,60,76)
SCALE=(16,18,36); SCALE_M=(30,36,72); SCALE_L=(58,70,128)
ALI=(28,34,74); ALI_L=(50,62,120)
KAJI=(30,34,70); KAJI_L=(56,64,118)
BIRD=(26,24,40); BIRD_D=(14,12,24); BIRD_L=(48,46,70)
EMBER=(44,30,40); EMBER_D=(28,18,26)
EMI={GOLD_D,GOLD,GOLD_L,WHITE,CYAN,CYAN_D}

def C(c): return c if len(c)==4 else tuple(c)+(255,)

class Fr:
    def __init__(s,size=48,cx=None,by=None,sx=1,sy=1,ox=0,oy=0):
        s.size=size; s.im=Image.new('RGBA',(size,size),(0,0,0,0)); s.d=ImageDraw.Draw(s.im)
        s.cx=size//2 if cx is None else cx; s.by=size-4 if by is None else by
        s.sx,s.sy,s.ox,s.oy=sx,sy,ox,oy
    def t(s,x,y): return (int(round(s.cx+s.ox+x*s.sx)),int(round(s.by+s.oy+y*s.sy)))
    def _box(s,x0,y0,x1,y1):
        a=s.t(x0,y0);b=s.t(x1,y1)
        return [min(a[0],b[0]),min(a[1],b[1]),max(a[0],b[0]),max(a[1],b[1])]
    def ell(s,x0,y0,x1,y1,c): s.d.ellipse(s._box(x0,y0,x1,y1),fill=C(c))
    def rect(s,x0,y0,x1,y1,c): s.d.rectangle(s._box(x0,y0,x1,y1),fill=C(c))
    def poly(s,pts,c): s.d.polygon([s.t(*p) for p in pts],fill=C(c))
    def line(s,pts,c,w=1): s.d.line([s.t(*p) for p in pts],fill=C(c),width=w)
    def px(s,x,y,c):
        X,Y=s.t(x,y)
        if 0<=X<s.size and 0<=Y<s.size: s.im.putpixel((X,Y),C(c))
    def raw(s,X,Y,c):
        X,Y=int(round(X)),int(round(Y))
        if 0<=X<s.size and 0<=Y<s.size: s.im.putpixel((X,Y),C(c))

def blend(a,b,t): return tuple(int(x*(1-t)+y*t) for x,y in zip(a,b))

def finish(im,rim=(150,210,255),rim_amt=.35,alpha=255,aura=None,frame=0,outline=OUT):
    W,H=im.size; p=im.load()
    op=[[p[x,y][3]>0 for x in range(W)] for y in range(H)]
    if rim:
        for y in range(H):
            for x in range(W):
                if op[y][x] and (x==0 or not op[y][x-1] or y==0 or not op[y-1][x]):
                    r,g,b,a=p[x,y]
                    if (r,g,b) not in EMI: p[x,y]=blend((r,g,b),rim,rim_amt)+(a,)
    if alpha<255:
        for y in range(H):
            for x in range(W):
                r,g,b,a=p[x,y]
                if a and (r,g,b) not in EMI: p[x,y]=(r,g,b,alpha)
    ol=[]
    for y in range(H):
        for x in range(W):
            if not op[y][x] and any(0<=x+a<W and 0<=y+b<H and op[y+b][x+a] for a,b in ((1,0),(-1,0),(0,1),(0,-1))):
                ol.append((x,y))
    oa=255 if alpha==255 else min(255,alpha+60)
    for x,y in ol: p[x,y]=outline+(oa,)
    if aura:
        ols=set(ol)
        for y in range(H):
            for x in range(W):
                if p[x,y][3]==0 and any((x+a,y+b) in ols for a,b in ((1,0),(-1,0),(0,1),(0,-1))):
                    k=(x*3+y*5+frame*2)%7
                    if k==0: p[x,y]=C(aura[0])
                    elif k==3: p[x,y]=C(aura[1])
    return im

def flash(im):
    p=im.load(); W,H=im.size
    for y in range(H):
        for x in range(W):
            r,g,b,a=p[x,y]
            if a: p[x,y]=blend((r,g,b),WHITE,.7)+(a,)
    return im

def opaque(im):
    p=im.load(); W,H=im.size
    return [(x,y) for y in range(H) for x in range(W) if p[x,y][3]>0]

def dissolve(base,keep,rise,nspark,seed,cols=(GOLD,GOLD_L,WHITE,CYAN),big=False,src=None):
    random.seed(seed); W,H=base.size
    out=Image.new('RGBA',base.size,(0,0,0,0)); bp=base.load(); op=out.load()
    pts=opaque(base) if src is None else src
    for (x,y) in opaque(base):
        if random.random()<keep: op[x,y]=bp[x,y]
    for _ in range(nspark):
        x,y=random.choice(pts)
        ny=y-random.randint(1,rise); nx=x+random.randint(-2,2)*(2 if big else 1)
        c=C(random.choice(cols))
        for dx,dy in (((0,0),(1,0),(0,1),(1,1)) if big and random.random()<.4 else ((0,0),)):
            if 0<=nx+dx<W and 0<=ny+dy<H: op[nx+dx,ny+dy]=c
    return out

def std_death(make,fin,fly=False,n=6,seed=1,big=False,size=48):
    fr=[]
    fr.append(finish(flash(make(dict(hit=1))),rim=None))
    if n==6: steps=((1.1,.78),(1.25,.5))
    else: steps=((1.05,.88),(1.15,.7),(1.25,.48))
    for i,(sx,sy) in enumerate(steps):
        oy=(i+1)*(size//24) if fly else 0
        fr.append(fin(make(dict(dead=1),sx=sx,sy=sy,oy=oy),i+1))
    base=fr[-1]
    rem=n-len(fr)
    sched={3:((.5,5,30),(.15,10,26),(0,16,12)),4:((.6,8,70),(.3,16,70),(.08,26,50),(0,36,20))}[rem]
    for k,(keep,rise,ns) in enumerate(sched):
        fr.append(dissolve(base,keep,rise*(2 if big else 1),ns,seed+k,big=big,src=opaque(base)))
    return fr

def save_strip(frames,path):
    W,H=frames[0].size
    s=Image.new('RGBA',(W*len(frames),H),(0,0,0,0))
    for i,f in enumerate(frames): s.alpha_composite(f,(i*W,0))
    s.save(path); return s
