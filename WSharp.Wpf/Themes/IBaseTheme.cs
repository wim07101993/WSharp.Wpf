using System.Windows.Media;

namespace WSharp.Wpf.Themes
{
    public interface IBaseTheme
    {
        Color ValidationErrorColor { get; }
        Color Background { get; }
        Color Paper { get; }
        Color CardBackground { get; }
        Color ToolBarBackground { get; }
        Color Body { get; }
        Color BodyLight { get; }
        Color ColumnHeader { get; }
        Color CheckBoxOff { get; }
        Color CheckBoxDisabled { get; }
        Color TextBoxBorder { get; }
        Color Divider { get; }
        Color Selection { get; }
        Color FlatButtonClick { get; }
        Color FlatButtonRipple { get; }
        Color ToolTipBackground { get; }
        Color ChipBackground { get; }
        Color SnackbarBackground { get; }
        Color SnackbarMouseOver { get; }
        Color SnackbarRipple { get; }
        Color TextFieldBoxBackground { get; }
        Color TextFieldBoxHoverBackground { get; }
        Color TextFieldBoxDisabledBackground { get; }
        Color TextAreaBorder { get; }
        Color TextAreaInactiveBorder { get; }
    }
}
