using System.Windows;

namespace WSharp.Wpf.Helpers
{
    public static class ComboBoxHelper
    {
        #region ClassicMode

        /// <summary>
        ///     By default ComboBox uses the wrapper popup. Popup can be switched to classic Windows
        ///     desktop view by means of this attached property.
        /// </summary>
        public static readonly DependencyProperty ClassicModeProperty = DependencyProperty.RegisterAttached(
            "ClassicMode",
            typeof(bool),
            typeof(ComboBoxHelper),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        public static bool GetClassicMode(DependencyObject element) => (bool)element.GetValue(ClassicModeProperty);

        public static void SetClassicMode(DependencyObject element, object value) => element.SetValue(ClassicModeProperty, value);

        #endregion ClassicMode

        #region ShowSelectedItem

        /// <summary>
        ///     By default the selected item is hidden from the drop down list, as per Material
        ///     Design specifications. To revert to a more classic Windows desktop behaviour, and
        ///     show the currently selected item again in the drop down, set this attached propety
        ///     to true.
        /// </summary>
        public static readonly DependencyProperty ShowSelectedItemProperty = DependencyProperty.RegisterAttached(
            "ShowSelectedItem",
            typeof(bool),
            typeof(ComboBoxHelper),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        public static bool GetShowSelectedItem(DependencyObject element) => (bool)element.GetValue(ShowSelectedItemProperty);

        public static void SetShowSelectedItem(DependencyObject element, object value) => element.SetValue(ShowSelectedItemProperty, value);

        #endregion ShowSelectedItem
    }
}
