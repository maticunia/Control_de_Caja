using CajaApp.Domain.Enums;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace CajaApp.UI.Converters;

public class TipoMovimientoToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TipoMovimiento tipo)
        {
            return tipo switch
            {
                TipoMovimiento.Ingreso => Brushes.Green,
                TipoMovimiento.Egreso => Brushes.Red,
                _ => Brushes.Gray
            };
        }
        return Brushes.Gray;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
