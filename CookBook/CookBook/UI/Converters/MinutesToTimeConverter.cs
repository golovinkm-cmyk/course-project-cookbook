using System;
using System.Globalization;
using System.Windows.Data;

namespace CookBook.UI.Converters;

public class MinutesToTimeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int minutes)
        {
            if (minutes < 60)
                return $"{minutes} мин";

            int hours = minutes / 60;
            int remainingMinutes = minutes % 60;

            if (remainingMinutes == 0)
                return $"{hours} ч";

            return $"{hours} ч {remainingMinutes} мин";
        }
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
