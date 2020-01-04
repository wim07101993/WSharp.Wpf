using System;
using System.Windows;
using System.Windows.Media;
using WSharp.Wpf.Colors;
using WSharp.Wpf.Extensions;
using WSharp.Wpf.Material.Themes;
using WSharp.Wpf.Themes;

namespace WSharp.Wpf.Material.Extensions
{
    public static class ResourceDictionaryExtensions
    {
        private static Guid CurrentThemeKey { get; } = Guid.NewGuid();
        private static Guid ThemeManagerKey { get; } = Guid.NewGuid();

        public static ITheme GetTheme(this ResourceDictionary resourceDictionary)
        {
            if (resourceDictionary == null) 
                throw new ArgumentNullException(nameof(resourceDictionary));

            if (resourceDictionary[CurrentThemeKey] is ITheme theme)
                return theme;

            Color secondaryMid = GetColor("SecondaryHueMidBrush", "SecondaryAccentBrush");
            Color secondaryMidForeground = GetColor("SecondaryHueMidForegroundBrush", "SecondaryAccentForegroundBrush");

            if (!TryGetColor("SecondaryHueLightBrush", out Color secondaryLight))
                secondaryLight = secondaryMid.Lighten();

            if (!TryGetColor("SecondaryHueLightForegroundBrush", out Color secondaryLightForeground))
                secondaryLightForeground = secondaryLight.ContrastingForegroundColor();

            if (!TryGetColor("SecondaryHueDarkBrush", out Color secondaryDark))
                secondaryDark = secondaryMid.Darken();

            if (!TryGetColor("SecondaryHueDarkForegroundBrush", out Color secondaryDarkForeground))
                secondaryDarkForeground = secondaryDark.ContrastingForegroundColor();

            //Attempt to simply look up the appropriate resources
            return new Theme
            {
                PrimaryLight = new ColorPair(GetColor("PrimaryHueLightBrush"), GetColor("PrimaryHueLightForegroundBrush")),
                PrimaryMid = new ColorPair(GetColor("PrimaryHueMidBrush"), GetColor("PrimaryHueMidForegroundBrush")),
                PrimaryDark = new ColorPair(GetColor("PrimaryHueDarkBrush"), GetColor("PrimaryHueDarkForegroundBrush")),

                SecondaryLight = new ColorPair(secondaryLight, secondaryLightForeground),
                SecondaryMid = new ColorPair(secondaryMid, secondaryMidForeground),
                SecondaryDark = new ColorPair(secondaryDark, secondaryDarkForeground),

                ValidationError = GetColor("ValidationErrorBrush"),
                Background = GetColor("MaterialDesignBackground"),
                Paper = GetColor("MaterialDesignPaper"),
                CardBackground = GetColor("MaterialDesignCardBackground"),
                ToolBarBackground = GetColor("MaterialDesignToolBarBackground"),
                Body = GetColor("MaterialDesignBody"),
                BodyLight = GetColor("MaterialDesignBodyLight"),
                ColumnHeader = GetColor("MaterialDesignColumnHeader"),
                CheckBoxOff = GetColor("MaterialDesignCheckBoxOff"),
                CheckBoxDisabled = GetColor("MaterialDesignCheckBoxDisabled"),
                TextBoxBorder = GetColor("MaterialDesignTextBoxBorder"),
                Divider = GetColor("MaterialDesignDivider"),
                Selection = GetColor("MaterialDesignSelection"),
                FlatButtonClick = GetColor("MaterialDesignFlatButtonClick"),
                FlatButtonRipple = GetColor("MaterialDesignFlatButtonRipple"),
                ToolTipBackground = GetColor("MaterialDesignToolTipBackground"),
                ChipBackground = GetColor("MaterialDesignChipBackground"),
                SnackbarBackground = GetColor("MaterialDesignSnackbarBackground"),
                SnackbarMouseOver = GetColor("MaterialDesignSnackbarMouseOver"),
                SnackbarRipple = GetColor("MaterialDesignSnackbarRipple"),
                TextFieldBoxBackground = GetColor("MaterialDesignTextFieldBoxBackground"),
                TextFieldBoxHoverBackground = GetColor("MaterialDesignTextFieldBoxHoverBackground"),
                TextFieldBoxDisabledBackground = GetColor("MaterialDesignTextFieldBoxDisabledBackground"),
                TextAreaBorder = GetColor("MaterialDesignTextAreaBorder"),
                TextAreaInactiveBorder = GetColor("MaterialDesignTextAreaInactiveBorder")
            };

            Color GetColor(params string[] keys)
            {
                foreach (string key in keys)
                    if (TryGetColor(key, out Color color))
                        return color;

                throw new InvalidOperationException($"Could not locate required resource with key(s) '{string.Join(", ", keys)}'");
            }

            bool TryGetColor(string key, out Color color)
            {
                if (resourceDictionary[key] is SolidColorBrush brush)
                {
                    color = brush.Color;
                    return true;
                }
                color = default;
                return false;
            }
        }
    }
}
