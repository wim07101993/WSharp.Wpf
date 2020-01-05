using System;
using System.Linq;
using System.Windows;
using WSharp.Wpf.Controls;

namespace WSharp.Wpf.Helpers
{
    public static class ThemeHelper
    {
        #region Theme
        public static readonly DependencyProperty ThemeProperty = DependencyProperty.RegisterAttached(
            "Theme",
            typeof(BaseTheme),
            typeof(ThemeHelper), 
            new PropertyMetadata(default(BaseTheme), OnThemeChanged));

        public static BaseTheme GetTheme(DependencyObject obj) => (BaseTheme)obj.GetValue(ThemeProperty);

        public static void SetTheme(DependencyObject obj, BaseTheme value) => obj.SetValue(ThemeProperty, value);

        private static void OnThemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is FrameworkElement element) || !(e.NewValue is BaseTheme newTheme))
                return;
         
            ChangeTheme(element.Resources, newTheme);
        }

        public static void ChangeTheme(ResourceDictionary resourceDictionary, BaseTheme newTheme)
        {
            if (resourceDictionary == null)
                throw new ArgumentNullException(nameof(resourceDictionary));

            var lightSource = GetResourceDictionarySource(BaseTheme.Light);
            var darkSource = GetResourceDictionarySource(BaseTheme.Dark);

            foreach (var mergedDictionary in resourceDictionary.MergedDictionaries.ToList())
            {
                if (string.Equals(mergedDictionary.Source?.ToString(), lightSource, StringComparison.OrdinalIgnoreCase))
                    _ = resourceDictionary.MergedDictionaries.Remove(mergedDictionary);

                if (string.Equals(mergedDictionary.Source?.ToString(), darkSource, StringComparison.OrdinalIgnoreCase))
                    _ = resourceDictionary.MergedDictionaries.Remove(mergedDictionary);
            }

            if (GetResourceDictionarySource(newTheme) is string newThemeSource)
                resourceDictionary.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri(newThemeSource) });
        }

        private static string GetResourceDictionarySource(BaseTheme theme)
        {
            return theme switch
            {
                BaseTheme.Light => "pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Light.xaml",
                BaseTheme.Dark => "pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Dark.xaml",
                _ => null,
            };
        }

        #endregion Theme
    }
}
