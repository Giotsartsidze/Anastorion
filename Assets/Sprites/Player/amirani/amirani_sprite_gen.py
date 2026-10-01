from PIL import Image
import math, random

W=H=48
MAG=(255,0,255)
OUT=(10,8,22)
SHD=(16,20,44); NAVY=(28,38,84); NAVY_L=(52,70,132)
CLK_D=(44,30,26); CLK=(78,52,38); CLK_L=(110,76,52)
SKIN_D=(150,90,64); SKIN=(204,138,98); SKIN_L=(232,176,132)
HAIR=(30,22,20); HAIR_L=(64,46,36)
GOLD_D=(168,106,30); GOLD=(236,176,56); GOLD_L=(255,228,140)
WHITE=(252,250,232); CYAN=(96,222,244); CYAN_D=(34,120,176)
STEEL=(96,102,128); STEEL_L=(170,178,200)
LTH=(66,42,28); LTH_L=(104,70,44)

class Canvas:
    def __init__(s):
        s.g=[[None]*W for _ in range(H)]; s.sword=set()
    def px(s,x,y,c,sw=False):
        if 0<=x<W and 0<=y<H and c:
            s.g[y][x]=c
            if sw: s.sword.add((x,y))
    def rect(s,x0,y0,x1,y1,c):
        for y in range(y0,y1+1):
            for x in range(x0,x1+1): s.px(x,y,c)

def draw_sword(c,hx,hy,d,glow_phase,length=10):
    dx,dy=d
    # pommel + grip
    c.px(hx-dx,hy-dy,GOLD,True)
    # guard perpendicular
    px_,py_=-dy,dx
    for k in (-2,-1,0,1,2):
        c.px(hx+dx+k*px_ if False else hx+dx+k*(1 if dx*dy>0 else 1)*(-dy if False else 0)+k*px_, hy+dy+k*py_, GOLD if abs(k)<2 else GOLD_D, True)
    for i in range(2,length+1):
        x,y=hx+dx*i,hy+dy*i
        c.px(x,y,WHITE,True)
        c.px(x+1 if dx<0 else x-1,y,GOLD_L if i<length else None,True)
    # glow sparkles along blade
    for i in range(3,length+1,3):
        j=(i+glow_phase)%4
        x,y=hx+dx*i,hy+dy*i
        if j==0: c.px(x-1 if dx<0 else x+1, y-1, CYAN, True)
        if j==2: c.px(x+2 if dx<0 else x-2, y+1, CYAN, True)
    tx,ty=hx+dx*(length+1),hy+dy*(length+1)
    c.px(tx,ty,CYAN if glow_phase%2 else GOLD_L,True)

def render(p):
    c=Canvas()
    bd=p.get('bd',0); lean=p.get('lean',0)
    T0=19+bd; hipY=32+bd; hy=10+bd
    L=lean
    # cloak behind
    c.rect(17+L,T0,30+L,min(hipY+3,43),CLK_D)
    for x in range(17+L,31+L,2): c.px(x,min(hipY+4,44),CLK_D)
    # legs
    for side,(x0,x1) in (('l',(19,22)),('r',(25,28))):
        lift,ldx=p.get(side+'lift',0),p.get(side+'dx',0)
        fb=44-lift
        x0+=ldx; x1+=ldx
        for y in range(hipY+1,fb-1):
            c.rect(x0,y,x1,y,LTH_L if (y+(0 if side=='l' else 1))%2 else CLK)
            c.px(x1,y,LTH)
        # boot
        c.rect(x0,fb-1,x1,fb,LTH)
        c.px(x0 if side=='l' else x1, fb, CLK_D)
        c.rect(x0,fb-1,x1,fb-1,LTH_L)
    # lower tunic
    c.rect(18+L,hipY-2,29+L,hipY+1,NAVY)
    for x in range(18+L,30+L):
        if x%2: c.px(x,hipY+2,NAVY)
    c.rect(27+L,hipY-2,29+L,hipY+1,SHD)
    # torso
    c.rect(18+L,T0+1,29+L,hipY-5,NAVY)
    c.rect(27+L,T0+1,29+L,hipY-5,SHD)
    c.rect(18+L,T0+1,19+L,hipY-5,NAVY_L)
    # strap
    for i in range(8):
        c.px(19+L+i,T0+1+i,LTH); c.px(20+L+i,T0+1+i,LTH_L if i%3 else LTH)
    c.px(22+L,T0+4,GOLD)
    # emblem
    ex,ey=25+L,T0+3
    for (a,b,col) in ((0,-1,GOLD),(-1,0,GOLD),(1,0,GOLD_D),(0,1,GOLD_D),(0,0,GOLD_L)): c.px(ex+a,ey+b,col)
    # belt
    c.rect(18+L,hipY-4,29+L,hipY-3,LTH)
    c.rect(23+L,hipY-4,24+L,hipY-3,GOLD); c.px(23+L,hipY-4,GOLD_L)
    c.px(20+L,hipY-4,GOLD_D); c.px(27+L,hipY-4,GOLD_D)
    # shoulders (cloak)
    c.rect(16+L,T0,31+L,T0+2,CLK); c.rect(16+L,T0+2,31+L,T0+2,CLK_D)
    c.px(17+L,T0,CLK_L); c.px(18+L,T0,CLK_L); c.px(21+L,T0+1,GOLD_D)
    # neck
    c.rect(22+L,T0-1,25+L,T0-1,SKIN_D)
    # hair back/long
    hs=p.get('hair',0)
    for x,y0,y1 in ((19,hy,hy+11),(28,hy,hy+11),(18,hy+3,hy+11),(29,hy+3,hy+11)):
        c.rect(x+L+hs,y0,x+L+hs,y1,HAIR)
    c.px(18+L+hs,hy+12,HAIR); c.px(29+L+hs,hy+12,HAIR)
    # face
    c.rect(20+L,hy,27+L,hy+7,SKIN); c.rect(27+L,hy,27+L,hy+7,SKIN_D)
    c.px(21+L,hy+2,SKIN_L); c.px(21+L,hy+3,SKIN_L)
    # hair top
    c.rect(19+L,hy-1,28+L,hy+1,HAIR); c.rect(20+L,hy-2,27+L,hy-2,HAIR)
    for x in (21,22,25): c.px(x+L,hy-1,HAIR_L)
    c.px(20+L,hy+2,HAIR); c.px(27+L,hy+2,HAIR)
    # gold headband
    c.rect(20+L,hy+1,27+L,hy+1,GOLD_D); c.px(23+L,hy+1,GOLD); c.px(24+L,hy+1,GOLD_L)
    # brows / eyes
    if not p.get('dead'):
        c.px(21+L,hy+3,HAIR); c.px(22+L,hy+3,HAIR); c.px(25+L,hy+3,HAIR); c.px(26+L,hy+3,HAIR)
        c.px(22+L,hy+4,OUT); c.px(25+L,hy+4,OUT)
    else:
        c.px(21+L,hy+4,HAIR); c.px(22+L,hy+4,HAIR); c.px(25+L,hy+4,HAIR); c.px(26+L,hy+4,HAIR)
    c.px(24+L,hy+5,SKIN_D)
    # beard
    c.rect(20+L,hy+6,27+L,hy+7,HAIR); c.rect(21+L,hy+8,26+L,hy+8,HAIR); c.rect(22+L,hy+9,25+L,hy+9,HAIR)
    c.rect(23+L,hy+7,24+L,hy+7,SKIN_D)
    c.px(21+L,hy+6,HAIR_L)
    # free arm (screen right)
    ad=p.get('rad',0)
    for y in range(T0+2,T0+10+ad):
        c.px(31+L,y,SKIN); c.px(32+L,y,SKIN_D)
    c.rect(31+L,T0+6+ad,32+L,T0+7+ad,GOLD_D); c.px(31+L,T0+6+ad,GOLD)
    c.rect(31+L,T0+10+ad,32+L,T0+11+ad,SKIN)
    # broken chain
    for i,(cx,cy) in enumerate(((33,7),(33,8),(34,9),(34,10),(34,11))):
        if i<p.get('chain',5): c.px(cx+L,T0+cy+ad,STEEL_L if i%2 else STEEL)
    # sword arm (screen left)
    sd=p.get('lad',0)
    for y in range(T0+2,T0+8+sd):
        c.px(15+L,y,SKIN_L); c.px(16+L,y,SKIN)
    c.rect(14+L,T0+8+sd,15+L,T0+9+sd,SKIN)
    c.rect(15+L,T0+5+sd,16+L,T0+6+sd,GOLD_D); c.px(15+L,T0+5+sd,GOLD)
    hx,hyy=13+L,T0+10+sd
    c.rect(hx,hyy,hx+1,hyy+1,SKIN)
    sw=p.get('sword','up')
    if sw=='up': draw_sword(c,hx,hyy,(-1,-1),p.get('glow',0))
    elif sw=='low': draw_sword(c,hx,hyy+1,(-1,1),p.get('glow',0),8)
    return c

def finish(c,frame=0,aura=True,rim=True):
    g=c.g
    filled=lambda x,y:0<=x<W and 0<=y<H and g[y][x] is not None
    # rim light (left edges)
    if rim:
        for y in range(H):
            for x in range(W):
                if g[y][x] and (x,y) not in c.sword and not filled(x-1,y):
                    col=g[y][x]; g[y][x]=tuple(int(a*0.6+b*0.4) for a,b in zip(col,GOLD_L))
    out=[[None]*W for _ in range(H)]
    for y in range(H):
        for x in range(W):
            if g[y][x] is None:
                nb=[(x+a,y+b) for a,b in ((1,0),(-1,0),(0,1),(0,-1))]
                if any(filled(*n) for n in nb):
                    out[y][x]=CYAN_D if any(n in c.sword for n in nb if filled(*n)) else OUT
    for y in range(H):
        for x in range(W):
            if out[y][x]: g[y][x]=out[y][x]
    if aura:
        cand=[]
        for y in range(H):
            for x in range(W):
                if g[y][x] is None and any(0<=x+a<W and 0<=y+b<H and out[y+b][x+a] for a,b in ((1,0),(-1,0),(0,1),(0,-1))):
                    cand.append((x,y))
        for x,y in cand:
            k=(x*3+y*5+frame*2)%7
            if k==0: g[y][x]=GOLD
            elif k==3: g[y][x]=GOLD_D
    return c

def to_img(c,bg=None):
    im=Image.new('RGBA',(W,H),(0,0,0,0) if bg is None else bg+(255,))
    for y in range(H):
        for x in range(W):
            if c.g[y][x]: im.putpixel((x,y),c.g[y][x]+(255,))
    return im

def from_grid(g,sword=set()):
    c=Canvas(); c.g=g; c.sword=sword; return c

# ---------- IDLE ----------
idle=[]
for i,(bd,glow) in enumerate(((0,0),(0,1),(1,2),(1,3))):
    idle.append(finish(render(dict(bd=bd,glow=glow,rad=0,lad=0)),i))

# ---------- RUN ----------
run=[]
for i in range(6):
    ph=i/6*2*math.pi
    s=math.sin(ph)
    p=dict(
        llift=max(0,round(3.4*s)), rlift=max(0,round(-3.4*s)),
        ldx=round(math.cos(ph)), rdx=-round(math.cos(ph)),
        bd=1 if i in (1,4) else 0,
        rad=round(-2*s), lad=round(1.5*s), glow=i, hair=0)
    run.append(finish(render(p),i))

# ---------- DEATH ----------
random.seed(7)
death=[]
# 0 hit flash
c=render(dict(lean=1,glow=0,chain=5)); 
for y in range(H):
    for x in range(W):
        if c.g[y][x]: c.g[y][x]=tuple(int(a*0.35+255*0.65) for a in c.g[y][x])
death.append(finish(c,0,aura=False,rim=False))
# 1 buckle
death.append(finish(render(dict(bd=3,sword='low',glow=1,rad=1,lad=0,dead=True)),1))
# 2 kneel
c=render(dict(bd=6,sword='none',rad=2,lad=2,dead=True,llift=0,rlift=0))
# dropped sword on ground
for i in range(10): c.px(2+i,44,WHITE if i<9 else GOLD_L,True); c.px(2+i,45,GOLD_D if i%3 else None,True)
c.px(12,43,GOLD,True); c.px(12,45,GOLD,True); c.px(13,44,GOLD,True)
death.append(finish(c,2,aura=False))
# 3 fallen: rotate an upright dead body 90deg
def fallen_grid():
    b=render(dict(sword='none',dead=True,rad=0,lad=0))
    im=to_img(b).rotate(90,expand=False)  # head to the left
    # find bbox and move to lie on ground
    bb=im.getbbox(); body=im.crop(bb)
    canvas=Image.new('RGBA',(W,H),(0,0,0,0))
    ox=(W-body.width)//2; oy=45-body.height
    canvas.paste(body,(ox,oy))
    g=[[None]*W for _ in range(H)]
    for y in range(H):
        for x in range(W):
            r,gg,bl,a=canvas.getpixel((x,y))
            if a: g[y][x]=(r,gg,bl)
    return g
fg=fallen_grid()
def add_sword_ground(c):
    for i in range(10): c.px(34+i,45,WHITE if i<9 else GOLD_L,True)
    c.px(33,44,GOLD,True); c.px(33,46,GOLD,True); c.px(32,45,GOLD,True)
c=from_grid([row[:] for row in fg],set()); add_sword_ground(c)
death.append(finish(c,3,aura=False))
# 4-6 dissolve to golden sparks
body_px=[(x,y) for y in range(H) for x in range(W) if fg[y][x]]
for step,(keep,rise,nspark) in enumerate(((0.55,4,40),(0.18,9,30),(0.0,15,14))):
    g=[[None]*W for _ in range(H)]
    for (x,y) in body_px:
        if random.random()<keep: g[y][x]=fg[y][x]
    c=from_grid(g,set())
    if keep>0: add_sword_ground(c)
    c=finish(c,4+step,aura=False)
    for _ in range(nspark):
        x,y=random.choice(body_px)
        ny=y-random.randint(1,rise)-step*2
        c.px(x+random.randint(-1,1),ny,random.choice((GOLD,GOLD_L,WHITE,CYAN)))
    death.append(c)

anims={'idle':idle,'run':run,'death':death}
import os
os.makedirs('out',exist_ok=True)
S=4
rows=[]
for name,frames in anims.items():
    for bgname,bg in (('magenta',MAG),('transparent',None)):
        sheet=Image.new('RGBA',(W*len(frames),H),(0,0,0,0) if bg is None else bg+(255,))
        for i,f in enumerate(frames):
            im=to_img(f); sheet.alpha_composite(im,(i*W,0))
        sheet.save(f'out/amirani_{name}_{bgname}_48px.png')
        sheet.resize((sheet.width*S,sheet.height*S),Image.NEAREST).save(f'out/amirani_{name}_{bgname}_x4.png')
    # gif preview on dark bg
    dur={'idle':180,'run':90,'death':140}[name]
    gifs=[]
    for f in frames:
        bgim=Image.new('RGBA',(W,H),(14,16,34,255)); bgim.alpha_composite(to_img(f))
        gifs.append(bgim.convert('RGB').resize((W*6,H*6),Image.NEAREST))
    gifs[0].save(f'out/preview_{name}.gif',save_all=True,append_images=gifs[1:],duration=dur,loop=0)
# combined atlas (rows: idle, run, death) magenta
maxf=max(len(v) for v in anims.values())
atlas=Image.new('RGBA',(W*maxf,H*3),MAG+(255,))
for r,(n,frames) in enumerate(anims.items()):
    for i,f in enumerate(frames): atlas.alpha_composite(to_img(f),(i*W,r*H))
atlas.save('out/amirani_atlas_magenta_48px.png')
# review sheet on dark
rev=Image.new('RGBA',(W*maxf,H*3),(14,16,34,255))
for r,(n,frames) in enumerate(anims.items()):
    for i,f in enumerate(frames): rev.alpha_composite(to_img(f),(i*W,r*H))
rev.resize((rev.width*5,rev.height*5),Image.NEAREST).save('review.png')
print('ok')
