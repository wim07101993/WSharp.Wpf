using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using WSharp.Wpf.Converters.Bases;

namespace WSharp.Wpf.Converters
{
    public class EditorBrowsableStateToBooleanConverter : ATypedValueConverter<EditorBrowsableState, bool>
    {
        private static EditorBrowsableStateToBooleanConverter _instance;
        public static EditorBrowsableStateToBooleanConverter Instance => _instance ?? (_instance = new EditorBrowsableStateToBooleanConverter());

        public EditorBrowsableState BrosableStates { get; set; } = EditorBrowsableState.Advanced | EditorBrowsableState.Always;

        protected override bool TInToTOut(EditorBrowsableState tin, object parameter, CultureInfo culture, out bool tout)
        {
            tout = (tin & BrosableStates) > 0;
            return true;
        }
    }
}
