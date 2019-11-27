using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using WSharp.Wpf.Controls;

namespace WSharp.Wpf.Converters
{
    public class ClockLineConverter : MarkupExtension, IValueConverter
    {
        public EClockDisplayMode DisplayMode { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var time = (DateTime)value;

            return DisplayMode == EClockDisplayMode.Hours
                ? ((time.Hour > 13) ? time.Hour - 12 : time.Hour) * (360 / 12)
                : DisplayMode == EClockDisplayMode.Minutes
                    ? (time.Minute == 0 ? 60 : time.Minute) * (360 / 60)
                    : (time.Second == 0 ? 60 : time.Second) * (360 / 60);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
