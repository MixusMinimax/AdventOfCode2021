using advent_of_code.util;
using Ardalis.GuardClauses;
using JetBrains.Annotations;

namespace advent_of_code.days;

[UsedImplicitly]
public class Day02 : IDay
{
    public Day02()
    {
        Steps = Common.CreateFileSteps(StepOne, StepTwo);
    }

    public IList<Func<string[], Task>> Steps { get; }

    private static IEnumerable<Instruction> GetInstructions(string path)
    {
        return File.ReadLines(path).Select(line =>
        {
            var words = line.Split(' ');
            Guard.Against.AgainstExpression(e => e.words.Length >= 2, (words, 0), nameof(words));
            var amount = 0;
            Guard.Against.AgainstExpression(e => int.TryParse(e.Item1, out amount), (words[1], 0),
                $"{nameof(words)}[1]");
            var result = words[0] switch
                {
                    "forward" => new Instruction(true, true),
                    "backward" => new Instruction(true, false),
                    "down" => new Instruction(false, true),
                    "up" => new Instruction(false, false),
                    _ => throw new ParseException(line)
                } with
                {
                    Amount = amount
                };
            return result;
        });
    }

    private void StepOne(string path)
    {
        Console.WriteLine("Step One: Parsing and aggregating input...");
        var pos = GetInstructions(path).Aggregate(new Position(), (a, b) => a.ApplyInstructionA(b));
        Console.WriteLine($"Done! {pos}, Result: {pos.Forward * pos.Down}");
    }

    private void StepTwo(string path)
    {
        Console.WriteLine("Step Two: Parsing and aggregating input...");
        var pos = GetInstructions(path).Aggregate(new Position(), (a, b) => a.ApplyInstructionB(b));
        Console.WriteLine($"Done! {pos}, Result: {pos.Forward * pos.Down}");
    }

    private record struct Instruction(bool Horizontal, bool Positive, int Amount = 0);

    private readonly record struct Position(int Forward = 0, int Down = 0, int Aim = 0)
    {
        public Position ApplyInstructionA(in Instruction i)
        {
            var sign = i.Positive ? 1 : -1;
            return i.Horizontal
                ? new Position(Forward + sign * i.Amount, Down)
                : new Position(Forward, Down + sign * i.Amount);
        }

        public Position ApplyInstructionB(in Instruction i)
        {
            var sign = i.Positive ? 1 : -1;
            return i.Horizontal
                ? new Position(Forward + sign * i.Amount, Down + sign * i.Amount * Aim, Aim)
                : new Position(Forward, Down, Aim + sign * i.Amount);
        }
    }

    private class ParseException : Exception
    {
        public ParseException(string message) : base($"Parse Error: [{message}]")
        {
        }
    }
}