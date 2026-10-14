"""Seamless horizontal chain-link strip for the Chain Lash LineRenderer (tiles along the line)."""
from PIL import Image, ImageDraw
import os
os.makedirs('out', exist_ok=True)

W, H = 24, 14          # one period = 2 links; tiles horizontally
STEEL_D=(70,76,98); STEEL=(140,148,178); STEEL_L=(214,220,240)

# Draw on a triple-width canvas with a period-12 pattern, then crop the middle W
# so the result tiles seamlessly.
CW = W*3
im = Image.new('RGBA', (CW, H), (0,0,0,0))
d = ImageDraw.Draw(im)
cy = H/2

def flat_link(cx):
    # horizontal oval ring
    d.ellipse([cx-6, cy-5, cx+6, cy+5], outline=STEEL_D+(255,), width=3)
    d.ellipse([cx-6, cy-5, cx+6, cy+5], outline=STEEL+(255,), width=2)
    d.arc([cx-6, cy-5, cx+6, cy+5], start=200, end=340, fill=STEEL_L+(255,), width=1)  # top sheen

def edge_link(cx):
    # thin vertical oval ring, sits between two flats (the interlock)
    d.ellipse([cx-2, cy-6, cx+2, cy+6], outline=STEEL_D+(255,), width=3)
    d.ellipse([cx-2, cy-6, cx+2, cy+6], outline=STEEL_L+(255,), width=1)

# period 12: flat at 6, edge at 12, across the whole triple canvas
x = 0
while x < CW + 12:
    flat_link(x+6)
    edge_link(x+12)
    x += 12

tile = im.crop((W, 0, 2*W, H))   # middle strip tiles seamlessly
tile.save('out/chain_link.png')

# preview: tiled x6 on dark
prev = Image.new('RGBA', (W*6, H), (24,26,36,255))
for i in range(6): prev.alpha_composite(tile, (i*W, 0))
prev.resize((prev.width*6, prev.height*6), Image.NEAREST).save('chain_tex_preview.png')
print('ok')
