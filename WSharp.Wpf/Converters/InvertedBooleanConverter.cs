using WSharp.Wpf.Converters.Bases;

namespace WSharp.Wpf.Converters
{
    public class InvertedBooleanConverter : ABooleanConverter<bool>
    {
        private static InvertedBooleanConverter instance;
        public static InvertedBooleanConverter Instance => instance ??= new InvertedBooleanConverter();

        public override bool TrueValue { get; } = false;
        public override bool FalseValue { get; } = true;
    }
}
