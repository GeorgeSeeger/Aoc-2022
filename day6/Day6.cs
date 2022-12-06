using System.IO;
using System.Linq;

namespace AoC_2022 {
    public class Day6 : IProblem {
        public string Name => nameof(Day6);
        public string Part1() {
            var input = File.ReadAllLines("./day6/input")[0];

            return FindPositionOfNDistinct(input, 4).ToString();
        }
        private static int FindPositionOfNDistinct(string input, int n) {
            for (var i = n - 1; i < input.Length; i++) {
                if (input.Substring(i - n + 1, n).Distinct().Count() == n) return (1 + i);
            }

            throw new System.Exception("Ahh I couldn't find it");
        }

        public string Part2() {
            var input = File.ReadAllLines("./day6/input")[0];
            return FindPositionOfNDistinct(input, 14).ToString();
        }
    }
}