using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace AoC_2022 {
    public class Day7 : IProblem {
        public string Name => nameof(Day7);

        public string Part1() {
            var root = ParseFiles();
            var folders = Flatten(root).ToArray();
            return folders.Where(d => d.Size <= 100_000).Sum(d => d.Size).ToString();
        }

        public Dir ParseFiles() {
            var input = File.ReadAllLines("./day7/input");
            
            var root = new Dir { Name = "/" };
            var current = root;

            for (var i = 1; i < input.Length; i++) {
                var line = input[i].Split(" ");
                if (line[0] == "$") {
                    if (line[1] == "cd") {
                        if (line[2] == "..") {
                            current = current.Parent;
                            continue;
                        }

                        current = current.Contents.First(d => d.Name == line[2]);
                        continue;
                    }

                    if (line[1] == "ls") continue;
                }

                if (line[0] == "dir") {
                    var child = new Dir { Name = line[1], Parent = current };
                    
                    current.Contents.Add(child);
                }

                if (int.TryParse(line[0], out var fileSize)) {
                    current.FileSizes += fileSize;
                } 
            }

            return root;
        }

        public IEnumerable<Dir> Flatten(Dir dir) {
            yield return dir;
            foreach (var c in dir.Contents) {
                foreach (var cd in Flatten(c))
                    yield return cd;
            }
        }

        public string Part2() {
            var capacity = 70_000_000;
            var needed = 30_000_000;
            var root = ParseFiles();
            var folders = Flatten(root).Where(d => d != root);

            var toDelete = needed - (capacity - root.Size);
            return folders.Where(d => d.Size > toDelete).OrderBy(d => d.Size).First().Size.ToString();
        }
    }

    public class Dir {
        public string Name;

        private int? size;

        public int Size { get {
                if (size.HasValue) return size.Value;

                size = this.FileSizes + Contents.Sum(d => d.Size);
                return size.Value;
            }
        }

        public int FileSizes;

        public Dir Parent;

        public List<Dir> Contents = new List<Dir>();

    }
}