public interface IDay
{
    public IList<Func<string[], Task>> Steps { get; }
}