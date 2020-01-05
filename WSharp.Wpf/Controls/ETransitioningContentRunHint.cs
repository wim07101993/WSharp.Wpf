using System;

namespace WSharp.Wpf.Controls
{
    [Flags]
    public enum ETransitioningContentRunHint
    {
        Loaded = 1,
        IsVisibleChanged = 2,
        All = Loaded | IsVisibleChanged
    }
}
