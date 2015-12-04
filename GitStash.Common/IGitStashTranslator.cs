using SecondLanguage;

namespace GitStash.Common
{
    public interface IGitStashTranslator
    {
        Translator Translator { get; }
    }
}