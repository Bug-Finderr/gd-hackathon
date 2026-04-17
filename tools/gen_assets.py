#!/usr/bin/env python3
"""WILL.EXE custom asset generator.

Uses Google Gemini 3 Pro Image Preview (Nano Banana Pro) for hero images:
grandpa-as-Epstein-handler reveal photos, fake flight manifest screenshot,
Minion meme, etc.

Key loaded from tools/.env (never committed). Never print key. Never pass as
CLI arg. Never write it into generated filenames or metadata.

Usage:
    cd tools/
    pip install -r requirements.txt
    python gen_assets.py                # generate all missing
    python gen_assets.py yacht          # regenerate one by id
    python gen_assets.py --force        # regenerate everything
"""
from __future__ import annotations

import os
import sys
from dataclasses import dataclass
from pathlib import Path

from dotenv import load_dotenv
from google import genai
from google.genai import types

ROOT = Path(__file__).resolve().parent.parent
OUT_DIR = ROOT / "Assets" / "Art" / "Generated"
MODEL = "gemini-3-pro-image-preview"  # Nano Banana Pro, highest-fidelity


@dataclass
class Asset:
    id: str
    filename: str
    prompt: str


ASSETS: list[Asset] = [
    Asset(
        id="yacht",
        filename="yacht_1997.png",
        prompt=(
            "A faded 1997 film-grain photograph on a yacht deck, Mediterranean daylight. "
            "Center frame: a tall, silver-haired man in his early 70s wearing a loose white "
            "linen shirt and aviator sunglasses, relaxed posture, subtly giving instructions, "
            "clearly the authority in the scene. Beside him, a shorter balding man in a "
            "tracksuit holds a clipboard, nodding deferentially (think: note-taker, not peer). "
            "Background: other figures in beach attire, slightly out of focus, respectful distance. "
            "The older man is unmistakably in charge. Vintage Kodak color palette, slight lens "
            "flare, handwritten date stamp in the lower right corner reading '7-12-97'. "
            "Candid documentary style, not posed. Do NOT include any real public figures' faces "
            "or any text other than the date stamp."
        ),
    ),
    Asset(
        id="island_ops",
        filename="island_operations_97.png",
        prompt=(
            "A grainy covert-briefing photo, shot from slight distance with a long lens through "
            "foliage. On a villa patio at dusk: a silver-haired older man at the head of a stone "
            "table, pointing at a map. Six associates in 1990s business-casual seated around, "
            "listening intently. Tiki torches. Document folders and a satellite phone on the table. "
            "Composition suggests surveillance photograph: slight motion blur, film grain, faded "
            "color. No identifiable real people. No logos. The older man is clearly the operations "
            "lead. Caption-ready: evidence-photo aesthetic."
        ),
    ),
    Asset(
        id="flight_manifest",
        filename="flight_manifest_n908je.png",
        prompt=(
            "A scanned private-jet flight manifest document, top-down view, aged off-white paper "
            "with crease lines and coffee ring stains. Header: 'PRIVATE AVIATION LOG - TAIL "
            "NUMBER N908JE'. A form-style table with columns: ROLE | NAME | BOARDING | DEPART. "
            "Rows are mostly [REDACTED] with black rectangles, but the PILOT row is hand-written "
            "over in ballpoint pen (left blank here, do not write names). One row labeled "
            "'INFANT' has a visible pen-strike redaction and a margin note in spidery handwriting "
            "reading 'swapped - see file'. Date stamp '04-17-1994' in upper right. Creased, "
            "slightly yellowed. Authentic 1990s aviation document aesthetic, top-down flat-scan "
            "style. Do NOT include any real person's name. The paper should look like evidence."
        ),
    ),
    Asset(
        id="minion_meme",
        filename="minion_live_laugh_load.png",
        prompt=(
            "A cringe boomer Facebook-style meme image. Background: a yellow Minion cartoon "
            "character from a knockoff parody (round goggles, blue overalls, one eye) standing "
            "in front of a kitchen dishwasher holding a wine glass. Over the image, bold white "
            "Impact-font text with heavy black outline, top line: 'LIVE LAUGH LOAD', bottom "
            "line: '(THE DISHWASHER)'. Lower-right watermark text in low-res arial reading "
            "'#blessed #wine-o-clock #grandma_life'. Slightly compressed JPEG artifacts, "
            "oversaturated colors, clearly shared-too-many-times quality. Wholesome-cursed "
            "boomer-Facebook energy. Not an actual Minion - a generic yellow knockoff."
        ),
    ),
    Asset(
        id="doge",
        filename="doge_biscuit.png",
        prompt=(
            "Classic Shiba Inu doge meme format: a Shiba Inu dog seen from low angle, head "
            "tilted, side-eye expression, on a simple neutral background. Multicolored Comic "
            "Sans text scattered around the dog in the classic doge style. Visible text phrases "
            "(use these exactly): 'such BISCUIT' (red), 'very loyal' (green), 'wow' (blue), "
            "'much goodboi' (orange), 'so remember' (purple). Low-res 2013-era meme quality, "
            "slight JPEG compression. Authentic doge meme style."
        ),
    ),
    Asset(
        id="rickroll",
        filename="rickroll_still.png",
        prompt=(
            "A YouTube-thumbnail style still frame of a 1980s red-haired man in a trench coat "
            "mid-dance, frozen mid-motion, in front of a plain urban wall, VHS-era color grading, "
            "slight scan-lines. Small play-button overlay in the center. Bottom caption bar: "
            "'Never Gonna Give You Up (Official Music Video) - 1987'. No real person likeness - "
            "stylized illustration of a generic red-haired male singer. 4:3 aspect-ratio frame."
        ),
    ),
    Asset(
        id="wallpaper",
        filename="desktop_wallpaper.png",
        prompt=(
            "A Windows-98-era desktop wallpaper, 16:9 aspect. Soft teal gradient background with "
            "a gentle bloom in the upper-left. No text. No logos. No watermark. Very subtle "
            "diagonal CRT scanlines barely visible. Extremely minimal - meant purely as backdrop "
            "for desktop icons. LOW contrast, soft mid-tones. Nothing busy in the center or edges - "
            "icons need to pop over this. Retro Win98 bliss-lite vibe."
        ),
    ),
    Asset(
        id="biscuit",
        filename="biscuit_dog.png",
        prompt=(
            "A 1987 sun-faded polaroid photograph of a scruffy mixed-breed medium dog (golden-brown "
            "short-hair, floppy ears, happy tongue-out grin) sitting on a suburban lawn. Heavy "
            "film grain, soft focus, over-exposed highlights, slight yellowing from age, white "
            "polaroid border with hand-written caption in blue ballpoint: 'Biscuit, 1987'. "
            "Warm nostalgic tone. No people in frame. Classic good-boy energy."
        ),
    ),
    Asset(
        id="nft_receipt",
        filename="nft_receipt.png",
        prompt=(
            "A screenshot of a crypto NFT purchase receipt, OpenSea-style interface circa 2021, "
            "dark-mode UI. Prominent header: 'MINTED 2013 :: DOGE.LEGACY #0069'. Below: a pixelated "
            "Shiba Inu JPEG thumbnail. Fields visible: Wallet '0xDEAD...BEEF', Balance '<REDACTED>', "
            "Gas '0.042 ETH', Date '2013-07-12'. Purple and teal accent colors. Subtle ledger-style "
            "table. Looks like a genuine receipt screenshot. No real brand logos."
        ),
    ),
    Asset(
        id="boot_bg",
        filename="boot_background.png",
        prompt=(
            "A cinematic hero backdrop for a retro-cyber game's title screen. Composition: "
            "a dusty, sun-lit home office from above/3-quarter angle at dusk. Center-left: an "
            "ancient beige CRT monitor on a cluttered desk, its screen glowing faintly green. "
            "Next to it: a coffee-stained keyboard, a cassette tape, a half-full glass of scotch, "
            "a framed photo of a man with a dog face-down, an ashtray, a curled yellow post-it "
            "that reads 'DON'T FORGET'. Dust motes in light shafts coming through blinds. "
            "A second smaller screen shows faint scrolling code. Color palette: moody teals, "
            "warm amber highlights, heavy shadow. Slight film grain, vignette. No text visible "
            "on the main monitor - leave the screen area mostly empty glow for UI overlay. "
            "16:9 aspect. Noir-meets-80s-basement aesthetic. No logos, no people in frame."
        ),
    ),
    Asset(
        id="area51",
        filename="area51_aliens.png",
        prompt=(
            "A blurry low-resolution bootleg UFO conspiracy photograph, 1990s chain-email style. "
            "A saucer-shaped UFO hovering over red desert rocks at dusk, extremely grainy and "
            "compressed, timestamp '06-14-1997' burned into lower corner in glowing red digital "
            "font. Clearly fake, clearly the kind of image a boomer would forward 500 times. "
            "Overexposed glow around the UFO. Low-quality JPEG artifacts everywhere. "
            "Cursed-authentic boomer-Facebook meme quality."
        ),
    ),
]


def load_key() -> str:
    load_dotenv(Path(__file__).parent / ".env")
    key = os.getenv("GEMINI_API_KEY")
    if not key:
        sys.exit("GEMINI_API_KEY missing. Put it in tools/.env (already gitignored).")
    return key


def generate(client: genai.Client, asset: Asset, out_path: Path) -> None:
    print(f"[gen] {asset.id} -> {out_path.name}", flush=True)
    resp = client.models.generate_content(
        model=MODEL,
        contents=asset.prompt,
        config=types.GenerateContentConfig(response_modalities=["TEXT", "IMAGE"]),
    )
    saved = False
    for part in resp.parts:
        if getattr(part, "inline_data", None) is not None:
            img = part.as_image()
            img.save(out_path)
            saved = True
            break
    if not saved:
        # Surface the text response so we can debug prompt refusals without leaking key
        text_parts = [p.text for p in resp.parts if getattr(p, "text", None)]
        msg = "\n".join(text_parts) or "(no text, no image)"
        raise RuntimeError(f"No image returned for {asset.id}. Model said: {msg}")


def main() -> None:
    argv = sys.argv[1:]
    force = "--force" in argv
    wanted = [a for a in argv if not a.startswith("--")]

    OUT_DIR.mkdir(parents=True, exist_ok=True)
    client = genai.Client(api_key=load_key())

    targets = [a for a in ASSETS if not wanted or a.id in wanted]
    if wanted and not targets:
        sys.exit(f"No asset id matches {wanted}. Known: {[a.id for a in ASSETS]}")

    for asset in targets:
        out_path = OUT_DIR / asset.filename
        if out_path.exists() and not force:
            print(f"[skip] {asset.id} ({out_path.name} exists; --force to regen)")
            continue
        try:
            generate(client, asset, out_path)
        except Exception as exc:  # noqa: BLE001
            # Fail loud for this one, keep going for the rest
            print(f"[error] {asset.id}: {exc}", file=sys.stderr)

    print(f"\nDone. Outputs in {OUT_DIR.relative_to(ROOT)}")
    print("Back in Unity: Assets > Refresh (Cmd+R) to import, then import as Sprite (2D/UI).")


if __name__ == "__main__":
    main()
