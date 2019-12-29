using System.Globalization;
using System.Windows.Controls;

using WSharp.Wpf.Converters.Bases;

namespace WSharp.Wpf.Converters
{
    public class LabelProxyConverter : ATypedValueConverter<Control, ILabelProxy>
    {
        private static LabelProxyConverter instance;

        public static LabelProxyConverter Instance => instance ?? (instance = new LabelProxyConverter());

        protected override bool TInToTOut(Control tin, object parameter, CultureInfo culture, out ILabelProxy tout)
        {
            tout = LabelProxyProvider.Get(tin);
            return true;
        }
    }
}
