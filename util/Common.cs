using Ardalis.GuardClauses;

namespace advent_of_code.util;

public static class Common
{
    public static string GetPath(ref string[] args)
    {
        var path = Guard.Against.NullOrEmpty(args, nameof(args)).First();
        Guard.Against.AgainstExpression(e => File.Exists(e.path), (path, 0),
            $"File doesn't exist! ({Path.GetFullPath(path)})");
        args = args.Skip(1).ToArray();
        return path;
    }

    public static IList<Func<string[], Task>> CreateFileSteps(params Action<string>[] functions)
    {
        return functions.Select(
            e => async (string[] args) =>
            {
                var path = GetPath(ref args);
                e(path);
                await Task.CompletedTask;
            }
        ).ToList();
    }

    public static IList<Func<string[], Task>> CreateFileSteps(params Action<string, string[]>[] functions)
    {
        return functions.Select(
            e => async (string[] args) =>
            {
                var path = GetPath(ref args);
                e(path, args);
                await Task.CompletedTask;
            }
        ).ToList();
    }

    public static Action<string, string[]> RequireInt(Action<string, int> function, string name, int? @default = null)
    {
        return (path, args) =>
        {
            var result = 0;

            bool TryParse(string[] e) => e.Length >= 1 && int.TryParse(e[0], out result) ||
                                         Assign(@default is not null, @default ?? 0, out result);

            Guard.Against.InvalidInput(args, name, TryParse,
                $"Parameter [{name}: int] required!");
            function(path, result);
        };
    }

    public static TRet Assign<TRet, TOut>(TRet ret, TOut val, out TOut @out)
    {
        @out = val;
        return ret;
    }

    public static void Swap<T>(ref T a, ref T b)
    {
        (a, b) = (b, a);
    }

    public static string ToString<T>(T t)
    {
        return t?.ToString() ?? "null";
    }

    public static IDay? GetCurrentDay(IDateProvider? dateProvider = null)
    {
        dateProvider ??= new DefaultDateProvider();
        return typeof(IDay)
            .Assembly
            .ExportedTypes
            .Where(e => typeof(IDay).IsAssignableFrom(e))
            .Where(e => e.Name == $"Day{DateOnly.FromDateTime(dateProvider.Now).Day:D2}")
            .Take(1)
            .Select(Activator.CreateInstance)
            .Cast<IDay>()
            .FirstOrDefault();
    }

    public static T GetValueOrDefault<T>(this T[,] arr, int x, int y, T @default = default)
    {
        return x < 0 || x >= arr.GetLength(0) || y < 0 || y >= arr.GetLength(1)
            ? @default
            : arr[x, y];
    }
}

public record struct Coordinate(int X, int Y);

public enum Axis
{
    X,
    Y
}