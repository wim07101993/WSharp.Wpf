using System.Windows;
using WSharp.Wpf.Converters.Bases;

namespace WSharp.Wpf.Converters
{
    public class InvertedMultiBooleanAndToVisibilityConverter : AMultiBooleanAndConverter<Visibility>
    {
        private static InvertedMultiBooleanAndToVisibilityConverter instance;
        public static InvertedMultiBooleanAndToVisibilityConverter Instance => instance ??= new InvertedMultiBooleanAndToVisibilityConverter();

        public override Visibility TrueValue { get; } = Visibility.Collapsed;

        public override Visibility FalseValue { get; } = Visibility.Visible;
    }
}
