namespace AoC_2022 {
    public interface IProblem {
        string Name { get; }

        string Part1();

        string Part2();
    }
}