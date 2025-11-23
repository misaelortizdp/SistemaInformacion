using System.Globalization;

namespace CashFlowSystem.MAUI.Converters;

public class TransactionTypeToColorConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values == null || values.Length == 0 || values[0] == null)
            return Colors.Gray;

        var type = values[0].ToString();
        return type == "Income" ? Color.FromArgb("#4CAF50") : Color.FromArgb("#F44336");
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
