"""Level-up jingle — a short bright bell arpeggio, 16-bit mono WAV (pure stdlib)."""
import wave, struct, math, os

SR = 44100
os.makedirs('out', exist_ok=True)

def bell(freq, dur, t0):
    """A bell-ish note: 2 partials, fast attack, exp decay."""
    n = int(dur * SR)
    out = []
    for i in range(n):
        t = i / SR
        env = math.exp(-t * 6.0) * (1 - math.exp(-t * 120))  # pluck attack + decay
        s = (math.sin(2*math.pi*freq*t) * 0.6
             + math.sin(2*math.pi*freq*2*t) * 0.25            # octave partial -> bell timbre
             + math.sin(2*math.pi*freq*3.01*t) * 0.1)
        out.append((t0 + i, s * env))
    return out

# C major arpeggio up to the octave, last note rings longer with a shimmer
notes = [
    (523.25, 0.00, 0.18),   # C5
    (659.25, 0.09, 0.18),   # E5
    (783.99, 0.18, 0.20),   # G5
    (1046.50, 0.30, 0.55),  # C6 (hold)
]

total = int(1.0 * SR)
buf = [0.0] * total
for freq, start, dur in notes:
    t0 = int(start * SR)
    for idx, val in bell(freq, dur, t0):
        if 0 <= idx < total:
            buf[idx] += val

# gentle shimmer on the tail of the last note
for i in range(int(0.30*SR), total):
    t = i / SR
    buf[i] += math.sin(2*math.pi*2093*t) * 0.06 * math.exp(-(t-0.30)*5)

# normalize
peak = max(1e-6, max(abs(v) for v in buf))
buf = [v / peak * 0.92 for v in buf]

with wave.open('out/level_up.wav', 'w') as w:
    w.setnchannels(1); w.setsampwidth(2); w.setframerate(SR)
    w.writeframes(b''.join(struct.pack('<h', int(max(-1,min(1,v))*32767)) for v in buf))
print('ok', round(total/SR,2), 's')
