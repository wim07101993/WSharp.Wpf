using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WSharp.Wpf.Controls
{
    public abstract class AControl : Control
    {
        protected T GetTemplateChild<T>(string childName)
            where T : class
        {
            if (!(GetTemplateChild(childName) is T child))
                throw new InvalidOperationException($"You failed to specify the {childName} in the template.");
            return child;
        }

        protected static ContentControl CreateContentWithTemplate(Binding contentBinding, DataTemplate template)
        {
            var control = new ContentControl { ContentTemplate = template };
            control.SetBinding(ContentControl.ContentProperty, contentBinding);
            return control;
        }

        protected ContentControl CreateContentWithTemplate(Binding contentBinding, string templateKey)
        {
            var template = TryFindResource(templateKey) as DataTemplate;
            var control = new ContentControl
            {
                ContentTemplate = template
            };

            control.SetBinding(ContentControl.ContentProperty, contentBinding);
            return control;
        }
    }
}