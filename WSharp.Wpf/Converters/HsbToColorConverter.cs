using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using MaterialDesignColors.ColorManipulation;
using WSharp.Wpf.Converters.Bases;

namespace WSharp.Wpf.Converters
{
    public class HsbToColorConverter : ATypedValueConverter<Hsb, SolidColorBrush>, IMultiValueConverter
    {
        private static HsbToColorConverter instance;
        public static HsbToColorConverter Instance => instance ??= new HsbToColorConverter();

        protected override bool TInToTOut(Hsb tin, object parameter, CultureInfo culture, out SolidColorBrush tout)
        {
            tout = new SolidColorBrush(tin.ToColor());
            return true;
        }

        protected override bool TOutToTIn(SolidColorBrush tout, object parameter, CultureInfo culture, out Hsb tin)
        {
            tin = tout.Color.ToHsb();
            return true;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var h = (double)values[0];
            var s = (double)values[1];
            var b = (double)values[2];

            return new SolidColorBrush(new Hsb(h, s, b).ToColor());
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
