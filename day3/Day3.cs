using System.IO;
using System.Linq;

namespace AoC_2022 {

    public class Day3 : IProblem {
        public string Name => "Day3";

        private static string[] GetInput() {
            return File.ReadAllLines("./day3/input");
        }

        public string Part1() {
            var input = GetInput();
            var answer = input.Select(line =>
            {
                var first = line.Take(line.Length / 2);
                var second = line.Skip(line.Length / 2);

                var itemInBoth = first.First(c => second.Contains(c));
                return ScoreFor(itemInBoth);
            }).Sum();

            return answer.ToString();
        }

        private static int ScoreFor(char itemInBoth) {
            return (char.IsLower(itemInBoth)
                       ? 1 + itemInBoth - 'a'
                       : 27 + itemInBoth - 'A');
        }

        public string Part2() {
            var input = GetInput();
            var answer = input.Chunk(3).Select(group => {
                var first = group.First();
                var second = group.Skip(1).First();
                var third = group.Last();

                var inAllThree = third.FirstOrDefault(c => second.Where(ch => first.Contains(ch)).Contains(c));

                return ScoreFor(inAllThree);
            }).Sum();

            return answer.ToString();
        }
    }
}