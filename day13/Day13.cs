using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AoC_2022 {
    public class Day13 : IProblem {
        public string Name => nameof(Day13);

        public string Part1() {
            var input = GetInput();
            return input.Select((pair, i) => (inOrder: pair.left < pair.right, i:i + 1))
            .Where(x => x.inOrder)
            .Sum(x => x.i)
            .ToString();
        }

        public string Part2() {
            var input = GetInput();
            var two = new Packet("[[2]]");
            var six = new Packet("[[6]]");
            var ordered = input.SelectMany(pair => new[] { pair.left, pair.right }).Append(two).Append(six).Order(new PacketComparer()).ToList();

            return ordered.Select((p, i) => (p, i: i + 1)).Where(x => Object.ReferenceEquals(x.p, two) || object.ReferenceEquals(x.p, six)).Aggregate(1, (acc, x) => acc * x.i).ToString();
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

            private Packet(int i) {
                this.Contents.Add(i);
            }

            public object this[int i] => i < this.Length ? this.Contents[i] : null;

            public int Length => this.Contents.Count;

            public override bool Equals(object other) => other is Packet o ? this == o : false;

            public static bool operator ==(Packet left, Packet right) {
                if (left.Length == right.Length) {
                    for (var i = 0; i < left.Length; i++) {
                        if (left[i].Equals(right[i])) continue;
                        return false;
                    }

                    return true;
                }

                return false;
            }

            public static bool operator !=(Packet left, Packet right) => !(left == right);

            public static bool operator <(Packet left, Packet right) {
                for (var i = 0; i < Math.Max(left.Length, right.Length); i++) {
                    var l = left[i];
                    var r = right[i];

                    var lp = l as Packet;
                    var rp = r as Packet;

                    if (l == null) return true;
                    if (r == null) return false;

                    if (l.GetType() == r.GetType() 
                        && l.GetType() == typeof(int)) {
                        if ((int)l == (int)r) continue;
                        return (int)l < (int)r;
                    }

                    if (l.GetType() == typeof(int)) {
                        lp = new Packet((int)l);
                    } 
                    if (r.GetType() == typeof(int)) {
                        rp = new Packet((int)r);
                    }

                    if (lp == rp) continue;
                    return lp < rp;
                }

                return false;
            }

            public static bool operator >(Packet left, Packet right) {
                return !(left < right);
            }

            public override int GetHashCode() {
                throw new NotImplementedException();
            }
        }

        public class PacketComparer : IComparer<Packet> {
            public int Compare(Packet x, Packet y) {
                if (x == y) return 0;
                if (x < y) return -1; 
                return 1;
            }
        }
    }
}