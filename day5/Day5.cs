using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AoC_2022 {
    public class Day5 : IProblem {
        public string Name => nameof(Day5);

        public Input GetInput() {
            var text = File.ReadAllText("./day5/input").Split("\n\n");

            var stack = text[0].Split("\n");

            var input = new Input() { Stacks = new Dictionary<int, Stack<char>>() };

            for (var i = stack.Length - 1; i >= 0; i--) {
                for (var j = 1; j < stack[0].Length; j += 4) {
                    var c = stack[i][j];
                    if (int.TryParse(c.ToString(), out var index)) {
                        input.Stacks.Add(index, new Stack<char>());
                        continue;
                    }

                    if (c == ' ') continue;

                    input.Stacks[1 + j / 4].Push(c);
                }
            }

            input.Instructions =  text[1].Split("\n", System.StringSplitOptions.RemoveEmptyEntries).Select(l => {
                var line = new Regex("move | from | to ", RegexOptions.None).Split(l);
                var instr = (take: int.Parse(line[1]), from: int.Parse(line[2]), to: int.Parse(line[3]));
                return instr;
            }).ToArray();

            return input;
        }

        public string Part1() {
            var input = GetInput();

            foreach (var (take, from, to) in input.Instructions) {
                for (var i = 0; i < take; i++) {
                    input.Stacks[to].Push(input.Stacks[from].Pop());
                }
            }

            return string.Join("", input.Stacks.Select(q => q.Value.First()));
        }

        public string Part2() {
            var input = GetInput();
            foreach (var (take, from, to) in input.Instructions) {
                var taking = Enumerable.Range(1, take).Select(i => input.Stacks[from].Pop()).ToList();
                taking.Reverse();
                foreach (var c in taking) input.Stacks[to].Push(c);
            }

            return string.Join("", input.Stacks.Select(q => q.Value.First()));
        }


        public struct Input {
            public Dictionary<int, Stack<char>> Stacks;

            public (int take, int from, int to)[] Instructions;
        }
    }
}