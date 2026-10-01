from core import *

def mk(draw,size=48):
    def make(p,sx=1,sy=1,ox=0,oy=0):
        f=Fr(size,sx=sx,sy=sy,ox=ox+p.get('ox',0),oy=oy+p.get('oy',0)); draw(f,p); return f.im
    return make

# ---------------- DEVI GRUNT ----------------
def devi(f,p):
    b=p.get('b',0); lg=p.get('lg',0); sw=p.get('sw',0); ph=p.get('ph',0)
    f.ell(-13,-3,13,3,(14,16,40))
    random.seed(ph)
    for _ in range(4): f.px(random.randint(-11,11),random.randint(-1,2),CYAN_D)
    l1=max(0,round(2*lg)); l2=max(0,round(-2*lg))
    f.rect(-8,-8,-3,-l1,DEVI_D); f.rect(3,-8,8,-l2,DEVI_D)
    f.rect(-9,-1-l1,-3,-l1,DEVI); f.rect(3,-1-l2,9,-l2,DEVI)
    f.ell(-12,-25+b,12,-5+b,DEVI)
    f.ell(-12,-14+b,12,-5+b,DEVI_D)
    f.ell(-6,-17+b,6,-7+b,DEVI_L)
    for x,y in ((-8,-19),(7,-13),(9,-20),(-9,-11)): f.px(x,y+b,DEVI_D)
    for s,a in ((-1,sw),(1,-sw)):
        f.ell(s*17,-21+b+a,s*10,-9+b+a,DEVI)
        f.ell(s*17,-14+b+a,s*10,-9+b+a,DEVI_D)
        for k in range(3): f.px(s*(16-k*2),-8+b+a,BONE)
    hb=b+(1 if p.get('dead') else 0)
    f.ell(-7,-30+hb,7,-18+hb,DEVI)
    f.poly([(-6,-27+hb),(-10,-33+hb),(-3,-28+hb)],BONE); f.poly([(6,-27+hb),(10,-33+hb),(3,-28+hb)],BONE)
    f.px(-9,-32+hb,BONE_D); f.px(9,-32+hb,BONE_D)
    f.rect(-6,-25+hb,6,-24+hb,DEVI_D)
    f.rect(-5,-23+hb,-2,-22+hb,DEVI_D); f.rect(2,-23+hb,5,-22+hb,DEVI_D)
    if not p.get('dead'): f.px(-3,-23+hb,CYAN); f.px(3,-23+hb,CYAN); f.px(-4,-23+hb,CYAN_D); f.px(4,-23+hb,CYAN_D)
    f.rect(-4,-20+hb,4,-20+hb,DEVI_D)
    for x in (-3,0,3): f.px(x,-20+hb,BONE)

# ---------------- ALI ----------------
def ali(f,p):
    ph=p.get('ph',0); air=p.get('air',0)
    random.seed(ph)
    for k in range(9):
        x=-9+k*2+random.randint(-1,1); y=-16+abs(x)//3
        L=random.randint(2,6)
        f.line([(x,y),(x+random.randint(-2,2),y-L)],ALI_L)
        if random.random()<.45: f.px(x+random.randint(-2,2),y-L-1,random.choice((WHITE,CYAN)))
    if air:
        f.line([(-6,-9),(-12,-4),(-14,1)],ALI,2); f.line([(6,-9),(12,-4),(14,1)],ALI,2)
    else:
        f.line([(-6,-9),(-11,-5),(-9,0)],ALI,2); f.line([(6,-9),(11,-5),(9,0)],ALI,2)
    f.ell(-9,-17,9,-6,ALI); f.ell(-6,-15,6,-8,ALI_L)
    if air:
        f.line([(-5,-12),(-8,-6),(-9,-1)],ALI,2); f.line([(5,-12),(8,-6),(9,-1)],ALI,2)
        claws=((-10,0),(-8,0),(8,0),(10,0))
    else:
        f.line([(-5,-12),(-6,-6),(-6,-1)],ALI,2); f.line([(5,-12),(6,-6),(6,-1)],ALI,2)
        claws=((-7,0),(-5,0),(5,0),(7,0))
    for c in claws: f.px(*c,WHITE)
    f.ell(-5,-22,5,-13,ALI)
    f.poly([(-5,-19),(-9,-26),(-2,-21)],ALI); f.poly([(5,-19),(9,-26),(2,-21)],ALI)
    f.px(-8,-25,ALI_L); f.px(8,-25,ALI_L)
    if not p.get('dead'):
        f.px(-3,-18,CYAN); f.px(-2,-18,CYAN); f.px(2,-18,CYAN); f.px(3,-18,CYAN)
        f.px(-2,-17,WHITE); f.px(2,-17,WHITE)
    f.rect(-2,-15,2,-15,SHD); f.px(-1,-14,WHITE); f.px(1,-14,WHITE)

# ---------------- KAJI ----------------
def kaji(f,p):
    lg=p.get('lg',0); ph=p.get('ph',0)
    f.line([(-3,-14),(-4+lg,-1)],KAJI,2); f.line([(3,-14),(4-lg,-1)],KAJI,2)
    f.px(-5+lg,0,KAJI_L); f.px(-4+lg,0,KAJI_L); f.px(4-lg,0,KAJI_L); f.px(5-lg,0,KAJI_L)
    f.poly([(-7,-30),(7,-30),(3,-13),(-3,-13)],KAJI)
    f.poly([(-7,-30),(-10,-33),(-5,-30)],KAJI_L); f.poly([(7,-30),(10,-33),(5,-30)],KAJI_L)
    f.line([(0,-29),(0,-15)],GOLD_D); f.px(0,-24,GOLD)
    a=p.get('arm',0)
    f.line([(-7,-29),(-10,-22),(-9,-16)],KAJI,2); f.px(-9,-15,KAJI_L)
    f.line([(7,-29),(10,-22),(11,-17+a)],KAJI,2)
    f.line([(11,-17+a),(11,-11+a)],WOOD); f.rect(9,-12+a,13,-10+a,IRON_L); f.px(9,-12+a,STEEL_L)
    f.poly([(0,-44),(4,-37),(3,-33),(0,-30),(-3,-33),(-4,-37)],KAJI)
    f.poly([(0,-44),(2,-40),(0,-38)],KAJI_L)
    if not p.get('dead'):
        f.px(-2,-36,GOLD_L); f.px(-1,-35,GOLD); f.px(2,-36,GOLD_L); f.px(1,-35,GOLD)
    random.seed(ph*7+3)
    for _ in range(5):
        f.px(random.randint(-8,8),random.randint(-44,-28),random.choice((GOLD,GOLD_L,GOLD_D)))
    if a: 
        for _ in range(3): f.px(11+random.randint(-3,3),-9+a+random.randint(-2,1),GOLD_L)

def glitch(im,amt,seed):
    if not amt: return im
    random.seed(seed); W,H=im.size; out=im.copy()
    for y in range(H):
        if random.random()<.25:
            row=im.crop((0,y,W,y+1)); out.paste((0,0,0,0),(0,y,W,y+1)); out.paste(row,(random.choice((-amt,amt)),y))
    return out

# ---------------- PASKUNJI ----------------
def wing(f,s,wy):
    sh=(5*s,-28); sh2=(5*s,-19); tip=(23*s,-25+wy); mid=(14*s,-30+wy*.5)
    pts=[sh,mid,tip]; jags=[]
    n=6
    for k in range(1,n+1):
        t=k/(n+1)
        bx=tip[0]+(sh2[0]-tip[0])*t; by=tip[1]+(sh2[1]-tip[1])*t+2+(3 if k%2 else 0)
        pts.append((bx,by)); 
        if k%2: jags.append((bx,by))
    pts.append(sh2)
    f.poly(pts,BIRD)
    f.line([(8*s,-25),(18*s,-24+wy*.8)],BIRD_D)
    f.line([(8*s,-22),(15*s,-20+wy*.7)],BIRD_D)
    f.line([sh,mid,tip],CYAN_D); f.px(*tip,CYAN); f.px(mid[0],mid[1],CYAN)
    for j in jags: f.px(*j,CYAN)

def pask(f,p):
    wy=p.get('wy',0)
    f.poly([(-3,-15),(3,-15),(6,-3),(2,-6),(0,-1),(-2,-6),(-6,-3)],BIRD)
    f.px(-6,-3,CYAN); f.px(6,-3,CYAN); f.px(0,-1,CYAN); f.line([(0,-14),(0,-4)],BIRD_D)
    wing(f,-1,wy); wing(f,1,wy)
    f.ell(-6,-31,6,-14,BIRD); f.ell(-4,-26,4,-16,BIRD_L)
    f.ell(-4,-37,4,-28,BIRD)
    f.line([(-1,-37),(-3,-41)],BIRD); f.line([(1,-37),(2,-41)],BIRD); f.px(-3,-41,CYAN_D); f.px(2,-41,CYAN_D)
    f.poly([(-1,-31),(1,-31),(0,-27)],GOLD_D)
    if not p.get('dead'): f.px(-2,-33,GOLD_L); f.px(2,-33,GOLD_L); f.px(-3,-33,GOLD); f.px(3,-33,GOLD)
    else: f.px(-2,-33,BIRD_D); f.px(2,-33,BIRD_D)

# ---------------- VESHAPI ----------------
def vesh(f,p):
    A=p.get('A',3); ph=p.get('ph',0); N=14
    pts=[]
    for i in range(N):
        x=-19+i*2.0; y=-20+A*math.sin(i*.6-ph); r=1+(i/(N-1))*3.2
        pts.append((x,y,r))
    tx,ty,_=pts[0]; f.poly([(tx+1,ty),(tx-4,ty-3),(tx-3,ty+3)],GOLD_D); f.px(tx-4,ty-3,GOLD)
    for x,y,r in pts: f.ell(x-r,y-r+1.2,x+r,y+r+1.2,CYAN)
    for i,(x,y,r) in enumerate(pts):
        f.ell(x-r,y-r-.5,x+r,y+r-.5,SCALE_M)
        if i%3==1 and i<N-1: f.poly([(x-1,y-r),(x+1,y-r),(x-2,y-r-3)],GOLD)
    for i,(x,y,r) in enumerate(pts):
        if i%2 and r>1.5: f.px(x,y-r+.5,SCALE_L)
    x,y,r=pts[-1]; hx,hy=x+3,y
    f.ell(hx-4,hy-4,hx+4,hy+3,SCALE_M)
    f.poly([(hx+2,hy-3),(hx+8,hy-1),(hx+8,hy+1),(hx+2,hy+3)],SCALE_M)
    f.line([(hx+1,hy+2),(hx+7,hy+1)],CYAN)
    f.px(hx+7,hy-1,SCALE)
    f.line([(hx-2,hy-4),(hx-5,hy-8)],BONE); f.line([(hx,hy-4),(hx-1,hy-8)],BONE)
    f.poly([(hx-3,hy+1),(hx-6,hy+4),(hx-2,hy+3)],GOLD)
    if not p.get('dead'): f.px(hx+2,hy-2,GOLD_L); f.px(hx+3,hy-2,WHITE)
    else: f.px(hx+2,hy-2,SCALE)
    f.px(hx-1,hy-3,SCALE_L)

# ---------------- ARMORED DEVI ----------------
def adevi(f,p):
    b=p.get('b',0); lg=p.get('lg',0)
    l1=max(0,round(2*lg)); l2=max(0,round(-2*lg))
    f.rect(-9,-12,-3,-l1,IRON_D); f.rect(3,-12,9,-l2,IRON_D)
    f.rect(-10,-2-l1,-2,-l1,IRON); f.rect(2,-2-l2,10,-l2,IRON)
    f.px(-6,-7-l1,IRON_L); f.px(6,-7-l2,IRON_L)
    f.rect(-11,-31+b,11,-12+b,IRON)
    f.rect(-11,-31+b,11,-31+b,GOLD_D)
    f.rect(-7,-28+b,7,-18+b,IRON_L); f.rect(-6,-27+b,6,-19+b,IRON)
    f.rect(-11,-15+b,11,-13+b,IRON_D); f.rect(-2,-15+b,1,-13+b,GOLD)
    f.px(0,-24+b,GOLD_L); f.px(-1,-23+b,GOLD); f.px(1,-23+b,GOLD); f.px(0,-22+b,GOLD); f.px(0,-25+b,GOLD)
    for s in (-1,1):
        f.rect(s*16,-27+b,s*12,-14+b,IRON_D)
        f.rect(s*17,-15+b,s*12,-11+b,IRON_L)
        f.ell(s*18,-35+b,s*8,-25+b,IRON)
        f.line([(s*17,-28+b),(s*9,-28+b)],GOLD_D)
        f.poly([(s*14,-34+b),(s*16,-38+b),(s*12,-34+b)],BONE)
    f.rect(-6,-41+b,6,-31+b,IRON); f.rect(-6,-41+b,6,-41+b,IRON_L)
    f.line([(0,-42+b),(0,-38+b)],GOLD)
    f.rect(-6,-32+b,6,-32+b,GOLD_D)
    f.rect(-4,-37+b,4,-36+b,OUT)
    if not p.get('dead'): f.px(-2,-37+b,CYAN); f.px(2,-37+b,CYAN); f.px(-3,-37+b,CYAN_D); f.px(3,-37+b,CYAN_D)
    f.line([(0,-35+b),(0,-33+b)],OUT)
    for s in (-1,1):
        f.poly([(s*6,-39+b),(s*11,-42+b),(s*11,-46+b),(s*8,-40+b)],BONE)
        f.px(s*11,-46+b,BONE_D)

def shield(frame=0):
    f=Fr(32,cx=16,by=16)
    f.ell(-14,-14,14,14,IRON_L); f.ell(-13,-13,13,13,IRON); f.ell(-10,-10,10,10,IRON_D)
    for k in range(8):
        a=k*math.pi/4; f.px(12*math.cos(a),12*math.sin(a),GOLD)
    g=GOLD_L if frame%2==0 else GOLD
    # sun/borjgali-like rune
    f.px(0,0,WHITE)
    for k in range(4):
        a=k*math.pi/2+(frame*math.pi/16)
        x1,y1=5*math.cos(a),5*math.sin(a)
        x2,y2=x1+3*math.cos(a+math.pi/2),y1+3*math.sin(a+math.pi/2)
        f.line([(0,0),(x1,y1),(x2,y2)],g)
    for k in range(8):
        a=k*math.pi/4+.3; f.px(8*math.cos(a),8*math.sin(a),CYAN if k%2 else CYAN_D)
    return finish(f.im,rim=(170,190,230),rim_amt=.3)

# ---------------- CHINKA ----------------
def chinka(f,p):
    lg=p.get('lg',0); b=p.get('b',0); ph=p.get('ph',0); look=p.get('look',0)
    sack=p.get('sack','held'); sk=p.get('sk',(0,0))
    l1=max(0,round(2*lg)); l2=max(0,round(-2*lg))
    f.line([(-3,-6),(-4+lg,-l1)],IMP_D,2); f.line([(3,-6),(4-lg,-l2)],IMP_D,2)
    f.px(-5+lg,-l1,IMP_L); f.px(5-lg,-l2,IMP_L)
    f.ell(-5,-15+b,5,-4+b,IMP)
    hb=b+(2 if p.get('dead') else 0)
    f.poly([(-5,-22+hb),(-15,-27+hb),(-7,-17+hb)],IMP); f.poly([(-6,-21+hb),(-12,-25+hb),(-7,-19+hb)],IMP_L)
    f.poly([(5,-22+hb),(15,-27+hb),(7,-17+hb)],IMP); f.poly([(6,-21+hb),(12,-25+hb),(7,-19+hb)],IMP_L)
    f.ell(-6,-26+hb,6,-14+hb,IMP)
    f.px(-4,-25+hb,IMP_L); f.px(-3,-26+hb,IMP_L)
    if not p.get('dead'):
        f.rect(-4,-22+hb,-2,-20+hb,WHITE); f.rect(2,-22+hb,4,-20+hb,WHITE)
        f.px(-3+look,-21+hb,OUT); f.px(3+look,-21+hb,OUT)
        f.line([(-3,-17+hb),(3,-17+hb)],WHITE); f.px(-4,-18+hb,OUT); f.px(4,-18+hb,OUT)
    else:
        f.line([(-4,-21+hb),(-2,-21+hb)],OUT); f.line([(2,-21+hb),(4,-21+hb)],OUT)
        f.line([(-2,-17+hb),(2,-17+hb)],OUT)
    random.seed(ph)
    if sack=='held':
        sx,sy=sk
        f.ell(0+sx,-15+b+sy,11+sx,-3+b+sy,GOLD_D); f.ell(2+sx,-14+b+sy,9+sx,-6+b+sy,GOLD)
        f.px(4+sx,-12+b+sy,GOLD_L); f.px(5+sx,-12+b+sy,GOLD_L); f.px(3+sx,-11+b+sy,WHITE)
        f.rect(4+sx,-17+b+sy,7+sx,-15+b+sy,WOOD)
        f.line([(-4,-11+b),(2+sx,-9+b+sy)],IMP_D,2)
        for _ in range(3): f.px(5+sx+random.randint(-7,7),-10+b+sy+random.randint(-9,6),random.choice((GOLD_L,WHITE)))
    else:
        f.line([(-4,-11+b),(-8,-16+b)],IMP_D,2); f.line([(4,-11+b),(8,-16+b)],IMP_D,2)

# ---------------- KUDIANI ----------------
def kud(f,p):
    ph=p.get('ph',0); arm=p.get('arm',0); orb=p.get('orb',0); bolt=p.get('bolt',None)
    random.seed(ph)
    f.line([(-16,-10),(14,-14)],WOOD,2); f.px(14,-15,WOOD_D)
    f.poly([(-15,-12),(-23,-16),(-24,-6),(-15,-8)],STRAW)
    for k in range(3): f.line([(-16,-10+k-1),(-23,-14+k*3)],WOOD_D)
    hem=[(10,-10),(7,-6+random.randint(0,1)),(4,-9),(1,-5+random.randint(0,1)),(-2,-9),(-5,-5+random.randint(0,1)),(-8,-9),(-10,-7)]
    f.poly([(-7,-31),(6,-31),(10,-10)]+hem,ROBE)
    f.poly([(-7,-31),(-10,-26),(-9,-15),(-5,-28)],ROBE_L)
    f.line([(2,-29),(3,-12)],ROBE_D)
    f.ell(-4,-35,4,-27,HAG); f.px(3,-31,HAG_D); f.px(4,-30,HAG_D)
    f.poly([(0,-31),(2,-31),(1,-27)],HAG_D)
    if not p.get('dead'): f.px(-2,-32,CYAN); f.px(2,-32,CYAN)
    else: f.px(-2,-32,HAG_D); f.px(2,-32,HAG_D)
    f.line([(-4,-30),(-5,-24)],HAIRW_D); f.line([(4,-30),(5,-24)],HAIRW_D)
    f.rect(-9,-35,9,-34,ROBE_D)
    f.poly([(-6,-35),(6,-35),(2,-40),(6,-46),(-1,-41),(-4,-38)],ROBE)
    f.px(6,-46,GOLD_D); f.line([(-5,-36),(5,-36)],GOLD_D)
    f.line([(-6,-27),(-4,-13)],HAG_D,2); f.px(-4,-12,HAG)
    hand={0:(6,-15),1:(11,-37),2:(15,-24)}[arm]
    f.line([(6,-28),((6+hand[0])/2+1,(-28+hand[1])/2),hand],ROBE_L,2)
    f.px(hand[0],hand[1],HAG); f.px(hand[0]+1,hand[1]-1,HAG_D); f.px(hand[0]-1,hand[1]-1,HAG_D)
    for _ in range(3 if arm else 2):
        f.px(hand[0]+random.randint(-3,3),hand[1]+random.randint(-3,2),random.choice((CYAN,GOLD_L)))
    if orb:
        ox,oy=hand[0],hand[1]-3-orb
        f.ell(ox-orb-1,oy-orb-1,ox+orb+1,oy+orb+1,CYAN_D); f.ell(ox-orb,oy-orb,ox+orb,oy+orb,CYAN)
        f.px(ox,oy,WHITE); f.px(ox+1,oy,GOLD_L)
    if bolt:
        bx,by,sz=bolt
        for k in range(1,6): f.px(bx-k*2,by-k,CYAN_D if k>2 else CYAN)
        f.ell(bx-sz,by-sz,bx+sz,by+sz,CYAN); f.px(bx,by,WHITE); f.px(bx+1,by+1,GOLD_L)

# ---------------- CINDER-WHELP ----------------
CRACKS=[[(-6,-20),(-3,-16),(-5,-12),(-2,-8)],[(4,-22),(2,-17),(6,-13),(4,-10)],[(-9,-11),(-5,-8)],[(8,-9),(4,-6),(6,-4)],[(-1,-25),(1,-22)]]
def whelp(f,p):
    lv=p.get('lv',0); lg=p.get('lg',0); sw=p.get('sw',1.0); ph=p.get('ph',0)
    l1=max(0,round(1.5*lg)); l2=max(0,round(-1.5*lg))
    f.rect(-5,-4,-3,-l1,EMBER_D); f.rect(3,-4,5,-l2,EMBER_D)
    r=10*sw; h=22*sw
    f.ell(-r,-4-h,r,-3,EMBER); f.ell(-r+2,-4-h*.55,r-2,-3,EMBER_D)
    f.px(-r*.6,-4-h*.8,(70,54,66)); f.px(-r*.5,-4-h*.85,(70,54,66))
    cols=[(GOLD_D,CYAN_D),(GOLD,CYAN),(GOLD_L,WHITE)][lv]
    for i,c in enumerate(CRACKS):
        pts=[(x*sw,-4+(y+4)*sw) for x,y in c]
        f.line(pts,cols[i%2])
    ey=-4-h*.7
    f.line([(-r-1,ey+6),(-r-3,ey+3)],EMBER); f.line([(r+1,ey+6),(r+3,ey+3)],EMBER)
    if not p.get('dead'):
        f.rect(-5,ey,-3,ey+2,WHITE); f.rect(3,ey,5,ey+2,WHITE)
        f.px(-4,ey+1,OUT); f.px(4,ey+1,OUT)
        f.line([(-6,ey-2),(-3,ey-3)],EMBER_D); f.line([(3,ey-3),(6,ey-2)],EMBER_D)
        f.ell(-1,ey+4,1,ey+6,OUT)
    else:
        f.line([(-5,ey+1),(-3,ey+1)],OUT); f.line([(3,ey+1),(5,ey+1)],OUT)
