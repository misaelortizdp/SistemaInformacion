using System.Globalization;

namespace CashFlowSystem.MAUI.Converters;

public class TransactionTypeToIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
            return "💰";

        var type = value.ToString();
        return type == "Income" ? "↗️" : "↘️";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
