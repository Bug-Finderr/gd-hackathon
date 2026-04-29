# WILL.EXE

Your grandfather is dead. You inherited his cursed Windows-98-style PC. Click through four password puzzles hidden across his cringe boomer-Facebook desktop to assemble the fragments of his will before the drive corrupts. Each puzzle peels back a layer of his cover identity.

- **Theme:** Inheritance - taken literally (you receive his estate) and figuratively (you also inherit his enemies).
- **Play:** [bug-finderr.itch.io/will-exe](https://bug-finderr.itch.io/will-exe)
- **Demo:** [Watch here](https://drive.google.com/file/d/1heqCAEyI-ILYCq2wChm9SkaMOeR-VRh1/view?usp=sharing)
- **Controls:** Mouse only - double-click icons, drag window titlebars, close via X, type passwords.
- **Difficulty:**

| Mode | Timer | Penalty |
|------|-------|---------|
| Easy | None | - |
| Normal | 15 min | - |
| Hard | 8 min | 30s per wrong password |

## How It Works

The desktop is a fully simulated Windows 98 environment. Icons, folders, text files, images, and password-locked files are all driven by ScriptableObject data (`IconDef`, `WindowDef`, `DesktopManifest`) - no puzzle logic is hardcoded into scenes.

**The four puzzles** are scattered across grandpa's files - a dog photo, a doge crypto wallet, a redacted flight manifest, and a bookmarked music video. Solve each password gate to unlock a **will fragment**. Collect all four to win.

A **"Drive Health" corruption timer** ticks down on Normal/Hard. On Hard, every wrong password costs 30 seconds. Certain files trigger a **dead-man switch** that forces a countdown even on Easy.

Other desktop clutter - a Minion meme, an NFT receipt, Area 51 conspiracy photos, a rickroll (80 sprite-frames at 12fps) - builds out grandpa's absurd boomer-meets-secret-agent cover identity.

## Art Generation

All hero images were generated using **Nano Banana Pro** (`gemini-3-pro-image-preview`) via `tools/gen_assets.py` - yacht surveillance photo, fake flight manifest, Minion meme, doge meme, NFT receipt, and more.

## Built With

- **Unity 6** (6000.4.0f1) - WebGL
- **URP** (2D Renderer) + **TextMesh Pro**
- **Nano Banana Pro** (`gemini-3-pro-image-preview`) - AI art generation
