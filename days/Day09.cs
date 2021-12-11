using advent_of_code.util;
using JetBrains.Annotations;

namespace advent_of_code.days;

[UsedImplicitly]
public class Day09 : IDay
{
    public Day09()
    {
        Steps = Common.CreateFileSteps(StepOne, StepTwo);
    }

    public IList<Func<string[], Task>> Steps { get; }

    private static int[][] ParseInput(string path)
    {
        return File
            .ReadLines(path)
            .Select(line => line.Select(ch => ch.ToString()).TryParseInt().ToArray())
            .ToArray();
    }

    private static int? TryGetValue(IReadOnlyList<int[]> f, int x, int y)
    {
        return y < 0 || y >= f.Count || x < 0 || x >= f[y].Length ? null : f[y][x];
    }

    private static void StepOne(string path)
    {
        Console.WriteLine("Parsing File...");
        var field = ParseInput(path);
        Console.WriteLine("Done! Calculating Risk...");
        var sum = 0;

        for (var y = 0; y < field.Length; ++y)
        for (var x = 0; x < field[y].Length; ++x)
        {
            var val = field[y][x];
            var isLowSpot =
                val < (TryGetValue(field, x - 1, y) ?? 10) &&
                val < (TryGetValue(field, x + 1, y) ?? 10) &&
                val < (TryGetValue(field, x, y - 1) ?? 10) &&
                val < (TryGetValue(field, x, y + 1) ?? 10);
            if (isLowSpot) sum += val + 1;
        }

        Console.WriteLine($"Done! Risk: {sum}");
    }

    private static int Traverse(IReadOnlyList<int[]> field, IReadOnlyList<bool[]> traversed, int x, int y)
    {
        if (traversed[y][x] || field[y][x] == 9) return 0;

        var frontier = new Queue<(int x, int y)>();
        frontier.Enqueue((x, y));

        var size = 0;

        while (frontier.Count > 0)
        {
            var (x1, y1) = frontier.Dequeue();
            if (traversed[y1][x1]) continue;
            traversed[y1][x1] = true;
            ++size;
            foreach (var (x2, y2) in stackalloc[] { (x1 - 1, y1), (x1 + 1, y1), (x1, y1 - 1), (x1, y1 + 1) })
            {
                var val = TryGetValue(field, x2, y2);
                if (val is null or 9) continue;
                if (traversed[y2][x2]) continue;
                frontier.Enqueue((x2, y2));
            }
        }

        return size;
    }

    private static void StepTwo(string path)
    {
        Console.WriteLine("Parsing File...");
        var field = ParseInput(path);
        Console.WriteLine("Done! Finding Basins...");

        var traversed = field.Select(row => row.Select(e => false).ToArray()).ToArray();

        var basins = new List<(int X, int Y, int Size)>();

        for (var y = 0; y < field.Length; ++y)
        for (var x = 0; x < field[y].Length; ++x)
        {
            var val = field[y][x];
            var basinSize = Traverse(field, traversed, x, y);
            if (basinSize > 0) basins.Add((x, y, basinSize));
        }

        var top3Basins = basins
            .OrderByDescending(e => e.Size)
            .Take(3)
            .ToList();

        Console.WriteLine("Top 3 Basins:");
        foreach (var (basinX, basinY, basinSize) in top3Basins)
            Console.WriteLine($"  ({basinX,2}, {basinY,2}) => {basinSize,3}");

        Console.WriteLine($"Multiplied sizes: {top3Basins.Aggregate(1, (a, b) => a * b.Size)}");
    }
}