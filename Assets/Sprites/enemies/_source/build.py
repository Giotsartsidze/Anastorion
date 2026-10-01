from core import *
from enemies1 import *
from enemies2 import *
import boss, os, shutil
S=math.sin; TAU=2*math.pi
OUTD='enemies'; shutil.rmtree(OUTD,ignore_errors=True); os.makedirs(OUTD+'/_previews')
RIMC=(150,210,255)

def fin_std(alpha=255,aura=None,rim=RIMC):
    return lambda im,i: finish(im,rim=rim,alpha=alpha,aura=aura,frame=i)

sheets={}
def emit(name,anim,frames):
    d=f'{OUTD}/{name}'; os.makedirs(d,exist_ok=True)
    save_strip(frames,f'{d}/{name}_{anim}.png')
    sheets.setdefault(name,{})[anim]=frames
    W=frames[0].size[0]; sc=6 if W==48 else 3
    dur={'idle':170,'walk':100,'death':120}.get(anim,110)
    g=[]
    for fr_ in frames:
        bg=Image.new('RGBA',fr_.size,(14,16,34,255)); bg.alpha_composite(fr_)
        g.append(bg.convert('RGB').resize((W*sc,W*sc),Image.NEAREST))
    g[0].save(f'{OUTD}/_previews/{name}_{anim}.gif',save_all=True,append_images=g[1:],duration=dur,loop=0)

def build(name,draw,idle,walk,extra={},fin=None,fly=False,death=None,post=None):
    make=mk(draw); fin=fin or fin_std()
    def F(p,i,**k):
        im=make(p,**k); im=fin(im,i)
        return post(im,p,i) if post else im
    emit(name,'idle',[F(p,i,sx=p.get('sx',1),sy=p.get('sy',1)) for i,p in enumerate(idle)])
    emit(name,'walk',[F(p,i,sx=p.get('sx',1),sy=p.get('sy',1)) for i,p in enumerate(walk)])
    for an,ps in extra.items(): emit(name,an,[F(p,i,sx=p.get('sx',1),sy=p.get('sy',1)) for i,p in enumerate(ps)])
    if death: emit(name,'death',death(make,fin))
    else: emit(name,'death',std_death(lambda p,**k: make({**idle[0],**p},**k),lambda im,i: fin(im,i),fly=fly,seed=len(name)))

# Devi grunt
build('devi_grunt',devi,
  [dict(b=b,ph=i) for i,b in enumerate((0,0,1,1))],
  [dict(lg=S(i/6*TAU),b=int(abs(S(i/6*TAU))>.5),sw=round(2*S(i/6*TAU)),ox=round(S(i/6*TAU)),ph=i) for i in range(6)])
# Ali
build('ali',ali,
  [dict(sy=s,sx=2-s,ph=i) for i,s in enumerate((1,.95,.92,.95))],
  [dict(sy=.85,sx=1.1,ph=0),dict(sy=.7,sx=1.2,ph=1),dict(sy=1.25,sx=.85,oy=-6,air=1,ph=2),
   dict(sy=1.0,sx=1.05,oy=-12,air=1,ph=3),dict(sy=1.15,sx=.9,oy=-5,air=1,ph=4),dict(sy=.75,sx=1.2,ph=5)],
  fin=fin_std(alpha=185))
# Kaji
KA_I=(200,140,210,70); KA_W=(210,170,90,200,55,180)
build('kaji',kaji,
  [dict(ph=i,al=a) for i,a in enumerate(KA_I)],
  [dict(lg=round(2*S(i/6*TAU)),ph=i,al=a) for i,a in enumerate(KA_W)],
  fin=lambda im,i: im, post=lambda im,p,i: glitch(finish(im,alpha=p.get('al',190)),1 if p.get('al',190)<120 else 0,i))
# Paskunji
build('paskunji',pask,
  [dict(wy=w,oy=o) for w,o in ((-4,0),(-2,-1),(0,-1),(-2,0))],
  [dict(wy=w,oy=o) for w,o in ((-16,1),(-6,0),(4,-1),(10,-2),(2,-1),(-8,0))],fly=True)
# Veshapi
build('veshapi_hatchling',vesh,
  [dict(A=2,ph=i/4*TAU,oy=round(S(i/4*TAU))) for i in range(4)],
  [dict(A=4,ph=i/6*TAU) for i in range(6)],fly=True)
# Armored Devi
build('armored_devi',adevi,
  [dict(b=b) for b in (0,0,1,1)],
  [dict(lg=S(i/6*TAU),b=(0,1,2,0,1,2)[i]) for i in range(6)])
# Chinka
def chinka_death(make,fin):
    base=dict(ph=0)
    fr=[finish(flash(make(dict(hit=1))),rim=None)]
    fr.append(fin(make(dict(sk=(3,-10),oy=-1,ph=1)),1))
    im=fin(make(dict(sack='none',dead=1),sx=1.15,sy=.6),2)
    f=Fr(48); f.im=im; f.d=ImageDraw.Draw(im)
    random.seed(5)
    for _ in range(26):
        a=random.random()*TAU; r=random.randint(3,14); f.px(10+r*math.cos(a),-10+r*math.sin(a),random.choice((GOLD,GOLD_L,WHITE,GOLD_D)))
    fr.append(im)
    fr+= [dissolve(im,k,r,n,9+j,cols=(GOLD,GOLD_L,WHITE,CYAN)) for j,(k,r,n) in enumerate(((.45,7,40),(.12,12,30),(0,18,14)))]
    return fr
build('chinka',chinka,
  [dict(b=b,look=l,ph=i) for i,(b,l) in enumerate(((0,-1),(0,0),(1,1),(1,0)))],
  [dict(lg=S(i/6*TAU),b=2*int(abs(S(i/6*TAU))>.5),ox=round(S(i/6*TAU)),sk=(0,-int(abs(S(i/6*TAU))>.5)),look=1,ph=i) for i in range(6)],
  death=chinka_death)
# Kudiani
build('kudiani',lambda f,p:(setattr(f,'ox',f.ox+3),kud(f,p)),
  [dict(oy=o,ph=i) for i,o in enumerate((0,-1,-2,-1))],
  [dict(oy=o,ph=i) for i,o in enumerate((0,-1,-2,-2,-1,0))],
  extra={'attack':[dict(arm=1,ph=0),dict(arm=1,orb=1,ph=1),dict(arm=1,orb=3,ph=2),dict(arm=2,bolt=(20,-24,2),ph=3),dict(arm=2,bolt=(24,-22,1),ph=4)]},fly=True)
# Cinder-whelp
def whelp_post(im,p,i):
    if p.get('boom'):
        f=Fr(48); f.im=im
        for (x,y) in opaque(im): im.putpixel((x,y),C(WHITE) if (x+y)%3 else C(GOLD_L))
        for k in range(12):
            a=k*TAU/12
            for r in range(15,22): f.px(r*math.cos(a),-16+r*math.sin(a),GOLD_L if r%2 else WHITE)
    return im
build('cinder_whelp',whelp,
  [dict(lv=l,sw=s) for l,s in ((0,1),(1,1.03),(0,1),(1,1.03))],
  [dict(lg=S(i/6*TAU),ox=round(S(i/6*TAU)),lv=i%2,sw=1) for i in range(6)],
  extra={'attack':[dict(sw=1.1,lv=1),dict(sw=1.22,lv=2),dict(sw=1.32,lv=2),dict(sw=1.38,lv=2,boom=1)]},post=whelp_post)
# Tkash-Mapa
GA=(GOLD,GOLD_D)
build('tkash_mapa',tkash,
  [dict(b=b,hs=h,ph=i) for i,(b,h) in enumerate(((0,0),(0,1),(1,0),(1,-1)))],
  [dict(hs=round(2*S(i/6*TAU)),b=int(abs(S(i/6*TAU))>.5),ph=i) for i in range(6)],
  extra={'summon':[dict(arm=.35,sig=0),dict(arm=.7,sig=1,ph=1),dict(arm=1,sig=2,ph=2),dict(arm=1,sig=3,ph=3),dict(arm=1,sig=4,ph=4,fall=1)]},
  fin=lambda im,i: finish(im,rim=(255,236,170),aura=GA,frame=i))
# Rokapi
build('rokapi',rokapi,
  [dict(spin=i*.8,spread=.3+.05*(i%2),inten=i%2) for i in range(4)],
  [dict(lg=round(2*S(i/6*TAU)),spin=i*.8,inten=i%2) for i in range(6)],
  extra={'channel':[dict(spread=1,spin=i*1.25,inten=2 if i%2 else 1,conv=20-i*4) for i in range(5)]})
# Kva-Devi (crumble death)
def kva_death(make,fin):
    base=fin(make(dict()),0)
    fr=[finish(flash(make(dict(hit=1))),rim=None)]
    shake=fin(make(dict(glow=2,ox=1)),1); fr.append(shake)
    random.seed(4); chunks=[]
    for cy in range(0,48,4):
        for cx in range(0,48,4):
            c=base.crop((cx,cy,cx+4,cy+4))
            if c.getbbox(): chunks.append((cx,cy,c,random.uniform(.8,1.6),random.randint(-2,2)))
    for k,t in enumerate((4,10,18,30)):
        im=Image.new('RGBA',(48,48),(0,0,0,0))
        for cx,cy,c,v,dx in chunks:
            ny=min(cy+int(t*v*(cy/48+.4)),44-random.randint(0,4)); nx=cx+int(dx*t/10)
            im.alpha_composite(c,(max(0,min(44,nx)),max(0,ny)))
        if k>=2: im=dissolve(im,1.0 if k==2 else .8,8,20 if k==2 else 26,k)
        fr.append(im)
    return fr
build('kva_devi',kva,
  [dict(b=b,glow=g) for b,g in ((0,0),(0,1),(1,0),(1,1))],
  [dict(lg=S(i/6*TAU),b=(0,1,1,0,1,1)[i],glow=i%2) for i in range(6)],
  extra={'cast':[dict(r=.45,glow=1),dict(r=1,glow=1),dict(r=1,glow=2),dict(slam=1,glow=2,oy=1),dict(slam=1,wall=1,glow=2)]},
  death=kva_death)
# Ochokochi
build('ochokochi',ocho,
  [dict(b=b) for b in (0,0,1,1)],
  [dict(lg=S(i/6*TAU),b=int(abs(S(i/6*TAU))>.5),ox=round(S(i/6*TAU))) for i in range(6)],
  extra={'windup':[dict(sy=1.1,oy=-2,lean=-1),dict(lg=1,dust=1,lean=-1,oy=-1),dict(lg=-1,dust=2,lean=-1,oy=-1),dict(sy=.82,sx=1.15,lean=2,speed=6,oy=1)]})

# Shield
os.makedirs(f'{OUTD}/armored_devi',exist_ok=True)
shield(0).save(f'{OUTD}/armored_devi/armored_devi_shield.png')
save_strip([shield(i) for i in range(4)],f'{OUTD}/armored_devi/armored_devi_shield_glow.png')

# Boss
B=boss.make; bf=lambda im,i: finish(im,rim=(150,210,255),rim_amt=.3)
def BA(ps): return [bf(B(p,sx=p.get('sx',1),sy=p.get('sy',1)),i) for i,p in enumerate(ps)]
name='gveleshapi_boss'
emit(name,'idle',BA([dict(ph=i*TAU/4,nx=(0,1,1,0)[i],hy=(0,-1,-2,-1)[i],jaw=(.2,.25,.3,.25)[i]) for i in range(4)]))
emit(name,'move',BA([dict(ph=i*TAU/6,nx=round(3*S(i/6*TAU)),hy=round(S(i/3*TAU))) for i in range(6)]))
emit(name,'attack_bite',BA([dict(hy=-4,hs=.92,jaw=.3,nx=-2),dict(hy=-7,hs=.86,jaw=.6,nx=-2),dict(hy=-8,hs=.85,jaw=1.0),
   dict(hy=12,hs=1.25,jaw=1.0),dict(hy=14,hs=1.3,jaw=.05),dict(hy=4,hs=1.05,jaw=.2)]))
emit(name,'attack_nova',BA([dict(hy=-5,ecl=.5,jaw=.3),dict(hy=-8,ecl=.8,gather=26,jaw=.5,ph=1),dict(hy=-9,ecl=1,gather=18,jaw=.9,ph=2),
   dict(hy=-9,ecl=1,nova=24,jaw=1),dict(hy=-7,ecl=.9,nova=38,jaw=.7),dict(hy=-4,ecl=.5,nova=48,jaw=.3)]))
bd=[finish(flash(B(dict())),rim=None)]
for i,(p,sx,sy) in enumerate(((dict(dead=1,hy=6,nx=4,jaw=.6),1,1),(dict(dead=1,hy=16,nx=6,jaw=.7,ecl=.1),1.05,.85),(dict(dead=1,hy=24,nx=6,jaw=.5,ecl=0),1.15,.6))):
    bd.append(bf(B(p,sx=sx,sy=sy),i))
src=opaque(bd[-1])
for k,(kp,r,n) in enumerate(((.6,10,120),(.3,22,130),(.08,36,90),(0,50,40))):
    bd.append(dissolve(bd[-1] if k==0 else bd[3],kp,r,n,k,big=True,src=src))
emit(name,'death',bd)

# contact sheet
rows=[]
for n,an in sheets.items():
    for a,frs in an.items(): rows.append((n,a,frs))
Wmax=max(sum(f.size[0] for f in frs) for _,_,frs in rows)
Ht=sum(frs[0].size[1] for _,_,frs in rows)
cs=Image.new('RGBA',(Wmax,Ht),(14,16,34,255)); y=0
for n,a,frs in rows:
    x=0
    for f in frs: cs.alpha_composite(f,(x,y)); x+=f.size[0]
    y+=frs[0].size[1]
cs.save(f'{OUTD}/_previews/_contact_sheet.png')
cs.resize((cs.width*3,cs.height*3),Image.NEAREST).save('review_full.png')
print(len(rows),'strips')
