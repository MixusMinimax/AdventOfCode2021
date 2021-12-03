using System.Collections;

namespace advent_of_code.util;

public static class Extensions
{
    public static int ToInt32(this IEnumerable<bool> bits)
    {
        return bits
            .Cast<bool>()
            .Aggregate(0, (a, b) => (a << 1) | (b ? 1 : 0));
    }
}