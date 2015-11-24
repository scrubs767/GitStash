namespace GitWrapper
{
    public interface IGitStashPopOptions
    {
        bool Index { get; set; }
        IGitStash Stash { get; set; }
    }
}