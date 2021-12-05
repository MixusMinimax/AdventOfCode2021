using Ardalis.GuardClauses;

namespace advent_of_code.util;

public static class Common
{
    public static string GetPath(ref string[] args)
    {
        var path = Guard.Against.NullOrEmpty(args, nameof(args)).First();
        Guard.Against.AgainstExpression(e => File.Exists(e.path), (path, 0), "File doesn't exist!");
        args = args.Skip(1).ToArray();
        return path;
    }

    public static IList<Func<string[], Task>> CreateFileSteps(params Action<string>[] functions)
    {
        return functions.Select(
            e => async (string[] args) =>
            {
                var path = Common.GetPath(ref args);
                e(path);
                await Task.CompletedTask;
            }
        ).ToList();
    }

    public static void Swap<T>(ref T a, ref T b)
    {
        (a, b) = (b, a);
    }
}