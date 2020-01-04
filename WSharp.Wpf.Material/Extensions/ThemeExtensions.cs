using System;
using WSharp.Wpf.Material.Themes;
using WSharp.Wpf.Themes;

namespace WSharp.Wpf.Material.Extensions
{
    public static class ThemeExtensions
    {
        public static IBaseTheme GetBaseTheme(this EBaseTheme baseTheme)
        {
            switch (baseTheme)
            {
                case EBaseTheme.Dark: return Theme.Dark;
                case EBaseTheme.Light: return Theme.Light;
                default: throw new InvalidOperationException();
            }
        }
    }
}
