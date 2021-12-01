using Ardalis.GuardClauses;

var day = typeof(IDay)
    .Assembly
    .ExportedTypes
    .Where(e => typeof(IDay).IsAssignableFrom(e))
    .Where(e => e.Name == $"Day{DateOnly.FromDateTime(DateTime.Now).Day:D2}")
    .Take(1)
    .Select(e =>
    {
        Console.WriteLine($"╔══════════════════╗\n║ Executing: {e.Name} ║\n╚══════════════════╝");
        return e;
    })
    .Select(Activator.CreateInstance)
    .Cast<IDay>()
    .FirstOrDefault();

Guard.Against.Null(day, nameof(day));

var stepCount = day.Steps.Count;
var step = 1;

if (args.Length > 0 && int.TryParse(args[0], out var s) && s >= 1 && s <= stepCount)
{
    step = s;
    args = args.Skip(1).ToArray();
}

await day.Steps[step - 1](args);
