using System.Text.RegularExpressions;
using advent_of_code.util;
using Humanizer;
using JetBrains.Annotations;

namespace advent_of_code.days;

[UsedImplicitly]
public class Day13 : IDay
{
    public Day13()
    {
        Steps = Common.CreateFileSteps(Step);
    }

    public IList<Func<string[], Task>> Steps { get; }

    private static void Step(string path)
    {
        Console.WriteLine("Day 13!");
        var (holes, foldInstructions, size) = ParseInput(path);

        PrintHoles(holes, size);
        Console.WriteLine($"Hole Count: {CountHoles(holes, size)}");

        for (var index = 0; index < foldInstructions.Count; ++index)
        {
            var instruction = foldInstructions[index];
            ExecuteInstruction(holes, ref size, instruction);

            if (index != 0 && index != foldInstructions.Count - 1) continue;
            Console.WriteLine($"\nAfter {(index + 1).ToOrdinalWords()} step:");
            PrintHoles(holes, size);
            Console.WriteLine($"Hole Count: {CountHoles(holes, size)}");
        }
    }

    private static (bool[,] Holes, IList<Instruction> Instructions, Coordinate size) ParseInput(string path)
    {
        var foldInstructionStrings = new List<string>();
        var coords = File
            .ReadLines(path)
            .TakeWhileAndSplit(e => !string.IsNullOrWhiteSpace(e), foldInstructionStrings)
            .TryParseCoordinates()
            .ToList();
        var foldInstructions = foldInstructionStrings
            .SelectWhere<string, Instruction>(TryParseInstruction)
            .ToList();
        var size = coords.Aggregate(
            new Coordinate(0, 0),
            (size, coord) =>
                new Coordinate(Math.Max(size.X, coord.X + 1), Math.Max(size.Y, coord.Y + 1))
        );
        var holes = new bool[size.X, size.Y];
        foreach (var (i, j) in coords) holes[i, j] = true;

        return (holes, foldInstructions, size);
    }

    private static bool TryParseInstruction(string line, out Instruction instruction)
    {
        var match = Regex.Match(line, @"^fold along ([xy])=(\d+)$");
        if (!match.Success)
        {
            instruction = default;
            return false;
        }

        instruction = new Instruction(
            match.Groups[1].Value switch { "x" => Axis.X, "y" => Axis.Y, _ => default },
            int.Parse(match.Groups[2].Value)
        );
        return true;
    }

    private static void ExecuteInstruction(bool[,] holes, ref Coordinate size, Instruction instruction)
    {
        var (axis, where) = instruction;
        size = axis switch
        {
            Axis.X => new Coordinate(where, size.Y),
            Axis.Y => new Coordinate(size.X, where),
            _ => new Coordinate(0, 0)
        };
        for (var y = 0; y < size.Y; ++y)
        for (var x = 0; x < size.X; ++x)
        {
            var (xx, yy) = axis switch
            {
                Axis.X => (2 * where - x, y),
                Axis.Y => (x, 2 * where - y),
                _ => (0, 0)
            };
            holes[x, y] |= holes.GetValueOrDefault(xx, yy);
        }
    }

    private static void PrintHoles(bool[,] holes, Coordinate size)
    {
        var (sx, sy) = size;
        if (sx > 64 || sy > 32)
        {
            Console.WriteLine("Too big to print!");
            return;
        }

        for (var y = 0; y < sy; ++y)
        {
            for (var x = 0; x < sx; ++x) Console.Write(holes[x, y] ? '#' : '.');

            Console.WriteLine();
        }
    }

    private static int CountHoles(bool[,] holes, Coordinate size)
    {
        var (sx, sy) = size;
        var sum = 0;
        for (var y = 0; y < sy; ++y)
        for (var x = 0; x < sx; ++x)
            sum += holes[x, y] ? 1 : 0;

        return sum;
    }

    private readonly record struct Instruction(Axis Axis, int Where);
}