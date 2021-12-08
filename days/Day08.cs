using System.Collections;
using advent_of_code.util;

namespace advent_of_code.days;

public class Day08 : IDay
{
    public IList<Func<string[], Task>> Steps { get; }

    public Day08()
    {
        Steps = Common.CreateFileSteps(StepOne, StepTwo);
    }

    private static int ParseDigit(string segments)
    {
        return segments switch
        {
            "abcefg" => 0,
            "cf" => 1,
            "acdeg" => 2,
            "acdfg" => 3,
            "bcdf" => 4,
            "abdfg" => 5,
            "abdefg" => 6,
            "acf" => 7,
            "abcdefg" => 8,
            "abcdfg" => 9,
            _ => -1
        };
    }

    private static IEnumerable<List<string>[]> GetLines(string path)
    {
        return File
            .ReadLines(path)
            .Select(line => line
                .Split('|')
                .Select(screen => screen
                    .Split(' ')
                    .Where(word => !string.IsNullOrWhiteSpace(word))
                    .ToList()).ToArray())
            .Where(e => e.Length >= 2);
    }

    private static void StepOne(string path)
    {
        var lines = GetLines(path);

        var result = lines.Select(line =>
            line[1].Count(e => e.Length switch { 2 or 3 or 4 or 7 => true, _ => false })
        ).Sum();

        Console.WriteLine($"Count: {result}");
    }

    private static void StepTwo(string path)
    {
        var lines = GetLines(path);

        var result = lines.Select(line =>
        {
            const string allLetters = "abcdefg";

            var words = line.SelectMany(e => e).ToList();

            // Brute force all possibilities because it's fast enough
            var permutation = FunctionalPermutations(allLetters, allLetters.Length)
                .Select(letters =>
                    string.Concat(letters)
                ).First(perm => words
                    .Select(word => string.Concat(
                        word.Select(letter => allLetters[perm.IndexOf(letter)])
                            .OrderBy(y => y)))
                    .All(word => ParseDigit(word) != -1)
                );

            return line[1]
                .Select(word => word
                    .Select(letter => allLetters[permutation.IndexOf(letter)]))
                .Select(word => word
                    .OrderBy(y => y)).Select(x => string.Concat(x))
                .Select(ParseDigit)
                .Aggregate(0, (a, b) => a * 10 + b);
        }).Sum();

        Console.WriteLine($"Sum: {result}");
    }

    private static IEnumerable<IEnumerable<T>> FunctionalPermutations<T>(IEnumerable<T> elements, int length)
    {
        var enumerated = elements.ToList();
        if (length < 2) return enumerated.Select(t => new[] { t });
        return enumerated.SelectMany((elementI, i) =>
            FunctionalPermutations(enumerated.Take(i).Concat(enumerated.Skip(i + 1)), length - 1)
                .Select(subEi => new[] { elementI }.Concat(subEi)));
    }
}

// 089 76762423
// 089 76760