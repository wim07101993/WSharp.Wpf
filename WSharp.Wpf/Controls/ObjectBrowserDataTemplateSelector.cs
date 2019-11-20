using System.Windows;
using System.Windows.Controls;
using WSharp.Reflection;

namespace WSharp.Wpf.Controls
{
    public class ObjectBrowserDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate StringTemplate { get; set; }
        public DataTemplate NumberWithoutDecimalTemplate { get; set; }
        public DataTemplate NumberWithDecimalTemplate { get; set; }
        public DataTemplate ObjectTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (!(item is ObjectBrowserProperty property))
                return base.SelectTemplate(item, container);

            if (property.Type == typeof(string))
                return StringTemplate;
            else if (property.Type == typeof(short) || property.Type == typeof(int) || property.Type == typeof(long) ||
                property.Type == typeof(ushort) || property.Type == typeof(uint) || property.Type == typeof(ulong))
                return NumberWithoutDecimalTemplate;
            else if (property.Type == typeof(float) || property.Type == typeof(decimal) || property.Type == typeof(double))
                return NumberWithDecimalTemplate;

            return ObjectTemplate;
        }
    }
}