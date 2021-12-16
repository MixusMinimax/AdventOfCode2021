using advent_of_code.util;
using JetBrains.Annotations;
using ShellProgressBar;

namespace advent_of_code.days;

[UsedImplicitly]
public class Day15 : IDay
{
    public Day15()
    {
        Steps = Common.CreateFileSteps(StepOne, StepTwo);
    }

    public IList<Func<string[], Task>> Steps { get; }

    private static void StepOne(string path)
    {
        var field = ParseCave(path);
        field[0][0].TotalRisk = 0;
        Dijkstra(field);
        ConstructRoute(field);

        Console.WriteLine($"Total Risk: {field[^1][^1].TotalRisk}");
    }

    private static void StepTwo(string path)
    {
        const int size = 5;

        Console.WriteLine("Parsing input...");
        var field = ParseCave(path);
        Console.WriteLine("Done! Tiling input 5x5...");

        Func<GraphNode, GraphNode> Increase(int offset) => e =>
        {
            var (cellRisk, totalRisk, _) = e;
            return new GraphNode((cellRisk + offset - 1) % 9 + 1, totalRisk, null);
        };

        field = field
            .Select(row => Enumerable.Range(0, size).SelectMany(i => row.Select(Increase(i))).ToArray())
            .ToArray();
        field = Enumerable
            .Range(0, size).SelectMany(i => field.Select(row => row.Select(Increase(i)).ToArray()))
            .ToArray();
        
        Console.WriteLine("Done! Finding shortest path...");

        field[0][0].TotalRisk = 0;

        Dijkstra(field);
        ConstructRoute(field);

        Console.WriteLine($"Done! Total Risk: {field[^1][^1].TotalRisk}");
    }

    private static LinkedList<(Coordinate Coords, GraphNode Node)> ConstructRoute(IReadOnlyList<GraphNode[]> field)
    {
        var route = new LinkedList<(Coordinate Coords, GraphNode Node)>();

        for (var curr = new Coordinate(field[^1].Length - 1, field.Count - 1);;)
        {
            var node = field[curr.Y][curr.X];
            route.AddFirst((curr, node));
            if (node.Predecessor is null) break;
            curr = node.Predecessor!.Value;
        }

        return route;
    }

    private static void Dijkstra(IReadOnlyList<GraphNode[]> field)
    {
        var queue = new Dictionary<Coordinate, GraphNode>();
        
        for (var y = 0; y < field.Count; ++y)
        for (var x = 0; x < field[y].Length; ++x)
            queue[new Coordinate(x, y)] = field[y][x];

        using var bar = new ProgressBar(queue.Count, "Finding shortest path...", new ProgressBarOptions
        {
            ForegroundColor = ConsoleColor.Yellow,
            ForegroundColorDone = ConsoleColor.DarkGreen,
            BackgroundColor = ConsoleColor.DarkGray,
            BackgroundCharacter = '\u2593'
        });
        while (queue.Count > 0)
        {
            // I can not use a priority queue or sort the list because the priority of each element can
            // change after each iteration. I would need to implement a Binary Heap.
            var (uCoords, value) = queue.MinBy(e => e.Value.TotalRisk);
            var (_, uTotalRisk, _) = value;
            queue.Remove(uCoords);
            if (uCoords == new Coordinate(field[^1].Length - 1, field.Count - 1))
                break;

            // Because of this lookup I chose a Dictionary.
            foreach (var (nx, ny) in GetNeighbors(field, uCoords).Where(n => queue.ContainsKey(n)))
            {
                ref var n = ref field[ny][nx];
                var alt = uTotalRisk + n.CellRisk;
                if (alt >= n.TotalRisk) continue;
                n.TotalRisk = alt;
                n.Predecessor = uCoords;
            }
            bar.Tick();
        }
    }

    private static GraphNode[][] ParseCave(string path)
    {
        var field = File
            .ReadLines(path)
            .Select(e => e.Select(x => x.ToString())
                .TryParseInt()
                .Select(x => new GraphNode(x, int.MaxValue, null))
                .ToArray())
            .ToArray();
        return field;
    }

    private static void PrintField(IEnumerable<IEnumerable<GraphNode>> field)
    {
        foreach (var row in field)
        {
            foreach (var cell in row) Console.Write(cell.CellRisk);

            Console.WriteLine();
        }
    }

    private static IEnumerable<Coordinate> GetNeighbors(IReadOnlyList<IReadOnlyList<GraphNode>> field,
        Coordinate coords)
    {
        var (x, y) = coords;
        var ret = new List<Coordinate>(4);
        if (x > 0)
            ret.Add(new Coordinate(x - 1, y));
        if (y > 0)
            ret.Add(new Coordinate(x, y - 1));
        if (y < field.Count - 1)
            ret.Add(new Coordinate(x, y + 1));
        if (x < field[y].Count - 1)
            ret.Add(new Coordinate(x + 1, y));
        return ret;
    }

    private class GraphNode
    {
        public GraphNode(int cellRisk, int totalRisk, Coordinate? predecessor)
        {
            CellRisk = cellRisk;
            TotalRisk = totalRisk;
            Predecessor = predecessor;
        }

        public int CellRisk { get; set; }
        public int TotalRisk { get; set; }
        public Coordinate? Predecessor { get; set; }

        public void Deconstruct(out int cellRisk, out int totalRisk, out Coordinate? predecessor)
        {
            cellRisk = CellRisk;
            totalRisk = TotalRisk;
            predecessor = Predecessor;
        }

        public override string ToString()
        {
            return $"GraphNode {{ CellRisk = {CellRisk}, TotalRisk = {TotalRisk}, Predecessor = {Predecessor} }}";
        }
    }
}