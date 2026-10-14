"""Replace the cactus with a weathered carved idol (standing stone) — more in-world."""
from PIL import Image
import os
os.makedirs('out', exist_ok=True)
OUT=(42,28,16)
STONE_D=(120,96,66); STONE=(170,140,98); STONE_L=(208,182,138); DARK=(30,22,16)

class P:
    def __init__(s,w=32,h=32): s.w,s.h=w,h; s.g=[[None]*w for _ in range(h)]
    def px(s,x,y,c):
        if 0<=x<s.w and 0<=y<s.h and c: s.g[y][x]=c
    def rect(s,x0,y0,x1,y1,c):
        for y in range(y0,y1+1):
            for x in range(x0,x1+1): s.px(x,y,c)
    def outline(s):
        add=[]
        for y in range(s.h):
            for x in range(s.w):
                if s.g[y][x] is None:
                    for a,b in ((1,0),(-1,0),(0,1),(0,-1),(1,1),(-1,-1),(1,-1),(-1,1)):
                        nx,ny=x+a,y+b
                        if 0<=nx<s.w and 0<=ny<s.h and s.g[ny][nx] not in (None,OUT): add.append((x,y)); break
        for x,y in add: s.px(x,y,OUT)
    def img(s):
        im=Image.new('RGBA',(s.w,s.h),(0,0,0,0))
        for y in range(s.h):
            for x in range(s.w):
                if s.g[y][x]: im.putpixel((x,y),s.g[y][x]+(255,))
        return im

p=P()
# body of the menhir/idol (slightly irregular column)
p.rect(11,6,20,28,STONE)
p.rect(11,6,12,28,STONE_L)     # lit left edge
p.rect(19,6,20,28,STONE_D)     # shaded right edge
# rounded/weathered top with two small horns
p.px(12,5,STONE); p.px(13,4,STONE_L); p.rect(14,4,17,4,STONE); p.px(18,4,STONE_D)
p.px(11,4,STONE); p.px(20,4,STONE_D)          # stubby horns
p.px(11,3,STONE_L); p.px(20,3,STONE_D)
# carved face
p.rect(13,12,14,14,DARK); p.rect(17,12,18,14,DARK)   # eye hollows
p.px(15,15,DARK); p.px(16,15,DARK)                   # nose
p.rect(13,18,18,19,DARK)                              # mouth slit
# carved markings on the lower body
for y in range(22,27,2):
    p.px(13,y,STONE_D); p.px(15,y,STONE_D); p.px(17,y,STONE_D)
# sand-buried base
p.rect(9,28,22,30,STONE_D); p.rect(9,28,22,28,STONE)
p.outline()
p.img().save('out/desert_idol.png')

# preview
p.img().resize((32*8,32*8),Image.NEAREST).save('idol_preview.png')
print('ok')
