using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AoC_2022 {
    public class Day14 : IProblem {
        public string Name => nameof(Day14);

        public string Part1() {
            var filledCave = GetInput("./day14/input").FillWithSand();

            return filledCave.SandCount.ToString();
        }

        public string Part2() {
            var cave = GetInput("./day14/input").AddFloor().FillWithSand();
          
            cave.WriteToFile("./day14/output");
            return cave.SandCount.ToString();
        }

        private Cave GetInput(string filePath) {
            var rockLocations = File.ReadAllLines(filePath)
                                    .Select(l => l.Split(" -> ").Select(s => s.Split(",").Select(a => int.Parse(a)).ToArray()).Select(a => new Point(a[0], a[1])))
                                    .SelectMany(l => l.Aggregate(new List<Point>(), (acc, p) => {
                                        if (!acc.Any()) {
                                            acc.Add(p);
                                            return acc;
                                        }

                                        var last = acc.Last();
                                        IEnumerable<Point> range;
                                        if (last.Y == p.Y) {
                                            range = Enumerable.Range(Math.Min(last.X, p.X), Math.Abs(last.X - p.X) + 1).Select(x => new Point(x, Math.Min(last.Y, p.Y)));
                                        } else {
                                            range = Enumerable.Range(Math.Min(last.Y, p.Y), Math.Abs(last.Y - p.Y) + 1).Select(y => new Point(Math.Min(last.X, p.X), y));
                                        }

                                        acc.AddRange(p.X < last.X || p.Y < last.Y ? range.Reverse() : range);
                                        return acc;
                                    })).Distinct().ToArray();
            return new Cave(rockLocations);
        }

        public record Point(int X, int Y) {
            public override string ToString() => $"({this.X}, {this.Y})";
        }

        public class Cave {
            private HashSet<Point> rocks;

            private HashSet<Point> sands = new HashSet<Point>();

            private int? floorY;
            public Cave(IEnumerable<Point> rocks) {
                this.rocks = rocks.ToHashSet();
            }

            public int SandCount => this.sands.Count();

            public Cave AddFloor() {
                this.floorY = this.rocks.Select(r => r.Y).Max() + 2;
                return this;
            }

            public Cave FillWithSand() {
                var yMax = rocks.Select(r => r.Y).Max();
                while (true) {
                    var (x, y) = (500, 0);
                    var fallenIntoVoid = true;
                    if (sands.Contains(new Point(x, y))) return this;

                    while (y < (yMax + 10)) {
                        var nextPlaces = new[] { new Point(x, y + 1), new Point(x - 1, y + 1), new Point(x + 1, y + 1) };
                        var nextPlace = nextPlaces.FirstOrDefault(IsNotOccupied);
                        
                        if (nextPlace == null) {
                            sands.Add(new Point(x, y));
                            fallenIntoVoid = false;
                            break;
                        }
                        (x,y) = nextPlace;
                    }

                    if (fallenIntoVoid)
                        return this;
                }
            }

            private bool IsNotOccupied(Point p) {
                return !this.rocks.Contains(p) && !this.sands.Contains(p) && (!this.floorY.HasValue || p.Y < this.floorY.Value);
            }

            public void WriteToFile(string filePath) {
                var yMin = 0;
                var yMax = rocks.Concat(sands).Select(r => r.Y).Max();
                var xMin = rocks.Concat(sands).Select(r => r.X).Min();
                var xMax = rocks.Concat(sands).Select(r => r.X).Max();
                var lines = Enumerable.Range(yMin, yMax + 2).Select(j => 
                    string.Join("", Enumerable.Range(xMin - 1, (xMax - xMin) + 2).Select(i => {
                        var p = new Point(i, j);
                        return sands.Contains(p) ? 'o' : this.IsNotOccupied(p) ? '.' : '#';
                    }))
                );

                File.WriteAllLines(filePath, lines);
            }
        }
    }
}