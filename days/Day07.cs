using advent_of_code.util;
using JetBrains.Annotations;

namespace advent_of_code.days;

[UsedImplicitly]
public class Day07 : IDay
{
    private static int[]? _costCache;

    public Day07()
    {
        Steps = Common.CreateFileSteps(StepOne, StepTwo);
    }

    public IList<Func<string[], Task>> Steps { get; }

    private static int GetCost(IEnumerable<int> numbers, int target)
    {
        return numbers.Select(e => Math.Abs(e - target)).Sum();
    }

    private static int GetCost(int steps)
    {
        int Cost(int s)
        {
            return s == 0 ? 0 : s + Cost(s - 1);
        }

        _costCache ??= Enumerable.Range(0, 128).Select(Cost).ToArray();
        return steps < _costCache.Length ? _costCache[steps] : Cost(steps);
    }

    private static int GetCostAlternative(IEnumerable<int> numbers, int target)
    {
        return numbers.Select(e => GetCost(Math.Abs(e - target))).Sum();
    }

    private static void StepOne(string path)
    {
        var numbers = File.ReadLines(path).FirstOrDefault("").SplitNumbers().ToList();
        numbers.Sort();
        var median = numbers.Count % 2 == 1
            ? numbers[numbers.Count / 2]
            : (numbers[numbers.Count / 2 - 1] + numbers[numbers.Count / 2]) / 2;

        Console.WriteLine($"Cost at target={median}: {GetCost(numbers, median)}");
    }

    private static void StepTwo(string path)
    {
        var numbers = File.ReadLines(path).FirstOrDefault("").SplitNumbers().ToList();

        var min = numbers.Min();
        var max = numbers.Max();

        var bestTarget = Enumerable.Range(min, max - min + 1).MinBy(e => GetCostAlternative(numbers, e));

        Console.WriteLine($"Cost at target={bestTarget}: {GetCostAlternative(numbers, bestTarget)}");
    }
}