﻿using System.Collections;

namespace advent_of_code.util;

public static class Extensions
{
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

    public static IEnumerable<int> SplitNumbers(this string line, char delimiter = ',')
    {
        return line.Split(delimiter).TryParseInt();
    }

    public delegate bool SelectWhereDelegate<in TFrom, TTo>(TFrom from, out TTo to);
}