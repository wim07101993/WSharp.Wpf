﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WSharp.Wpf.Converters
{
    public class ToStringConverter : IValueConverter
    {
        private static ToStringConverter _instance;
        public static ToStringConverter Instance => _instance ?? (_instance = new ToStringConverter());

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value?.ToString();

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => DependencyProperty.UnsetValue;
    }
}
