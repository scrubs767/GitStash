namespace GitWrapper
{
    public interface IGitStash
    {
        int Index { get; }
        string Branch { get; }
        string Message { get; }
    }
}