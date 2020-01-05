using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

using WSharp.Wpf.Extensions;
using WSharp.Wpf.Helpers;

namespace WSharp.Wpf.Controls
{
    [TemplateVisualState(GroupName = "CommonStates", Name = TemplateStateNormal)]
    [TemplateVisualState(GroupName = "CommonStates", Name = TemplateStateMousePressed)]
    [TemplateVisualState(GroupName = "CommonStates", Name = TemplateStateMouseOut)]
    public class Ripple : ContentControl
    {
        public const string TemplateStateNormal = "Normal";
        public const string TemplateStateMousePressed = "MousePressed";
        public const string TemplateStateMouseOut = "MouseOut";

        private static readonly HashSet<Ripple> pressedInstances = new HashSet<Ripple>();

        #region DEPENDENCY PROPERTIES

        public static readonly DependencyProperty FeedbackProperty = DependencyProperty.Register(
            nameof(Feedback),
            typeof(Brush),
            typeof(Ripple),
            new PropertyMetadata(default(Brush)));

        private static readonly DependencyPropertyKey rippleSizePropertyKey = DependencyProperty.RegisterReadOnly(
                "RippleSize",
                typeof(double),
                typeof(Ripple),
                new PropertyMetadata(default(double)));

        public static readonly DependencyProperty RippleSizeProperty = rippleSizePropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey rippleXPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "RippleX",
                typeof(double),
                typeof(Ripple),
                new PropertyMetadata(default(double)));

        public static readonly DependencyProperty RippleXProperty = rippleXPropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey rippleYPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "RippleY",
                typeof(double),
                typeof(Ripple),
                new PropertyMetadata(default(double)));

        public static readonly DependencyProperty RippleYProperty = rippleYPropertyKey.DependencyProperty;

        /// <summary>
        ///     The DependencyProperty for the RecognizesAccessKey property. Default Value: false
        /// </summary>
        public static readonly DependencyProperty RecognizesAccessKeyProperty = DependencyProperty.Register(
            nameof(RecognizesAccessKey),
            typeof(bool),
            typeof(Ripple),
            new PropertyMetadata(default(bool)));

        #endregion DEPENDENCY PROPERTIES

        static Ripple()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Ripple), new FrameworkPropertyMetadata(typeof(Ripple)));

            EventManager.RegisterClassHandler(typeof(ContentControl), Mouse.PreviewMouseUpEvent, new MouseButtonEventHandler(MouseButtonEventHandler), true);
            EventManager.RegisterClassHandler(typeof(ContentControl), Mouse.MouseMoveEvent, new MouseEventHandler(MouseMoveEventHandler), true);
            EventManager.RegisterClassHandler(typeof(Popup), Mouse.PreviewMouseUpEvent, new MouseButtonEventHandler(MouseButtonEventHandler), true);
            EventManager.RegisterClassHandler(typeof(Popup), Mouse.MouseMoveEvent, new MouseEventHandler(MouseMoveEventHandler), true);
        }

        public Ripple()
        {
            SizeChanged += OnSizeChanged;
        }

        #region PROPERTIES

        public Brush Feedback
        {
            get => (Brush)GetValue(FeedbackProperty);
            set => SetValue(FeedbackProperty, value);
        }

        public double RippleSize
        {
            get => (double)GetValue(RippleSizeProperty);
            private set => SetValue(rippleSizePropertyKey, value);
        }

        public double RippleX
        {
            get => (double)GetValue(RippleXProperty);
            private set => SetValue(rippleXPropertyKey, value);
        }

        public double RippleY
        {
            get => (double)GetValue(RippleYProperty);
            private set => SetValue(rippleYPropertyKey, value);
        }

        /// <summary>Determine if Ripple should use AccessText in its style</summary>
        public bool RecognizesAccessKey
        {
            get => (bool)GetValue(RecognizesAccessKeyProperty);
            set => SetValue(RecognizesAccessKeyProperty, value);
        }

        #endregion PROPERTIES

        #region METHODS

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (RippleHelper.GetIsCentered(this))
            {
                if (Content is FrameworkElement innerContent)
                {
                    var position = innerContent.TransformToAncestor(this)
                        .Transform(new Point(0, 0));

                    RippleX = FlowDirection == FlowDirection.RightToLeft
                        ? position.X - (innerContent.ActualWidth / 2) - (RippleSize / 2)
                        : position.X + (innerContent.ActualWidth / 2) - (RippleSize / 2);

                    RippleY = position.Y + (innerContent.ActualHeight / 2) - (RippleSize / 2);
                }
                else
                {
                    RippleX = (ActualWidth / 2) - (RippleSize / 2);
                    RippleY = (ActualHeight / 2) - (RippleSize / 2);
                }
            }
            else
            {
                var point = e.GetPosition(this);
                RippleX = point.X - (RippleSize / 2);
                RippleY = point.Y - (RippleSize / 2);
            }

            if (!RippleHelper.GetIsDisabled(this))
            {
                _ = VisualStateManager.GoToState(this, TemplateStateNormal, false);
                _ = VisualStateManager.GoToState(this, TemplateStateMousePressed, true);
                _ = pressedInstances.Add(this);
            }

            base.OnPreviewMouseLeftButtonDown(e);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _ = VisualStateManager.GoToState(this, TemplateStateNormal, false);
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            if (!(Content is FrameworkElement innerContent))
                return;

            double width, height;

            if (RippleHelper.GetIsCentered(this) && innerContent != null)
            {
                width = innerContent.ActualWidth;
                height = innerContent.ActualHeight;
            }
            else
            {
                width = sizeChangedEventArgs.NewSize.Width;
                height = sizeChangedEventArgs.NewSize.Height;
            }

            var radius = Math.Sqrt(Math.Pow(width, 2) + Math.Pow(height, 2));

            RippleSize = 2 * radius * RippleHelper.GetRippleSizeMultiplier(this);
        }

        private static void MouseButtonEventHandler(object sender, MouseButtonEventArgs e)
        {
            foreach (var ripple in pressedInstances)
            {
                // adjust the transition scale time according to the current animated scale
                if (ripple.TryGetTemplateChild<ScaleTransform>("ScaleTransform", out var scaleTrans))
                {
                    var currentScale = scaleTrans.ScaleX;
                    var newTime = TimeSpan.FromMilliseconds(300 * (1.0 - currentScale));

                    // change the scale animation according to the current scale
                    if (ripple.TryGetTemplateChild<EasingDoubleKeyFrame>("MousePressedToNormalScaleXKeyFrame", out var scaleXKeyFrame))
                        scaleXKeyFrame.KeyTime = KeyTime.FromTimeSpan(newTime);

                    if (ripple.TryGetTemplateChild<EasingDoubleKeyFrame>("MousePressedToNormalScaleYKeyFrame", out var scaleYKeyFrame))
                        scaleYKeyFrame.KeyTime = KeyTime.FromTimeSpan(newTime);
                }

                _ = VisualStateManager.GoToState(ripple, TemplateStateNormal, true);
            }
            pressedInstances.Clear();
        }

        private static void MouseMoveEventHandler(object sender, MouseEventArgs e)
        {
            foreach (var ripple in pressedInstances.ToList())
            {
                var relativePosition = Mouse.GetPosition(ripple);
                if (relativePosition.X < 0
                    || relativePosition.Y < 0
                    || relativePosition.X >= ripple.ActualWidth
                    || relativePosition.Y >= ripple.ActualHeight)

                {
                    _ = VisualStateManager.GoToState(ripple, TemplateStateMouseOut, true);
                    _ = pressedInstances.Remove(ripple);
                }
            }
        }

        #endregion METHODS
    }
}
