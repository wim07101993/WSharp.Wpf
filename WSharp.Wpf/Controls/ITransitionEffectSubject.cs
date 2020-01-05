﻿using System;

namespace WSharp.Wpf.Controls
{
    public interface ITransitionEffectSubject
    {
        string MatrixTransformName { get; }
        string RotateTransformName { get; }
        string ScaleTransformName { get; }
        string SkewTransformName { get; }
        string TranslateTransformName { get; }
        TimeSpan Offset { get; }
    }
}
