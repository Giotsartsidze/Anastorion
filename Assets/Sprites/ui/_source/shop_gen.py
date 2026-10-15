"""Shop UI set matching the premium menu: panel, item-row frame, gold buy button. 9-sliceable."""
from PIL import Image, ImageDraw, ImageFont
import os
os.makedirs('out', exist_ok=True)
OUT=(12,12,20)
GOLD_D=(150,96,26); GOLD=(224,168,52); GOLD_L=(255,226,138)
NAVY_D=(16,18,34); NAVY=(26,30,54); NAVY_L=(40,46,80)

def grid(w,h): return [[None]*w for _ in range(h)]
def save(g,name):
    h=len(g); w=len(g[0]); im=Image.new('RGBA',(w,h),(0,0,0,0))
    for y in range(h):
        for x in range(w):
            if g[y][x]: im.putpixel((x,y),g[y][x]+(255,))
    im.save('out/'+name); return im
def rect(g,x0,y0,x1,y1,c):
    h=len(g); w=len(g[0])
    for y in range(max(0,y0),min(h,y1+1)):
        for x in range(max(0,x0),min(w,x1+1)): g[y][x]=c

def gold_frame(w,h, inner):
    """dark outline + gold bevel band + inner fill color."""
    g=grid(w,h)
    rect(g,0,0,w-1,h-1,OUT)
    rect(g,1,1,w-2,h-2,GOLD_D)
    rect(g,2,2,w-3,h-3,GOLD)
    rect(g,2,2,w-3,2,GOLD_L); rect(g,2,2,2,h-3,GOLD_L)           # top/left sheen
    rect(g,w-3,3,w-3,h-3,GOLD_D); rect(g,3,h-3,w-3,h-3,GOLD_D)   # bottom/right shade
    rect(g,4,4,w-5,h-5,OUT)                                       # inner lip
    rect(g,5,5,w-6,h-6,inner)                                     # inner fill
    return g

# PANEL 64x48, border 12  (the whole shop window)
save(gold_frame(64,48, NAVY_D), 'shop_panel.png')
# inner subtle inset line on the panel for depth
# (kept simple; the frame already reads well)

# ITEM ROW 48x24, border 8 (one upgrade row background)
g=gold_frame(48,24, NAVY)
rect(g,5,5,42,5,NAVY_L)   # faint top highlight inside the row
save(g,'shop_item_frame.png')

# BUY BUTTON 32x20, border 6 (raised gold button; use Button color tint for hover/press)
g=grid(32,20)
rect(g,0,0,31,19,OUT)
rect(g,1,1,30,18,GOLD_D)
rect(g,2,2,29,17,GOLD)
rect(g,2,2,29,3,GOLD_L)       # top sheen
rect(g,2,2,3,17,GOLD_L)
rect(g,2,16,29,17,GOLD_D)     # bottom shade
save(g,'shop_button.png')

# ---- MOCKUP of one styled row so the look is clear ----
scale=6
panel=Image.open('out/shop_panel.png').resize((64*8,48*8),Image.NEAREST)
row=Image.open('out/shop_item_frame.png').resize((420,96),Image.NEAREST)
btn=Image.open('out/shop_button.png').resize((150,70),Image.NEAREST)
shard=Image.open('../../Anastorion/Anastorion/Assets/Sprites/pickups/shard.png') if os.path.exists('../../Anastorion/Anastorion/Assets/Sprites/pickups/shard.png') else None

mock=Image.new('RGBA',(480,130),(14,16,30,255))
mock.alpha_composite(row,(30,17))
d=ImageDraw.Draw(mock)
try:
    f_big=ImageFont.truetype('/System/Library/Fonts/Supplemental/Georgia Bold.ttf',26)
    f_sm=ImageFont.truetype('/System/Library/Fonts/Supplemental/Georgia.ttf',18)
except:
    f_big=ImageFont.load_default(); f_sm=f_big
d.text((60,34),"Max Health",font=f_big,fill=(255,236,190,255))
d.text((60,70),"Lvl 2 / 5",font=f_sm,fill=(150,160,190,255))
d.text((300,40),"120",font=f_big,fill=(255,226,120,255))
mock.alpha_composite(btn,(300,70))
d.text((318,84),"BUY",font=f_sm,fill=(40,28,10,255))
mock.save('shop_mock.png')
print('ok')
