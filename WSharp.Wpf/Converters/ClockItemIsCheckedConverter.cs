using System;
using System.Globalization;

using WSharp.Wpf.Controls;
using WSharp.Wpf.Converters.Bases;

namespace WSharp.Wpf.Converters
{
    public class ClockItemIsCheckedConverter : ATypedValueConverter<DateTime, bool>
    {
        private readonly Func<DateTime> _currentTimeGetter;
        private readonly EClockDisplayMode _displayMode;
        private readonly bool _is24Hours;

        public ClockItemIsCheckedConverter(Func<DateTime> currentTimeGetter, EClockDisplayMode displayMode, bool is24Hours)
        {
            _currentTimeGetter = currentTimeGetter ?? throw new ArgumentNullException(nameof(currentTimeGetter));
            _displayMode = displayMode;
            _is24Hours = is24Hours;
        }

        protected override bool TInToTOut(DateTime tin, object parameter, CultureInfo culture, out bool tout)
        {
            if (!(parameter is int i))
            {
                tout = default;
                return false;
            }

            int converted;
            switch (_displayMode)
            {
                case EClockDisplayMode.Hours:
                    converted = MassageHour(tin.Hour, _is24Hours);
                    break;

                case EClockDisplayMode.Minutes:
                    converted = MassageMinuteSecond(tin.Minute);
                    break;

                default:
                    converted = MassageMinuteSecond(tin.Second);
                    break;
            }

            tout = converted == i;
            return true;
        }

        protected override bool TOutToTIn(bool tout, object parameter, CultureInfo culture, out DateTime tin)
        {
            if (!(parameter is int i))
            {
                tin = default;
                return false;
            }

            var currentTime = _currentTimeGetter();

            tin = new DateTime(
                currentTime.Year,
                currentTime.Month,
                currentTime.Day,
                (_displayMode == EClockDisplayMode.Hours) ? ReverseMassageHour(i, currentTime, _is24Hours) : currentTime.Hour,
                (_displayMode == EClockDisplayMode.Minutes) ? ReverseMassageMinuteSecond(i) : currentTime.Minute,
                (_displayMode == EClockDisplayMode.Seconds) ? ReverseMassageMinuteSecond(i) : currentTime.Second);

            return true;
        }

        private static int MassageHour(int val, bool is24Hours)
        {
            if (is24Hours)
                return val == 0 ? 24 : val;

            if (val == 0)
                return 12;
            if (val > 12)
                return val - 12;

            return val;
        }

        private static int MassageMinuteSecond(int val) => val == 0 ? 60 : val;

        private static int ReverseMassageHour(int val, DateTime currentTime, bool is24Hours)
        {
            if (is24Hours)
                return val == 24 ? 0 : val;

            return currentTime.Hour < 12
                ? (val == 12 ? 0 : val)
                : (val == 12 ? 12 : val + 12);
        }

        private static int ReverseMassageMinuteSecond(int val) => val == 60 ? 0 : val;
    }
}
