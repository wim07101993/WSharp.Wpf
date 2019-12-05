using System.Globalization;
using System.Windows;
using WSharp.Wpf.Converters.Bases;

namespace WSharp.Wpf.Converters
{
    public class StringToVisibilityConverter : AStringNullOrEmptyConverter<Visibility>
    {
        private static StringToVisibilityConverter _instance;
        public static StringToVisibilityConverter Instance => _instance ?? (_instance = new StringToVisibilityConverter());

        public override Visibility NullOrEmptyValue { get; } = Visibility.Collapsed;

        public override Visibility NotNullOrEmptyValue { get; } = Visibility.Visible;
    }
}
