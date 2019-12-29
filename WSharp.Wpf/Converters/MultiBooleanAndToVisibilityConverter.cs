using System.Windows;

using WSharp.Wpf.Converters.Bases;

namespace WSharp.Wpf.Converters
{
    public class MultiBooleanAndToVisibilityConverter : AMultiBooleanAndConverter<Visibility>
    {
        private static MultiBooleanAndToVisibilityConverter instance;
        public static MultiBooleanAndToVisibilityConverter Instance => instance ?? (instance = new MultiBooleanAndToVisibilityConverter());

        public override Visibility TrueValue { get; } = Visibility.Visible;

        public override Visibility FalseValue { get; } = Visibility.Collapsed;
    }
}
