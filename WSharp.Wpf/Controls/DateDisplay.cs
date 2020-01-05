using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using WSharp.Extensions;
using WSharp.Wpf.Extensions;

using Calendar = System.Windows.Controls.Calendar;

namespace WSharp.Wpf.Controls
{
    public class DateDisplay : Control
    {
        public static readonly DependencyProperty DisplayDateProperty = DependencyProperty.Register(
            nameof(DisplayDate),
            typeof(DateTime),
            typeof(DateDisplay),
            new PropertyMetadata(default(DateTime), DisplayDatePropertyChangedCallback));

        private static readonly DependencyPropertyKey componentOneContentPropertyKey = DependencyProperty.RegisterReadOnly(
                nameof(ComponentOneContent),
                typeof(string),
                typeof(DateDisplay),
                new PropertyMetadata(default(string)));

        public static readonly DependencyProperty ComponentOneContentProperty = componentOneContentPropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey componentTwoContentPropertyKey = DependencyProperty.RegisterReadOnly(
                nameof(ComponentTwoContent),
                typeof(string),
                typeof(DateDisplay),
                new PropertyMetadata(default(string)));

        public static readonly DependencyProperty ComponentTwoContentProperty = componentTwoContentPropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey componentThreeContentPropertyKey = DependencyProperty.RegisterReadOnly(
                nameof(ComponentThreeContent),
                typeof(string),
                typeof(DateDisplay),
                new PropertyMetadata(default(string)));

        public static readonly DependencyProperty ComponentThreeContentProperty = componentThreeContentPropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey isDayInFirstComponentPropertyKey = DependencyProperty.RegisterReadOnly(
                nameof(IsDayInFirstComponent),
                typeof(bool),
                typeof(DateDisplay),
                new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty IsDayInFirstComponentProperty = isDayInFirstComponentPropertyKey.DependencyProperty;

        static DateDisplay()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DateDisplay), new FrameworkPropertyMetadata(typeof(DateDisplay)));
        }

        public DateDisplay()
        {
            SetCurrentValue(DisplayDateProperty, DateTime.Today);
        }

        public DateTime DisplayDate
        {
            get => (DateTime)GetValue(DisplayDateProperty);
            set => SetValue(DisplayDateProperty, value);
        }

        public string ComponentOneContent
        {
            get => (string)GetValue(ComponentOneContentProperty);
            private set => SetValue(componentOneContentPropertyKey, value);
        }

        public string ComponentTwoContent
        {
            get => (string)GetValue(ComponentTwoContentProperty);
            private set => SetValue(componentTwoContentPropertyKey, value);
        }

        public string ComponentThreeContent
        {
            get => (string)GetValue(ComponentThreeContentProperty);
            private set => SetValue(componentThreeContentPropertyKey, value);
        }

        public bool IsDayInFirstComponent
        {
            get => (bool)GetValue(IsDayInFirstComponentProperty);
            private set => SetValue(isDayInFirstComponentPropertyKey, value);
        }

        private void UpdateComponents()
        {
            var culture = Language.GetSpecificCulture();
            var dateTimeFormatInfo = culture.GetDateFormat();
            var minDateTime = dateTimeFormatInfo.Calendar.MinSupportedDateTime;
            var maxDateTime = dateTimeFormatInfo.Calendar.MaxSupportedDateTime;

            if (DisplayDate < minDateTime)
            {
                SetDisplayDateOfCalendar(minDateTime);

                // return to avoid second formatting of the same value
                return;
            }

            if (DisplayDate > maxDateTime)
            {
                SetDisplayDateOfCalendar(maxDateTime);

                // return to avoid second formatting of the same value
                return;
            }

            ComponentOneContent = DisplayDate.ToString(dateTimeFormatInfo.MonthDayPattern.Replace("MMMM", "MMM"), culture).ToTitleCase(culture); //Day Month following culture order. We don't want the month to take too much space
            ComponentTwoContent = DisplayDate.ToString("ddd,", culture).ToTitleCase(culture);   // Day of week first
            ComponentThreeContent = DisplayDate.ToString("yyyy", culture).ToTitleCase(culture); // Year always top
        }

        private void SetDisplayDateOfCalendar(DateTime displayDate)
        {
            var calendarControl = this.GetVisualAncestry().OfType<Calendar>().FirstOrDefault();
            if (calendarControl != null)
                calendarControl.DisplayDate = displayDate;
        }

        private static void DisplayDatePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is DateDisplay materialDateDisplay))
                return;

            materialDateDisplay.UpdateComponents();
        }
    }
}
