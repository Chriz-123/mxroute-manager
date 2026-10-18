#!/usr/bin/env python3
"""Rasterize Assets/app.svg into PNG + multi-size ICO."""
from __future__ import annotations

from io import BytesIO
from pathlib import Path

import cairosvg
from PIL import Image

ROOT = Path(__file__).resolve().parents[1]
ASSETS = ROOT / "src" / "MxRouteManager" / "Assets"
SVG = ASSETS / "app.svg"
SIZES = (16, 20, 24, 32, 40, 48, 64, 128, 256)


def render(size: int) -> Image.Image:
    png = cairosvg.svg2png(url=str(SVG), output_width=size, output_height=size)
    return Image.open(BytesIO(png)).convert("RGBA")


def main() -> None:
    ASSETS.mkdir(parents=True, exist_ok=True)
    master = render(256)
    master.save(ASSETS / "app.png", format="PNG")

    master.save(
        ASSETS / "app.ico",
        format="ICO",
        sizes=[(s, s) for s in SIZES],
    )
    print(f"Wrote {ASSETS / 'app.png'} and {ASSETS / 'app.ico'}")


if __name__ == "__main__":
    main()
