using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using WSharp.Wpf.Extensions;

namespace WSharp.Wpf.Controls
{
    /// <summary>A card is a content control, styled according to Material Design guidelines.</summary>
    [TemplatePart(Name = ClipBorderPartName, Type = typeof(Border))]
    public class ClippedContentControl : ContentControl
    {
        public const string ClipBorderPartName = "PART_ClipBorder";
        protected Border partClipBorder;

        public static readonly DependencyProperty UniformCornerRadiusProperty = DependencyProperty.Register(
            nameof(UniformCornerRadius),
            typeof(double),
            typeof(ClippedContentControl),
            new FrameworkPropertyMetadata(2.0, FrameworkPropertyMetadataOptions.AffectsMeasure));

        private static readonly DependencyPropertyKey contentClipPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(ContentClip),
            typeof(Geometry),
            typeof(ClippedContentControl),
            new PropertyMetadata(default(Geometry)));

        public static readonly DependencyProperty ContentClipProperty = contentClipPropertyKey.DependencyProperty;

        static ClippedContentControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ClippedContentControl), new FrameworkPropertyMetadata(typeof(ClippedContentControl)));
        }

        public double UniformCornerRadius
        {
            get => (double)GetValue(UniformCornerRadiusProperty);
            set => SetValue(UniformCornerRadiusProperty, value);
        }

        public Geometry ContentClip
        {
            get => (Geometry)GetValue(ContentClipProperty);
            private set => SetValue(contentClipPropertyKey, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            partClipBorder = this.GetTemplateChild<Border>(ClipBorderPartName);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            if (partClipBorder == null)
                return;

            var farPoint = new Point(Math.Max(0, partClipBorder.ActualWidth), Math.Max(0, partClipBorder.ActualHeight));
            var clipRect = new Rect(new Point(), new Point(farPoint.X, farPoint.Y));

            ContentClip = new RectangleGeometry(clipRect, UniformCornerRadius, UniformCornerRadius);
        }
    }
}
