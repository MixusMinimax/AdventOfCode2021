namespace advent_of_code.util;

public interface IDateProvider
{
    public DateTime Now { get; }
}

public class DefaultDateProvider : IDateProvider
{
    public DateTime Now => DateTime.Now;
}

public class MockDateProvider : IDateProvider
{
    public MockDateProvider(DateTime now)
    {
        Now = now;
    }

    public DateTime Now { get; }
}