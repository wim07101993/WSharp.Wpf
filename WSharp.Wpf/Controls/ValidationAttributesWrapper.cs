using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Windows;

namespace WSharp.Wpf.Controls
{
    public class ValidationAttributesWrapper : DependencyObject
    {
        public static readonly DependencyProperty ValuePoperty = DependencyProperty.Register(
           nameof(Value),
           typeof(IEnumerable<ValidationAttribute>),
           typeof(ValidationAttributesWrapper),
           new PropertyMetadata(default, Callback));

        private static void Callback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        public IEnumerable<ValidationAttribute> Value
        {
            get => (IEnumerable<ValidationAttribute>)GetValue(ValuePoperty);
            set => SetValue(ValuePoperty, value);
        }
    }
}
