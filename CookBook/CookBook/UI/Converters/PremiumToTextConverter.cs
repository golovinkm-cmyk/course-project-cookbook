using System;
using System.Globalization;
using System.Windows.Data;

namespace CookBook.UI.Converters;

public class PremiumToTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isPremium)
        {
            return isPremium ? "Премиум" : "Базовый";
        }
        return "Неизвестно";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}