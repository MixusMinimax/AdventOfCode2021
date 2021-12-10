using advent_of_code.util;

namespace advent_of_code.days;

public class Day10 : IDay
{
    public IList<Func<string[], Task>> Steps { get; }

    public Day10()
    {
        Steps = Common.CreateFileSteps(StepOne, StepTwo);
    }

    private enum ChunkType
    {
        Correct,
        Incomplete,
        Corrupted
    }

    private static Dictionary<char, char> _characters = new()
    {
        { '(', ')' },
        { '[', ']' },
        { '{', '}' },
        { '<', '>' }
    };

    private static Dictionary<char, (int ScoreA, int ScoreB)> _scores = new()
    {
        { ')', (3, 1) },
        { ']', (57, 2) },
        { '}', (1197, 3) },
        { '>', (25137, 4) }
    };


    private readonly struct Frame
    {
        public char OpeningChar { get; init; }

        public char GetClosingChar() => _characters
            .TryGetValue(OpeningChar, out var res)
            ? res
            : '\0';
    }

    private static (ChunkType ChunkType, char? IncorrectCharacter, Stack<Frame> Stack) ParseChunk(string chunk)
    {
        var stack = new Stack<Frame>();
        foreach (var c in chunk)
        {
            if (stack.Count > 0 && stack.Peek().GetClosingChar() == c)
            {
                stack.Pop();
                continue;
            }

            if (!_characters.ContainsKey(c))
            {
                return (ChunkType.Corrupted, c, stack);
            }

            stack.Push(new Frame { OpeningChar = c });
        }

        return (stack.Count > 0 ? ChunkType.Incomplete : ChunkType.Correct, null, stack);
    }

    private static void StepOne(string path)
    {
        var score = File.ReadLines(path).Select(chunk =>
        {
            var (chunkType, incorrectCharacter, _) = ParseChunk(chunk);
            return chunkType == ChunkType.Corrupted
                ? _scores[incorrectCharacter!.Value].ScoreA
                : 0;
        }).Sum();

        Console.WriteLine($"Done! Final Score: {score}");
    }

    private static void StepTwo(string path)
    {
        var score = File.ReadLines(path).Select(chunk =>
        {
            var (chunkType, _, stack) = ParseChunk(chunk);
            return chunkType == ChunkType.Incomplete
                ? stack.Select(e => e.GetClosingChar()).Aggregate(0L, (a, b) => a * 5 + _scores[b].ScoreB)
                : 0L;
        }).Where(e => e != 0L).Mean();

        Console.WriteLine($"Done! Final Score: {score}");
    }
}