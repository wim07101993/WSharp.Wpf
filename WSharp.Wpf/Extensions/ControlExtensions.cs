using System;
using System.Windows.Controls;

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
    }
}
