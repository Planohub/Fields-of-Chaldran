"""Check trial references and source asset integrity without requiring Unity."""
from pathlib import Path
import json
import re
import struct
import wave

ROOT = Path(__file__).resolve().parents[1]
ASSETS = ROOT / 'Assets'
TRIAL = ASSETS / 'Chaldran'


def verify():
    identities = {}
    for path in ASSETS.rglob('*.meta'):
        match = re.search(r'^guid: ([0-9a-f]{32})$', path.read_text(), re.M)
        assert match, f'Missing GUID: {path}'
        identity = match.group(1)
        assert identity not in identities, f'Duplicate GUID: {path}'
        identities[identity] = path
    for path in [TRIAL] + list(TRIAL.rglob('*')):
        if path.suffix != '.meta':
            assert Path(str(path) + '.meta').exists(), f'Missing metadata: {path}'

    scene_path = TRIAL / 'Scenes/QuarantinePrototype.unity'
    scene = scene_path.read_text()
    package_material = '9dfc825aed78fcd4ba02077103263b40'
    assert package_material in (ASSETS / 'Settings/Renderer2D.asset').read_text()
    for identity in re.findall(r'guid: ([0-9a-f]{32})', scene):
        assert identity in identities or identity == package_material or identity.startswith('0000000000000000'), f'Unresolved scene GUID: {identity}'
    file_ids = re.findall(r'^--- !u!\d+ &(\d+)', scene, re.M)
    assert len(file_ids) == len(set(file_ids)), 'Duplicate scene document IDs'
    assert 'path: Assets/Chaldran/Scenes/QuarantinePrototype.unity' in (ROOT / 'ProjectSettings/EditorBuildSettings.asset').read_text()

    inputs = json.loads((ASSETS / 'PlayerControls.inputactions').read_text())['maps'][0]
    actions = {action['name']: action['id'] for action in inputs['actions']}
    assert len(actions) == len(inputs['actions']), 'Duplicate input action names'
    original = {'Move': '1df652db-e4ff-43cc-979e-ae1ea09e966e', 'Sprint': '020b9e31-8905-4780-815a-ce4ba1999b08',
                'Interact': '5aede3f3-f4d8-45b9-815c-33d37286437e', 'Attack': '67893c94-a9b4-4d12-890f-8eef0e728b03'}
    for name, identity in original.items():
        assert actions[name] == identity, f'Existing input action identity changed: {name}'
    for name in ['Block', 'Ability', 'Pause', 'Retry', 'Aim']:
        assert name in actions and any(binding['action'] == name for binding in inputs['bindings'])
    binding_ids = [binding['id'] for binding in inputs['bindings']]
    assert len(binding_ids) == len(set(binding_ids)), 'Duplicate binding IDs'

    layers = (ROOT / 'ProjectSettings/TagManager.asset').read_text().split('  layers:\n', 1)[1].split('  m_SortingLayers:', 1)[0].splitlines()
    assert [layers[n].strip() for n in [8, 9, 10]] == ['- WorldSolid', '- Interactable', '- Actors']
    png = (TRIAL / 'Art/TrialAtlas.png').read_bytes()
    assert png[:8] == b'\x89PNG\r\n\x1a\n'
    assert struct.unpack('>II', png[16:24]) == (128, 32), 'Atlas must match the runtime 16x16 sprite layout'
    for path in (TRIAL / 'Audio').glob('*.wav'):
        with wave.open(str(path), 'rb') as clip:
            assert clip.getnchannels() == 1 and clip.getsampwidth() == 2 and clip.getframerate() == 44100
            samples = struct.unpack('<' + 'h' * clip.getnframes(), clip.readframes(clip.getnframes()))
            assert samples and max(abs(value) for value in samples) < 32767, f'Clipped audio: {path}'
    assert len(list((TRIAL / 'Audio').glob('*.wav'))) == 6
    print('PASS trial metadata, scene references, layers, preserved input IDs, atlas layout, and six audio clips.')


if __name__ == '__main__':
    verify()
