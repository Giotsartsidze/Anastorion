from core import *

# ---------------- TKASH-MAPA ----------------
def sigil(f,stage,ph):
    if not stage: return
    cy=-40; rx=[0,6,11,13,12][stage]; ry=rx*.38
    col=GOLD_L if stage==3 else GOLD
    for k in range(0,360,6):
        a=math.radians(k); f.px(rx*math.cos(a),cy+ry*math.sin(a),col)
    if stage>=2:
        P=[(rx*.75*math.cos(math.radians(k*72-90+ph*10)),cy+ry*.75*math.sin(math.radians(k*72-90+ph*10))) for k in range(5)]
        for k in range(5): f.line([P[k],P[(k+2)%5]],CYAN)
    if stage==3:
        for k in range(-12,13,4): f.line([(k,cy-2),(k,cy-6-abs(k)%3)],WHITE)
        f.px(0,cy,WHITE)

def tkash(f,p):
    arm=p.get('arm',0); hs=p.get('hs',0); b=p.get('b',0); ph=p.get('ph',0)
    f.poly([(-6,-30+b),(6,-30+b),(8+hs,-12),(4,-10),(-4,-10),(-8+hs,-12)],HAIRW)
    for x in (-5,-2,2,5): f.line([(x,-28+b),(x*1.3+hs,-13)],HAIRW_D)
    f.poly([(-5,-24+b),(5,-24+b),(11+hs,0),(-11+hs,0)],ROBE)
    f.poly([(-5,-24+b),(-2,-24+b),(-7+hs,0),(-11+hs,0)],ROBE_L)
    f.line([(-11+hs,-1),(11+hs,-1)],GOLD_D); f.line([(-10+hs,-2),(10+hs,-2)],GOLD)
    f.line([(0,-23+b),(hs*.5,-2)],GOLD_D)
    for y in (-18,-12,-6): f.px(hs*(24+y)/24*.5-1,y,GOLD); f.px(hs*(24+y)/24*.5+1,y,GOLD)
    f.line([(-4,-24+b),(0,-19+b),(4,-24+b)],GOLD)
    for s in (-1,1):
        hx=s*(9+2*arm); hy=-9+b-27*arm
        f.line([(s*5,-23+b),(s*(8+arm),(-23-9-27*arm)/2+b),(hx,hy)],ROBE_L,3)
        f.px(hx,hy-(1 if arm>.5 else -1),PALE)
    f.ell(-4,-32+b,4,-23+b,PALE)
    f.line([(-4,-30+b),(-5,-20+b)],HAIRW); f.line([(4,-30+b),(5,-20+b)],HAIRW)
    f.rect(-4,-32+b,4,-31+b,HAIRW)
    if not p.get('dead'): f.px(-2,-28+b,CYAN); f.px(2,-28+b,CYAN); f.px(-2,-27+b,CYAN_D); f.px(2,-27+b,CYAN_D)
    else: f.px(-2,-28+b,HAIRW_D); f.px(2,-28+b,HAIRW_D)
    f.px(0,-25+b,(200,196,214))
    for s in (-1,1):
        f.line([(s*2,-32+b),(s*5,-36+b),(s*8,-38+b)],BONE)
        f.line([(s*5,-36+b),(s*5,-39+b)],BONE); f.line([(s*7,-37+b),(s*10,-36+b)],BONE)
        f.px(s*8,-38+b,GOLD_L)
    f.px(0,-33+b,GOLD_L)
    sigil(f,p.get('sig',0),ph)
    if p.get('fall'):
        random.seed(ph)
        for _ in range(10): f.px(random.choice((-1,1))*random.randint(8,20),-random.randint(0,30),random.choice((GOLD,CYAN,GOLD_L)))

# ---------------- ROKAPI ----------------
def chainlink(f,x,y,dx,dy,n):
    for i in range(n): f.px(x+dx*i,y+dy*i,STEEL_L if i%2 else CHAIN)

def rokapi(f,p):
    lg=p.get('lg',0); spread=p.get('spread',.3); spin=p.get('spin',0); inten=p.get('inten',0); conv=p.get('conv',None)
    f.line([(-3,-13),(-4+lg,-1)],ROK,2); f.line([(3,-13),(4-lg,-1)],ROK,2)
    f.rect(-5+lg,-1,-3+lg,0,ROK_D); f.rect(3-lg,-1,5-lg,0,ROK_D)
    f.rect(-4,-15,4,-12,ROK)
    f.line([(0,-16),(0,-12)],ROK_L)
    for s in (-1,1):
        hx=s*(10+9*spread); hy=-14-16*spread
        f.line([(s*7,-30),(s*(8+6*spread),-22-8*spread),(hx,hy)],ROK,2)
        f.px(hx+s,hy+1,BONE); f.px(hx,hy+2,BONE)
        chainlink(f,hx,hy+2,0,1,5+int(2*(1-spread)))
        f.px(hx+s,hy+2+5,STEEL)
    f.ell(-8,-31,8,-13,ROK)
    f.ell(-6,-29,6,-15,VOID)
    for yy in (-27,-24,-21,-18):
        f.line([(-8,yy),(-3,yy+1)],ROK_L); f.line([(3,yy+1),(8,yy)],ROK_L)
    cols=[(CYAN_D,SHD),(CYAN,CYAN_D),(WHITE,CYAN)][inten]
    for k in range(2,15):
        a=k*.75+spin; r=k*.4
        f.px(r*math.cos(a),-22+r*math.sin(a)*.95,cols[k%2] if k>3 else GOLD_L)
    f.px(0,-22,VOID)
    f.line([(-8,-30),(8,-16)],CHAIN); 
    for i in range(0,16,2): f.px(-8+i,-30+i*14/16,STEEL_L)
    f.rect(-9,-32,9,-30,ROK_L)
    f.ell(-4,-40,4,-31,ROK); f.rect(-3,-33,3,-31,ROK_D)
    f.rect(-3,-37,-1,-36,VOID); f.rect(1,-37,3,-36,VOID)
    if not p.get('dead'): f.px(-2,-37,CYAN); f.px(2,-37,CYAN)
    for x in (-2,0,2): f.px(x,-32,BONE)
    f.line([(-3,-39),(-6,-42),(-6,-44)],BONE_D); f.line([(3,-39),(6,-42),(6,-44)],BONE_D)
    if conv is not None:
        for k in range(10):
            a=k*math.pi/5+spin*.3; r=conv+(k%3)
            f.px(r*math.cos(a),-22+r*math.sin(a)*.9,[CYAN,GOLD_L,WHITE][k%3])
            f.px((r+2)*math.cos(a),-22+(r+2)*math.sin(a)*.9,CYAN_D)

# ---------------- KVA-DEVI ----------------
KRUNE=[[(-8,-28),(-5,-24),(-7,-19),(-4,-14)],[(7,-30),(5,-25),(8,-21)],[(2,-18),(4,-13),(1,-10)]]
def kva(f,p):
    raise_=p.get('r',0); slam=p.get('slam',0); wall=p.get('wall',0); glow=p.get('glow',0); b=p.get('b',0); lg=p.get('lg',0)
    l1=max(0,round(1.5*lg)); l2=max(0,round(-1.5*lg))
    f.rect(-11,-9,-4,-l1,STONE_D); f.rect(4,-9,11,-l2,STONE_D)
    f.rect(-12,-2-l1,-3,-l1,STONE); f.rect(3,-2-l2,12,-l2,STONE)
    def arms(front):
        for s in (-1,1):
            if slam:
                x0,x1,y0,y1=s*4,s*14,-11,0
            else:
                cx=s*(17-9*raise_); cy=-20-22*raise_+b
                x0,x1,y0,y1=cx-4,cx+4,cy-11,cy+11
            if front:
                f.rect(x0,y0,x1,y1,STONE); f.rect(x0,y0,x1,y0+2,STONE_L)
                f.rect(x0,y1-3,x1,y1,STONE_D)
                f.px((x0+x1)/2,(y0+y1)/2,GOLD if glow else GOLD_D)
                f.px((x0+x1)/2,(y0+y1)/2+2,CYAN if glow else CYAN_D)
                f.px(x0+1,y0,MOSS); f.px(x0+2,y0,MOSS_L)
    f.poly([(-12,-34+b),(12,-34+b),(15,-20+b),(12,-8),(-12,-8),(-15,-20+b)],STONE)
    f.poly([(-15,-20+b),(15,-20+b),(12,-8),(-12,-8)],STONE_D)
    f.poly([(-12,-34+b),(-2,-34+b),(-6,-26+b),(-14,-24+b)],STONE_L)
    rc=[(GOLD_D,CYAN_D),(GOLD,CYAN),(GOLD_L,WHITE)][glow]
    for i,c in enumerate(KRUNE): f.line([(x,y+b*(1 if y<-20 else 0)) for x,y in c],rc[i%2])
    for x in range(-11,12,2): 
        if (x*7)%5<3: f.px(x,-34+b,MOSS if x%4 else MOSS_L)
    f.rect(-5,-39+b,5,-32+b,STONE); f.rect(-5,-36+b,5,-36+b,STONE_D)
    f.rect(-5,-39+b,5,-39+b,MOSS)
    if not p.get('dead'): f.px(-2,-35+b,rc[0]); f.px(2,-35+b,rc[1])
    arms(True)
    if slam:
        random.seed(3)
        for _ in range(8): f.px(random.randint(-18,18),random.randint(-4,0),STONE_L)
    if wall:
        for i,x in enumerate(range(-20,21,5)):
            h=wall*(6+(i*3)%4)
            f.poly([(x-3,0),(x+3,0),(x+1,-h),(x-1,-h-1)],STONE if i%2 else STONE_L)
            f.px(x,-h/2,rc[i%2])

# ---------------- OCHOKOCHI ----------------
def ocho(f,p):
    b=p.get('b',0); lg=p.get('lg',0); lean=p.get('lean',0); dust=p.get('dust',0); speed=p.get('speed',0)
    l1=max(0,round(2*lg)); l2=max(0,round(-2*lg))
    f.rect(-10,-10,-4,-l1,FUR_D); f.rect(4,-10,10,-l2,FUR_D)
    for x in (-9,-7,-5): f.px(x,-l1,BONE)
    for x in (5,7,9): f.px(x,-l2,BONE)
    random.seed(11)
    for k in range(22):
        a=math.pi*2*k/22; x=14*math.cos(a); y=-20+b+12*math.sin(a)
        f.poly([(x*.85,y*.85-20*.15+b*.15),(x*1.15,y+(2 if y>-20 else -2)),(x*1.0+2,y)],FUR)
    f.ell(-14,-32+b,14,-8+b,FUR)
    for s in (-1,1):
        f.ell(s*19,-28+b,s*11,-6+b+lean,FUR)
        f.ell(s*18,-18+b,s*12,-6+b+lean,FUR_D)
        for k in range(3): f.px(s*(17-k*2),-5+b+lean,BONE)
    for k in range(12):
        f.px(random.randint(-12,12),random.randint(-30,-10)+b,FUR_L)
    f.poly([(-6,-25+b),(6,-25+b),(4,-12+b),(-4,-12+b)],BONE)
    for yy in (-22,-18,-15): f.line([(-4,yy+b),(4,yy+b)],BONE_D)
    f.line([(0,-24+b),(0,-13+b)],BONE_D)
    hy=-3*lean
    f.ell(-6,-36+b-hy,6,-25+b-hy,FUR_D)
    for x in (-5,-2,2,5): f.line([(x,-36+b-hy),(x*1.2,-39+b-hy)],FUR)
    if not p.get('dead'): f.px(-2,-31+b-hy,GOLD_L); f.px(2,-31+b-hy,GOLD_L); f.px(-3,-31+b-hy,GOLD_D); f.px(3,-31+b-hy,GOLD_D)
    f.rect(-3,-28+b-hy,3,-27+b-hy,OUT)
    f.line([(-3,-27+b-hy),(-4,-30+b-hy)],BONE); f.line([(3,-27+b-hy),(4,-30+b-hy)],BONE)
    if dust:
        random.seed(dust)
        for _ in range(7): f.px(random.randint(-14,-4),random.randint(-3,0),random.choice((STONE_L,BONE_D)))
    if speed:
        for x in (-12,-6,0,6,12): f.line([(x,-44),(x,-44+speed+(abs(x)%3))],WHITE)
