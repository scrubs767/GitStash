using System;
using System.Windows.Data;
using SecondLanguage;
using GitStash.Common;

namespace GitStash.UI
{
    public class LanguageConverter : IValueConverter
    {
        Translator T;
        public LanguageConverter()
        {
            IGitStashTranslator t = (IGitStashTranslator) GitStashPackage.GetGlobalService(typeof(IGitStashTranslator));
            this.T = t.Translator;
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