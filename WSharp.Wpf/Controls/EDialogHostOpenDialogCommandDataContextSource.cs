namespace WSharp.Wpf.Controls
{
    public enum EDialogHostOpenDialogCommandDataContextSource
    {
        /// <summary>
        ///     The data context from the sender element (typically a <see cref="Button"/>) is
        ///     applied to the content.
        /// </summary>
        SenderElement,

        /// <summary>
        ///     The data context from the <see cref="DialogHost"/> is applied to the content.
        /// </summary>
        DialogHostInstance,

        /// <summary>The data context is explicitly set to <c>null</c>.</summary>
        None
    }
}
