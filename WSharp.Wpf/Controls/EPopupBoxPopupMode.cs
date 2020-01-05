namespace WSharp.Wpf.Controls
{
    /// <summary>Defines what causes the <see cref="PopupBox"/> to open it's popup.</summary>
    public enum EPopupBoxPopupMode
    {
        /// <summary>Open when the toggle button is clicked.</summary>
        Click,

        /// <summary>Open when the mouse goes over the toggle button.</summary>
        MouseOver,

        /// <summary>
        ///     Open when the mouse goes over the toggle button, or the space in which the popup box
        ///     would occupy should it be open.
        /// </summary>
        MouseOverEager
    }
}
