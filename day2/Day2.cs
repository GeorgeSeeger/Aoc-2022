using System;
using System.IO;
using System.Linq;

namespace AoC_2022 {
    public class Day2 : IProblem {
        public string Name => "day2";

        public static string[] GetInput() {
            return File.ReadAllLines("./day2/input");
        }
        public string Part1() {
            return GetInput().Select(l => ScorePart1(l[0], l[2])).Sum().ToString();
        }

        private int ScorePart1(char first, char second) {
            var score = 1 + second - 'X';
            switch (first) {
                case 'A': // rock
                    score += second == 'X' ? 3 : second == 'Y' ? 6 : 0;
                    break;
                case 'B': // paper
                    score += second == 'X' ? 0 : second == 'Y' ? 3 : 6;
                    break;
                case 'C':
                    score += second == 'X' ? 6 : second == 'Y' ? 0 : 3;
                    break;
            }
            return score;
        }

        public string Part2() {
            return GetInput().Select(l => ScorePart2(l[0], l[2])).Sum().ToString();
        }

        private int ScorePart2(char abc, char xyz) {
            var score = 3 * (xyz - 'X');
            switch(abc) {
                case 'A':
                    score += xyz == 'X' ? 3 : xyz == 'Y' ? 1 : 2;
                    break;
                case 'B': 
                    score += xyz == 'X' ? 1 : xyz == 'Y' ? 2 : 3;
                    break;
                case 'C':
                    score += xyz == 'X' ? 2 : xyz == 'Y' ? 3 : 1;
                    break;
            }
            return score;
        }
    }
}