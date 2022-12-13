using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AoC_2022 {
    public class Day12 : IProblem {
        public string Name => nameof(Day12);

        public string Part1() {
            var input = GetInput();
            var start = input.SelectMany((l, y) => (l.Select((c,x ) => (c: c, x: x, y: y)))).First(p => p.c == 'S');
            var end = input.SelectMany((l, y) => (l.Select((c,x ) => (c: c, x: x, y: y)))).First(p => p.c == 'E');
            var path = ExploreToEnd(input, (start.x, start.y), (end.x, end.y));

            WriteTo("./day12/output", input, path);
            return (path.Count() - 1).ToString();
        }

        private void WriteTo(string v, char[][] input, List<(int x, int y)> path) {
            var pathSet = new HashSet<(int, int)>(path);
            var output = input.Select((l, y) => string.Join("", l.Select((c, x) => path.Contains((x, y)) ? '#' : c)));

            File.WriteAllLines(v, output);
        }

        public string Part2() {
            var input = GetInput();
            var end = input.SelectMany((l, y) => (l.Select((c,x ) => (c: c, x: x, y: y)))).First(p => p.c == 'E');

            var starts = input.SelectMany((l, y) => (l.Select((c,x ) => (c: c, x: x, y: y)))).Where(p => p.c == 'a' && p.x <= 2).ToArray();

            var paths = starts.Select(s => ExploreToEnd(input, (s.x, s.y), (end.x, end.y))).Where(p => p != null);

            var shortest = paths.OrderBy(p => p.Count()).First();
            WriteTo("./day12/output-2", input, shortest);
            return (shortest.Count() - 1).ToString();
        }

        public List<(int x,int y)> ExploreToEnd(char[][] input, (int x, int y) start, (int x, int y) end) {
            var visited = new HashSet<(int x, int y)> { start };
            var paths = new List<List<(int x,int y)>>() { new List<(int x, int y)> { start }};
            
            while (!paths.Any(p => p.Last() == end)) {
                if (!paths.Any()) return default;

                paths = paths.SelectMany(path => {
                    var tail = path.Last();
                    var level = input[tail.y][tail.x];

                    var adjacent = NeighboursOf(input, tail)
                        .Where(p => !visited.Contains(p))
                        .Where(p => {
                            var next = input[p.y][p.x];
                            return next <= level + 1 && next != 'E'
                                || next == 'a'
                                || (level == 'z');
                        })
                        .ToArray();

                    foreach (var p in adjacent) visited.Add(p);
                    var list = adjacent.Select(p => path.Append(p).ToList());
                    return list;
                }).ToList();
            }

            return paths.First(p => p.Last() == end);
        }

        private IEnumerable<(int x, int y)> NeighboursOf(char[][] input, (int x, int y) tail) {
            if (tail.x > 0) yield return (tail.x - 1, tail.y);
            if (tail.y > 0) yield return (tail.x, tail.y - 1);
            if (tail.x < input[0].Length - 1) yield return (tail.x + 1, tail.y);
            if (tail.y < input.Length - 1) yield return (tail.x , tail.y + 1);
        }

        public char[][] GetInput() {
            return File.ReadAllLines("./day12/input").Select(l => l.ToArray()).ToArray();
        }
    }
}