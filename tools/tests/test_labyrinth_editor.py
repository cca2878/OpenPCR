"""Black-box Labyrinth editor tests with synthetic requests and optional read-only resources."""
import argparse
import copy
import json
from pathlib import Path
import resource
import shutil
import subprocess
import tempfile
import unittest

REPO = Path(__file__).resolve().parents[2]
CONFIGURATION = 'Release'
RESOURCES = None
ARTIFACTS = None


def no_core():
    resource.setrlimit(resource.RLIMIT_CORE, (0, 0))


class Cli:
    def executable(self):
        return REPO / f'tools/OpenPCR.BattleEditor/bin/{CONFIGURATION}/net10.0/OpenPCR.BattleEditor.dll'

    def invoke(self, *args, ok=True):
        editor = self.executable()
        process = subprocess.run(['dotnet', str(editor), *map(str, args)], cwd=REPO,
                                 capture_output=True, text=True, preexec_fn=no_core)
        if ARTIFACTS is not None:
            with (self.root / 'commands.jsonl').open('a') as trace:
                trace.write(json.dumps({'args': list(map(str, args)), 'exit': process.returncode,
                                        'stdout': process.stdout, 'stderr': process.stderr}) + '\n')
        self.assertEqual(process.returncode, 0 if ok else 1, process.stdout + process.stderr)
        value = json.loads(process.stdout if ok else process.stderr)
        self.assertEqual(value['Ok'], ok)
        return value['Result'] if ok else value

    def example(self, ex=False):
        return REPO / 'examples/battles' / ('labyrinth-ex.json' if ex else 'labyrinth-normal.json')

    def setUp(self):
        self.temporary = tempfile.TemporaryDirectory(prefix='openpcr-labyrinth-editor-')
        self.addCleanup(self.temporary.cleanup)
        self.root = Path(self.temporary.name)
        self.source = self.root / 'input.json'
        self.source.write_bytes(self.example().read_bytes())
        if ARTIFACTS is not None:
            self.addCleanup(shutil.copytree, self.root, ARTIFACTS / self.id(), dirs_exist_ok=True)

    def write(self, name, value):
        path = self.root / name
        path.write_text(json.dumps(value))
        return path


class ResourceFreeTests(Cli, unittest.TestCase):
    def test_inspect_both_independent_requests(self):
        for ex in (False, True):
            path = self.example(ex)
            raw = json.loads(path.read_text())
            result = self.invoke('inspect', '--input', path)
            self.assertEqual(result['Schema'], 'openpcr-battle/v2')
            self.assertEqual(result['Labyrinth'], raw['Labyrinth'])
            self.assertIsNone(result['ClanBattle'])
            self.assertEqual(len(result['Battles']), 3 if ex else 1)

    def test_schema_dispatch_never_infers_category(self):
        raw = json.loads(self.example().read_text())
        for change in ('outer-schema', 'old-schema', 'missing-mode', 'both-modes', 'unknown-release'):
            request = copy.deepcopy(raw)
            if change == 'outer-schema':
                request['Schema'] = 'unsupported'
            elif change == 'old-schema':
                request['Schema'] = 'openpcr-labyrinth-battle/v2'
            elif change == 'missing-mode':
                del request['Labyrinth']
            elif change == 'both-modes':
                request['ClanBattle'] = {'clan_battle_id': 1001, 'phase': 1, 'order_num': 1, 'kind': 'trial'}
            else:
                request['Player']['Releases']['DerivedTotal'] = 100
            self.invoke('inspect', '--input', self.write('invalid.json', request), ok=False)

    def test_party_and_challenge_shape_is_validated(self):
        normal, ex = (json.loads(self.example(flag).read_text()) for flag in (False, True))
        cases = [
            (normal, lambda r: r['Battles'][0]['Opening'].__setitem__('Speed', 3), 'Speed'),
            (normal, lambda r: r['Battles'][0]['Opening'].__setitem__('SetUnits', [100201]), 'SetUnits'),
            (normal, lambda r: r['Battles'][0].pop('Seed'), 'Seed'),
            (normal, lambda r: r['Battles'][0].__setitem__('Operations', [{'Anchor': 'BattleClock', 'At': 1, 'Kind': 'auto_mode'}]), 'operations'),
            (ex, lambda r: r['Battles'].append(r['Battles'][0]), 'three'),
            (ex, lambda r: r['Battles'][0].__setitem__('Party', [0, 0, 0, 0, 0]), 'at least one'),
        ]
        for base, edit, message in cases:
            request = copy.deepcopy(base); edit(request)
            error = self.invoke('inspect', '--input', self.write('invalid.json', request), ok=False)
            self.assertIn(message, error['Message'])

    def test_duplicate_properties_are_rejected(self):
        self.source.write_text(self.example().read_text().replace('"difficulty": 1', '"difficulty": 1, "difficulty": 2'))
        error = self.invoke('inspect', '--input', self.source, ok=False)
        self.assertIn('Duplicate', error['Message'])


def run_battle(request, csv_dir, frames=30000):
    runner = REPO / f'tools/OpenPCR.Runner/bin/{CONFIGURATION}/net10.0/OpenPCR.Runner.dll'
    return subprocess.run(['dotnet', str(runner), 'run', '--resources', str(RESOURCES), '--request', str(request),
                           '--max-engine-frames', str(frames), '--csv-dir', str(csv_dir)],
                          cwd=REPO, capture_output=True, text=True, preexec_fn=no_core)


class ResourceTests(Cli, unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        if RESOURCES is None:
            raise unittest.SkipTest('Provide --resources with master 202611031924 and format-1 prefabs.')

    def edit(self, command, *args, ok=True):
        return self.invoke(command, '--input', self.source, '--resources', RESOURCES,
                           '--output', self.source, *args, ok=ok)

    def read(self):
        return json.loads(self.source.read_text())

    def test_prepares_both_categories_and_uses_mode_catalogs(self):
        for ex in (False, True):
            result = self.invoke('validate', '--input', self.example(ex), '--resources', RESOURCES)
            self.assertTrue(result['Valid'])
            self.assertEqual(result['Prepared']['Enemies'], 3)
        raw = self.read()
        for kind, identity in [('labyrinth-enemies', 770110101), ('labyrinth-quests', raw['Labyrinth']['quest_id'])]:
            result = self.invoke('catalog', '--resources', RESOURCES, '--master-version', '202611031924',
                                 '--kind', kind, '--id', identity)
            self.assertEqual(result['Items'][0]['Id'], identity)
        relics = self.invoke('catalog', '--resources', RESOURCES, '--master-version', '202611031924',
                             '--kind', 'labyrinth-relics', '--limit', 1)['Items']
        self.assertEqual(len(relics), 1)
        self.assertIn('EffectGroupId', relics[0])

    def test_common_edits_keep_explicit_mode_and_preview_is_read_only(self):
        original = self.source.read_bytes()
        preview = self.invoke('scene', '--input', self.source, '--resources', RESOURCES, '--seed', 5, '--dry-run', 'true')
        self.assertEqual(self.source.read_bytes(), original)
        self.assertIn('/Battles/0/Seed', preview['ChangedPaths'])
        self.edit('scene', '--seed', 5)
        mode = self.read()['Labyrinth']
        self.edit('unit-edit', '--id', 100101, '--level', 99)
        self.assertEqual(self.read()['Labyrinth'], mode)
        self.assertEqual(self.read()['Battles'][0]['Seed'], 5)
        self.assertEqual(self.read()['Player']['unit_list'][0]['unit_level'], 99)

    def test_party_settings_and_slots_edit_the_battle(self):
        self.edit('party', '--units', '0,100101')
        self.assertEqual(self.read()['Battles'][0]['Party'], [0, 100101, 0, 0, 0])
        self.edit('unit-edit', '--id', 100101, '--slot', 5)
        self.assertEqual(self.read()['Battles'][0]['Party'], [0, 0, 0, 0, 100101])
        self.edit('labyrinth', '--party-units', '100101', '--party-auto', 'true', '--party-speed', 4, '--party-set', '100101')
        self.assertEqual(self.read()['Battles'][0]['Party'], [100101, 0, 0, 0, 0])
        self.assertEqual(self.read()['Battles'][0]['Opening'], {'Auto': True, 'Speed': 4, 'SetUnits': [100101]})

    def test_ex_parties_and_seeds_are_edited_by_index(self):
        self.source.write_bytes(self.example(True).read_bytes())
        original = self.read()['Battles']
        # The example's party 1 sets its own unit; changing only the units leaves SetUnits outside the party.
        error = self.edit('labyrinth', '--ex-party', 1, '--party-units', '100101', ok=False)
        self.assertIn('SetUnits', error['Message'])
        result = self.edit('labyrinth', '--ex-party', 1, '--party-units', '100101', '--party-auto', 'true', '--party-speed', 2, '--party-set', 'none')
        self.assertTrue(any(path.startswith('/Battles/1') for path in result['ChangedPaths']))
        battles = self.read()['Battles']
        self.assertEqual((battles[1]['Party'], battles[1]['Opening']), ([100101, 0, 0, 0, 0], {'Auto': True, 'Speed': 2, 'SetUnits': []}))
        self.assertEqual(battles[0], original[0])
        self.edit('labyrinth', '--ex-seeds', '7,8,9')
        self.assertEqual([battle['Seed'] for battle in self.read()['Battles']], [7, 8, 9])
        for removed in ('--ex-log-ids', '--enter-id'):
            error = self.edit('labyrinth', removed, '1', ok=False)
            self.assertIn('openpcr-battle/v2', error['Message'])
        error = self.edit('scene', '--enemies', '770110101', ok=False)
        self.assertIn('quest', error['Message'])

    def test_mode_and_explicit_npc_edit(self):
        raw = self.read()
        npc = copy.deepcopy(raw['Player']['unit_list'][0])
        sources = [{'unit_id': 100101, 'unit_type': 2, 'unit_data': npc}]
        self.edit('labyrinth', '--difficulty', 2, '--alpha-rupee', 123,
                  '--unit-sources', self.write('sources.json', sources),
                  '--relics', self.write('relics.json', []), '--reinforcement-nodes', 'none')
        mode = self.read()['Labyrinth']
        self.assertEqual((mode['difficulty'], mode['alpha_rupee']), (2, 123))
        self.assertEqual(mode['unit_list'][0]['unit_data']['id'], 100101)
        # Player edits do not replace the independent NPC payload.
        self.edit('unit-edit', '--id', 100101, '--level', 90)
        self.assertEqual(self.read()['Labyrinth']['unit_list'], mode['unit_list'])

    def test_rejected_edits_preserve_input_and_existing_output(self):
        cases = [
            ('party', '--units', '1'),
            ('party', '--units', '100201'),
            ('unit-add', '--id', 100201, '--slot', 2),
            ('labyrinth', '--difficulty', -1),
            ('labyrinth', '--party-speed', 3),
            ('labyrinth', '--ex-seeds', '1,2,3'),
            ('labyrinth', '--unit-sources', self.write('missing-sources.json', [])),
            ('labyrinth', '--relics', self.write('bad-relic.json', [{'relic_id': 1, 'effected_count': 0, 'enable': 1}])),
            ('set', '--path', '/Battles/0/Unknown', '--value', '1'),
            ('set', '--path', '/Labyrinth/start_state', '--value', '{}'),
            ('scene', '--battle-id', 1),
            ('scene', '--enemies', '770110101'),
            ('set', '--path', '/Scene/Seed', '--value', '1'),
        ]
        output = self.root / 'existing.json'
        output.write_bytes(b'existing output')
        before = self.source.read_bytes()
        for command, *args in cases:
            with self.subTest(command=command, args=args):
                self.invoke(command, '--input', self.source, '--resources', RESOURCES,
                            '--output', output, *args, ok=False)
                self.assertEqual(self.source.read_bytes(), before)
                self.assertEqual(output.read_bytes(), b'existing output')
        for command, args in [
            ('set', ('--path', '/Labyrinth/unit_list', '--value', '[{"unit_id":100101,"unit_id":100201}]')),
            ('set', ('--patch', self.root / 'duplicate-patch.json')),
            ('labyrinth', ('--unit-sources', self.root / 'duplicate-sources.json')),
        ]:
            (self.root / 'duplicate-patch.json').write_text('[{"Path":"/Labyrinth/alpha_rupee","Value":0,"Value":1}]')
            (self.root / 'duplicate-sources.json').write_text('[{"unit_id":100101,"unit_id":100201}]')
            error = self.edit(command, *args, ok=False)
            self.assertIn('Duplicate', error['Message'])
            self.assertEqual(self.source.read_bytes(), before)

    def test_create_labyrinth_preset(self):
        output = self.root / 'created.json'
        args = ['create', '--resources', RESOURCES, '--master-version', '202611031924', '--preset', 'labyrinth-basic',
                '--units', '100101,100201', '--quest-id', 770110101, '--difficulty', 2, '--output', output]
        for missing in ('--quest-id', '--difficulty'):
            partial = list(args)
            del partial[partial.index(missing):partial.index(missing) + 2]
            error = self.invoke(*partial, ok=False)
            self.assertIn(missing, error['Message'])
            self.assertFalse(output.exists())
        for removed in (('--enemies', '770110101'), ('--now', 1793976058), ('--enter-id', 7)):
            error = self.invoke(*args, *removed, ok=False)
            self.assertIn('openpcr-battle/v2', error['Message'])
        self.invoke(*args, '--seed', 30, '--dry-run', 'true')
        self.invoke(*args)
        raw = json.loads(output.read_text())
        mode = raw['Labyrinth']
        self.assertEqual(raw['Battles'], [{'Party': [100101, 100201, 0, 0, 0], 'Opening': {'Auto': False, 'Speed': 1, 'SetUnits': []}, 'Seed': 12345}])
        self.assertEqual((mode['quest_id'], mode['difficulty']), (770110101, 2))
        self.assertEqual(mode['unit_list'], [{'unit_id': u, 'unit_type': 1} for u in (100101, 100201)])
        self.assertEqual((mode['relic_list'], mode['labyrinth_enhance_node_list'], mode['alpha_rupee']), ([], [], 0))
        self.assertNotIn('start_state', mode)
        # An EX quest with one described party is a one-party challenge.
        self.invoke('labyrinth', '--input', output, '--resources', RESOURCES, '--quest-id', 770331301, '--output', output)

    def test_runner_runs_one_battle_or_the_whole_challenge(self):
        process = run_battle(self.example(), self.root / 'normal')
        self.assertEqual(process.returncode, 0, process.stderr)
        result = json.loads(process.stdout)
        self.assertEqual(result['Schema'], 'openpcr-battle-result/v2')
        self.assertTrue(result['Completed'] and result['Battles'][0]['Settlement'] is not None)
        raw = json.loads(self.example(True).read_text())
        raw['Battles'][1] = {'Party': [100101, 0, 0, 0, 0], 'Opening': {'Auto': True, 'Speed': 4, 'SetUnits': [100101]}, 'Seed': 12346}
        process = run_battle(self.write('ex2.json', raw), self.root / 'ex')
        self.assertEqual(process.returncode, 0, process.stderr)
        challenge = json.loads(process.stdout)
        self.assertTrue(challenge['Completed'])
        self.assertEqual([battle['Index'] for battle in challenge['Battles']], [0, 1])
        self.assertEqual(len(challenge['Outcome']['Waves']), 2)
        self.assertIsNotNone(challenge['Battles'][0]['Next'])
        for index in (0, 1):
            self.assertEqual((self.root / 'ex' / f'battle-{index}.csv').read_text(), challenge['Battles'][index]['Battle']['Csv'])

    def test_export_runs_with_identical_csv_rng(self):
        for ex in (False, True):
            raw = json.loads(self.example(ex).read_text())
            raw['Battles'][0]['Seed'] = 8
            reference = self.write(f'reference-{ex}.json', raw)
            output = self.root / f'export-{ex}.json'
            self.invoke('scene', '--input', self.example(ex), '--resources', RESOURCES, '--output', output, '--seed', 8)
            exported = json.loads(output.read_text())
            self.assertEqual(exported, raw)
            results = []
            for i, path in enumerate((reference, output)):
                process = run_battle(path, self.root / f'{ex}-{i}', 12000)
                self.assertEqual(process.returncode, 0, process.stdout + process.stderr)
                results.append(json.loads(process.stdout))
            self.assertEqual(results[0], results[1])


if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument('--configuration', choices=('Release', 'ReleaseOptimized'), default='Release')
    parser.add_argument('--resources', type=Path)
    parser.add_argument('--artifacts', type=Path, help='Keep input, output and CSV artifacts per test.')
    options, remaining = parser.parse_known_args()
    CONFIGURATION = options.configuration
    RESOURCES = options.resources.resolve() if options.resources else None
    ARTIFACTS = options.artifacts.resolve() if options.artifacts else None
    unittest.main(argv=[__file__, *remaining])
