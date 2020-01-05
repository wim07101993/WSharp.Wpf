using System.Windows;

namespace WSharp.Wpf.Helpers
{
    /// <summary>
    /// Allows transitions to be disabled where supported.
    /// </summary>
    public static class TransitionHelper
    {
        #region DisableTransitions
        /// <summary>
        /// Allows transitions to be disabled where supported.  Note this is an inheritable property.
        /// </summary>
        public static readonly DependencyProperty DisableTransitionsProperty = DependencyProperty.RegisterAttached(
            "DisableTransitions", 
            typeof(bool), 
            typeof(TransitionHelper), 
            new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Allows transitions to be disabled where supported.  Note this is an inheritable property.
        /// </summary>
        public static bool GetDisableTransitions(DependencyObject element) => (bool)element.GetValue(DisableTransitionsProperty);

        /// <summary>
        /// Allows transitions to be disabled where supported.  Note this is an inheritable property.
        /// </summary>
        public static void SetDisableTransitions(DependencyObject element, bool value) => element.SetValue(DisableTransitionsProperty, value);

        #endregion DisableTransitions
    }
}
