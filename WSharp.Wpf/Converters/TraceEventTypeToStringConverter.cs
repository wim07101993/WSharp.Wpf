using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace WSharp.Wpf.Converters
{
    public class TraceEventTypeToStringConverter : IValueConverter
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

        public static TraceEventTypeToStringConverter Instance { get; } = new TraceEventTypeToStringConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TraceEventType traceEventType))
                return null;

            if (traceEventType == AllEventTypes)
                return "Everything";

            return traceEventType.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = (value as ComboBoxItem).Content as string ?? value as string;
            if (string.IsNullOrWhiteSpace(str))
                return AllEventTypes;

            return Enum.TryParse<TraceEventType>(str, true, out var eventType)
                ? eventType
                : AllEventTypes;
        }
    }
}
