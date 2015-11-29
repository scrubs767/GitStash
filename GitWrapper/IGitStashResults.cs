namespace GitWrapper
{
    public interface IGitStashResults
    {
        bool Success { get; }
        string Message { get; }
    }
}