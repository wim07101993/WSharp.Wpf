using System;
using System.Windows;

namespace WSharp.Wpf.Controls
{
    public class DateTimePicker : AControl
    {
        #region DEPENDENCY PROPERTIES

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value),
            typeof(DateTime?),
            typeof(DateTimePicker),
            new PropertyMetadata(default(DateTime), OnValueChanged));

        public static readonly DependencyProperty TimeOfDayProperty = DependencyProperty.Register(
          nameof(TimeOfDay),
          typeof(DateTime?),
          typeof(DateTimePicker),
            new PropertyMetadata(DateTime.Now, OnTimeOfDayChanged));

        public static readonly DependencyProperty DateProperty = DependencyProperty.Register(
          nameof(Date),
          typeof(DateTime?),
          typeof(DateTimePicker),
            new PropertyMetadata(DateTime.Now, OnDateChanged));

        #endregion DEPENDENCY PROPERTIES


        #region FIELDS

        private bool _isUpdatingValue;

        #endregion FIELDS


        #region CONSTRUCTORS

        static DateTimePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DateTimePicker), new FrameworkPropertyMetadata(typeof(DateTimePicker)));
        }

        #endregion CONSTRUCTORS


        #region PROPERTIES

        public DateTime? Value
        {
            get => (DateTime?)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public DateTime? TimeOfDay
        {
            get => (DateTime?)GetValue(TimeOfDayProperty);
            set => SetValue(ValueProperty, value);
        }

        public DateTime? Date
        {
            get => (DateTime?)GetValue(DateProperty);
            set => SetValue(ValueProperty, value);
        }

        #endregion PROPERTIES


        #region METHDOS

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is DateTimePicker dateTimePicker) ||
                Equals(e.NewValue, e.OldValue) ||
                dateTimePicker._isUpdatingValue)
                return;

            dateTimePicker._isUpdatingValue = true;

            if (e.NewValue is DateTime newValue)
            {
                dateTimePicker.TimeOfDay = newValue;
                dateTimePicker.Date = newValue.Date;
            }
            else
            {
                dateTimePicker.TimeOfDay = default;
                dateTimePicker.Date = default;
            }

            dateTimePicker._isUpdatingValue = false;
        }

        private static void OnTimeOfDayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is DateTimePicker dateTimePicker) ||
                 Equals(e.NewValue, e.OldValue) ||
                 dateTimePicker._isUpdatingValue)
                return;

            dateTimePicker._isUpdatingValue = true;

            var newValue = e.NewValue as DateTime?;
            dateTimePicker.Value = new DateTime(
                year: dateTimePicker.Value?.Year ?? dateTimePicker.Date?.Year ?? default,
                month: dateTimePicker.Value?.Month ?? dateTimePicker.Date?.Month ?? default,
                day: dateTimePicker.Value?.Day ?? dateTimePicker.Date?.Day ?? default,
                hour: newValue?.TimeOfDay.Hours ?? default,
                minute: newValue?.TimeOfDay.Minutes ?? default,
                second: newValue?.TimeOfDay.Seconds ?? default,
                millisecond: newValue?.TimeOfDay.Milliseconds ?? default);

            dateTimePicker._isUpdatingValue = false;
        }

        private static void OnDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is DateTimePicker dateTimePicker) ||
                Equals(e.NewValue, e.OldValue) ||
                dateTimePicker._isUpdatingValue)
                return;

            dateTimePicker._isUpdatingValue = true;

            var newValue = e.NewValue as TimeSpan?;
            dateTimePicker.Value = new DateTime(
                year: dateTimePicker.Date?.Year ?? default,
                month: dateTimePicker.Date?.Month ?? default,
                day: dateTimePicker.Date?.Day ?? default,
                hour: dateTimePicker.Value?.Hour ?? newValue?.Hours ?? default,
                minute: dateTimePicker.Value?.Minute ?? newValue?.Minutes ?? default,
                second: dateTimePicker.Value?.Second ?? newValue?.Seconds ?? default,
                millisecond: dateTimePicker.Value?.Millisecond ?? newValue?.Milliseconds ?? default);

            dateTimePicker._isUpdatingValue = false;
        }

        #endregion METHODS
    }
}
