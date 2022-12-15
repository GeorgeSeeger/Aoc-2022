using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AoC_2022 {
    public class Day15 : IProblem
    {
        public string Name => nameof(Day15);

        public static Regex extractor = new Regex(@"Sensor at x=(-?\d+), y=(-?\d+): closest beacon is at x=(-?\d+), y=(-?\d+)");

        public string Part1() {
            // var (y, path) = (10, "./day15/test");
            var (y, path) = (2_000_000, "./day15/input");
            var input = GetInput(path);

            var definitelyNoBeacon = new List<Point>();
            foreach (var pair in input) {
                var (sensor, beacon) = pair;
                var d = Math.Abs(sensor.x - beacon.x) + Math.Abs(sensor.y - beacon.y);
                var r = d - Math.Abs(sensor.y - y);

                if (r <= 0) continue;

                definitelyNoBeacon.AddRange(Enumerable.Range(sensor.x - r, 2 * r + 1).Select(x => new Point(x, y)));
            }

            return definitelyNoBeacon.Distinct().Except(input.Select(p => p.beacon)).Count().ToString();
        }

        public string Part2() {
            //  var (yxMin, yxMax, path) = (0, 20, "./day15/test");
            var (yxMin, yxMax, path) = (0, 4_000_000, "./day15/input");
            
            var input = GetInput(path);
            
            var xyPossibles = new Dictionary<int, int[]>();
            for (var y = yxMin; y < yxMax; y++) {
                var gaps = FindGaps(input, yxMin, yxMax, (d, sensor) =>{
                    int r = d - Math.Abs(sensor.y - y);
                    return (sensor.x, r);
                }).ToArray();
                if (gaps.Length > 1) xyPossibles.Add(y, gaps.Where(i => yxMin <= i && i <= yxMax).ToArray());
            }

            xyPossibles = xyPossibles.ToDictionary(kvp => kvp.Key, kvp => {
                var y = kvp.Key;
                return kvp.Value.Select(x => (x, possibles: FindGaps(input, yxMin, yxMax, (d, sensor) => {
                    var r = d - Math.Abs(sensor.x - x);
                    return (sensor.y, r);
                }))).Where(pair => pair.possibles.Contains(y))
                .Select(pair => pair.x)
                .ToArray();
            });

            if (xyPossibles.Values.Sum(v => v.Length) == 1) {
                var (y, x) = (xyPossibles.Keys.Single(), xyPossibles.Single().Value.Single());
                return (4_000_000L * x + y) .ToString();
            }

            return "I could not find the beacon";
        }

        private IEnumerable<int> FindGaps((Point sensor, Point beacon)[] input, int yxMin, int yxMax, Func<int, Point, (int i, int r)> blockedOutFunc) {
            var blocks = input.Select(pair => {
                    var (sensor, beacon) = pair;
                    var d = Math.Abs(sensor.x - beacon.x) + Math.Abs(sensor.y - beacon.y);
                    return blockedOutFunc(d, sensor);
            }).Where(p => p.r > 0)
            .Select(p => (low: p.i - p.r, high: p.i + p.r))
            .OrderBy(p => p.low)
            .ToArray();

            for (var i = yxMin; i < yxMax; i++) {
                var isFree = true;

                foreach (var bound in blocks) {
                    isFree = true;
                    if (bound.low <= i && i < bound.high) {
                        i = bound.high;
                        isFree = false;
                    }
                }
                if (isFree)
                    yield return i;
            }
        }

        private (Point sensor, Point beacon)[] GetInput(string path) {
            return File.ReadAllLines(path)
                       .Select(line => extractor.Match(line))
                       .Select(m => m.Groups.Values.Skip(1).Select(s => int.Parse(s.Value)).ToArray())
                       .Select(a => (sensor: new Point(a[0], a[1]), beacon: new Point(a[2], a[3])))
                       .ToArray();
        }

        public record Point(int x, int y);
    }
}