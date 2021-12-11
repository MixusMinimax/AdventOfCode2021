using advent_of_code.util;
using JetBrains.Annotations;

namespace advent_of_code.days;

[UsedImplicitly]
public class Day11 : IDay
{
    public Day11()
    {
        Steps = Common.CreateFileSteps(
            Common.RequireInt(StepOne, "steps"),
            (path, _) => StepTwo(path)
        );
    }

    public IList<Func<string[], Task>> Steps { get; }

    private static void StepOne(string path, int steps)
    {
        var cave = ParseCave(path);

        var flashCount = 0;
        for (var step = 0; step < steps; ++step)
        {
            ResetFlashedThisStep(cave);
            for (var y = 0; y < cave.Length; ++y)
            for (var x = 0; x < cave[y].Length; ++x)
                flashCount += MaybeFlash(cave, x, y);

            ResetFlashedEnergyLevels(cave);
        }

        Console.WriteLine($"Total Flash Count: {flashCount}");
    }

    private static void StepTwo(string path)
    {
        var cave = ParseCave(path);
        var size = cave.Aggregate(0, (a, b) => a + b.Length);
        var step = 0;

        for (; step < int.MaxValue; ++step)
        {
            var flashCount = 0;
            ResetFlashedThisStep(cave);
            for (var y = 0; y < cave.Length; ++y)
            for (var x = 0; x < cave[y].Length; ++x)
                flashCount += MaybeFlash(cave, x, y);

            if (flashCount == size) break;

            ResetFlashedEnergyLevels(cave);
        }

        Console.WriteLine($"Steps required to synchronize: {step + 1}");
    }

    private static int MaybeFlash(IReadOnlyList<Octopus[]> cave, int x, int y)
    {
        if (y < 0 || y >= cave.Count || x < 0 || x >= cave[y].Length) return 0;

        ref var val = ref cave[y][x];
        val.EnergyState++;
        if (val.EnergyState < 10 || val.FlashedThisStep) return 0;

        val.FlashedThisStep = true;
        var flashCount = 1;
        for (var yy = y - 1; yy <= y + 1; ++yy)
        for (var xx = x - 1; xx <= x + 1; ++xx)
            if (xx != x || yy != y)
                flashCount += MaybeFlash(cave, xx, yy);

        return flashCount;
    }

    private static void ResetFlashedThisStep(IEnumerable<Octopus[]> cave)
    {
        foreach (var row in cave)
            for (var i = 0; i < row.Length; ++i)
                row[i].FlashedThisStep = false;
    }

    private static void ResetFlashedEnergyLevels(IEnumerable<Octopus[]> cave)
    {
        foreach (var row in cave)
            for (var i = 0; i < row.Length; ++i)
                if (row[i].FlashedThisStep)
                    row[i].EnergyState = 0;
    }

    private static Octopus[][] ParseCave(string path)
    {
        var cave = File
            .ReadLines(path)
            .Select(line => line.Select(Common.ToString).TryParseInt().Select(e => new Octopus(e)).ToArray())
            .ToArray();
        return cave;
    }

    [UsedImplicitly]
    private static void PrintCave(IEnumerable<Octopus[]> cave)
    {
        foreach (var row in cave)
        {
            foreach (var octopus in row) Console.Write(octopus.EnergyState);

            Console.WriteLine();
        }
    }

    private record struct Octopus(int EnergyState, bool FlashedThisStep = false);
}