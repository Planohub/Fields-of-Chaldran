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

    package_material = '9dfc825aed78fcd4ba02077103263b40'
    assert package_material in (ASSETS / 'Settings/Renderer2D.asset').read_text()
    for path in list((TRIAL / 'Scenes').glob('*.unity')) + list((TRIAL / 'Data').rglob('*.asset')):
        scene = path.read_text()
        for identity in re.findall(r'guid: ([0-9a-f]{32})', scene):
            assert identity in identities or identity == package_material or identity.startswith('0000000000000000'), f'Unresolved GUID in {path}: {identity}'
        file_ids = re.findall(r'^--- !u!\d+ &(\d+)', scene, re.M)
        assert len(file_ids) == len(set(file_ids)), f'Duplicate scene document IDs: {path}'
    build_settings = (ROOT / 'ProjectSettings/EditorBuildSettings.asset').read_text()
    for name in ['QuarantinePrototype', 'OverlandPrototype']:
        assert f'path: Assets/Chaldran/Scenes/{name}.unity' in build_settings
    for name, cell, buffer in [('QuarantinePresentation',16,(256,144)),('OverlandPresentation',32,(512,288))]:
        profile = (TRIAL / 'Data' / (name + '.asset')).read_text()
        for field,value in [('cellPixels',cell),('bufferWidth',buffer[0]),('bufferHeight',buffer[1])]:
            assert f'  {field}: {value}\n' in profile
        assert len(re.findall(r'^  - \{fileID: 8300000',profile,re.M)) == 6

    story = (TRIAL / 'Data/Story/UserDirectoryOpening.asset').read_text()
    steps = re.findall(r'^  - beat: (\d+)\n    objective: (.+)\n    dialogue: \{fileID: 11400000, guid: ([0-9a-f]{32}), type: 2\}', story, re.M)
    assert [int(step[0]) for step in steps] == list(range(5)), 'Story beats must preserve checkpoint order'
    dialogue_script = re.search(r'^guid: (\w+)', (TRIAL / 'Runtime/DialogueDefinition.cs.meta').read_text(), re.M).group(1)
    for _, objective, identity in steps:
        assert json.loads(objective).strip(), 'Empty quest objective'
        path = Path(str(identities[identity])[:-5])
        dialogue = path.read_text()
        assert f'guid: {dialogue_script}' in dialogue, f'Wrong dialogue type: {path}'
        lines = re.findall(r'^  - speaker: (.+)\n    text: (.+)', dialogue, re.M)
        assert 1 <= len(lines) <= 64, f'Invalid page count: {path}'
        for speaker, text in lines:
            assert json.loads(speaker).strip()
            assert 0 < len(json.loads(text).strip()) <= 500, f'Invalid dialogue page: {path}'
    identity = re.search(r'^guid: (\w+)', (TRIAL / 'Data/Story/UserDirectoryOpening.asset.meta').read_text(), re.M).group(1)
    overland = (TRIAL / 'Scenes/OverlandPrototype.unity').read_text()
    assert f'directoryStory: {{fileID: 11400000, guid: {identity}, type: 2}}' in overland
    quarantine = (TRIAL / 'Data/QuarantinePresentation.asset').read_text()
    assert '  retroDialogue: 1\n' in quarantine and '  charactersPerSecond: 60\n' in quarantine
    assert re.search(r'typingSound: \{fileID: 8300000, guid: ([0-9a-f]{32})', quarantine).group(1) in identities
    intro = (TRIAL / 'Data/Story/QuarantineIntroduction.asset').read_text()
    intro_guid = re.search(r'^guid: (\w+)', (TRIAL / 'Data/Story/QuarantineIntroduction.asset.meta').read_text(), re.M).group(1)
    assert f'introduction: {{fileID: 11400000, guid: {intro_guid}, type: 2}}' in quarantine
    assert len(re.findall(r'^  - speaker:', intro, re.M)) == 2

    inputs = json.loads((ASSETS / 'PlayerControls.inputactions').read_text())['maps'][0]
    actions = {action['name']: action['id'] for action in inputs['actions']}
    assert len(actions) == len(inputs['actions']), 'Duplicate input action names'
    original = {'Move': '1df652db-e4ff-43cc-979e-ae1ea09e966e', 'Sprint': '020b9e31-8905-4780-815a-ce4ba1999b08',
                'Interact': '5aede3f3-f4d8-45b9-815c-33d37286437e', 'Attack': '67893c94-a9b4-4d12-890f-8eef0e728b03'}
    for name, identity in original.items():
        assert actions[name] == identity, f'Existing input action identity changed: {name}'
    for name in ['Block', 'Ability', 'Pause', 'Retry', 'Aim', 'Continue', 'NewTrial']:
        assert name in actions and any(binding['action'] == name for binding in inputs['bindings'])
    binding_ids = [binding['id'] for binding in inputs['bindings']]
    assert len(binding_ids) == len(set(binding_ids)), 'Duplicate binding IDs'

    layers = (ROOT / 'ProjectSettings/TagManager.asset').read_text().split('  layers:\n', 1)[1].split('  m_SortingLayers:', 1)[0].splitlines()
    assert [layers[n].strip() for n in [8, 9, 10]] == ['- WorldSolid', '- Interactable', '- Actors']
    png = (TRIAL / 'Art/TrialAtlas.png').read_bytes()
    assert png[:8] == b'\x89PNG\r\n\x1a\n'
    assert struct.unpack('>II', png[16:24]) == (128, 32), 'Atlas must match the runtime 16x16 sprite layout'
    assert struct.unpack('>II',(TRIAL / 'Art/OverlandAtlas.png').read_bytes()[16:24]) == (256,64)
    clips = list((TRIAL / 'Audio').rglob('*.wav'))
    for path in clips:
        with wave.open(str(path), 'rb') as clip:
            assert clip.getnchannels() == 1 and clip.getsampwidth() == 2 and clip.getframerate() in [8000,16000,22050,44100]
            samples = struct.unpack('<' + 'h' * clip.getnframes(), clip.readframes(clip.getnframes()))
            assert samples and max(abs(value) for value in samples) < 32767, f'Clipped audio: {path}'
    assert len(clips) == 15
    print('PASS metadata, scenes/profiles/atlases, ordered story/dialogue references, retro introduction/typing cue, input IDs, and 15 audio clips.')


if __name__ == '__main__':
    verify()
