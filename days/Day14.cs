using advent_of_code.util;
using Humanizer;
using JetBrains.Annotations;

namespace advent_of_code.days;

using InsertionRules = IReadOnlyDictionary<(char A, char B), char>;

[UsedImplicitly]
public class Day14 : IDay
{
    public Day14()
    {
        Steps = Common.CreateFileSteps(
            Common.RequireInt(StepOne, "steps", 10)
        );
    }

    public IList<Func<string[], Task>> Steps { get; }

    private static InsertionRules ParseInsertionRules(IEnumerable<string> lines)
    {
        return lines
            .Select(line => line.Split("->").Select(e => e.Trim()).ToArray())
            .Where(e => e.Length == 2)
            .Where(e => e[0].Length == 2 && e[1].Length == 1)
            .ToDictionary(e => (e[0][0], e[0][1]), e => e[1][0]);
    }

    private static (string Template, InsertionRules InsertionRules) ParseInput(string path)
    {
        var rules = new List<string>();
        var template = File.ReadLines(path).Split(1, rules).ToList().FirstOrDefault("");
        return (template, ParseInsertionRules(rules));
    }

    private static IEnumerable<string> ApplyInsertionRules(string template, InsertionRules insertionRules)
    {
        var current = new LinkedList<char>(template);
        
        while (true)
        {
            for (var curr = current.First; curr?.Next is not null; curr = curr.Next)
            {
                var pair = (curr.Value, curr.Next.Value);
                if (!insertionRules.ContainsKey(pair)) continue;
                var insert = insertionRules[pair];
                current.AddAfter(curr, insert);
                curr = curr.Next;
            }

            yield return string.Concat(current);
        }
        // ReSharper disable once IteratorNeverReturns
    }

    private static ((char Key, int Count) Min, (char Key, int Count) Max) GetLeastAndMostCommonElement(string polymer)
    {
        var min = polymer.GroupBy(e => e).MinBy(e => e.Count());
        var max = polymer.GroupBy(e => e).MaxBy(e => e.Count());
        return ((min!.Key, min.Count()), (max!.Key, max.Count()));
    }

    private static void StepOne(string path, int steps)
    {
        Console.WriteLine("Parsing input...");
        var (template, insertionRules) = ParseInput(path);
        Console.WriteLine("Done!");

        var maxPolLength = Console.WindowWidth - 61;

        Console.WriteLine(
            $"{"Template:",-16}{template.Truncate(maxPolLength, "...").PadRight(maxPolLength, ' ')} | LENGTH | MIN      | MAX       | MAX - MIN");

        foreach (var (polymer, step) in ApplyInsertionRules(template, insertionRules).Enumerate().Take(steps))
        {
            var (min, max) = GetLeastAndMostCommonElement(polymer);
            Console.WriteLine(
                $"{$"After step {step + 1,3}:",-16}{polymer.Truncate(maxPolLength, "...").PadRight(maxPolLength, ' ')} | {polymer.Length,6} | {min,-8} | {max,-9} | {max.Count - min.Count}");
        }
    }
}