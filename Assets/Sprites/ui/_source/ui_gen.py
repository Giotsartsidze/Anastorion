"""UI frames for Anastorion — 9-sliceable. Light/white so the code's rarity color
tints the whole card; dark translucent center so text/icon stay readable.
Outputs: card_frame.png (96x96, border 14), bar_frame.png + bar_fill.png (HUD).
"""
from PIL import Image, ImageDraw

WHITE = (246, 244, 230)
PANEL = (12, 13, 22, 190)

# ---- CARD FRAME (9-slice, border=14) ----
S = 96
im = Image.new('RGBA', (S, S), (0, 0, 0, 0))
d = ImageDraw.Draw(im)
d.rectangle([2, 2, S-3, S-3], fill=PANEL)                       # translucent panel
d.rectangle([1, 1, S-2, S-2], outline=WHITE + (255,), width=2)  # outer frame
d.rectangle([6, 6, S-7, S-7], outline=WHITE + (160,), width=1)  # inner line
# corner accents (only corners — safe for 9-slice stretching)
def corner_diamond(cx, cy):
    d.polygon([(cx, cy-3), (cx+3, cy), (cx, cy+3), (cx-3, cy)], fill=WHITE + (255,))
for (cx, cy) in [(9, 9), (S-10, 9), (9, S-10), (S-10, S-10)]:
    corner_diamond(cx, cy)
im.save('card_frame.png')

# ---- HUD BAR (9-slice horizontal: frame + fill) ----
BW, BH, bb = 64, 16, 6
frame = Image.new('RGBA', (BW, BH), (0, 0, 0, 0))
fd = ImageDraw.Draw(frame)
fd.rectangle([1, 1, BW-2, BH-2], fill=(10, 11, 18, 210))
fd.rectangle([0, 0, BW-1, BH-1], outline=WHITE + (255,), width=2)
frame.save('bar_frame.png')

fill = Image.new('RGBA', (BW, BH), (0, 0, 0, 0))
fl = ImageDraw.Draw(fill)
fl.rectangle([0, 0, BW-1, BH-1], fill=WHITE + (255,))      # white → tint per bar (HP red, XP cyan)
fl.rectangle([0, 0, BW-1, 3], fill=(255, 255, 255, 120))   # top sheen
fill.save('bar_fill.png')
print('ok')
