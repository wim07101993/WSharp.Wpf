using System.ComponentModel;
using System.Globalization;

using WSharp.Wpf.Converters.Bases;

namespace WSharp.Wpf.Converters
{
    public class EditorBrowsableStateToBooleanConverter : ATypedValueConverter<EditorBrowsableState, bool>
    {
        private static EditorBrowsableStateToBooleanConverter instance;
        public static EditorBrowsableStateToBooleanConverter Instance => instance ??= new EditorBrowsableStateToBooleanConverter();

        public EditorBrowsableState BrosableStates { get; set; } = EditorBrowsableState.Advanced | EditorBrowsableState.Always;

        protected override bool TInToTOut(EditorBrowsableState tin, object parameter, CultureInfo culture, out bool tout)
        {
            tout = (tin & BrosableStates) > 0;
            return true;
        }
    }
}
