using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AoC_2022 {
    public class Day14_2016 : IProblem {
        public string Name => "2016_Day14";

        const string salt = "yjdafjpo";

        public string Part1() {
            var keys = 0;
            var index = 0;
            while (keys < 64) {
                var hash = Hash($"{salt}{++index}");
                var potentialKeys = TripleChars.Match(hash);
                if (potentialKeys.Success) {
                    var triple = potentialKeys.Value[0];

                    var isKey = DoNext1000HashsContain(string.Join("", Enumerable.Repeat(triple, 5)), index, s => Hash(s));
                    if (isKey) keys++;
                }
            }

            return index.ToString();
        }

        public string Part2() {
            var keys = new ConcurrentBag<int>();
            const int parallelChunkSize = 10_000;
            
            for (var i = 0; keys.Count() <= 64; i++) {
                Parallel.For(parallelChunkSize * i, parallelChunkSize * (i + 1) - 1, (index, state) => {
                    var hash = StretchedHash($"{salt}{index}");
                    var potentialKeys = TripleChars.Match(hash);
                    if (potentialKeys.Success) {
                        var triple = potentialKeys.Value[0];

                        var isKey = DoNext1000HashsContain(string.Join("", Enumerable.Repeat(triple, 5)), index, s => StretchedHash(s));
                        if (isKey) keys.Add(index);
                    }
                });
            }

            return keys.OrderBy(i => i).ToArray()[63].ToString();
        }

        private bool DoNext1000HashsContain(string quints, int index, Func<string, string> hashFn) {
            for (var i = 1; i <= 1000; i++) {
                if (hashFn($"{salt}{index + i}").Contains(quints)) return true;
            }

            return false;
        }

        private string Hash(string input, bool cache = true) {
            if (cache && Hashes.TryGetValue(input, out var val)) return val;

            var hash = Convert.ToHexString(MD5.HashData(Encoding.UTF8.GetBytes(input))).ToLower();
            if (cache) Hashes.Add(input, hash);
            return hash;
        }

        private string StretchedHash(string input) {
            var str = input;

            if (StretchedHashes.TryGetValue(input, out var hash)) return hash;
        
            for (var i = 0; i <= 2016; i++) {
                str = Hash(str, cache: false);
            }

            StretchedHashes.TryAdd(input, str);

            return str;
        }

        private static Regex TripleChars = new Regex("([0-9a-f])\\1\\1", RegexOptions.Compiled);

        private ConcurrentDictionary<string, string> StretchedHashes = new ConcurrentDictionary<string, string>();

        private IDictionary<string, string> Hashes = new Dictionary<string, string>();
    }
}