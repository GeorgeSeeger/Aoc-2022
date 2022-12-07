using System.Collections.Generic;
using System.Linq;

namespace AoC_2022 {
    public class Day13_2016 : IProblem
    {
        public string Name => "2016_day13";

        private const uint sneed = 1362;

        public string Part1()
        {
            var target = new Point(31, 39);
            var (paths, _) = FindPaths(pts => pts.All(p => p.head != target));
            return paths.First().distance.ToString();
        }

        private static (List<Path>, HashSet<Point>) FindPaths(System.Func<IList<Path>, bool> matchCondition) {
            var start = new Point(1, 1);
            var visited = new HashSet<Point> { start };
            var paths = new List<Path> { new Path(start, 0) };
            while (matchCondition(paths)) {
                var newPaths = new List<Path>();
                foreach (var path in paths) {
                    var visiting = path.head.VisitableNeighbours(sneed);
                    newPaths.AddRange(visiting.Where(p => !visited.Contains(p)).Select(p => new Path(p, path.distance + 1)));
                }

                paths = newPaths;
                foreach (var p in paths) visited.Add(p.head);
            }

            return (paths, visited);
        }

        public string Part2() {
            var target = new Point(31, 39);
            var (paths, visited) = FindPaths(pts => pts.First().distance < 50);
            return visited.Count().ToString();
        }

        public record Path (Point head, int distance);

        public record Point(uint x, uint y) {
            public bool CanMove(uint num) {
                var v = x * x + 3 * x + 2 * x * y + y + y * y + num;
                return (System.Convert.ToString(v, 2).Count(c => c == '1') & 1) == 0;
            }

            public IEnumerable<Point> Neighbours() {
                if (this.x > 0) yield return new Point(x - 1, y);
                if (this.y > 0) yield return new Point(x, y - 1);
                yield return new Point(x + 1, y);
                yield return new Point(x, y + 1);
            }

            public Point[] VisitableNeighbours(uint num) {
                return Neighbours().Where(p => p.CanMove(num)).ToArray();
            }
        }
    }
}