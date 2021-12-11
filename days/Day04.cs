using System.Text;
using advent_of_code.util;
using JetBrains.Annotations;

namespace advent_of_code.days;

[UsedImplicitly]
public class Day04 : IDay
{
    public Day04()
    {
        Steps = Common.CreateFileSteps(StepOne, StepTwo);
    }

    public IList<Func<string[], Task>> Steps { get; }

    private static (List<int> numbers, List<Board> boards) ParseInput(string path)
    {
        var lines = File.ReadLines(path).ToList();
        var numbers = lines
            .First()
            .Split(',')
            .TryParseInt()
            .ToList();

        var boards = lines
            .Skip(1)
            .Chunk(6)
            .Select(chunk =>
            {
                var field = chunk
                    .Skip(1)
                    .Select(line => line
                        .Split(' ')
                        .Where(e => !string.IsNullOrEmpty(e))
                        .TryParseInt()
                        .ToArray()
                    ).ToArray();
                Dictionary<int, (bool IsMarked, Coordinate Coordinate)> d = new();
                for (var y = 0; y < field.Length; ++y)
                for (var x = 0; x < field[y].Length; ++x)
                    d[field[y][x]] = (false, new Coordinate(x, y));

                return new Board(d);
            }).ToList();

        return (numbers, boards);
    }

    private static void StepOne(string path)
    {
        Console.WriteLine("Parsing Input...");
        var (numbers, boards) = ParseInput(path);
        Console.WriteLine("Done! Running Game...");

        // Oh my god this is so cool
        // It iterates over numbers, and marks all boards per number.
        // For each number, all winning boards are returned.
        // The first overall board is returned, and enumeration stops (no more boards are marked after one wins)
        // var winner = numbers.SelectMany(number => boards.Where(board => board.Mark(number))).First();
        // This is the same in query syntax:
        var (lastNumber, winner) =
        (
            from number in numbers
            from board in boards
            where board.Mark(number)
            select (number, board)
        ).First();

        Console.WriteLine("Done! Winning Board:\n");
        Console.WriteLine(winner);
        Console.WriteLine($"\nFinal Score: {winner.GetScore(lastNumber)}");
    }

    private static void StepTwo(string path)
    {
        Console.WriteLine("Parsing Input...");
        var (numbers, boards) = ParseInput(path);
        Console.WriteLine("Done! Running Game...");

        var (lastNumber, loser) =
        (
            from number in numbers
            from board in boards
            where board.Mark(number)
            select (number, board)
        ).Take(boards.Count).Last();

        Console.WriteLine("Done! Losing Board:\n");
        Console.WriteLine(loser);
        Console.WriteLine($"\nFinal Score: {loser.GetScore(lastNumber)}");
    }

    private class Board
    {
        private readonly int[] _columnCounts = new int[5];
        private readonly int[] _rowCounts = new int[5];
        private readonly Dictionary<int, (bool IsMarked, Coordinate Coordinate)> _values;
        private bool _alreadyWon;

        public Board(Dictionary<int, (bool IsMarked, Coordinate Coordinate)> values)
        {
            _values = values;
        }

        /// <summary>
        ///     Marks the cell. This only runs if the board has not already won, since it would be included multiple times
        ///     in the resulting enumerable otherwise; in other words, it only returns true on the winning move.
        /// </summary>
        /// <param name="value">The Bingo Value to mark</param>
        /// <returns>True if Bingo happened, but false if it already won</returns>
        public bool Mark(int value)
        {
            if (_alreadyWon || !_values.ContainsKey(value) || _values[value].IsMarked) return false;

            _values[value] = _values[value] with { IsMarked = true };
            var (_, (x, y)) = _values[value];
            _columnCounts[x]++;
            _rowCounts[y]++;

            var won = _columnCounts.Any(i => i == 5) || _rowCounts.Any(i => i == 5);
            _alreadyWon = won;
            return won;
        }

        public int GetScore(int number)
        {
            return number * (
                from pair in _values
                where !pair.Value.IsMarked
                select pair.Key
            ).Sum();
        }

        public override string ToString()
        {
            // This data structure is not optimized for x,y indexing, so only use this for debugging
            var field = new (int Value, bool IsMarked)[5, 5];
            foreach (var (key, (isMarked, (i, y))) in _values) field[i, y] = (key, isMarked);

            StringBuilder builder = new();

            for (var y = 0; y < 5; ++y)
            {
                for (var x = 0; x < 5; ++x)
                {
                    var (value, isMarked) = field[x, y];
                    builder.Append(isMarked ? '[' : ' ');
                    builder.Append(value.ToString().PadLeft(2, ' '));
                    builder.Append(isMarked ? ']' : ' ');
                }

                if (y != 4) builder.Append('\n');
            }

            return builder.ToString();
        }
    }
}