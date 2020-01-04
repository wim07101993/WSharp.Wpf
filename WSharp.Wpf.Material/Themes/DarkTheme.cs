using System.Windows.Media;
using WSharp.Wpf.Themes;

namespace WSharp.Wpf.Material.Themes
{
    public class DarkTheme : IBaseTheme
    {
        public Color ValidationErrorColor { get; } = (Color)ColorConverter.ConvertFromString("#f44336");
        public Color Background { get; } = (Color)ColorConverter.ConvertFromString("#FF000000");
        public Color Paper { get; } = (Color)ColorConverter.ConvertFromString("#FF303030");
        public Color CardBackground { get; } = (Color)ColorConverter.ConvertFromString("#FF424242");
        public Color ToolBarBackground { get; } = (Color)ColorConverter.ConvertFromString("#FF212121");
        public Color Body { get; } = (Color)ColorConverter.ConvertFromString("#DDFFFFFF");
        public Color BodyLight { get; } = (Color)ColorConverter.ConvertFromString("#89FFFFFF");
        public Color ColumnHeader { get; } = (Color)ColorConverter.ConvertFromString("#BCFFFFFF");
        public Color CheckBoxOff { get; } = (Color)ColorConverter.ConvertFromString("#89FFFFFF");
        public Color CheckBoxDisabled { get; } = (Color)ColorConverter.ConvertFromString("#FF647076");
        public Color TextBoxBorder { get; } = (Color)ColorConverter.ConvertFromString("#89FFFFFF");
        public Color Divider { get; } = (Color)ColorConverter.ConvertFromString("#1FFFFFFF");
        public Color Selection { get; } = (Color)ColorConverter.ConvertFromString("#757575");
        public Color FlatButtonClick { get; } = (Color)ColorConverter.ConvertFromString("#19757575");
        public Color FlatButtonRipple { get; } = (Color)ColorConverter.ConvertFromString("#FFB6B6B6");
        public Color ToolTipBackground { get; } = (Color)ColorConverter.ConvertFromString("#eeeeee");
        public Color ChipBackground { get; } = (Color)ColorConverter.ConvertFromString("#FF2E3C43");
        public Color SnackbarBackground { get; } = (Color)ColorConverter.ConvertFromString("#FFCDCDCD");
        public Color SnackbarMouseOver { get; } = (Color)ColorConverter.ConvertFromString("#FFB9B9BD");
        public Color SnackbarRipple { get; } = (Color)ColorConverter.ConvertFromString("#FF494949");
        public Color TextFieldBoxBackground { get; } = (Color)ColorConverter.ConvertFromString("#1AFFFFFF");
        public Color TextFieldBoxHoverBackground { get; } = (Color)ColorConverter.ConvertFromString("#1FFFFFFF");
        public Color TextFieldBoxDisabledBackground { get; } = (Color)ColorConverter.ConvertFromString("#0DFFFFFF");
        public Color TextAreaBorder { get; } = (Color)ColorConverter.ConvertFromString("#BCFFFFFF");
        public Color TextAreaInactiveBorder { get; } = (Color)ColorConverter.ConvertFromString("#1AFFFFFF");
    }
}
