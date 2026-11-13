"""Black-box CLI tests: exported requests run, edits preserve inputs, rejected edits never replace files."""
import argparse
import json
from pathlib import Path
import sqlite3
import subprocess
import tempfile
import unittest

REPO = Path(__file__).resolve().parents[2]
EDITOR = REPO / 'tools/OpenPCR.BattleEditor/bin/Release/net10.0/OpenPCR.BattleEditor.dll'
RUNNER = REPO / 'tools/OpenPCR.Runner/bin/Release/net10.0/OpenPCR.Runner.dll'
RESOURCES = None
# Integration assertions below require this exact data set, including rank-38 equipment holes.
SUPPORTED_VERSION = '202609081902'
VERSION = SUPPORTED_VERSION


def unit(request, unit_id):
    return next(item for item in request['Player']['unit_list'] if item['id'] == unit_id)


class ResourceFreeTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        if not EDITOR.exists():
            raise RuntimeError(f'Build Release first: {EDITOR}')

    def invoke(self, *args):
        return subprocess.run(['dotnet', str(EDITOR), *map(str, args)],
                              capture_output=True, text=True, cwd=REPO)

    def test_help_explains_explicit_inputs(self):
        result = self.invoke('--help')
        self.assertEqual(result.returncode, 0, result.stderr)
        for option in ('--resources', '--master-version', '--knight-enhance', '--quad-speed'):
            self.assertIn(option, result.stdout)

    def test_inspect_examples_without_resources(self):
        for name in ('trial.json', 'clan-rehearsal.json', 'labyrinth-normal.json', 'labyrinth-ex.json'):
            result = self.invoke('inspect', '--input', REPO / 'examples/battles' / name)
            self.assertEqual(result.returncode, 0, result.stderr)
            self.assertTrue(json.loads(result.stdout)['Ok'])

    def test_rejected_command_preserves_output_without_resources(self):
        with tempfile.TemporaryDirectory(prefix='openpcr-editor-public-') as directory:
            output = Path(directory) / 'scene.json'
            output.write_bytes(b'existing output')
            result = self.invoke('create', '--preset', 'trial-basic', '--output', output)
            self.assertEqual(result.returncode, 1)
            self.assertFalse(json.loads(result.stderr)['Ok'])
            self.assertIn('--resources', json.loads(result.stderr)['Message'])
            self.assertEqual(output.read_bytes(), b'existing output')

    def test_inspect_rejects_invalid_input(self):
        with tempfile.TemporaryDirectory(prefix='openpcr-editor-public-') as directory:
            source = Path(directory) / 'scene.json'
            source.write_text('{"Schema":"unsupported"}')
            result = self.invoke('inspect', '--input', source)
            self.assertEqual(result.returncode, 1)
            self.assertFalse(json.loads(result.stderr)['Ok'])

    def test_inspect_rejects_missing_required_skill_fields_without_resources(self):
        baseline = json.loads((REPO / 'examples/battles/trial.json').read_text())
        with tempfile.TemporaryDirectory(prefix='openpcr-editor-public-') as directory:
            source = Path(directory) / 'scene.json'
            for field in ('skill_id', 'skill_level'):
                with self.subTest(field=field):
                    request = json.loads(json.dumps(baseline))
                    del request['Player']['unit_list'][0]['main_skill'][0][field]
                    source.write_text(json.dumps(request))
                    result = self.invoke('inspect', '--input', source)
                    self.assertEqual(result.returncode, 1, result.stdout)
                    error = json.loads(result.stderr)
                    self.assertFalse(error['Ok'])
                    self.assertIn(field, error['Message'])


class BattleEditorTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        if RESOURCES is None:
            raise unittest.SkipTest('Optional integration suite: provide --resources ROOT '
                                    f'--master-version {SUPPORTED_VERSION}')
        for path in (EDITOR, RUNNER):
            if not path.exists():
                raise RuntimeError(f'Build Release first: {path}')
        cls.directory = tempfile.TemporaryDirectory(prefix='openpcr-battle-editor-')
        cls.root = Path(cls.directory.name)
        cls.baseline = cls.root / 'baseline.json'
        cls.invoke('create', '--preset', 'trial-basic', '--resources', RESOURCES,
                   '--master-version', VERSION, '--units', '100101', '--output', cls.baseline)

    @classmethod
    def tearDownClass(cls):
        cls.directory.cleanup()

    @classmethod
    def invoke(cls, *args, ok=True):
        process = subprocess.run(['dotnet', str(EDITOR), *map(str, args)], capture_output=True, text=True, cwd=REPO)
        if process.returncode != (0 if ok else 1):
            raise AssertionError(f'{args}\nexit={process.returncode}\n{process.stdout}\n{process.stderr}')
        result = json.loads(process.stdout if ok else process.stderr)
        if result['Ok'] != ok:
            raise AssertionError(result)
        return result['Result'] if ok else result

    def setUp(self):
        self.input = self.root / (self._testMethodName + '.json')
        self.input.write_bytes(self.baseline.read_bytes())

    def edit(self, command, *args, ok=True):
        return self.invoke(command, '--resources', RESOURCES, '--input', self.input, '--output', self.input, *args, ok=ok)

    def read(self):
        return json.loads(self.input.read_text())

    def run_battle(self, request):
        csv_dir = self.root / (Path(request).stem + '-csv')
        process = subprocess.run(['dotnet', str(RUNNER), 'run', '--resources', str(RESOURCES),
                                  '--request', str(request), '--max-engine-frames', '20000', '--csv-dir', str(csv_dir)],
                                 capture_output=True, text=True, cwd=REPO)
        self.assertEqual(process.returncode, 0, process.stderr)
        return json.loads(process.stdout), (csv_dir / 'battle-0.csv').read_bytes()

    def test_catalog_real_resource_identity_and_rejections(self):
        for kind, identity in [('units', 100101), ('enemies', 401010101), ('backgrounds', 710011)]:
            result = self.invoke('catalog', '--resources', RESOURCES, '--master-version', VERSION,
                                 '--kind', kind, '--id', identity)
            self.assertEqual(result['Items'][0]['Id'], identity)
        info = self.invoke('unit-info', '--resources', RESOURCES, '--master-version', VERSION, '--id', 100101)
        self.assertTrue(info['Promotions'])
        self.assertEqual(info['Skills']['union_burst'], 1001001)
        for args in [('--kind', 'bad'), ('--kind', 'units', '--limit', 0), ('--kind', 'units', '--extra', 'bad')]:
            self.invoke('catalog', '--resources', RESOURCES, '--master-version', VERSION, *args, ok=False)

    def test_created_json_is_standalone_and_runs_like_existing_example(self):
        expected = json.loads((REPO / 'examples/battles/trial.json').read_text())
        self.assertEqual(self.read(), expected)
        self.assertEqual(self.run_battle(self.input), self.run_battle(REPO / 'examples/battles/trial.json'))
        result = self.invoke('validate', '--input', self.input, '--resources', RESOURCES)
        self.assertTrue(result['Valid'])
        self.assertEqual(result['Prepared']['Stage'], 'prepared')

    def test_scene_in_place_and_dry_run(self):
        before = self.input.read_bytes()
        preview = self.invoke('scene', '--input', self.input, '--resources', RESOURCES, '--seed', -1, '--dry-run', 'true')
        self.assertTrue(preview['DryRun'])
        self.assertIn('/Battles/0/Seed', preview['ChangedPaths'])
        self.assertEqual(self.input.read_bytes(), before)
        self.edit('scene', '--seed', -1, '--fps', 30, '--team-level', 80, '--auto', 'true', '--speed', 2, '--knight-enhance', 'false')
        edited = self.read()
        self.assertEqual(edited['Battles'][0]['Seed'], -1)
        self.assertEqual(edited['Battles'][0]['Opening'], {'Auto': True, 'Speed': 2, 'SetUnits': []})
        self.assertEqual(edited['Player']['user_info']['team_level'], 80)
        self.assertFalse(edited['Player']['Releases']['KnightEnhance'])
        self.assertEqual(edited['Player']['unit_list'], json.loads(before)['Player']['unit_list'])
        self.assertTrue(self.run_battle(self.input)[0]['Completed'])

    def test_unit_creation_rank_equipment_and_party(self):
        before = self.input.read_bytes()
        # Rank 2 includes unenhanceable rank-1 equipment. A uniform level 1 must fail, not clamp.
        self.edit('unit-add', '--id', 100201, '--rank', 2, '--equipment', 'all', '--enhance', 1, ok=False)
        self.assertEqual(self.input.read_bytes(), before)
        self.edit('unit-add', '--id', 100201, '--slot', 5, '--level', 80, '--rank', 3,
                  '--skills', 70, '--rarity', 4, '--equipment', 'all', '--enhance', 1)
        raw = unit(self.read(), 100201)
        self.assertEqual(raw['unit_level'], 80)
        self.assertEqual(len(raw['equip_slot']), 6)
        self.assertTrue(all(e['enhancement_level'] == 1 and e['enhancement_pt'] > 0 for e in raw['equip_slot']))
        before = self.input.read_bytes()
        self.edit('unit-edit', '--id', 100201, '--rank', 4, ok=False)
        self.assertEqual(self.input.read_bytes(), before)
        self.edit('unit-edit', '--id', 100201, '--rank', 4, '--equipment', 'none', '--skills', 75)
        self.edit('party', '--units', '0,100201,0,100101')
        self.assertEqual(self.read()['Battles'][0]['Party'], [0, 100201, 0, 100101, 0])
        self.edit('unit-remove', '--id', 100201, ok=False)
        self.assertTrue(self.run_battle(self.input)[0]['Completed'])
        self.edit('party', '--units', '100101')
        self.edit('unit-remove', '--id', 100201)
        self.assertNotIn(100201, [item['id'] for item in self.read()['Player']['unit_list']])

    def test_rank38_equipment_preserves_unreleased_slot_positions(self):
        with sqlite3.connect(f'file:{RESOURCES}/masterdata/{VERSION}_unhashed.db?mode=ro', uri=True) as master:
            expected = master.execute('SELECT equip_slot_1,equip_slot_2,equip_slot_3,equip_slot_4,equip_slot_5,equip_slot_6 '
                                      'FROM unit_promotion WHERE unit_id=100101 AND promotion_level=38').fetchone()
        self.assertEqual(expected[0], 999999)
        self.assertIn(999999, expected[1:])  # Also cover holes between real equipment, not just the first slot.

        def check_slots():
            slots = unit(self.read(), 100101)['equip_slot']
            self.assertEqual(len(slots), 6)
            self.assertEqual([slot['id'] for slot in slots], list(expected))
            for slot in slots:
                if slot['id'] == 999999:
                    self.assertEqual(slot, {'is_slot': False, 'id': 999999, 'enhancement_level': 0, 'enhancement_pt': 0, 'rank': 0})
                else:
                    self.assertTrue(slot['is_slot'])
            return slots

        self.invoke('create', '--preset', 'trial-basic', '--resources', RESOURCES,
                    '--master-version', VERSION, '--units', '100101', '--rank', 38,
                    '--equipment', 'all', '--enhance', 5, '--output', self.input)
        check_slots()
        self.edit('unit-edit', '--id', 100101, '--enhance', 3)
        slots = check_slots()
        self.assertTrue(all(slot['enhancement_level'] == 3 for slot in slots if slot['is_slot']))
        self.edit('unit-edit', '--id', 100101, '--equipment', 'all', '--enhance', 4)
        check_slots()

    def test_clan_preset_hp_long_values_and_run(self):
        self.invoke('create', '--preset', 'clan-rehearsal-basic', '--resources', RESOURCES,
                    '--master-version', VERSION, '--units', '100101,100201', '--boss-hp', 3000000000,
                    '--boss-current-hp', 2800000000, '--output', self.input)
        self.assertEqual(self.read()['ClanBattle']['boss_hp'], {'max_hp': 3000000000, 'current_hp': 2800000000})
        before = self.input.read_bytes()
        self.edit('scene', '--phase', 9, '--order', 3, '--lap', 4, ok=False)
        self.assertEqual(self.input.read_bytes(), before)
        self.edit('scene', '--phase', 1, '--order', 3, '--lap', 4, '--quad-speed', 'false')
        self.assertTrue(self.run_battle(self.input)[0]['Completed'])
        before = self.input.read_bytes()
        self.edit('scene', '--boss-current-hp', 4000000000, ok=False)
        self.assertEqual(self.input.read_bytes(), before)

    def test_advanced_edits_preserve_data_and_reject_protocol_violations(self):
        self.edit('set', '--path', '/Player/Releases/ChangeRarity', '--value', 'false')
        self.assertFalse(self.read()['Player']['Releases']['ChangeRarity'])
        self.edit('stories', '--ids', '1001002')
        self.assertEqual(self.read()['Player']['read_story_ids'], [1001002])
        self.edit('stories', '--ids', 'none')
        before = self.input.read_bytes()
        for args in [('--path', '/Battles/0/Seed', '--value', 'true'),
                     ('--path', '/Battles/0/Seedd', '--value', '2'),
                     ('--path', '/Operations', '--value', '[]'),
                     ('--path', '/Battles/0/Party/05', '--value', '0'),
                     ('--path', '/Player/Releases/Extra', '--value', 'true'),
                     ('--path', '/Player/Releases/ChangeRarity~2', '--value', 'true')]:
            self.edit('set', *args, ok=False)
            self.assertEqual(self.input.read_bytes(), before)

    def test_bad_inputs_never_replace_output(self):
        before = self.input.read_bytes()
        cases = [('scene', '--enemies', '2147483647'), ('scene', '--fps', 45), ('scene', '--seconds', 12),
                 ('scene', '--now', 1790000000), ('scene', '--boss-hp', 123), ('scene', '--speed', 3),
                 ('scene', '--seed', 1, '--seed', 2), ('scene', '--bad', 'x'),
                 ('party', '--units', '100101,100101'), ('unit-add', '--id', 100101),
                 ('unit-edit', '--id', 100101, '--level', -1), ('unit-edit', '--id', 100101, '--rank', 9999),
                 ('unit-edit', '--id', 100101, '--battle-rarity', 5), ('stories', '--ids', '1,1')]
        for command, *args in cases:
            self.edit(command, *args, ok=False)
            self.assertEqual(self.input.read_bytes(), before)
        bad = self.root / 'invalid.json'
        bad.write_text(self.input.read_text().replace('"Schema":', '"Schema":"duplicate","Schema":', 1))
        self.invoke('validate', '--input', bad, '--resources', RESOURCES, ok=False)
        self.assertEqual(self.input.read_bytes(), before)

    def test_batch_patch_updates_linked_fields_together(self):
        clan = json.loads((REPO / 'examples/battles/clan-rehearsal.json').read_text())
        patch = self.root / 'patch.json'
        patch.write_text(json.dumps([{'Path': '/ClanBattle', 'Value': clan['ClanBattle']},
                                     {'Path': '/Battles/0/Seed', 'Value': 7}]))
        self.edit('set', '--patch', patch)
        self.assertEqual(self.read()['ClanBattle']['kind'], 'clan')
        self.assertEqual(self.read()['Battles'][0]['Seed'], 7)
        self.assertEqual(self.read()['Player'], json.loads(self.baseline.read_text())['Player'])
        before = self.input.read_bytes()
        # An invalid later change prevents the preceding valid change from being saved.
        patch.write_text(json.dumps([{'Path': '/Battles/0/Seed', 'Value': 4}, {'Path': '/Unknown', 'Value': 5}]))
        self.edit('set', '--patch', patch, ok=False)
        self.assertEqual(self.input.read_bytes(), before)


if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument('--resources', type=Path,
                        help='Explicit resource root for the optional integration suite')
    parser.add_argument('--master-version', choices=[SUPPORTED_VERSION],
                        help='Required data version for the fixed integration cases')
    options, remaining = parser.parse_known_args()
    if (options.resources is None) != (options.master_version is None):
        parser.error('--resources and --master-version must be supplied together')
    RESOURCES = options.resources.resolve() if options.resources else None
    if RESOURCES is not None and not (RESOURCES / 'masterdata' / f'{options.master_version}_unhashed.db').is_file():
        parser.error('The explicitly requested integration master database does not exist')
    unittest.main(argv=[__file__, *remaining])
