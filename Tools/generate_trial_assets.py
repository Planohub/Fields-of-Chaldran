"""Reproduce the original Quarantine Trial pixel atlas and short synthesized SFX.

Run from any directory with Python 3 and Pillow. Existing scene/balance tuning is
preserved. Generated asset GUIDs are deterministic and existing metadata is kept.
"""
from pathlib import Path
import json
import math
import random
import struct
import uuid
import wave
from PIL import Image, ImageDraw

ROOT = Path(__file__).resolve().parents[1]
BASE = ROOT / 'Assets' / 'Chaldran'


def guid(path):
    return uuid.uuid5(uuid.NAMESPACE_URL, 'https://github.com/Planohub/Fields-of-Chaldran/' + path).hex


def atlas():
    output = Image.new('RGBA', (128, 32), (0, 0, 0, 0))
    tiles = []
    for index in range(16):
        tile = Image.new('RGBA', (16, 16), (0, 0, 0, 0))
        d = ImageDraw.Draw(tile)
        dark, edge, stone, gold, teal = '#101827', '#344453', '#657782', '#deb879', '#76e5d7'
        if index == 0:  # dark, readable stone flooring
            d.rectangle((0, 0, 15, 15), fill='#202c3c')
            d.line((0, 0, 15, 0), fill='#354557')
            d.line((0, 8, 15, 8), fill='#172333')
            d.line((7, 0, 7, 7), fill='#172333')
            d.line((3, 8, 3, 15), fill='#172333')
            d.point((12, 4), fill='#3a4858')
            d.point((10, 12), fill='#2d3b4e')
        elif index == 1:  # metallic containment masonry
            d.rectangle((0, 0, 15, 15), fill=dark)
            for box in [(1, 1, 14, 6), (1, 9, 6, 14), (9, 9, 14, 14)]:
                d.rectangle(box, fill=edge)
                d.line((box[0], box[1], box[2], box[1]), fill=stone)
            d.point((2, 2), fill=gold)
            d.point((13, 13), fill=gold)
        elif index == 2:  # awakened avatar
            d.ellipse((3, 13, 13, 15), fill='#101421')
            d.rectangle((4, 6, 11, 12), fill='#285c82')
            d.rectangle((3, 7, 4, 11), fill=teal)
            d.rectangle((11, 7, 12, 11), fill=teal)
            d.rectangle((5, 1, 10, 5), fill='#d9b89a')
            d.rectangle((4, 0, 11, 2), fill='#19343f')
            d.point((6, 4), fill='#10252e')
            d.point((9, 4), fill='#10252e')
            d.rectangle((5, 12, 6, 14), fill='#76aabd')
            d.rectangle((9, 12, 10, 14), fill='#76aabd')
            d.rectangle((7, 8, 8, 10), fill=teal)
        elif index == 3:  # bronze mechanical sentinel
            d.ellipse((1, 13, 14, 15), fill='#111423')
            d.rectangle((3, 4, 12, 11), fill='#82654c', outline=gold)
            d.rectangle((5, 0, 10, 4), fill='#ad8b61')
            d.rectangle((5, 2, 10, 3), fill='#76ecde')
            d.rectangle((0, 5, 3, 10), fill='#53616c')
            d.rectangle((12, 5, 15, 10), fill='#53616c')
            d.rectangle((4, 12, 6, 14), fill='#bf9562')
            d.rectangle((9, 12, 11, 14), fill='#bf9562')
            d.rectangle((6, 6, 9, 8), fill='#aee9db')
        elif index in (4, 5, 8):  # anomaly, recovery relay, exit terminal
            d.rectangle((2, 11, 13, 14), fill=edge, outline=stone)
            d.rectangle((4, 7, 11, 11), fill='#29394c')
            tint = teal if index == 4 else '#a3e091' if index == 5 else '#ddc491'
            d.polygon([(8, 0), (12, 5), (8, 9), (4, 5)], fill=tint, outline='#d0fff0')
            d.line((8, 2, 8, 7), fill='#f0fffa')
            d.point((1, 5), fill=tint)
            d.point((14, 3), fill=tint)
        elif index == 6:  # access token
            d.rectangle((4, 4, 11, 11), fill='#154d59', outline=teal)
            d.rectangle((6, 6, 9, 9), fill='#d0fff2')
            for n in (4, 7, 10):
                d.line((n, 1, n, 3), fill=gold)
                d.line((n, 12, n, 14), fill=gold)
                d.line((1, n, 3, n), fill=gold)
                d.line((12, n, 14, n), fill=gold)
        elif index == 7:  # red permission grid
            d.rectangle((0, 0, 15, 15), fill=(135, 34, 62, 125))
            for n in range(1, 16, 4):
                d.line((n, 0, n, 15), fill='#f37679')
                d.line((0, n, 15, n), fill=(230, 110, 100, 170))
            d.rectangle((6, 5, 9, 9), outline='#ffddbf')
        elif index == 9:  # slash facing right
            d.arc((0, 0, 14, 15), -80, 80, fill='#ffffff', width=2)
            d.line((13, 4, 15, 8, 13, 12), fill='#ceffff', width=1)
        elif index == 10:  # precise circular warning / burst footprint
            d.ellipse((0, 0, 15, 15), fill=(255, 255, 255, 30), outline=(255, 255, 255, 230), width=1)
            d.line((7, 1, 7, 3), fill='#ffffff')
            d.line((7, 12, 7, 14), fill='#ffffff')
            d.line((1, 7, 3, 7), fill='#ffffff')
            d.line((12, 7, 14, 7), fill='#ffffff')
        elif index == 11:
            d.rectangle((5, 3, 10, 11), fill=gold)
            d.line((3, 12, 12, 12), fill=stone, width=2)
        elif index == 12:  # solid pillar
            d.rectangle((2, 1, 13, 14), fill=dark, outline=stone)
            d.rectangle((4, 3, 11, 12), fill=edge)
            d.line((4, 3, 11, 3), fill=gold)
            d.line((4, 11, 11, 11), fill=gold)
            d.rectangle((7, 5, 8, 8), fill=teal)
        elif index == 13:
            d.ellipse((1, 1, 14, 14), outline='#ffffff', width=2)
        elif index == 14:  # inlaid floor markings
            d.rectangle((2, 2, 13, 13), outline=(116, 205, 209, 80))
            d.polygon([(8, 4), (11, 8), (8, 11), (4, 8)], outline=(116, 205, 209, 130))
        else:
            d.rectangle((0, 0, 15, 15), fill='#ffffff')
        tiles.append(tile)
        output.alpha_composite(tile, ((index % 8) * 16, (index // 8) * 16))
    path = BASE / 'Art' / 'TrialAtlas.png'
    path.parent.mkdir(parents=True, exist_ok=True)
    output.save(path)


def audio():
    specs = [('Strike', .13, 660, 160), ('Burst', .34, 160, 60), ('Hit', .14, 130, 65),
             ('Pickup', .26, 520, 1040), ('Unlock', .38, 260, 780), ('Complete', .65, 523, 1046)]
    rate = 44100
    target = BASE / 'Audio'
    target.mkdir(parents=True, exist_ok=True)
    rng = random.Random(241)
    for name, duration, first, last in specs:
        samples = []
        phase = 0.0
        for i in range(int(rate * duration)):
            t = i / rate
            progress = t / duration
            frequency = first + (last - first) * progress
            phase += 2 * math.pi * frequency / rate
            signal = math.sin(phase) + .22 * math.sin(phase * 2)
            if name in ('Strike', 'Burst', 'Hit'):
                signal = signal * .35 + rng.uniform(-1, 1) * .5
            envelope = min(1, t / .008) * (1 - progress) ** 1.6
            samples.append(int(max(-1, min(1, signal * envelope * .32)) * 32767))
        with wave.open(str(target / (name + '.wav')), 'wb') as output:
            output.setnchannels(1)
            output.setsampwidth(2)
            output.setframerate(rate)
            output.writeframes(struct.pack('<' + 'h' * len(samples), *samples))


def metadata():
    for path in sorted(BASE.rglob('*')) + [BASE]:
        if path.suffix == '.meta':
            continue
        meta = Path(str(path) + '.meta')
        if meta.exists():
            continue
        identity = guid(path.relative_to(ROOT).as_posix())
        head = f'fileFormatVersion: 2\nguid: {identity}\n'
        if path.is_dir():
            body = 'folderAsset: yes\nDefaultImporter:\n  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n'
        elif path.suffix == '.cs':
            body = 'MonoImporter:\n  externalObjects: {}\n  serializedVersion: 2\n  defaultReferences: []\n  executionOrder: 0\n  icon: {instanceID: 0}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n'
        elif path.suffix == '.asmdef':
            body = 'AssemblyDefinitionImporter:\n  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n'
        elif path.suffix == '.asset':
            body = 'NativeFormatImporter:\n  externalObjects: {}\n  mainObjectFileID: 11400000\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n'
        elif path.suffix == '.png':
            body = ('TextureImporter:\n  externalObjects: {}\n  serializedVersion: 13\n'
                    '  mipmaps:\n    enableMipMap: 0\n    sRGBTexture: 1\n'
                    '  isReadable: 0\n  maxTextureSize: 256\n  textureSettings:\n    serializedVersion: 2\n'
                    '    filterMode: 0\n    aniso: 1\n    wrapU: 1\n    wrapV: 1\n    wrapW: 1\n'
                    '  nPOTScale: 0\n  alphaIsTransparency: 1\n  textureType: 0\n  textureShape: 1\n'
                    '  platformSettings:\n  - serializedVersion: 4\n    buildTarget: DefaultTexturePlatform\n'
                    '    maxTextureSize: 256\n    textureFormat: -1\n    textureCompression: 0\n    overridden: 0\n'
                    '  userData: \n  assetBundleName: \n  assetBundleVariant: \n')
        elif path.suffix == '.wav':
            body = ('AudioImporter:\n  externalObjects: {}\n  serializedVersion: 7\n'
                    '  defaultSettings:\n    serializedVersion: 2\n    loadType: 0\n    sampleRateSetting: 0\n'
                    '    sampleRateOverride: 44100\n    compressionFormat: 1\n    quality: 1\n'
                    '    conversionMode: 0\n  platformSettingOverrides: {}\n  forceToMono: 0\n  normalize: 0\n'
                    '  preloadAudioData: 1\n  loadInBackground: 0\n  ambisonic: 0\n  userData: \n'
                    '  assetBundleName: \n  assetBundleVariant: \n')
        else:
            body = 'DefaultImporter:\n  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n'
        meta.write_text('\n'.join(line.rstrip() for line in (head + body).splitlines()) + '\n', encoding='utf-8')


if __name__ == '__main__':
    atlas()
    audio()
    metadata()
    print('Generated original trial atlas and six SFX; preserved existing Unity metadata.')
