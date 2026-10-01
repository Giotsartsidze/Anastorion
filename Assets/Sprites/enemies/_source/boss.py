from core import *

def seg(f,x,y,r,belly=False):
    f.ell(x-r,y-r,x+r,y+r,SCALE)
    f.ell(x-r+1,y-r+1,x+r-2,y+r-3,SCALE_M)
    f.px(x-r*.4,y-r*.5,SCALE_L); f.px(x+r*.2,y-r*.6,SCALE_L)
    if belly: f.line([(x-r*.5,y+r*.5),(x+r*.5,y+r*.5)],CYAN_D)

def gvele(f,p):
    ph=p.get('ph',0); nx=p.get('nx',0); hy=p.get('hy',0); hs=p.get('hs',1.0); jaw=p.get('jaw',0.2)
    ecl=p.get('ecl',0.3); cs=p.get('cs',1.0); dead=p.get('dead',0); nova=p.get('nova',0); gather=p.get('gather',0)
    HC=(nx,-56+hy)
    # eclipse aura behind head
    R=12+10*ecl
    for k in range(0,360,4):
        a=math.radians(k); rr=R+2+(k//4+ph)%3
        f.px(HC[0]+rr*math.cos(a),HC[1]-4+rr*math.sin(a),GOLD if (k//4)%3 else GOLD_L)
    f.ell(HC[0]-R-1,HC[1]-4-R-1,HC[0]+R+1,HC[1]-4+R+1,GOLD_D)
    f.ell(HC[0]-R,HC[1]-4-R,HC[0]+R,HC[1]-4+R,VOID)
    if ecl>.8:
        for k in range(12):
            a=k*math.pi/6+ph*.2
            f.line([(HC[0]+(R+3)*math.cos(a),HC[1]-4+(R+3)*math.sin(a)),(HC[0]+(R+8)*math.cos(a),HC[1]-4+(R+8)*math.sin(a))],CYAN if k%2 else GOLD_L)
    # coil
    segs=[]
    N=30
    for i in range(N):
        a=math.pi*.5+i/N*2*math.pi*1.05
        rx=32*cs; ry=11
        x=rx*math.cos(a)+1.5*math.sin(i*.9-ph*2); y=-14+ry*math.sin(a)
        r=(3+7*(i/N))*(1+.06*math.sin(i*.7-ph*2))
        segs.append((math.sin(a),x,y,r))
    for d,x,y,r in sorted(segs): 
        seg(f,x,y,r)
        if d>0.3 and r>5: f.px(x,y-r+1,CYAN_D)
    # tail tip
    d,x,y,r=segs[0]; f.poly([(x,y),(x-7,y+3),(x-4,y-3)],GOLD_D)
    # wing-fins
    for s in (-1,1):
        bx,byy=HC[0]*.6+s*6,-44+hy*.5
        tip=(bx+s*30,byy-18); pts=[(bx,byy-4),(bx+s*14,byy-16),tip]
        for k in range(1,6):
            t=k/6; X=tip[0]+(bx-tip[0])*t; Y=tip[1]+(byy+6-tip[1])*t+(5 if k%2 else 0)
            pts.append((X,Y))
        pts.append((bx,byy+6))
        f.poly(pts,SCALE)
        f.line([(bx,byy-4),(bx+s*14,byy-16),tip],CYAN_D); f.px(*tip,CYAN)
        f.line([(bx+s*4,byy-2),(bx+s*22,byy-12)],SCALE_M)
    # neck
    M=14
    for i in range(M):
        t=i/(M-1)
        x=nx*t+4*math.sin(t*math.pi*1.4+ph)*(1-t); y=-8+(-52+hy+8)*t*(1)+0
        y=-8+( (HC[1]+8) +8)*t
        r=10-3*t
        seg(f,x,y,r)
        f.px(x,y+r*.3,GOLD_D if i%2 else CYAN_D); f.px(x+1,y+r*.3,GOLD_D if i%2 else CYAN_D)
    # head (scaled around HC)
    def H(x,y): return (HC[0]+x*hs,HC[1]+y*hs)
    def hp(pts,c): f.poly([H(*q) for q in pts],c)
    for s in (-1,1):
        f.line([H(s*7,-5),H(s*14,-13),H(s*16,-22)],BONE,3)
        f.line([H(s*16,-22),H(s*17,-24)],BONE_D,2)
        f.line([H(s*4,-7),H(s*7,-14)],BONE,2)
    hp([(-12,-6),(12,-6),(14,2),(8,6),(-8,6),(-14,2)],SCALE)
    hp([(-10,-5),(10,-5),(11,1),(-11,1)],SCALE_M)
    J=jaw*8
    # maw glow
    hp([(-8,5),(8,5),(6,9+J),(-6,9+J)],GOLD)
    hp([(-5,6),(5,6),(3,8+J*.8),(-3,8+J*.8)],CYAN)
    f.px(*H(0,7+J*.5),WHITE)
    # upper snout
    hp([(-9,2),(9,2),(7,8),(-7,8)],SCALE)
    hp([(-6,2),(6,2),(4,6),(-4,6)],SCALE_M)
    for x in (-6,-3,3,6): f.px(*H(x,8),WHITE)
    # lower jaw
    hp([(-7,8+J),(7,8+J),(5,12+J),(-5,12+J)],SCALE)
    for x in (-5,0,5): f.px(*H(x,8+J),WHITE)
    f.px(*H(-2,4),OUT); f.px(*H(2,4),OUT)
    # eyes
    for s in (-1,1):
        if not dead:
            f.line([H(s*4,-2),H(s*9,-4)],GOLD_L); f.px(*H(s*6,-3),WHITE); f.px(*H(s*9,-5),GOLD)
        else:
            f.line([H(s*4,-2),H(s*9,-3)],SCALE)
        f.line([H(s*3,-5),H(s*10,-7)],SCALE_L)
    if gather:
        random.seed(int(ph*10))
        for k in range(24):
            a=random.random()*2*math.pi; r=gather+random.randint(0,6)
            f.px(HC[0]+r*math.cos(a),HC[1]-4+r*math.sin(a),random.choice((CYAN,GOLD_L,WHITE,VOID)))
    if nova:
        for rr,c in ((nova,WHITE),(nova-2,GOLD_L),(nova-4,CYAN),(nova-7,GOLD)):
            if rr<=0: continue
            for k in range(0,360,2):
                a=math.radians(k); f.px(HC[0]+rr*math.cos(a),HC[1]-4+rr*math.sin(a)*.85,c)

def make(p,sx=1,sy=1,ox=0,oy=0):
    f=Fr(96,cx=48,by=92,sx=sx,sy=sy,ox=ox,oy=oy); gvele(f,p); return f.im
