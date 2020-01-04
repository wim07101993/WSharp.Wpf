using System.Windows.Media;
using WSharp.Wpf.Themes;

namespace WSharp.Wpf.Material.Themes
{
    public class LightTheme : IBaseTheme
    {
        public Color ValidationErrorColor { get; } = (Color)ColorConverter.ConvertFromString("#f44336");
        public Color Background { get; } = (Color)ColorConverter.ConvertFromString("#FFFFFFFF");
        public Color Paper { get; } = (Color)ColorConverter.ConvertFromString("#FFfafafa");
        public Color CardBackground { get; } = (Color)ColorConverter.ConvertFromString("#FFFFFFFF");
        public Color ToolBarBackground { get; } = (Color)ColorConverter.ConvertFromString("#FFF5F5F5");
        public Color Body { get; } = (Color)ColorConverter.ConvertFromString("#DD000000");
        public Color BodyLight { get; } = (Color)ColorConverter.ConvertFromString("#89000000");
        public Color ColumnHeader { get; } = (Color)ColorConverter.ConvertFromString("#BC000000");
        public Color CheckBoxOff { get; } = (Color)ColorConverter.ConvertFromString("#89000000");
        public Color CheckBoxDisabled { get; } = (Color)ColorConverter.ConvertFromString("#FFBDBDBD");
        public Color TextBoxBorder { get; } = (Color)ColorConverter.ConvertFromString("#89000000");
        public Color Divider { get; } = (Color)ColorConverter.ConvertFromString("#1F000000");
        public Color Selection { get; } = (Color)ColorConverter.ConvertFromString("#FFDeDeDe");
        public Color FlatButtonClick { get; } = (Color)ColorConverter.ConvertFromString("#FFDeDeDe");
        public Color FlatButtonRipple { get; } = (Color)ColorConverter.ConvertFromString("#FFB6B6B6");
        public Color ToolTipBackground { get; } = (Color)ColorConverter.ConvertFromString("#757575");
        public Color ChipBackground { get; } = (Color)ColorConverter.ConvertFromString("#12000000");
        public Color SnackbarBackground { get; } = (Color)ColorConverter.ConvertFromString("#FF323232");
        public Color SnackbarMouseOver { get; } = (Color)ColorConverter.ConvertFromString("#FF464642");
        public Color SnackbarRipple { get; } = (Color)ColorConverter.ConvertFromString("#FFB6B6B6");
        public Color TextFieldBoxBackground { get; } = (Color)ColorConverter.ConvertFromString("#0F000000");
        public Color TextFieldBoxHoverBackground { get; } = (Color)ColorConverter.ConvertFromString("#14000000");
        public Color TextFieldBoxDisabledBackground { get; } = (Color)ColorConverter.ConvertFromString("#08000000");
        public Color TextAreaBorder { get; } = (Color)ColorConverter.ConvertFromString("#BC000000");
        public Color TextAreaInactiveBorder { get; } = (Color)ColorConverter.ConvertFromString("#0F000000");
    }
}
