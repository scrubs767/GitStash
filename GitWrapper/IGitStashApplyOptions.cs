namespace GitWrapper
{
    public interface IGitStashApplyOptions
    {
        bool Index { get; set; }
        IGitStash Stash { get; set; }
    }
}