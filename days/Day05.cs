using System.Collections;
using System.Text;
using advent_of_code.util;

namespace advent_of_code.days;

public class Day05 : IDay
{
    public IList<Func<string[], Task>> Steps { get; }

    public Day05()
    {
        Steps = Common.CreateFileSteps(
            path => Step(path, false),
            path => Step(path, true)
        );
    }

    record struct Coordinate(int X, int Y);

    class Map
    {
        private readonly Dictionary<int, Dictionary<int, int>> _values = new();
        public int HighPoints { get; private set; }
        private Coordinate Size { get; set; }

        private int this[int x, int y]
        {
            get
            {
                if (_values.ContainsKey(x)) return _values[x].ContainsKey(y) ? _values[x][y] : 0;
                _values[x] = new Dictionary<int, int>();
                return 0;
            }
            set
            {
                Size = Size with
                {
                    X = Math.Max(Size.X, x + 1),
                    Y = Math.Max(Size.Y, y + 1)
                };
                if (!_values.ContainsKey(x)) _values[x] = new Dictionary<int, int>();
                _values[x][y] = value;
            }
        }

        private void MarkCell(int x, int y)
        {
            var curr = this[x, y];
            if (curr == 1) HighPoints++;
            this[x, y] = curr + 1;
        }
        
        public void MarkLine(Coordinate from, Coordinate to, bool includeDiagonal = false)
        {
            if (from.X == to.X)
            {
                var (y, b) = (Math.Min(from.Y, to.Y), Math.Max(from.Y, to.Y));
                for (; y <= b; ++y)
                {
                    MarkCell(from.X, y);
                }
            }
            else if (from.Y == to.Y)
            {
                var (x, b) = (Math.Min(from.X, to.X), Math.Max(from.X, to.X));
                for (; x <= b; ++x)
                {
                    MarkCell(x, from.Y);
                }
            }
            else if (includeDiagonal && to.X - from.X == to.Y - from.Y)
            {
                if (from.X > to.X) Common.Swap(ref from, ref to);
                for (var (x, y) = from; x <= to.X; ++x, ++y)
                {
                    MarkCell(x, y);
                }
            }
            else if (includeDiagonal && to.X - from.X == from.Y - to.Y)
            {
                if (from.X > to.X) Common.Swap(ref from, ref to);
                for (var (x, y) = from; x <= to.X; ++x, --y)
                {
                    MarkCell(x, y);
                }
            }
        }

        public override string ToString()
        {
            if (Size.X > 20 || Size.Y > 20)
                return "Too big to print!";
            StringBuilder result = new();
            for (var y = 0; y < Size.Y; ++y)
            {
                if (y != 0) result.Append('\n');
                for (var x = 0; x < Size.X; ++x)
                {
                    var num = this[x, y];
                    result.Append(num != 0 ? num.ToString() : ".");
                }
            }

            return result.ToString();
        }
    }

    private static IEnumerable<(Coordinate From, Coordinate To)> GetLines(string path)
    {
        return File.ReadLines(path)
            .Select(e => e.Split("->")
                .Select(x => x.Trim())
                .Select(x => x.Split(',')
                    .TryParseInt()
                    .ToArray())
                .Where(x => x.Length >= 2)
                .Select(x => new Coordinate(x[0], x[1]))
                .ToArray())
            .Where(e => e.Length >= 2)
            .Select(e => (From: e[0], To: e[1]));
    }

    private static void Step(string path, bool includeDiagonals)
    {
        Console.WriteLine("Parsing input and calculating map...");
        Map map = new();
        foreach (var (from, to) in GetLines(path))
        {
            map.MarkLine(from, to, includeDiagonals);
        }

        Console.WriteLine("Done! Resulting map:");
        Console.WriteLine();
        Console.WriteLine(map);
        Console.WriteLine();
        Console.WriteLine($"Number of high points: {map.HighPoints}");
    }
}