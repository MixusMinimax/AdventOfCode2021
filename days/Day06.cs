using advent_of_code.util;
using JetBrains.Annotations;

namespace advent_of_code.days;

[UsedImplicitly]
public class Day06 : IDay
{
    public Day06()
    {
        Steps = Common.CreateFileSteps(StepOne, StepTwo, StepThree);
    }

    public IList<Func<string[], Task>> Steps { get; }

    private static string NumbersToString(IEnumerable<int> numbers, int maxLength = 64, string overflow = "...")
    {
        var result = string.Join(',', numbers);
        return result.Length > maxLength ? result[..(maxLength - overflow.Length)] + overflow : result;
    }

    private static void StepOne(string path, string[] args)
    {
        var numbers = File.ReadLines(path).FirstOrDefault("").Split(',').TryParseInt().ToList();
        var days = args.TryParseInt().FirstOrDefault(18);
        var maxLength = Console.WindowWidth - "After XX days: ".Length;

        Console.WriteLine($"Initial state: {NumbersToString(numbers, maxLength)}");

        for (var day = 1; day <= days; ++day)
        {
            var zeroes = numbers.Count(e => e == 0);
            for (var i = 0; i < numbers.Count; ++i)
            {
                numbers[i]--;
                if (numbers[i] < 0) numbers[i] = 6;
            }

            numbers.AddRange(Enumerable.Repeat(8, zeroes));

            if (day == days || day == 1 || days / 20 == 0 || day % (days / 20) == 0)
                Console.WriteLine(
                    @$"After {day.ToString().PadLeft(2, ' ')} {(day == 1 ? "day" : "days")}: {(day == 1 ? " " : "")}{NumbersToString(numbers, maxLength)}");
        }

        Console.WriteLine($"After {days.ToString().PadLeft(2, ' ')} days, there are {numbers.Count} lanternfish.");
    }

    private static void StepTwo(string path, string[] args)
    {
        var numbers = File.ReadLines(path).FirstOrDefault("").Split(',').TryParseInt().ToList();
        var days = args.TryParseInt().FirstOrDefault(18);
        var simulation = new List<int> { 6 };

        var results = new int[7];

        for (var day = 1; day < days + 7; ++day)
        {
            var zeroes = simulation.Count(e => e == 0);
            for (var i = 0; i < simulation.Count; ++i)
            {
                simulation[i]--;
                if (simulation[i] < 0) simulation[i] = 6;
            }

            simulation.AddRange(Enumerable.Repeat(8, zeroes));

            if (day >= days) results[day - days] = simulation.Count;

            if (day == days || day == 1 || days / 20 == 0 || day % (days / 20) == 0)
                Console.WriteLine(
                    $"After {day.ToString().PadLeft(2, ' ')} {(day == 1 ? "day" : "days")}, there are {simulation.Count} lanternfish.");
        }

        var count = numbers.Select(e => results[6 - e]).Sum();

        Console.WriteLine($"After {days} {(days == 1 ? "day" : "days")}, there are {count} lanternfish.");
    }

    private static void StepThree(string path, string[] args)
    {
        var numbers = File.ReadLines(path).FirstOrDefault("").Split(',').TryParseInt().ToList();
        var days = args.TryParseInt().FirstOrDefault(18);
        var maxLength = Console.WindowWidth - "After XX days: ".Length;

        Console.WriteLine($"Initial state: {NumbersToString(numbers, maxLength)}");

        var simulation = new long[10];
        simulation[6] = 1;

        var results = new long[7];

        for (var day = 1; day < days + 7; ++day)
        {
            simulation[9] = simulation[0];
            simulation[7] += simulation[0];
            for (var i = 0; i < simulation.Length - 1; ++i) simulation[i] = simulation[i + 1];

            simulation[9] = 0;

            if (day >= days) results[day - days] = simulation.Sum();
        }

        var count = numbers.Select(e => results[6 - e]).Sum();

        Console.WriteLine($"After {days} {(days == 1 ? "day" : "days")}, there are {count} lanternfish.");
    }
}