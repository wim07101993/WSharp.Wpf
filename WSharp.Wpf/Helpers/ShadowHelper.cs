using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace WSharp.Wpf.Helpers
{
    public static class ShadowHelper
    {
        #region shadow depth

        public static readonly DependencyProperty ShadowDepthProperty = DependencyProperty.RegisterAttached(
            "ShadowDepth",
            typeof(EShadowDepth),
            typeof(ShadowHelper),
            new FrameworkPropertyMetadata(default(EShadowDepth), FrameworkPropertyMetadataOptions.AffectsRender));

        public static EShadowDepth GetShadowDepth(DependencyObject element) => (EShadowDepth)element.GetValue(ShadowDepthProperty);

        public static void SetShadowDepth(DependencyObject element, EShadowDepth value) => element.SetValue(ShadowDepthProperty, value);

        #endregion shadow depth

        #region local info

        private static readonly DependencyPropertyKey localInfoPropertyKey = DependencyProperty.RegisterAttachedReadOnly(
            "LocalInfo",
            typeof(ShadowLocalInfo),
            typeof(ShadowHelper),
            new PropertyMetadata(default(ShadowLocalInfo)));

        private static ShadowLocalInfo GetLocalInfo(DependencyObject element) => (ShadowLocalInfo)element.GetValue(localInfoPropertyKey.DependencyProperty);

        private static void SetLocalInfo(DependencyObject element, ShadowLocalInfo value) => element.SetValue(localInfoPropertyKey, value);

        #endregion local info

        #region darken

        public static readonly DependencyProperty DarkenProperty = DependencyProperty.RegisterAttached(
            "Darken",
            typeof(bool),
            typeof(ShadowHelper),
            new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.AffectsRender, DarkenPropertyChangedCallback));

        public static bool GetDarken(DependencyObject element) => (bool)element.GetValue(DarkenProperty);

        public static void SetDarken(DependencyObject element, bool value) => element.SetValue(DarkenProperty, value);

        private static void DarkenPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var uiElement = dependencyObject as UIElement;
            if (!(uiElement?.Effect is DropShadowEffect dropShadowEffect))
                return;

            double toValue;
            if ((bool)dependencyPropertyChangedEventArgs.NewValue)
            {
                SetLocalInfo(dependencyObject, new ShadowLocalInfo(dropShadowEffect.Opacity));
                toValue = 1;
            }
            else
            {
                var shadowLocalInfo = GetLocalInfo(dependencyObject);
                if (shadowLocalInfo == null)
                    return;

                toValue = shadowLocalInfo.StandardOpacity;
            }

            var doubleAnimation = new DoubleAnimation(toValue, new Duration(TimeSpan.FromMilliseconds(350)))
            {
                FillBehavior = FillBehavior.HoldEnd
            };

            dropShadowEffect.BeginAnimation(DropShadowEffect.OpacityProperty, doubleAnimation);
        }

        #endregion darken

        #region cache mode

        public static readonly DependencyProperty CacheModeProperty = DependencyProperty.RegisterAttached(
            "CacheMode", 
            typeof(CacheMode), 
            typeof(ShadowHelper), 
            new FrameworkPropertyMetadata(new BitmapCache { EnableClearType = true, SnapsToDevicePixels = true }, FrameworkPropertyMetadataOptions.Inherits));

        public static CacheMode GetCacheMode(DependencyObject element) => (CacheMode)element.GetValue(CacheModeProperty);

        public static void SetCacheMode(DependencyObject element, CacheMode value) => element.SetValue(CacheModeProperty, value);

        #endregion cache mode

        #region shadow edges

        public static readonly DependencyProperty ShadowEdgesProperty = DependencyProperty.RegisterAttached(
            "ShadowEdges", 
            typeof(EShadowEdges), 
            typeof(ShadowHelper), 
            new PropertyMetadata(EShadowEdges.All));

        public static EShadowEdges GetShadowEdges(DependencyObject element) => (EShadowEdges)element.GetValue(ShadowEdgesProperty);

        public static void SetShadowEdges(DependencyObject element, EShadowEdges value) => element.SetValue(ShadowEdgesProperty, value);

        #endregion shadow edges
    }
}
