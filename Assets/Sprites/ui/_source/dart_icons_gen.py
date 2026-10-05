"""Two distinct dart-upgrade icons: dart_count (three darts) and dart_firerate
(one dart + speed streaks). 32x32, cosmic palette, matches the other upgrade icons."""
from PIL import Image, ImageDraw
import math

GOLD=(236,176,56); GOLD_L=(255,228,140); CYAN=(96,222,244); WHITE=(246,244,230)
S=32; C=S//2

def canvas(): return Image.new('RGBA',(S,S),(0,0,0,0))
def glow(im,col,r=14,strength=90):
    base=Image.new('RGBA',(S,S),(0,0,0,0)); p=base.load()
    for y in range(S):
        for x in range(S):
            dd=math.hypot(x-C+0.5,y-C+0.5)
            if dd<r: p[x,y]=(col[0],col[1],col[2],int(strength*(1-dd/r)**2))
    base.alpha_composite(im); return base

def dart(d,cx,cy,ln=8,col=GOLD):
    d.line([(cx-ln,cy),(cx+ln,cy)],fill=col+(255,),width=2)                # shaft
    d.polygon([(cx+ln,cy),(cx+ln-4,cy-3),(cx+ln-4,cy+3)],fill=col+(255,))  # head
    d.point([(cx+ln-1,cy)],fill=WHITE+(255,))                             # tip glint

# DART COUNT — three darts (multishot)
im=canvas(); d=ImageDraw.Draw(im)
for oy in (-7,0,7): dart(d,C,C+oy,ln=8,col=GOLD)
glow(im,GOLD).save('dart_count.png')

# DART FIRE RATE — one dart + speed streaks
im=canvas(); d=ImageDraw.Draw(im)
dart(d,C+1,C,ln=9,col=GOLD_L)
for oy,lx in [(-4,11),(0,13),(4,11)]:
    d.line([(C-lx,C+oy),(C-4,C+oy)],fill=CYAN+(220,),width=1)
glow(im,CYAN).save('dart_firerate.png')
print('ok')
