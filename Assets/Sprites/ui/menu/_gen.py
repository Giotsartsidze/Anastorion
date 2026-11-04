from PIL import Image, ImageDraw
import math, os
os.makedirs('out', exist_ok=True)
GOLD=(216,176,106); GOLD_L=(246,221,166); GOLD_HI=(255,246,218); GOLD_D=(138,106,51)
OUT=(6,6,9); W=(246,244,235,255)

# ---------- button backgrounds (9-slice, border 8) ----------
def button(fill, fa, b1, b2, name):
    w,h=72,26
    im=Image.new('RGBA',(w,h),(0,0,0,0)); p=im.load()
    for y in range(h):
        for x in range(w): p[x,y]=(fill[0],fill[1],fill[2],fa)
    d=ImageDraw.Draw(im)
    d.rectangle([0,0,w-1,h-1], outline=OUT+(255,))             # outer dark edge
    d.rectangle([1,1,w-2,h-2], outline=b1+(225,))              # bright gold border
    d.rectangle([5,5,w-6,h-6], outline=b2+(150,))              # inner frame line
    for x in range(2,w-2): p[x,2]=(255,255,255,26)             # faint top sheen
    im.save('out/'+name)
button((16,14,18),235, GOLD_L, GOLD, 'menu_button.png')
button((36,28,18),242, GOLD_HI, GOLD_L, 'menu_button_hero.png')  # PLAY, warmer + brighter

# ---------- icons (white line art, tinted gold in Unity) ----------
def make(fn, out=48, S=192, wdt=11):
    im=Image.new('RGBA',(S,S),(0,0,0,0)); d=ImageDraw.Draw(im); fn(d,S,wdt)
    return im.resize((out,out), Image.LANCZOS)

def play(d,S,wd):
    d.ellipse([S*0.13,S*0.13,S*0.87,S*0.87], outline=W, width=wd)
    d.polygon([(S*0.41,S*0.33),(S*0.69,S*0.5),(S*0.41,S*0.67)], fill=W)
def lock(d,S,wd):
    d.rounded_rectangle([S*0.24,S*0.45,S*0.76,S*0.84], radius=S*0.05, outline=W, width=wd)
    d.arc([S*0.33,S*0.2,S*0.67,S*0.58], 180, 360, fill=W, width=wd)
    d.ellipse([S*0.45,S*0.58,S*0.55,S*0.68], fill=W)
def sliders(d,S,wd):
    d.line([(S*0.18,S*0.37),(S*0.82,S*0.37)], fill=W, width=wd)
    d.line([(S*0.18,S*0.63),(S*0.82,S*0.63)], fill=W, width=wd)
    d.ellipse([S*0.3,S*0.27,S*0.46,S*0.47], fill=(12,11,14,255), outline=W, width=wd)
    d.ellipse([S*0.54,S*0.53,S*0.7,S*0.73], fill=(12,11,14,255), outline=W, width=wd)
def sun(d,S,wd):
    c=S/2; d.ellipse([S*0.37,S*0.37,S*0.63,S*0.63], outline=W, width=wd)
    for a in range(0,360,45):
        x1=c+math.cos(math.radians(a))*S*0.33; y1=c+math.sin(math.radians(a))*S*0.33
        x2=c+math.cos(math.radians(a))*S*0.45; y2=c+math.sin(math.radians(a))*S*0.45
        d.line([(x1,y1),(x2,y2)], fill=W, width=wd)
def door(d,S,wd):
    d.line([(S*0.28,S*0.18),(S*0.28,S*0.82)], fill=W, width=wd)
    d.line([(S*0.28,S*0.18),(S*0.55,S*0.18)], fill=W, width=wd)
    d.line([(S*0.28,S*0.82),(S*0.55,S*0.82)], fill=W, width=wd)
    d.line([(S*0.45,S*0.5),(S*0.82,S*0.5)], fill=W, width=wd)
    d.line([(S*0.68,S*0.38),(S*0.84,S*0.5)], fill=W, width=wd)
    d.line([(S*0.68,S*0.62),(S*0.84,S*0.5)], fill=W, width=wd)
def chev(d,S,wd):
    d.line([(S*0.4,S*0.25),(S*0.65,S*0.5)], fill=W, width=wd)
    d.line([(S*0.4,S*0.75),(S*0.65,S*0.5)], fill=W, width=wd)

for fn,n in [(play,'icon_play'),(lock,'icon_shop'),(sliders,'icon_modifier'),
             (sun,'icon_settings'),(door,'icon_quit'),(chev,'icon_chevron')]:
    make(fn).save(f'out/{n}.png')

# ---------- preview mock of one assembled row ----------
scale=8
bg=Image.new('RGBA',(620,120),(18,16,22,255))
btn=Image.open('out/menu_button_hero.png').resize((560,96),Image.NEAREST)
bg.alpha_composite(btn,(30,12))
ic=Image.open('out/icon_play.png').resize((60,60),Image.LANCZOS)
# tint icon gold
tin=Image.new('RGBA',ic.size,(236,200,120,0))
for yy in range(ic.height):
    for xx in range(ic.width):
        r,g,b,a=ic.getpixel((xx,yy))
        if a>0: tin.putpixel((xx,yy),(236,200,120,a))
bg.alpha_composite(tin,(70,30))
cv=Image.open('out/icon_chevron.png').resize((34,34),Image.LANCZOS)
cvt=Image.new('RGBA',cv.size,(0,0,0,0))
for yy in range(cv.height):
    for xx in range(cv.width):
        r,g,b,a=cv.getpixel((xx,yy))
        if a>0: cvt.putpixel((xx,yy),(246,221,166,a))
bg.alpha_composite(cvt,(545,42))
d=ImageDraw.Draw(bg)
try:
    from PIL import ImageFont
    f=ImageFont.truetype('/System/Library/Fonts/Supplemental/Georgia Bold.ttf',40)
except: f=None
d.text((150,38),"P L A Y",font=f,fill=(246,221,166,255))
bg.save('menu_preview.png')

# contact sheet of icons
sheet=Image.new('RGBA',(6*60,70),(18,16,22,255))
for i,n in enumerate(['icon_play','icon_shop','icon_modifier','icon_settings','icon_quit','icon_chevron']):
    ic=Image.open(f'out/{n}.png').resize((48,48),Image.LANCZOS)
    t=Image.new('RGBA',ic.size,(0,0,0,0))
    for yy in range(ic.height):
        for xx in range(ic.width):
            r,g,b,a=ic.getpixel((xx,yy))
            if a>0: t.putpixel((xx,yy),(236,200,120,a))
    sheet.alpha_composite(t,(i*60+6,11))
sheet.save('icons_preview.png')
print('ok')
