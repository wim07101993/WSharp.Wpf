using System;

namespace WSharp.Wpf.Controls
{
    /// <summary>
    /// Enum NumericInput which indicates what input is allowed for NumericUpdDown.
    /// </summary>
    [Flags]
    public enum NumericInput
    {
        /// <summary>
        /// Only numbers are allowed
        /// </summary>
        Numbers = 0b0001,
        /// <summary>
        /// Numbers with decimal point and allowed scientific input
        /// </summary>
        Decimal = 0b0010,
        /// <summary>
        /// All is allowed
        /// </summary>
        All = Numbers | Decimal
    }
}
