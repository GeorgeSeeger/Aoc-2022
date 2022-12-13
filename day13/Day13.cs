using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AoC_2022 {
    public class Day13 : IProblem {
        public string Name => nameof(Day13);

        public static PacketComparer comparer = new();

        public string Part1() {
            var input = GetInput();
            return input.Select((pair, i) => (inOrder: comparer.Compare(pair.left, pair.right) < 0, index: i + 1))
                        .Where(x => x.inOrder)
                        .Sum(x => x.index)
                        .ToString();
        }

        public string Part2() {
            var input = GetInput();
            var two = new Packet("[[2]]");
            var six = new Packet("[[6]]");
            var ordered = input.SelectMany(pair => (new[] { pair.left, pair.right })).Append(two).Append(six).Order(comparer).ToList();

            return ordered.Select((p, i) => (p, i: i + 1))
                          .Where(x => x.p == two || x.p == six)
                          .Aggregate(1, (acc, x) => acc * x.i)
                          .ToString();
        }

        public (Packet left, Packet right)[] GetInput() {
            var input = Regex.Split(File.ReadAllText("./day13/input"), "\r?\n\r?\n");

            return input.Select(pair => {
                var lines = Regex.Split(pair, "\r?\n");
                return (new Packet(lines[0]), new Packet(lines[1]));
            }).ToArray();
        }

        public class Packet {
            private List<object> Contents = new List<object>();

            public Packet(string line) : this(line, 1, out var _) { }

            private Packet(string line, int start, out int i) {
                var bag = new List<char>();
                for (i = start; i < line.Length; i++) {
                    var c = line[i];
                    switch (c) {
                        case '[':
                            var p = new Packet(line, i + 1, out i);
                            this.Contents.Add(p);
                            break;
                        case ']': 
                            ReadBag();
                            return;
                        case ',':
                            ReadBag();
                            break;
                        default: 
                            bag.Add(c);
                            break;
                    }
                }

                void ReadBag() {
                    if (bag.Any()) {
                        this.Contents.Add(int.Parse(string.Join("", bag)));
                        bag = new List<char>();
                    }
                }
            }

            public Packet(int i) {
                this.Contents.Add(i);
            }

            public object this[int i] => i < this.Length ? this.Contents[i] : null;

            public int Length => this.Contents.Count;
        }

        public class PacketComparer : IComparer<Packet> {
            public int Compare(Packet left, Packet right) {
                for (var i = 0; i < Math.Max(left.Length, right.Length); i++) {
                    var l = left[i];
                    var r = right[i];

                    var lp = l as Packet;
                    var rp = r as Packet;

                    if (l == null) return -1;
                    if (r == null) return 1;

                    if (l.GetType() == r.GetType() 
                        && l.GetType() == typeof(int)) {
                        if ((int)l == (int)r) continue;
                        return (int)l - (int)r;
                    }

                    if (l.GetType() == typeof(int)) {
                        lp = new Packet((int)l);
                    } 
                    if (r.GetType() == typeof(int)) {
                        rp = new Packet((int)r);
                    }

                    var compare = Compare(lp, rp);
                    if (compare == 0) continue;
                    return compare;
                }

                return 0;
            }
        }
    }
}