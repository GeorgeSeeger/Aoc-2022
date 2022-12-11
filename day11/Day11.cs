using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AoC_2022 {
    public class Day11 : IProblem
    {
        public string Name => nameof(Day11);

        public string Part1() {
            var input = GetInput().ToDictionary(m => m.Id, m => m);

            for (var i = 0; i < 20; i++) {
                foreach (var monkey in input.Values.OrderBy(m => m.Id)) {
                    monkey.TakeTurn(input, i => i / 3);
                }
            }

            return input.Values.OrderByDescending(m => m.InspectedItems).Take(2).Aggregate(1L, (acc, m) => m.InspectedItems * acc).ToString();
        }


        public string Part2() {
            var input = GetInput().ToDictionary(m => m.Id, m => m);
            var modulo = input.Values.Select(m => m.TestFactor).Aggregate(1, (acc, tf) => acc * tf);

            for (var i = 0; i < 10_000; i++) {
                foreach (var monkey in input.Values.OrderBy(m => m.Id)) {
                    monkey.TakeTurn(input, i => i % modulo);
                }
            }

            return input.Values.OrderByDescending(m => m.InspectedItems).Take(2).Aggregate(1L, (acc, m) => m.InspectedItems * acc).ToString();
        }

        private IEnumerable<Monkey> GetInput() {
            var file = File.ReadAllText("./day11/input");
            var monkeys = file.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);
            foreach (var text in monkeys) {
                var id = int.Parse(new Regex(@"Monkey (\d):").Match(text).Groups[1].Value);
                var items = new Regex(@"items: ([\d, ]+)").Match(text).Groups[1].Value.Split(", ").Select(s => long.Parse(s));
                var testFactor = int.Parse(new Regex(@"divisible by (\d+)").Match(text).Groups[1].Value);
                var operation = ParseOperation(new Regex(@"new = (.*)").Match(text).Groups[1].Value);
                var ifTrueMonkey = int.Parse(new Regex(@"If true: throw to monkey (\d)").Match(text).Groups[1].Value);
                var ifFalseMonkey = int.Parse(new Regex(@"If false: throw to monkey (\d)").Match(text).Groups[1].Value);

                yield return new Monkey(id, items, operation, testFactor, ifTrueMonkey, ifFalseMonkey);
            }

            Func<long, long> ParseOperation(string op) {
                if (op == "old * old") return i => i * i;
                var factor = long.Parse(new Regex(" [*+] ").Split(op)[1]);
                if (op.Contains("*")) return i => i * factor;
                return i => i + factor;
            }
        }

        public class Monkey {
            public int Id;

            public Queue<long> Items = new Queue<long>();

            private Func<long, long> Operation;
            
            public int TestFactor;

            private Func<long, bool> Test;

            private int ThrowToIfTrue;

            private int ThrowToIfFalse;

            public Monkey(int id, IEnumerable<long> items, Func<long, long> operation, int testFactor, int ifTrueMonkey, int ifFalseMonkey) {
                Id = id;
                this.Items = new Queue<long>(items);
                Operation = operation;
                TestFactor = testFactor;
                Test = i => i % TestFactor == 0;
                this.ThrowToIfTrue = ifTrueMonkey;
                this.ThrowToIfFalse = ifFalseMonkey;
            }

            public long InspectedItems { get; private set; }

            public void TakeTurn(IDictionary<int, Monkey> monkeys, Func<long, long> boredFunc) {
                while (Items.Any()) {
                    var item = Items.Dequeue();
                    var now = boredFunc(Operation(item));

                    if (Test(now)) 
                        monkeys[ThrowToIfTrue].Items.Enqueue(now);
                    else    
                        monkeys[ThrowToIfFalse].Items.Enqueue(now);
                    InspectedItems++;
                }
            }
        }
    }
}