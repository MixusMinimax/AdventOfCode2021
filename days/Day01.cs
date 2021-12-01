using Ardalis.GuardClauses;

public class Day01 : IDay
{
    public IList<Func<string[], Task>> Steps { get; }

    public Day01()
    {
        Steps = new List<Action<string>>() { StepOne, StepTwo }.Select(
            e => async (string[] args) =>
            {
                var path = GetPath(ref args);
                e(path);
                await Task.CompletedTask;
            }
        ).ToList();
    }

    private static string GetPath(ref string[] args)
    {
        string path = Guard.Against.NullOrEmpty(args, nameof(args)).First();
        Guard.Against.AgainstExpression(e => File.Exists(e.path), (path, 0), "File doesn't exist!");
        args = args.Skip(1).ToArray();
        return path;
    }

    private static IList<int> GetNumbers(string path)
    {
        return (
            from entry in
                from line in File.ReadLines(path)
                select (isValid: int.TryParse(line, out var num), num)
            where entry.isValid
            select entry.num
        ).ToList();
    }

    private static int CountIncreases(IList<int> numbers)
    {
        return numbers
            .Zip(numbers.Skip(1))
            .Count(e => e.Second > e.First);
    }

    private void StepOne(string path)
    {
        var numbers = GetNumbers(path);
        var increases = CountIncreases(numbers);

        Console.WriteLine($"There are {increases} increases in {numbers.Count} measurements, yarr!");
    }

    private void StepTwo(string path)
    {
        var numbers = GetNumbers(path);
        var sums = numbers
            .Zip(numbers.Skip(1), numbers.Skip(2))
            .Select(element => element.First + element.Second + element.Third)
            .ToList();
        var increases = CountIncreases(sums);

        Console.WriteLine($"There are {increases} increases in {sums.Count} measurement triplets, yarr!");
    }
}