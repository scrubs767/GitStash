using System;
using System.Windows.Data;
using SecondLanguage;

namespace GitStash.UI
{
    public class LanguageConverter : IValueConverter
    {
        Translator T = Translator.Default;
        public LanguageConverter()
        {
            T.RegisterTranslationsByCulture(@"po\*.po");
        }
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            
            if (parameter is string)
                return T[parameter as string];
            else
                return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}