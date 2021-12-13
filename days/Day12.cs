using advent_of_code.util;
using JetBrains.Annotations;

namespace advent_of_code.days;

[UsedImplicitly]
public class Day12 : IDay
{
    public Day12()
    {
        Steps = Common.CreateFileSteps(StepOne, StepTwo);
    }

    public IList<Func<string[], Task>> Steps { get; }

    private static (IList<string> Nodes, bool[,] Edges) ParseGraph(string path)
    {
        var stringEdges = File
            .ReadLines(path)
            .Select(line => line.Split('-').Take(2).ToArray())
            .Where(e => e.Length == 2)
            .Where(e => e[0] != e[1])
            .ToList();
        var nodes = stringEdges
            .SelectMany(e => e)
            .Where(e => !string.IsNullOrWhiteSpace(e))
            .Distinct()
            .ToList();
        var edges = new bool[nodes.Count, nodes.Count];

        foreach (var edge in stringEdges)
        {
            var i = nodes.IndexOf(edge[0]);
            if (i == -1) continue;
            var j = nodes.IndexOf(edge[1]);
            if (j == -1) continue;
            edges[i, j] = edges[j, i] = true;
        }

        return (nodes, edges);
    }

    private static bool ContainsLoops(IList<string> nodes, bool[,] edges)
    {
        var bigCaves = nodes.Where(e => e == e.ToUpper()).ToList();
        for (var from = 0; from < bigCaves.Count - 1; ++from)
        {
            for (var to = from + 1; to < bigCaves.Count; ++to)
            {
                if (edges[nodes.IndexOf(bigCaves[from]), nodes.IndexOf(bigCaves[to])]) return true;
            }
        }

        return false;
    }


    private static IList<string[]> GetPossiblePaths(IList<string> nodes, bool[,] edges, int startIndex, int endIndex,
        bool allowExtraVisit = false)
    {
        static IList<string[]> GetPossiblePathsImpl(IList<string> nodes, bool[,] edges, int startIndex, int endIndex,
            ISet<string> visited, bool allowExtraVisit)
        {
            if (startIndex == endIndex)
                return new List<string[]> { new[] { nodes[startIndex] } };

            var thisNode = nodes[startIndex];

            IList<string[]> GetSubGraphs() =>
            (
                from subGraphs in
                    from node in nodes
                    where !visited.Contains(node)
                    where edges[startIndex, nodes.IndexOf(node)]
                    select GetPossiblePathsImpl(nodes, edges, nodes.IndexOf(node), endIndex,
                        new HashSet<string>(visited), allowExtraVisit)
                from subGraph in subGraphs
                select subGraph.Prepend(thisNode).ToArray()
            ).ToList();

            IList<string[]> result = new List<string[]>();

            if (allowExtraVisit && thisNode != "start" && thisNode != "end")
            {
                allowExtraVisit = false;
                result = GetSubGraphs();
                allowExtraVisit = true;
            }

            if (thisNode != thisNode.ToUpper())
            {
                visited.Add(nodes[startIndex]);
            }

            return result.Concat(GetSubGraphs()).ToList();
        }

        return ContainsLoops(nodes, edges)
            ? new List<string[]>()
            : GetPossiblePathsImpl(nodes, edges, startIndex, endIndex, new HashSet<string>(), allowExtraVisit);
    }

    private static void StepOne(string path)
    {
        Console.WriteLine("Parsing graph...");
        var (nodes, edges) = ParseGraph(path);
        Console.WriteLine("Done! Calculating possible paths...");
        var possiblePaths = GetPossiblePaths(nodes, edges,
            nodes.IndexOf("start"), nodes.IndexOf("end"));

#if false
        foreach (var possiblePath in possiblePaths)
        {
            Console.WriteLine(string.Join(',', possiblePath));
        }
#endif

        Console.WriteLine($"Done! Possible path count: {possiblePaths.Count}");
    }

    private static void StepTwo(string path)
    {
        Console.WriteLine("Parsing graph...");
        var (nodes, edges) = ParseGraph(path);
        Console.WriteLine("Done! Calculating possible paths...");
        var possiblePaths = GetPossiblePaths(nodes, edges,
            nodes.IndexOf("start"), nodes.IndexOf("end"),
            true
        );

        possiblePaths = possiblePaths
            .Select(e => string.Join(',', e))
            .Distinct()
            .Select(e => e.Split(','))
            .ToArray();

#if false
        foreach (var possiblePath in possiblePaths)
        {
            Console.WriteLine(string.Join(',', possiblePath));
        }
#endif

        Console.WriteLine($"Done! Possible path count: {possiblePaths.Count}");
    }
}