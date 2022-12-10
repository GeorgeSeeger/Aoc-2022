using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace AoC_2022 {
    public class Day8 : IProblem {
        public string Name => nameof(Day8);

        public string Part1() {
            var visible = new HashSet<(int, int)>();
            var input = GetInput();

            // left to right and right to left
            for (var i = 0; i < input.Length; i++) {
                var highest = 0;
                for (var j = 0; j < input[i].Length; j++) {
                    // add the outside rims
                    if (i == 0 || j == 0 || i == input[i].Length - 1 || j == input[i].Length - 1) {
                        visible.Add((i, j));
                        highest = input[i][j];
                        continue;
                    }

                    if (input[i][j] > highest) {
                        if (!visible.Contains((i, j)))
                            visible.Add((i, j));
                        highest = input[i][j];
                    }
                }
                
                highest = 0;
                for (var j = input[i].Length - 1; j > 0; j--) {
                    if (input[i][j] > highest) {
                        if (!visible.Contains((i, j)))
                            visible.Add((i, j));
                        highest = input[i][j];
                    }
                }
            }

            // top to bottom and bottom to top
            for (var j = 0; j < input[0].Length; j++) {
                var highest = 0;
                for (var i = 0; i < input.Length; i++) {
                    if (input[i][j] > highest) {
                        if (!visible.Contains((i, j)))
                            visible.Add((i, j));
                        highest = input[i][j];
                    }
                }
                
                highest = 0;
                for (var i = input.Length - 1; i > 0; i--) {
                    if (input[i][j] > highest) {
                        if (!visible.Contains((i, j)))
                            visible.Add((i, j));
                        highest = input[i][j];
                    }
                }
            }

            return visible.Count().ToString();
        }

        public string Part2() {
            return GetInput().SelectMany((line, i) => line.Select((h, j) => ScenicScore(GetInput(), h,  i, j)))
                        .OrderByDescending(i => i).First().ToString();
        }

        private int ScenicScore(int[][] input, int height, int y, int x) {
            if (y == 0 || x == 0 || y == input.Length - 1 || x == input[y].Length - 1) return 0;

            var up = 0;
            for (var i = y - 1; i >= 0; i--) {
                up++;
                if (input[i][x] >= height) break;
            }

            var down = 0;
            for (var i = y + 1; i < input.Length; i++) {
                down++;
                if (input[i][x] >= height) break;
            }

            var left = 0;
            for (var j = x - 1; j >= 0; j--) {
                left++;
                if (input[y][j] >= height) break;
            }
            var right = 0;
            for (var j = x + 1; j < input[y].Length; j++) {
                right++;
                if (input[y][j] >= height) break;
            }

            return up * down * left * right;
        }

        private int[][] GetInput() {
            return File.ReadAllLines("./day8/input").Select(line => line.Select(c => c - '0').ToArray()).ToArray();
        }
    }
}