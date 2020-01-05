﻿using System.Windows;
using System.Windows.Controls.Primitives;

namespace WSharp.Wpf.Helpers
{
    public static class ToolTipHelper
    {
        public static CustomPopupPlacementCallback CustomPopupPlacementCallback => CustomPopupPlacementCallbackImpl;

        public static CustomPopupPlacement[] CustomPopupPlacementCallbackImpl(Size popupSize, Size targetSize, Point offset)
        {
            return new []
            {
                new CustomPopupPlacement(new Point(targetSize.Width/2 - popupSize.Width/2, targetSize.Height + 14), PopupPrimaryAxis.Horizontal) 
            };
        }
    }
}
