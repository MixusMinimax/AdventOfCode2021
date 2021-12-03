using advent_of_code.util;
using Ardalis.GuardClauses;

namespace advent_of_code.days;

public class Day02 : IDay
{
    public IList<Func<string[], Task>> Steps { get; }

    public Day02()
    {
        Steps = Common.CreateFileSteps(StepOne, StepTwo);
    }

    record struct Instruction(bool Horizontal, bool Positive, int Amount = 0);

    record struct Position(int forward = 0, int down = 0, int aim = 0)
    {
        public Position ApplyInstructionA(in Instruction i)
        {
            var sign = i.Positive ? 1 : -1;
            return i.Horizontal
                ? new Position(forward + sign * i.Amount, down)
                : new Position(forward, down + sign * i.Amount);
        }

        public Position ApplyInstructionB(in Instruction i)
        {
            var sign = i.Positive ? 1 : -1;
            return i.Horizontal
                ? new Position(forward + sign * i.Amount, down + sign * i.Amount * aim, aim)
                : new Position(forward, down, aim + sign * i.Amount);
        }
    }

    class ParseException : Exception
    {
        public ParseException(string message) : base($"Parse Error: [{message}]")
        {

        }
    }

    private static IEnumerable<Instruction> GetInstructions(string path)
    {
        return File.ReadLines(path).Select(line =>
        {
            var words = line.Split(' ');
            Guard.Against.AgainstExpression(e => e.words.Length >= 2, (words, 0), nameof(words));
            int amount = 0;
            Guard.Against.AgainstExpression(e => int.TryParse(e.Item1, out amount), (words[1], 0), $"{nameof(words)}[1]");
            var result = words[0] switch
            {
                "forward" => new Instruction(Horizontal: true, Positive: true),
                "backward" => new Instruction(Horizontal: true, Positive: false),
                "down" => new Instruction(Horizontal: false, Positive: true),
                "up" => new Instruction(Horizontal: false, Positive: false),
                _ => throw new ParseException(line)
            } with
            { Amount = amount };
            return result;
        });
    }

    private void StepOne(string path)
    {
        Console.WriteLine("Step One: Parsing and aggregating input...");
        var pos = GetInstructions(path).Aggregate(new Position(), (a, b) => a.ApplyInstructionA(b));
        Console.WriteLine($"Done! {pos}, Result: {pos.forward * pos.down}");
    }

    private void StepTwo(string path)
    {
        Console.WriteLine("Step Two: Parsing and aggregating input...");
        var pos = GetInstructions(path).Aggregate(new Position(), (a, b) => a.ApplyInstructionB(b));
        Console.WriteLine($"Done! {pos}, Result: {pos.forward * pos.down}");
    }
}
