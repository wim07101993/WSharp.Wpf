using System.Windows;
using WSharp.Wpf.Converters.Bases;

namespace WSharp.Wpf.Converters
{
    public class BooleanToVisibilityConverter : ABooleanConverter<Visibility>
    {
        private static BooleanToVisibilityConverter _instance;
        public static BooleanToVisibilityConverter Instance => _instance ?? (_instance = new BooleanToVisibilityConverter());

        public override Visibility TrueValue { get; } = Visibility.Visible;
        public override Visibility FalseValue { get; } = Visibility.Collapsed;
    }
}
