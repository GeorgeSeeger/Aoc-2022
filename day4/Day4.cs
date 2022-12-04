using System.IO;
using System.Linq;

namespace AoC_2022 {
    public class Day4 : IProblem {
        public string Name => "Day4";

        private static (Range First, Range Second)[] GetInput() {
            return File.ReadAllLines("./day4/input").Select(l => {
                var entries = l.Split(",").Select(s => s.Split("-").Select(j => int.Parse(j)).ToArray()).ToArray();
                return (new Range { Start = entries[0][0], End = entries[0][1] },
                       new Range { Start = entries[1][0], End = entries[1][1]});
            }).ToArray();
        }

        public string Part1(){
            var input = GetInput();

            return input.Count(pair => pair.First.Start <= pair.Second.Start && pair.Second.End <= pair.First.End
                                       || pair.Second.Start <= pair.First.Start && pair.First.End <= pair.Second.End).ToString();
        }

        public string Part2() {
            var input = GetInput();

            return input.Count(pair => pair.First.Start <= pair.Second.Start && pair.First.End >= pair.Second.Start
                                    || pair.Second.Start <= pair.First.Start && pair.Second.End >= pair.First.Start).ToString();
        }

        struct Range {
            public int Start;
            
            public int End;
        }
    }
}