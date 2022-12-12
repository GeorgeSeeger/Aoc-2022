using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace AoC_2022 { 
    public class Day9 : IProblem {
        public string Name => nameof(Day9);

        public string Part1() {
            var input = GetInput();
            var snake = new Segment(2);

            foreach (var instr in input) {
                snake.Move(instr.Direction, instr.Steps);
            }

            return snake.Tail.DistinctVisited.ToString();
        }

        public string Part2() {
            var input = GetInput();
            var snake = new Segment(10);
            
            foreach (var instr in input) {
                snake.Move(instr.Direction, instr.Steps);
            }

            return snake.Tail.DistinctVisited.ToString();
        }

        public (char Direction, int Steps)[] GetInput() {
            return File.ReadAllLines("./day9/input").Select(l => {
                var instrs = l.Split(" ");
                return (instrs[0][0], int.Parse(instrs[1]));
            }).ToArray();
        }

        public class Segment {
            public Segment() {
                this.Position = new Point(0, 0);
                this.visited = new List<Point> { this.Position };
            }

            public Segment(int tailSegments) : this() {
                if (tailSegments == 1) return;
                this.next = new Segment(tailSegments - 1);
            }

            public Point Position { get; set; }

            private List<Point> visited;

            private Segment next;

            public Segment Tail => this.next == null ? this : this.next.Tail;

            public void Move(char dir, int steps) {
                for (var i = 0; i < steps; i++) Move(dir);
            }

            private void Move(char dir) {
                switch (dir) {
                    case 'U': 
                        this.Position = new Point(this.Position.X, this.Position.Y + 1);
                        break;
                    case 'D': 
                        this.Position = new Point(this.Position.X, this.Position.Y - 1);
                        break;
                    case 'R':
                        this.Position = new Point(this.Position.X + 1, this.Position.Y);
                        break;
                    case 'L':
                        this.Position = new Point(this.Position.X - 1, this.Position.Y);
                        break;
                    default: throw new System.ArgumentOutOfRangeException();
                }

                this.visited.Add(this.Position);
                this.next.CatchUp(this.Position);
            }

            private void CatchUp(Point to) {
                var x = this.Position.X;
                var y = this.Position.Y;
                if (Math.Abs(to.X - x) > 1 || Math.Abs(to.Y - y) > 1) {
                    var delta = to.Compare(this.Position);
                    this.Position = new Point(x + delta.dX, y + delta.dY);                     
                    this.visited.Add(this.Position);
                }

                if (this.next != null) this.next.CatchUp(this.Position);
            }

            public int DistinctVisited => this.visited.Distinct().Count();
        }

        public record Point(int X, int Y) {
            public (int dX, int dY) Compare(Point p) {
                return (this.X == p.X ? 0 : this.X > p.X ? 1 : -1, this.Y == p.Y ? 0 : this.Y > p.Y ? 1 : -1);
            }
        };
    }
}