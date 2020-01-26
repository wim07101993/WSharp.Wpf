using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WSharp.Wpf.Extensions
{
    public static class ControlExtensions
    {
        public static T MustHaveTemplateChild<T>(this Control control, string childName)
        {
            if (control == null)
                throw new ArgumentNullException(nameof(control));
            if (childName == null)
                throw new ArgumentNullException(nameof(childName));

            if (!(control.Template.FindName(childName, control) is T child))
                throw new InvalidOperationException($"You forgot to specify {childName} in the template.");

            return child;
        }

        public static T GetTemplateChild<T>(this Control control, string childName)
            where T : class
        {
            if (control == null)
                throw new ArgumentNullException(nameof(control));
            if (childName == null)
                throw new ArgumentNullException(nameof(childName));

            return control.Template.FindName(childName, control) as T;
        }

        public static bool TryGetTemplateChild<T>(this Control control, string childName, out T child)
            where T : class
        {
            if (control == null)
                throw new ArgumentNullException(nameof(control));
            if (childName == null)
                throw new ArgumentNullException(nameof(childName));

            child = control.Template.FindName(childName, control) as T;
            return child != null;
        }

        public static BindingBase CreateBinding(this Control control,
            DependencyProperty property,
            object owner = null,
            IValueConverter converter = null,
            object converterParameter = null)
        {
            return new Binding(property.Name)
            {
                Source = owner ?? control,
                Converter = converter,
                ConverterParameter = converterParameter
            };
        }

        public static BindingBase CreateBinding(this Control control,
            string propertyName,
            object owner = null,
            IValueConverter converter = null,
            object converterParameter = null)
        {
            return new Binding(propertyName)
            {
                Source = owner ?? control,
                Converter = converter,
                ConverterParameter = converterParameter
            };
        }
    }
}
