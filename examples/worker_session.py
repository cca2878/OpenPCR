"""Run an openpcr-battle/v2 request over JSON Lines and verify a checkpoint replay."""
import argparse
from concurrent.futures import ThreadPoolExecutor
import json
from pathlib import Path
import subprocess


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--runner', required=True, type=Path)
    parser.add_argument('--resources', required=True, type=Path)
    parser.add_argument('--request', required=True, type=Path)
    parser.add_argument('--max-engine-frames', type=int, default=5000)
    parser.add_argument('--timeout', type=float, default=60,
                        help='Maximum seconds per response (default: 60)')
    args = parser.parse_args()
    if args.timeout <= 0 or args.max_engine_frames <= 0:
        parser.error('--timeout and --max-engine-frames must be positive')
    request = json.loads(args.request.read_text(encoding='utf-8'))
    options = {'Resources': str(args.resources.resolve()),
               'MaxEngineFrames': args.max_engine_frames}
    sequence = 0
    with ThreadPoolExecutor(max_workers=1) as reader:
        process = subprocess.Popen(['dotnet', str(args.runner.resolve()), 'worker'],
                                   stdin=subprocess.PIPE, stdout=subprocess.PIPE,
                                   text=True, encoding='utf-8')
        try:
            def send(command, arguments=None):
                nonlocal sequence
                sequence += 1
                envelope = {'id': sequence, 'command': command, 'arguments': arguments}
                process.stdin.write(json.dumps(envelope) + '\n')
                process.stdin.flush()
                line = reader.submit(process.stdout.readline).result(timeout=args.timeout)
                if not line:
                    raise RuntimeError('Worker exited before responding')
                response = json.loads(line)
                if response.get('id') != sequence or response.get('ok') is not True:
                    raise RuntimeError(f'Worker rejected {command}: {response}')
                return response['result']

            prepared = send('prepare', {'request': request, 'resources': options['Resources']})
            send('create', {'preparedId': prepared['preparedId'], 'options': options})
            checkpoint = send('capture')['checkpointId']
            send('run')
            expected = send('result')
            if not expected['Completed']:
                raise RuntimeError('Scene did not settle within the host frame budget')
            send('restore', {'checkpointId': checkpoint})
            send('run')
            if send('result') != expected:
                raise RuntimeError('Checkpoint replay changed the result, CSV or RNG state')
            send('drop-checkpoint', {'checkpointId': checkpoint})
            send('dispose')
            send('shutdown')
            process.stdin.close()
            if process.wait(timeout=args.timeout) != 0:
                raise RuntimeError('Worker shutdown failed')
            print(json.dumps({'Completed': True, 'EngineFrame': expected['Battles'][-1]['Battle']['EngineFrame'],
                              'CheckpointReplayEqual': True, 'Commands': sequence}))
        finally:
            if process.poll() is None:
                process.kill()
                process.wait()
            process.stdin.close()
            process.stdout.close()


if __name__ == '__main__':
    main()
