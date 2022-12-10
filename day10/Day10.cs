using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AoC_2022 {
    public class Day10 : IProblem {
        public string Name => nameof(Day10);

        public string Part1() {
            var input = GetInput();
            var registerValues = Execute(input);

            return registerValues.Select((r, i) => (i + 1) * r).Where((v, i) => ShouldReadValue(i)).Sum().ToString();
        }

        public string Part2() {
            var input = GetInput();
            var registerValues = Execute(input);

            var isLit = new List<List<bool>>();
            for (var y = 0; y < 6; y++) {
                isLit.Add(new List<bool>());
                for (var x = 0; x < 40; x++) {
                    isLit.Last().Add(Math.Abs(x - registerValues[40 * y + x]) <= 1);
                }
            }

            var answer = string.Join("\n", isLit.Select(l => string.Join("", l.Select(b => b ? '#' : '.'))));
            Console.Write("\n" + answer + "\n");
            return "see above";
        }

        public int[] Execute((string instr, int? value)[] input) {
            var register = 1;
            var values = new Queue<int>();
            var output = new int[240];
            for (var i = 0; i < output.Length; i++) {
                 if (i < input.Length) {
                    var (instr, value) = input[i];
                    values.Enqueue(0);
                    if (instr == "addx"){
                        values.Enqueue(value.Value);
                    }
                }

                output[i] = register;

                if (values.Any())
                    register += values.Dequeue();
            }

            return output;
        }

        private bool ShouldReadValue(int i) => (i - 19) % 40 == 0;

        private (string instr, int? val)[] GetInput() {
            return File.ReadAllLines("./day10/input").Select(l =>{
                var line = l.Split(' ');
                return (line[0], line.Length == 1 ? (int?)null : int.Parse(line[1]));
            }).ToArray();
        }
    }

}