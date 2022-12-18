using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AoC_2022 {
    public class Day18 : IProblem
    {
        public string Name => nameof(Day18);

        public string Part1() {
            var input = GetInput("./day18/input").ToHashSet();

            var answer = 0;
            foreach (var cube in input) {
                foreach (var dir in Directions)
                    if (!FaceIsEnclosed(input, cube, dir, 0)) answer++;
            }
            return answer.ToString();
        }

        private Point[] GetInput(string path) {
            return File.ReadAllLines(path).Select(l => l.Split(",").Select(i => int.Parse(i)).ToArray()).Select(l => new Point(l[0], l[1], l[2])).ToArray();
        }

        public string Part2() {
            var input = GetInput("./day18/input").ToHashSet();
            
            var answer = input.AsParallel()
            .SelectMany(cube => 
                Directions.Select(dir => FaceIsEnclosed(input, cube, dir))
            )
            .ToArray()
            .Sum(b => b ? 0 : 1);

            return answer.ToString();
        }

        private static bool FaceIsEnclosed(HashSet<Point> structure, Point point, int[] direction, int steps = 30) {
            var start = point.Add(direction);

            // do we have a neighbour?
            if (structure.Contains(start)) return true;

            // send out a ray, if it encounters nothing, then it's almost certainly not enclosed
            if (Enumerable.Range(0, steps).Select(i => start.Add(direction.Select(x => i * x).ToArray())).All(p => !structure.Contains(p))) return false;

            // else try an expanding gas cloud
            var gas = new HashSet<Point>{ start };
            var gasShell = new HashSet<Point> { start };
            for (var i = 0; i < steps; i++) {
                gasShell = gasShell.SelectMany(p => Directions.Select(d => p.Add(d)))
                    .Where(p => !structure.Contains(p) && !gas.Contains(p))
                    .ToHashSet();

                if (!gasShell.Any()) return true;
                foreach (var p in gasShell) {
                    gas.Add(p);
                }
            }

            return false;
        }

        private static int[][] Directions = new[] { 
            new[] { 1, 0, 0 },
            new[] { -1, 0, 0 },
            new[] { 0, 1, 0 },
            new[] { 0, -1, 0 },
            new[] { 0, 0, 1 },
            new[] { 0, 0, -1 },
        };

        private record Point(int x, int y, int z) {
            public Point Add(int[] direction) {
                return new Point(x + direction[0], y + direction[1], z + direction[2]);
            }
        }
    }
}