using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace WSharp.Wpf.Helpers
{
    public static class ToggleButtonHelper
    {
        #region HasContent

        private static readonly DependencyPropertyKey hasOnContentPropertyKey = DependencyProperty.RegisterAttachedReadOnly(
                "HasOnContent",
                typeof(bool),
                typeof(ToggleButtonHelper),
                new PropertyMetadata(false));

        public static readonly DependencyProperty HasOnContentProperty = hasOnContentPropertyKey.DependencyProperty;

        private static void SetHasOnContent(DependencyObject element, object value) => element.SetValue(hasOnContentPropertyKey, value);

        /// <summary>Framework use only.</summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool GetHasOnContent(DependencyObject element) => (bool)element.GetValue(HasOnContentProperty);

        #endregion HasContent

        #region OnContent

        /// <summary>
        ///     Allows on (IsChecked) content to be provided on supporting
        ///     <see cref="ToggleButton"/> styles.
        /// </summary>
        public static readonly DependencyProperty OnContentProperty = DependencyProperty.RegisterAttached(
            "OnContent",
            typeof(object),
            typeof(ToggleButtonHelper),
            new PropertyMetadata(default(object), OnContentPropertyChangedCallback));

        private static void OnContentPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs args) => SetHasOnContent(d, args.NewValue != null);

        /// <summary>
        ///     Allows on (IsChecked) content to be provided on supporting
        ///     <see cref="ToggleButton"/> styles.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="value"></param>
        public static void SetOnContent(DependencyObject element, object value) => element.SetValue(OnContentProperty, value);

        /// <summary>
        ///     Allows on (IsChecked) content to be provided on supporting
        ///     <see cref="ToggleButton"/> styles.
        /// </summary>
        public static object GetOnContent(DependencyObject element) => element.GetValue(OnContentProperty);

        #endregion OnContent

        #region OnContentTemplate

        /// <summary>
        ///     Allows an on (IsChecked) template to be provided on supporting
        ///     <see cref="ToggleButton"/> styles.
        /// </summary>
        public static readonly DependencyProperty OnContentTemplateProperty = DependencyProperty.RegisterAttached(
            "OnContentTemplate",
            typeof(DataTemplate),
            typeof(ToggleButtonHelper),
            new PropertyMetadata(default(DataTemplate)));

        /// <summary>
        ///     Allows an on (IsChecked) template to be provided on supporting
        ///     <see cref="ToggleButton"/> styles.
        /// </summary>
        public static void SetOnContentTemplate(DependencyObject element, DataTemplate value) => element.SetValue(OnContentTemplateProperty, value);

        /// <summary>
        ///     Allows an on (IsChecked) template to be provided on supporting
        ///     <see cref="ToggleButton"/> styles.
        /// </summary>
        public static DataTemplate GetOnContentTemplate(DependencyObject element) => (DataTemplate)element.GetValue(OnContentTemplateProperty);

        #endregion OnContentTemplate

        #region SwitchTrackOnBackground

        public static DependencyProperty SwitchTrackOnBackgroundProperty = DependencyProperty.RegisterAttached(
                "SwitchTrackOnBackground",
                typeof(SolidColorBrush),
                typeof(ToggleButtonHelper));

        public static void SetSwitchTrackOnBackground(DependencyObject element, SolidColorBrush value) => element.SetValue(SwitchTrackOnBackgroundProperty, value);

        public static SolidColorBrush GetSwitchTrackOnBackground(DependencyObject element) => (SolidColorBrush)element.GetValue(SwitchTrackOnBackgroundProperty);

        #endregion SwitchTrackOnBackground

        #region SwitchTrackOffBackground

        public static DependencyProperty SwitchTrackOffBackgroundProperty = DependencyProperty.RegisterAttached(
                "SwitchTrackOffBackground",
                typeof(SolidColorBrush),
                typeof(ToggleButtonHelper));

        public static void SetSwitchTrackOffBackground(DependencyObject element, SolidColorBrush value) => element.SetValue(SwitchTrackOffBackgroundProperty, value);

        public static SolidColorBrush GetSwitchTrackOffBackground(DependencyObject element) => (SolidColorBrush)element.GetValue(SwitchTrackOffBackgroundProperty);

        #endregion SwitchTrackOffBackground
    }
}
