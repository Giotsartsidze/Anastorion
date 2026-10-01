"""Upgrade card icons for Anastorion — 32x32, cosmic palette, transparent.
One per in-run upgrade. Bold readable shapes with a soft glow.
"""
from PIL import Image, ImageDraw
import math

GOLD=(236,176,56); GOLD_L=(255,228,140); CYAN=(96,222,244); WHITE=(246,244,230); RED=(232,64,64)
S=32; C=S//2

def canvas(): return Image.new('RGBA',(S,S),(0,0,0,0))
def glow(im,col,r=14,strength=90):
    base=Image.new('RGBA',(S,S),(0,0,0,0)); p=base.load()
    for y in range(S):
        for x in range(S):
            d=math.hypot(x-C+0.5,y-C+0.5)
            if d<r: p[x,y]=(col[0],col[1],col[2],int(strength*(1-d/r)**2))
    base.alpha_composite(im); return base

def save(im,n): glow(im, CYAN if 'wisp' in n or 'pulse' in n or 'magnet' in n else GOLD).save(n)

# darts_speed — diagonal dart + streaks
im=canvas(); d=ImageDraw.Draw(im)
d.line([(5,27),(13,19)],fill=CYAN+(220,),width=2); d.line([(3,22),(10,15)],fill=CYAN+(160,),width=1)
d.polygon([(10,22),(24,8),(26,16),(18,18)],fill=GOLD+(255,)); d.polygon([(14,19),(24,9),(24,14)],fill=GOLD_L+(255,))
save(im,'darts_speed.png')

# pulse_cooldown — ring + clock hands
im=canvas(); d=ImageDraw.Draw(im)
d.ellipse([C-9,C-9,C+8,C+8],outline=CYAN+(255,),width=2)
d.line([(C,C),(C,C-6)],fill=GOLD_L+(255,),width=2); d.line([(C,C),(C+5,C+2)],fill=GOLD+(255,),width=2)
save(im,'pulse_cooldown.png')

# pulse_range — expanding rings
im=canvas(); d=ImageDraw.Draw(im)
for r,col,w in [(12,CYAN,1),(8,GOLD,2),(4,GOLD_L,2)]:
    d.ellipse([C-r,C-r,C+r,C+r],outline=col+(255,),width=w)
save(im,'pulse_range.png')

# magnet_range — horseshoe magnet + orb
im=canvas(); d=ImageDraw.Draw(im)
d.arc([C-9,C-11,C+8,C+6],start=180,end=360,fill=GOLD+(255,),width=4)
d.rectangle([C-9,C-2,C-6,C+8],fill=GOLD+(255,)); d.rectangle([C+5,C-2,C+8,C+8],fill=GOLD+(255,))
d.rectangle([C-9,C+6,C-6,C+9],fill=CYAN+(255,)); d.rectangle([C+5,C+6,C+8,C+9],fill=RED+(255,))
save(im,'magnet_range.png')

# max_health — heart
im=canvas(); d=ImageDraw.Draw(im)
d.ellipse([C-8,C-7,C-1,C],fill=RED+(255,)); d.ellipse([C,C-7,C+7,C],fill=RED+(255,))
d.polygon([(C-8,C-3),(C+7,C-3),(C,C+9)],fill=RED+(255,))
d.rectangle([C-1,C-4,C+1,C+2],fill=WHITE+(255,)); d.rectangle([C-3,C-2,C+3,C],fill=WHITE+(255,))
save(im,'max_health.png')

# move_speed — forward chevrons
im=canvas(); d=ImageDraw.Draw(im)
for off,col in [(-6,CYAN),(0,GOLD),(6,GOLD_L)]:
    d.line([(C-4+off,C-7),(C+4+off,C),(C-4+off,C+7)],fill=col+(255,),width=2)
save(im,'move_speed.png')

# speed_boost — lightning bolt
im=canvas(); d=ImageDraw.Draw(im)
d.polygon([(C+2,C-10),(C-6,C+1),(C-1,C+1),(C-3,C+10),(C+6,C-2),(C+1,C-2)],fill=GOLD_L+(255,))
d.line([(C+2,C-10),(C-6,C+1)],fill=WHITE+(200,),width=1)
save(im,'speed_boost.png')

# wisp_speed — wisp + motion arc
im=canvas(); d=ImageDraw.Draw(im)
d.arc([C-10,C-10,C+9,C+9],start=20,end=300,fill=CYAN+(200,),width=2)
d.ellipse([C-3,C-3,C+3,C+3],fill=CYAN+(255,)); d.ellipse([C-2,C-2,C+1,C+1],fill=WHITE+(255,))
save(im,'wisp_speed.png')

# wisp_count — three wisps
im=canvas(); d=ImageDraw.Draw(im)
for (ox,oy) in [(0,-6),(-6,5),(6,5)]:
    d.ellipse([C+ox-3,C+oy-3,C+ox+2,C+oy+2],fill=CYAN+(255,))
    d.ellipse([C+ox-1,C+oy-1,C+ox+1,C+oy+1],fill=WHITE+(255,))
save(im,'wisp_count.png')

print('ok')
