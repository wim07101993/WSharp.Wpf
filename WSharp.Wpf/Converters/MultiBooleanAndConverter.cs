using WSharp.Wpf.Converters.Bases;

namespace WSharp.Wpf.Converters
{
    public class MultiBooleanAndConverter : AMultiBooleanAndConverter<bool>
    {
        private static MultiBooleanAndConverter _instance;
        public static MultiBooleanAndConverter Instance => _instance ?? (_instance = new MultiBooleanAndConverter());

        public override bool TrueValue { get; } = true;

        public override bool FalseValue { get; } = false;
    }
}
