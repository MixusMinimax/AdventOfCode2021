using System.Collections;
using advent_of_code.util;
using JetBrains.Annotations;

namespace advent_of_code.days;

[UsedImplicitly]
public class Day03 : IDay
{
    public Day03()
    {
        Steps = Common.CreateFileSteps(StepOne, StepTwo);
    }

    public IList<Func<string[], Task>> Steps { get; }

    private static IList<BitArray> GetNumbers(string path)
    {
        return File.ReadLines(path)
            .Select(e => new BitArray(e.Select(x => x == '1').ToArray()))
            .ToList();
    }

    private static (int Bits, int Length) GetCommonBits(ICollection<BitArray> numbers)
    {
        var (bitCounts, length) = numbers
            .Aggregate((BitCounts: new int[64], Length: 0), (a, b) =>
            {
                for (var i = 0; i < b.Length; ++i) a.BitCounts[i] += b[b.Length - i - 1] ? 1 : 0;

                a.Length = Math.Max(a.Length, b.Length);
                return a;
            });
        var gamma = bitCounts
            .Take(length)
            .Select(e => e >= numbers.Count / 2.0)
            .Reverse()
            .ToInt32();
        return (gamma, length);
    }

    private static void StepOne(string path)
    {
        Console.WriteLine("Parsing File...");
        var numbers = GetNumbers(path);
        var (gamma, length) = GetCommonBits(numbers);
        var epsilon = gamma ^ ((1 << length) - 1);

        Console.WriteLine($"Done! gamma = {gamma}, epsilon = {epsilon}");
        Console.WriteLine($"Power consumption: {gamma * epsilon}");
    }

    private static void StepTwo(string path)
    {
        IList<BitArray> FilterNumbers(IList<BitArray> bitArrays, bool isOxygen)
        {
            for (var bitIndex = 0; bitArrays.Count > 1; ++bitIndex)
            {
                var (gamma, length) = GetCommonBits(bitArrays);
                var epsilon = gamma ^ ((1 << length) - 1);
                bitArrays = bitArrays
                    .Where(e =>
                    {
                        var bit = (((isOxygen ? gamma : epsilon) >> (e.Length - bitIndex - 1)) & 1) == 1;
                        return bit == e[bitIndex];
                    })
                    .ToList();
            }

            return bitArrays;
        }

        Console.WriteLine("Parsing File...");
        var numbers = GetNumbers(path);
        var oxygen = numbers;
        var co2 = numbers;
        oxygen = FilterNumbers(oxygen, true);
        co2 = FilterNumbers(co2, false);

        var oxygenResult = oxygen
            .First()
            .Cast<bool>()
            .ToInt32();
        var co2Result = co2
            .First()
            .Cast<bool>()
            .ToInt32();

        Console.WriteLine($"Done! Oxygen = {oxygenResult}, Co2 = {co2Result}");
        Console.WriteLine($"Life Support: {oxygenResult * co2Result}");
    }
}