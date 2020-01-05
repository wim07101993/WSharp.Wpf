namespace WSharp.Wpf.Controls
{
    /// <summary>
    ///     Defines how the <see cref="PopupBox"/> popup is aligned to the toggle part of the control.
    /// </summary>
    public enum EPopupBoxPlacementMode
    {
        /// <summary>Display the popup below the toggle, and align the left edges.3</summary>
        BottomAndAlignLeftEdges,

        /// <summary>Display the popup below the toggle, and align the right edges.</summary>
        BottomAndAlignRightEdges,

        /// <summary>
        ///     Display the popup below the toggle, and align the center of the popup with the
        ///     center of the toggle.
        /// </summary>
        BottomAndAlignCentres,

        /// <summary>Display the popup above the toggle, and align the left edges.</summary>
        TopAndAlignLeftEdges,

        /// <summary>Display the popup above the toggle, and align the right edges.</summary>
        TopAndAlignRightEdges,

        /// <summary>
        ///     Display the popup above the toggle, and align the center of the popup with the
        ///     center of the toggle.
        /// </summary>
        TopAndAlignCentres,

        /// <summary>Display the popup to the left of the toggle, and align the top edges.</summary>
        LeftAndAlignTopEdges,

        /// <summary>Display the popup to the left of the toggle, and align the bottom edges.</summary>
        LeftAndAlignBottomEdges,

        /// <summary>Display the popup to the left of the toggle, and align the middles.</summary>
        LeftAndAlignMiddles,

        /// <summary>Display the popup to the right of the toggle, and align the top edges.</summary>
        RightAndAlignTopEdges,

        /// <summary>Display the popup to the right of the toggle, and align the bottom edges.</summary>
        RightAndAlignBottomEdges,

        /// <summary>Display the popup to the right of the toggle, and align the middles.</summary>
        RightAndAlignMiddles,
    }
}
