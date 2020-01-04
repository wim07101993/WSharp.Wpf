using System;
using System.Windows.Media;
using WSharp.Wpf.Themes;

namespace WSharp.Wpf.Extensions
{
    public static class ThemeExtensions
    {
        public static void SetBaseTheme(this ITheme theme, IBaseTheme baseTheme)
        {
            if (theme == null) throw new ArgumentNullException(nameof(theme));

            theme.ValidationError = baseTheme.ValidationErrorColor;
            theme.Background = baseTheme.Background;
            theme.Paper = baseTheme.Paper;
            theme.CardBackground = baseTheme.CardBackground;
            theme.ToolBarBackground = baseTheme.ToolBarBackground;
            theme.Body = baseTheme.Body;
            theme.BodyLight = baseTheme.BodyLight;
            theme.ColumnHeader = baseTheme.ColumnHeader;
            theme.CheckBoxOff = baseTheme.CheckBoxOff;
            theme.CheckBoxDisabled = baseTheme.CheckBoxDisabled;
            theme.Divider = baseTheme.Divider;
            theme.Selection = baseTheme.Selection;
            theme.FlatButtonClick = baseTheme.FlatButtonClick;
            theme.FlatButtonRipple = baseTheme.FlatButtonRipple;
            theme.ToolTipBackground = baseTheme.ToolTipBackground;
            theme.ChipBackground = baseTheme.ChipBackground;
            theme.SnackbarBackground = baseTheme.SnackbarBackground;
            theme.SnackbarMouseOver = baseTheme.SnackbarMouseOver;
            theme.SnackbarRipple = baseTheme.SnackbarRipple;
            theme.TextBoxBorder = baseTheme.TextBoxBorder;
            theme.TextFieldBoxBackground = baseTheme.TextFieldBoxBackground;
            theme.TextFieldBoxHoverBackground = baseTheme.TextFieldBoxHoverBackground;
            theme.TextFieldBoxDisabledBackground = baseTheme.TextFieldBoxDisabledBackground;
            theme.TextAreaBorder = baseTheme.TextAreaBorder;
            theme.TextAreaInactiveBorder = baseTheme.TextAreaInactiveBorder;
        }

        public static void SetPrimaryColor(this ITheme theme, Color primaryColor)
        {
            if (theme == null) 
                throw new ArgumentNullException(nameof(theme));

            theme.PrimaryLight = primaryColor.Lighten();
            theme.PrimaryMid = primaryColor;
            theme.PrimaryDark = primaryColor.Darken();
        }

        public static void SetSecondaryColor(this ITheme theme, Color accentColor)
        {
            if (theme == null)
                throw new ArgumentNullException(nameof(theme));

            theme.SecondaryLight = accentColor.Lighten();
            theme.SecondaryMid = accentColor;
            theme.SecondaryDark = accentColor.Darken();
        }
    }
}
