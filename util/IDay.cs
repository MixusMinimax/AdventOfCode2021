namespace advent_of_code.util;

public interface IDay
{
    public IList<Func<string[], Task>> Steps { get; }
}