namespace advent_of_code.util;

public static class Extensions
{
    public delegate bool SelectWhereDelegate<in TFrom, TTo>(TFrom from, out TTo to);

    public static int ToInt32(this IEnumerable<bool> bits)
    {
        return bits
            .Aggregate(0, (a, b) => (a << 1) | (b ? 1 : 0));
    }

    public static IEnumerable<TResult> SelectWhere<TSource, TResult>(
        this IEnumerable<TSource> enumerable,
        SelectWhereDelegate<TSource, TResult> func)
    {
        return enumerable
            .Select(e => (isValid: func(e, out var value), value))
            .Where(e => e.isValid)
            .Select(e => e.value);
    }

    public static IEnumerable<int> TryParseInt(this IEnumerable<string> enumerable)
    {
        return enumerable.SelectWhere<string, int>(int.TryParse);
    }

    public static IEnumerable<Coordinate> TryParseCoordinates(this IEnumerable<string> enumerable, char delim = ',')
    {
        return enumerable.SelectWhere((string line, out Coordinate coord) =>
        {
            var nums = line.Split(delim).TryParseInt().ToArray();
            coord = default;
            if (nums.Length < 2) return false;
            coord.X = nums[0];
            coord.Y = nums[1];
            return true;
        });
    }

    public static IEnumerable<TSource> TakeWhileAndSplit<TSource>(this IEnumerable<TSource> enumerable,
        Predicate<TSource> predicate, IList<TSource> remainder)
    {
        var split = false;
        foreach (var e in enumerable)
            if (!split && predicate(e))
            {
                yield return e;
            }
            else
            {
                split = true;
                remainder.Add(e);
            }
    }

    public static IEnumerable<int> SplitNumbers(this string line, char delimiter = ',')
    {
        return line.Split(delimiter).TryParseInt();
    }

    public static double Mean(this IEnumerable<int> numbers)
    {
        return numbers.ToArray().Mean();
    }

    public static double Mean(this IReadOnlyList<int> numbers)
    {
        return numbers.ToArray().Mean(numbers.Count);
    }

    public static double Mean(this IEnumerable<int> numbers, int count)
    {
        var enumerated = numbers.OrderBy(e => e).Take(count / 2 + 1).ToArray();
        return count == 0 ? 0 : count % 2 == 1 ? enumerated[^1] : (enumerated[^2] + enumerated[^1]) / 2.0;
    }

    public static double Mean(this IEnumerable<long> numbers)
    {
        return numbers.ToArray().Mean();
    }

    public static double Mean(this IReadOnlyList<long> numbers)
    {
        return numbers.ToArray().Mean(numbers.Count);
    }

    public static double Mean(this IEnumerable<long> numbers, int count)
    {
        var enumerated = numbers.OrderBy(e => e).Take(count / 2 + 1).ToArray();
        return count == 0 ? 0 : count % 2 == 1 ? enumerated[^1] : (enumerated[^2] + enumerated[^1]) / 2.0;
    }
}