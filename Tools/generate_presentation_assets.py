"""Extend the project's deterministic pixel/synth asset pipeline for two tiers.

The original TrialAtlas and six effects stay unchanged. New 32px sprites and a
richer arrangement demonstrate the next tier; they are still development art.
"""
from pathlib import Path
import math
import random
import struct
import wave
from PIL import Image, ImageDraw
from generate_trial_assets import BASE, metadata


def hero(draw, stride=0):
    draw.ellipse((6, 27, 26, 31), fill=(12, 27, 34, 110))
    draw.polygon([(10, 12), (21, 12), (25, 26), (7, 26)], fill='#244c78', outline='#112a43')
    draw.polygon([(12, 13), (19, 13), (20, 24), (11, 24)], fill='#397fa0')
    draw.rectangle((13, 18, 18, 20), fill='#e3bd6c')
    draw.rectangle((10, 26-stride, 13, 29-stride), fill='#b2c9c3')
    draw.rectangle((18, 26+stride, 21, 29+stride), fill='#b2c9c3')
    draw.rectangle((12, 5, 20, 12), fill='#c78d68', outline='#734e43')
    draw.rectangle((13, 6, 19, 10), fill='#edc19a')
    draw.polygon([(11, 7), (12, 2), (20, 2), (22, 7), (17, 5)], fill='#263b47')
    draw.point((14, 9), fill='#182d38'); draw.point((18, 9), fill='#182d38')
    draw.line((25, 9, 25, 24), fill='#e2faf0', width=2)
    draw.line((26, 10, 26, 23), fill='#82bac2')
    draw.line((22, 24, 28, 24), fill='#d7aa54', width=2)
    draw.line((25, 25, 25, 28), fill='#5a3c37', width=2)


def atlas():
    output = Image.new('RGBA', (256, 64), (0, 0, 0, 0))
    for index in range(16):
        tile = Image.new('RGBA', (32, 32), (0, 0, 0, 0))
        d = ImageDraw.Draw(tile)
        rng = random.Random(412 + index)
        if index == 0:
            d.rectangle((0, 0, 31, 31), fill='#4c7050')
            for _ in range(90):
                x, y = rng.randrange(32), rng.randrange(32)
                d.line((x, y, x+1, y), fill=rng.choice(['#537c53','#628e5a','#3d604c','#759860']))
        elif index == 1:
            d.ellipse((5, 26, 27, 31), fill=(14, 39, 33, 100))
            d.rectangle((13, 19, 18, 29), fill='#685344', outline='#3d3434')
            d.line((15, 21, 15, 28), fill='#9a7554')
            for bounds, tint in [((4,9,28,24),'#214d43'),((2,6,25,21),'#326751'),((8,1,29,18),'#4b805b'),((8,2,23,12),'#6b9a64')]:
                d.ellipse(bounds, fill=tint)
            for _ in range(28):
                x,y=rng.randrange(7,25),rng.randrange(5,20)
                d.point((x,y),fill=rng.choice(['#78a66a','#46775a','#aec17b']))
        elif index in (2,11,15):
            hero(d, 0 if index == 2 else -1 if index == 11 else 1)
        elif index == 3:
            d.rounded_rectangle((5, 7, 26, 24), radius=3, fill='#857059', outline='#e4c88a', width=2)
            d.rectangle((10, 1, 21, 9), fill='#b69668', outline='#514c45')
            d.line((12,5,19,5),fill='#a3f2d6',width=2)
            d.rectangle((12,12,19,18), fill='#6ab8aa', outline='#cef6d2')
            d.rectangle((6,25,11,30),fill='#b79871'); d.rectangle((20,25,25,30),fill='#b79871')
        elif index in (4,5,8):
            d.ellipse((3,25,28,31),fill=(19,39,39,130))
            d.polygon([(8,27),(7,8),(12,2),(21,2),(26,8),(24,27)],fill='#647878',outline='#273d48')
            d.line((10,8,10,24),fill='#9aaba0',width=2)
            color = '#bde2c2' if index == 4 else '#6fdcba' if index == 5 else '#edcf83'
            d.polygon([(16,6),(21,14),(16,22),(12,14)],fill=color,outline='#ecf3c4')
            d.line((16,10,16,18),fill='#ffffff')
            if index == 5:
                d.ellipse((1,24,30,30),fill='#397e8a',outline='#99d0bd')
        elif index == 6:
            d.rectangle((8,8,23,23),fill='#325c6b',outline='#e8d293',width=2)
            d.rectangle((12,12,19,19),fill='#9decce',outline='#d8ffdf')
        elif index == 7:
            for n in range(0,32,4):d.line((n,0,n,31),fill=(100,240,218,170))
            d.rectangle((10,10,21,21),outline='#e4f5c5',width=2)
        elif index in (9,10,13):
            if index == 9:
                d.arc((1,1,30,30),-80,80,fill='#dffff4',width=3)
                d.arc((5,5,27,27),-70,70,fill=(203,255,235,100),width=2)
            else:
                d.ellipse((1,1,30,30),outline=(255,255,255,230),width=2)
                d.ellipse((4,4,27,27),outline=(255,255,255,70),width=1)
        elif index == 12:
            d.polygon([(3,23),(7,9),(19,5),(28,14),(26,27),(8,28)],fill='#717e7d',outline='#34454b')
            d.polygon([(7,10),(18,7),(25,14),(12,17)],fill='#a1aba0')
            d.line((12,18,25,16),fill='#4c6166',width=2)
        elif index == 14:
            d.rectangle((0,0,31,31),fill='#a18c69')
            for _ in range(80):
                x,y=rng.randrange(32),rng.randrange(32)
                d.point((x,y),fill=rng.choice(['#bcaa82','#82785f','#968263','#c7b58a']))
        output.alpha_composite(tile, ((index % 8)*32, (index // 8)*32))
    output.save(BASE / 'Art/OverlandAtlas.png')


def write_wave(path, rate, samples):
    path.parent.mkdir(parents=True, exist_ok=True)
    with wave.open(str(path), 'wb') as output:
        output.setnchannels(1); output.setsampwidth(2); output.setframerate(rate)
        output.writeframes(struct.pack('<'+'h'*len(samples),*samples))


def audio():
    # The melody is shared; the second tier adds harmony and a gentler timbre.
    notes = [261.63,329.63,392,329.63,293.66,349.23,440,392]
    for rich, rate, filename in [(False,8000,'QuarantineMusic.wav'),(True,16000,'Overland/Music.wav')]:
        duration=3.2; samples=[]
        for i in range(int(rate*duration)):
            t=i/rate; step=int(t/.4); nt=t-step*.4; f=notes[min(step,7)]
            envelope=min(1,nt/.02)*min(1,(.4-nt)/.035)
            if rich:
                sig=math.sin(2*math.pi*f*t)*.30+math.sin(2*math.pi*f*2*t)*.06
                sig+=math.sin(2*math.pi*130.815*t)*.13+math.sin(2*math.pi*196*t)*.10
                sig*=min(1,t/.04)*min(1,(duration-t)/.04)
            else:
                sig=(1 if math.sin(2*math.pi*f*t)>=0 else -1)*.24
            samples.append(int(sig*envelope*18000))
        write_wave(BASE/'Audio'/filename,rate,samples)
    specs=[('Strike',.13,660,160),('Burst',.34,160,60),('Hit',.14,130,65),
           ('Pickup',.26,520,1040),('Unlock',.38,260,780),('Complete',.65,523,1046)]
    for name,duration,first,last in specs:
        rate=22050; samples=[]; phase=0; rng=random.Random(name)
        for i in range(int(rate*duration)):
            t=i/rate; p=t/duration; phase+=2*math.pi*(first+(last-first)*p)/rate
            sig=math.sin(phase)*.4+math.sin(phase*1.5)*.16+math.sin(phase*2)*.09
            if name in ['Strike','Burst','Hit']:sig+=rng.uniform(-1,1)*.18
            env=min(1,t/.006)*(1-p)**1.8
            samples.append(int(sig*env*22000))
        write_wave(BASE/'Audio/Overland'/(name+'.wav'),rate,samples)


if __name__ == '__main__':
    atlas(); audio(); metadata()
    print('Generated the 32px overland atlas, two musical tiers, richer SFX, and Unity metadata.')
