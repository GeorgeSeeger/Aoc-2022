using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace AoC_2022 {
    class Program {
        static void Main(string[] args) {
            var problemName = args.Any() ? args.First() : string.Empty;

            var problems = typeof(Program)
                                   .Assembly
                                   .GetTypes()
                                   .Where(t => t.GetInterfaces().Any(it => it == typeof(IProblem)))
                                   .Select(t => (IProblem)Activator.CreateInstance(t))
                                   .Where(p => string.IsNullOrWhiteSpace(problemName) ? true : string.Equals(p.Name, problemName, StringComparison.InvariantCultureIgnoreCase))
                                   .ToArray();

            foreach (var problem in problems) {
                var (firstPart, elapsedMillisecondsPt1) = Time(() => problem.Part1());
                if (!string.IsNullOrWhiteSpace(firstPart))
                    Console.WriteLine($"{problem.Name} Part 1: {firstPart} ({elapsedMillisecondsPt1}ms)");

                var (secondPart, elapsedMillisecondsPt2) = Time(() => problem.Part2());
                if (!string.IsNullOrWhiteSpace(secondPart))
                    Console.WriteLine($"{problem.Name} Part 2: {secondPart} ({elapsedMillisecondsPt2}ms)");
            }
        }

        static (string, long elapsedMilliseconds) Time(Func<string> block) {
            try {
                sw.Start();
                var result = block.Invoke();
                sw.Stop();
                return (result, sw.ElapsedMilliseconds);
            }
            catch (Exception e) {
                 return (e.Message, sw.ElapsedMilliseconds);
            }
             finally {
                sw.Reset();
            }
        }

        static Stopwatch sw = new Stopwatch();
    }
}
