namespace GitWrapper
{
    public interface IGitStashSaveOptions
    {
        bool KeepIndex { get; set; }
        bool All { get; set; }
        bool Untracked { get; set; }
        bool Ignored { get; set; }
        string Message { get; set; }
    }
}