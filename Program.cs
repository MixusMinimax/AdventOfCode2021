using advent_of_code.util;
using Ardalis.GuardClauses;

var day = Common.GetCurrentDay();

Guard.Against.Null(day, nameof(day));

Console.WriteLine($"╔══════════════════╗\n║ Executing: {day.GetType().Name} ║\n╚══════════════════╝");

var stepCount = day.Steps.Count;
var step = 1;

if (args.Length > 0 && int.TryParse(args[0], out var s) && s >= 1 && s <= stepCount)
{
    step = s;
    args = args.Skip(1).ToArray();
}

await day.Steps[step - 1](args);