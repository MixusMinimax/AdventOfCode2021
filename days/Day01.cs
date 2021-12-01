using Ardalis.GuardClauses;

public class Day01 : IDay
{
    public async Task Run(string[] args)
    {
        string path = Guard.Against.NullOrEmpty(args, nameof(args)).First();
        Guard.Against.AgainstExpression(e => File.Exists(e.path), (path, 0), "File doesn't exist!");


        var numbers = (
            from entry in
                from line in File.ReadLines(path)
                select (isValid: int.TryParse(line, out var num), num)
            where entry.isValid
            select entry.num
        ).ToList();

        var increases = numbers
            .Zip(numbers.Skip(1))
            .Count(e => e.Second > e.First);

        Console.WriteLine($"There are {increases} increases in {numbers.Count} measurements, yarr!");

        await Task.CompletedTask;
    }
}