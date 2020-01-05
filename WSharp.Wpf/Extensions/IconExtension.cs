using System;
using System.Windows.Markup;

namespace WSharp.Wpf.Controls
{
    [MarkupExtensionReturnType(typeof(Icon))]
    public class IconExtension : MarkupExtension
    {
        public IconExtension()
        { }

        public IconExtension(string resource)
        {
            Resource = resource;
        }

        public IconExtension(string resource, double size)
        {
            Resource = resource;
            Size = size;
        }

        [ConstructorArgument("resource")]
        public string Resource { get; set; }

        [ConstructorArgument("size")]
        public double? Size { get; set; }        

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var result = new Icon {Resource = Resource};

            if (Size.HasValue)
            {
                result.Height = Size.Value;
                result.Width = Size.Value;
            }

            return result;
        }
    }
}
