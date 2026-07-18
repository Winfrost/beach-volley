"""
Beach Volley - menu placeholder sprite generator.

Generates pixel-art PLACEHOLDER sprites for the themed main menu at the
project's native pixel scale (PPU 25, internal 480x270). Everything is drawn
1:1 in native pixels; Unity upscales x4 with Filter Point -> crisp pixels.

These are throwaway placeholders to lock the LAYOUT. Swap with final art
later (same file names / sizes) without touching the scene.

All sizes/palette are named constants (no magic numbers scattered around).
Run: python3 gen_menu_placeholders.py
"""

from PIL import Image, ImageDraw, ImageFont
import os

OUT_DIR = os.path.join(os.path.dirname(__file__), "Menu")
os.makedirs(OUT_DIR, exist_ok=True)

# ---- Palette (beach warm, from progetto) -------------------------------------
SKY        = (134, 197, 224)   # light azure
SKY_HI     = (176, 220, 238)
SUN        = (255, 210, 74)
SUN_CORE   = (255, 230, 130)
SEA        = (58, 176, 200)    # turchese
SEA_DEEP   = (27, 58, 87)      # blu profondo
SAND       = (232, 216, 160)   # sabbia
SAND_DK    = (201, 174, 120)
MOUNTAIN   = (110, 130, 150)
MOUNTAIN_2 = (140, 158, 176)
PALM_TRUNK = (150, 100, 52)
PALM_LEAF  = (62, 142, 79)
PALM_LEAF2 = (86, 168, 100)
NET        = (255, 246, 230)

WOOD       = (169, 116, 60)
WOOD_DK    = (110, 74, 36)
WOOD_HI    = (200, 154, 94)
WOOD_LINE  = (132, 90, 46)

CASTLE     = (232, 210, 160)
CASTLE_DK  = (201, 174, 120)
CASTLE_HI  = (245, 230, 190)
DOOR       = (110, 74, 36)
FLAG       = (242, 107, 94)    # corallo

BOX_BODY   = (150, 158, 168)   # boombox metal
BOX_DK     = (96, 104, 116)
BOX_HI     = (196, 202, 210)
SPEAKER    = (52, 58, 68)
SPEAKER_HI = (96, 104, 116)

TRACK_BG   = (110, 74, 36)
TRACK_FILL = (58, 176, 200)
BALL_BASE  = (255, 246, 230)
BALL_RED   = (242, 107, 94)
BALL_BLUE  = (58, 176, 200)
OUTLINE    = (46, 32, 18)

TRANSPARENT = (0, 0, 0, 0)

# ---- Asset sizes (native px @ PPU 25) ----------------------------------------
BG_W, BG_H          = 480, 270
SIGN_W, SIGN_H      = 160, 54     # 9-slice border 6 ; reused for Gioca + Torneo
CASTLE_SIZES = {                   # (w, h, n_towers) -> difficolta crescente
    "easy":   (44, 40, 2),
    "medium": (58, 52, 3),
    "hard":   (72, 64, 4),
}
BOOMBOX_W, BOOMBOX_H = 120, 72
TRACK_W, TRACK_H     = 48, 10      # 9-slice border 4 (horizontal)
BALL_D               = 14
POST_W, POST_H       = 14, 110     # vertical post the two signs are nailed to


def new_sprite(w, h):
    return Image.new("RGBA", (w, h), TRANSPARENT)


# ============================================================ BACKGROUND =======
def build_background():
    img = Image.new("RGB", (BG_W, BG_H), SKY)
    d = ImageDraw.Draw(img)

    # sky gradient (two flat bands, pixel-art style)
    d.rectangle([0, 0, BG_W, 40], fill=SKY_HI)

    # sun (top-right), simple disc + core
    sx, sy, r = 410, 52, 26
    d.ellipse([sx - r, sy - r, sx + r, sy + r], fill=SUN)
    d.ellipse([sx - r + 7, sy - r + 7, sx + r - 7, sy + r - 7], fill=SUN_CORE)

    horizon = 150
    # distant mountains
    d.polygon([(60, horizon), (150, 96), (240, horizon)], fill=MOUNTAIN)
    d.polygon([(180, horizon), (270, 108), (360, horizon)], fill=MOUNTAIN_2)
    d.polygon([(300, horizon), (390, 100), (470, horizon)], fill=MOUNTAIN)

    # sea band
    d.rectangle([0, horizon, BG_W, 188], fill=SEA)
    d.rectangle([0, horizon, BG_W, horizon + 6], fill=SEA_DEEP)
    # a couple of wave lines
    for wy in (162, 174):
        for wx in range(0, BG_W, 32):
            d.rectangle([wx, wy, wx + 14, wy + 2], fill=SKY_HI)

    # sand band
    d.rectangle([0, 188, BG_W, BG_H], fill=SAND)
    d.rectangle([0, 188, BG_W, 194], fill=SAND_DK)
    # sand speckles
    for (px, py) in [(70, 230), (110, 250), (300, 220), (360, 244), (200, 258)]:
        d.rectangle([px, py, px + 2, py + 2], fill=SAND_DK)

    # net (accent, center-right, behind UI)
    net_x = 360
    d.rectangle([net_x, 120, net_x + 2, 210], fill=NET)       # right pole
    d.rectangle([250, 118, 252, 150], fill=NET)               # left pole (short)
    d.rectangle([250, 120, net_x + 2, 122], fill=NET)         # top cord
    for gx in range(252, net_x, 10):                          # mesh (light)
        d.line([(gx, 122), (gx, 148)], fill=(255, 246, 230, 90))
    for gy in range(126, 148, 8):
        d.line([(252, gy), (net_x, gy)], fill=NET)

    _palm(d, 30, 200, flip=False)     # left palm
    _palm(d, 452, 196, flip=True)     # right palm

    # tiny crab (decor)
    d.rectangle([150, 250, 158, 254], fill=FLAG)
    d.rectangle([148, 248, 150, 250], fill=FLAG)
    d.rectangle([158, 248, 160, 250], fill=FLAG)

    img.save(os.path.join(OUT_DIR, "bg_menu.png"))


def _palm(d, base_x, base_y, flip=False):
    s = -1 if flip else 1
    # trunk
    for i in range(7):
        d.rectangle([base_x + s * i * 2, base_y - i * 12,
                     base_x + s * i * 2 + 6, base_y - i * 12 + 12],
                    fill=PALM_TRUNK)
    top_x = base_x + s * 12
    top_y = base_y - 84
    # fronds (four simple leaves)
    for (dx, dy, col) in [(-30, -6, PALM_LEAF), (30, -6, PALM_LEAF),
                          (-18, -22, PALM_LEAF2), (18, -22, PALM_LEAF2)]:
        d.polygon([(top_x, top_y), (top_x + dx, top_y + dy),
                   (top_x + dx // 2, top_y + dy + 10)], fill=col)


# ============================================================ WOOD SIGN ========
def build_sign():
    img = new_sprite(SIGN_W, SIGN_H)
    d = ImageDraw.Draw(img)
    b = 3  # outline
    # plank body
    d.rectangle([0, 0, SIGN_W - 1, SIGN_H - 1], fill=WOOD, outline=OUTLINE, width=b)
    # top/bottom highlight & shadow (keeps 9-slice edges readable)
    d.rectangle([b, b, SIGN_W - 1 - b, b + 4], fill=WOOD_HI)
    d.rectangle([b, SIGN_H - 1 - b - 4, SIGN_W - 1 - b, SIGN_H - 1 - b], fill=WOOD_DK)
    # wood grain lines
    for gy in (18, 30, 42):
        d.line([(b + 4, gy), (SIGN_W - b - 4, gy)], fill=WOOD_LINE)
    # nails in the corners
    for (nx, ny) in [(10, 10), (SIGN_W - 12, 10), (10, SIGN_H - 12), (SIGN_W - 12, SIGN_H - 12)]:
        d.rectangle([nx, ny, nx + 3, ny + 3], fill=WOOD_DK)
    img.save(os.path.join(OUT_DIR, "ui_sign.png"))


def build_post():
    """Vertical wooden post: decorative, sits BEHIND the two sign buttons."""
    img = new_sprite(POST_W, POST_H)
    d = ImageDraw.Draw(img)
    d.rectangle([0, 0, POST_W - 1, POST_H - 1], fill=WOOD, outline=OUTLINE, width=1)
    d.rectangle([1, 1, 3, POST_H - 2], fill=WOOD_HI)          # lit side
    d.rectangle([POST_W - 4, 1, POST_W - 2, POST_H - 2], fill=WOOD_DK)  # shaded side
    for gy in range(8, POST_H - 4, 13):                        # grain
        d.line([(4, gy), (POST_W - 5, gy)], fill=WOOD_LINE)
    img.save(os.path.join(OUT_DIR, "ui_sign_post.png"))


# ============================================================ SAND CASTLES =====
def build_castle(name, w, h, n_towers):
    img = new_sprite(w, h)
    d = ImageDraw.Draw(img)
    body_top = h - int(h * 0.55)
    # main body
    d.rectangle([2, body_top, w - 3, h - 1], fill=CASTLE, outline=OUTLINE, width=1)
    d.rectangle([2, body_top, w - 3, body_top + 3], fill=CASTLE_HI)
    d.rectangle([2, h - 5, w - 3, h - 1], fill=CASTLE_DK)
    # door
    door_w = max(6, w // 7)
    door_x = w // 2 - door_w // 2
    d.rectangle([door_x, h - int(h * 0.32), door_x + door_w, h - 1], fill=DOOR)

    # towers spread across the width
    tower_w = max(7, w // (n_towers + 1))
    gap = (w - n_towers * tower_w) // (n_towers + 1)
    tx = gap
    tallest = h - body_top
    for i in range(n_towers):
        # center towers slightly taller -> castle silhouette
        extra = tallest // 2 if i == n_towers // 2 else tallest // 4
        t_top = body_top - extra
        d.rectangle([tx, t_top, tx + tower_w - 1, body_top + 2],
                    fill=CASTLE, outline=OUTLINE, width=1)
        d.rectangle([tx, t_top, tx + tower_w - 1, t_top + 2], fill=CASTLE_HI)
        # crenellations (two notches on top)
        d.rectangle([tx + tower_w // 3, t_top - 3, tx + tower_w // 3 + 2, t_top], fill=CASTLE)
        d.rectangle([tx + 2 * tower_w // 3, t_top - 3, tx + 2 * tower_w // 3 + 2, t_top], fill=CASTLE)
        # flag on the center tower
        if i == n_towers // 2:
            d.line([(tx + tower_w // 2, t_top - 3), (tx + tower_w // 2, t_top - 10)], fill=OUTLINE)
            d.polygon([(tx + tower_w // 2, t_top - 10), (tx + tower_w // 2 + 6, t_top - 8),
                       (tx + tower_w // 2, t_top - 6)], fill=FLAG)
        tx += tower_w + gap
    img.save(os.path.join(OUT_DIR, f"castle_{name}.png"))


# ============================================================ BOOMBOX ==========
def build_boombox():
    img = new_sprite(BOOMBOX_W, BOOMBOX_H)
    d = ImageDraw.Draw(img)
    # handle
    d.arc([BOOMBOX_W // 2 - 30, -8, BOOMBOX_W // 2 + 30, 24], 180, 360, fill=BOX_DK, width=3)
    # body
    d.rectangle([4, 14, BOOMBOX_W - 5, BOOMBOX_H - 3], fill=BOX_BODY, outline=OUTLINE, width=2)
    d.rectangle([6, 16, BOOMBOX_W - 7, 20], fill=BOX_HI)
    d.rectangle([6, BOOMBOX_H - 8, BOOMBOX_W - 7, BOOMBOX_H - 4], fill=BOX_DK)
    # two speakers
    for cx in (26, BOOMBOX_W - 26):
        d.ellipse([cx - 13, 34, cx + 13, 60], fill=SPEAKER, outline=OUTLINE)
        d.ellipse([cx - 5, 42, cx + 5, 52], fill=SPEAKER_HI)
    # cassette window (center)
    d.rectangle([BOOMBOX_W // 2 - 16, 30, BOOMBOX_W // 2 + 16, 44], fill=SPEAKER, outline=OUTLINE)
    d.ellipse([BOOMBOX_W // 2 - 10, 34, BOOMBOX_W // 2 - 4, 40], fill=BOX_HI)
    d.ellipse([BOOMBOX_W // 2 + 4, 34, BOOMBOX_W // 2 + 10, 40], fill=BOX_HI)
    # two slots where the UI sliders will sit (visual hint only)
    d.rectangle([BOOMBOX_W // 2 - 16, 50, BOOMBOX_W // 2 + 16, 53], fill=BOX_DK)
    d.rectangle([BOOMBOX_W // 2 - 16, 58, BOOMBOX_W // 2 + 16, 61], fill=BOX_DK)
    img.save(os.path.join(OUT_DIR, "ui_boombox.png"))


# ============================================================ SLIDER PARTS =====
def build_slider_track(fill_color, fname):
    img = new_sprite(TRACK_W, TRACK_H)
    d = ImageDraw.Draw(img)
    d.rounded_rectangle([0, 0, TRACK_W - 1, TRACK_H - 1], radius=4,
                        fill=fill_color, outline=OUTLINE, width=1)
    d.rectangle([2, 2, TRACK_W - 3, 3], fill=(255, 255, 255, 60))
    img.save(os.path.join(OUT_DIR, fname))


def build_ball():
    img = new_sprite(BALL_D, BALL_D)
    d = ImageDraw.Draw(img)
    d.ellipse([0, 0, BALL_D - 1, BALL_D - 1], fill=BALL_BASE, outline=OUTLINE)
    # two colored wedges (beach ball)
    d.pieslice([0, 0, BALL_D - 1, BALL_D - 1], 200, 260, fill=BALL_RED)
    d.pieslice([0, 0, BALL_D - 1, BALL_D - 1], 20, 80, fill=BALL_BLUE)
    d.ellipse([BALL_D // 2 - 2, 2, BALL_D // 2, 4], fill=(255, 255, 255, 180))
    img.save(os.path.join(OUT_DIR, "beach_ball.png"))


# ============================================================ PREVIEW ==========
def build_preview():
    """Composite the placeholders into the wireframe layout (for review only)."""
    scale = 3
    bg = Image.open(os.path.join(OUT_DIR, "bg_menu.png")).convert("RGBA")
    canvas = bg.copy()

    post = Image.open(os.path.join(OUT_DIR, "ui_sign_post.png"))
    canvas.alpha_composite(post, (106, 80))       # behind: drawn first
    sign = Image.open(os.path.join(OUT_DIR, "ui_sign.png"))
    canvas.alpha_composite(sign, (40, 70))
    canvas.alpha_composite(sign, (40, 140))

    for i, name in enumerate(["easy", "medium", "hard"]):
        c = Image.open(os.path.join(OUT_DIR, f"castle_{name}.png"))
        canvas.alpha_composite(c, (176 + i * 34, 210 - c.height))

    box = Image.open(os.path.join(OUT_DIR, "ui_boombox.png"))
    canvas.alpha_composite(box, (346, 128))
    ball = Image.open(os.path.join(OUT_DIR, "beach_ball.png"))
    canvas.alpha_composite(ball, (352, 172))
    canvas.alpha_composite(ball, (392, 180))

    d = ImageDraw.Draw(canvas)
    try:
        font = ImageFont.load_default()
    except Exception:
        font = None
    d.text((70, 84), "GIOCA", fill=(255, 246, 230), font=font)
    d.text((66, 154), "TORNEO", fill=(255, 246, 230), font=font)
    d.text((362, 116), "Musica  SFX", fill=(255, 246, 230), font=font)

    big = canvas.resize((BG_W * scale, BG_H * scale), Image.NEAREST)
    big.save(os.path.join(OUT_DIR, "_preview_menu.png"))


def main():
    build_background()
    build_sign()
    build_post()
    for name, (w, h, n) in CASTLE_SIZES.items():
        build_castle(name, w, h, n)
    build_boombox()
    build_slider_track(TRACK_BG, "ui_slider_track.png")
    build_slider_track(TRACK_FILL, "ui_slider_fill.png")
    build_ball()
    build_preview()
    print("Generated in:", OUT_DIR)
    for f in sorted(os.listdir(OUT_DIR)):
        p = os.path.join(OUT_DIR, f)
        im = Image.open(p)
        print(f"  {f:26s} {im.size[0]}x{im.size[1]}")


if __name__ == "__main__":
    main()
