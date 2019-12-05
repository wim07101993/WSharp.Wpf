using System.Windows;
using WSharp.Wpf.Converters.Bases;

namespace WSharp.Wpf.Converters
{
    public class MultiBooleanAndToVisibilityConverter : AMultiBooleanAndConverter<Visibility>
    {
        private static MultiBooleanAndToVisibilityConverter _instance;
        public static MultiBooleanAndToVisibilityConverter Instance => _instance ?? (_instance = new MultiBooleanAndToVisibilityConverter());

        public override Visibility TrueValue { get; } = Visibility.Visible;

        public override Visibility FalseValue { get; } = Visibility.Collapsed;
    }
}
