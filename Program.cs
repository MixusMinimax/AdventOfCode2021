await (typeof(IDay)
    .Assembly
    .ExportedTypes
    .Where(e => typeof(IDay).IsAssignableFrom(e))
    .Where(e => e.Name == $"Day{DateOnly.FromDateTime(DateTime.Now).Day:D2}")
    .Take(1)
    .Select(e => {
        Console.WriteLine($"╔══════════════════╗\n║ Executing: {e.Name} ║\n╚══════════════════╝");
        return e;
    })
    .Select(Activator.CreateInstance)
    .Cast<IDay>()
    .FirstOrDefault()
    ?.Run(args) ?? Task.CompletedTask);
