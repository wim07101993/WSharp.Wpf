using WSharp.Wpf.Converters.Bases;

namespace WSharp.Wpf.Converters
{
    public class MultiBooleanAndConverter : AMultiBooleanAndConverter<bool>
    {
        private static MultiBooleanAndConverter instance;
        public static MultiBooleanAndConverter Instance => instance ??= new MultiBooleanAndConverter();

        public override bool TrueValue { get; } = true;

        public override bool FalseValue { get; } = false;
    }
}
