using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Controls;
using WSharp.Wpf.Converters.Bases;

namespace WSharp.Wpf.Converters
{
    public class TraceEventTypeToStringConverter : ATypedValueConverter<TraceEventType, string>
    {
        private const TraceEventType AllEventTypes =
#pragma warning disable RECS0016 // Bitwise operation on enum which has no [Flags] attribute
                    TraceEventType.Critical |
                    TraceEventType.Error |
                    TraceEventType.Warning |
                    TraceEventType.Information |
                    TraceEventType.Verbose |
                    TraceEventType.Start |
                    TraceEventType.Stop |
                    TraceEventType.Suspend |
                    TraceEventType.Resume |
                    TraceEventType.Transfer;
#pragma warning restore RECS0016 // Bitwise operation on enum which has no [Flags] attribute

        private static TraceEventTypeToStringConverter _instance;
        public static TraceEventTypeToStringConverter Instance => _instance ?? (_instance = new TraceEventTypeToStringConverter());

        protected override bool TInToTOut(TraceEventType tin, object parameter, CultureInfo culture, out string tout)
        {
            tout = tin == AllEventTypes 
                ? "Everything" 
                : tin.ToString();

            return true;
        }

        protected override bool ValidateTOut(object value, CultureInfo culture, out string typedValue)
        {
            if (base.ValidateTOut(value, culture, out typedValue))
                return true;

            typedValue = value is ComboBoxItem comboBoxItem 
                ? comboBoxItem.Content as string ?? comboBoxItem.Content?.ToString() 
                : value?.ToString();

            return true;
        }

        protected override bool TOutToTIn(string tout, object parameter, CultureInfo culture, out TraceEventType tin)
        {
            if (string.IsNullOrWhiteSpace(tout))
                tin = AllEventTypes;
            else if (!Enum.TryParse(tout, true, out tin))
                tin = AllEventTypes;

            return true;
        }
    }
}
