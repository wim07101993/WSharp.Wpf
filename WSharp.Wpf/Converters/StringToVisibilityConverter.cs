using System.Windows;

using WSharp.Wpf.Converters.Bases;

namespace WSharp.Wpf.Converters
{
    public class StringToVisibilityConverter : AStringNullOrEmptyConverter<Visibility>
    {
        private static StringToVisibilityConverter instance;
        public static StringToVisibilityConverter Instance => instance ?? (instance = new StringToVisibilityConverter());

        public override Visibility NullOrEmptyValue { get; } = Visibility.Collapsed;

        public override Visibility NotNullOrEmptyValue { get; } = Visibility.Visible;
    }
}
