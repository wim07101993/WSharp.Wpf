using System.Windows;

using WSharp.Wpf.Converters.Bases;

namespace WSharp.Wpf.Converters
{
    public class InvertedBooleanToVisibilityConverter : ABooleanConverter<Visibility>
    {
        private static InvertedBooleanToVisibilityConverter instance;
        public static InvertedBooleanToVisibilityConverter Instance => instance ?? (instance = new InvertedBooleanToVisibilityConverter());

        public override Visibility TrueValue { get; } = Visibility.Collapsed;
        public override Visibility FalseValue { get; } = Visibility.Visible;
    }
}
