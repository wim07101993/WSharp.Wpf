using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;

using WSharp.Wpf.Extensions;

namespace WSharp.Wpf.Controls
{
    /// <summary>
    ///     Popup box, similar to a <see cref="ComboBox"/>, but allows more customizable content.
    /// </summary>
    [TemplatePart(Name = PopupPartName, Type = typeof(Popup))]
    [TemplatePart(Name = PopupContentControlPartName, Type = typeof(ContentControl))]
    [TemplatePart(Name = TogglePartName, Type = typeof(ToggleButton))]
    [TemplateVisualState(GroupName = "PopupStates", Name = PopupIsOpenStateName)]
    [TemplateVisualState(GroupName = "PopupStates", Name = PopupIsClosedStateName)]
    [ContentProperty("PopupContent")]
    public class PopupBox : ContentControl
    {
        #region FIELDS

        public const string PopupPartName = "PART_Popup";
        public const string TogglePartName = "PART_Toggle";
        public const string PopupContentControlPartName = "PART_PopupContentControl";
        public const string PopupIsOpenStateName = "IsOpen";
        public const string PopupIsClosedStateName = "IsClosed";

        /// <summary>Routed command to be used inside of a popup content to close it.</summary>
        public static RoutedCommand ClosePopupCommand = new RoutedCommand();

        private PopupEx _popup;
        private ContentControl _popupContentControl;
        private ToggleButton _toggleButton;
        private Point _popupPointFromLastRequest;
        private Point _lastRelativePosition;

        #endregion FIELDS

        /// <summary>Event raised when the checked toggled content (if set) is clicked.</summary>
        public static readonly RoutedEvent ToggleCheckedContentClickEvent = EventManager.RegisterRoutedEvent(
            nameof(ToggleCheckedContentClick),
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(PopupBox));

        public static readonly RoutedEvent OpenedEvent = EventManager.RegisterRoutedEvent(
             nameof(Opened),
            RoutingStrategy.Bubble,
            typeof(EventHandler),
            typeof(PopupBox));

        public static readonly RoutedEvent ClosedEvent = EventManager.RegisterRoutedEvent(
            nameof(Closed),
            RoutingStrategy.Bubble,
            typeof(EventHandler),
            typeof(PopupBox));

        #region DEPENDENCY PROPERTIES

        public static readonly DependencyProperty ToggleContentProperty = DependencyProperty.Register(
            nameof(ToggleContent),
            typeof(object),
            typeof(PopupBox),
            new PropertyMetadata(default(object)));

        public static readonly DependencyProperty ToggleContentTemplateProperty = DependencyProperty.Register(
            nameof(ToggleContentTemplate),
            typeof(DataTemplate),
            typeof(PopupBox),
            new PropertyMetadata(default(DataTemplate)));

        public static readonly DependencyProperty ToggleCheckedContentProperty = DependencyProperty.Register(
            nameof(ToggleCheckedContent),
            typeof(object),
            typeof(PopupBox),
            new PropertyMetadata(default(object)));

        public static readonly DependencyProperty ToggleCheckedContentTemplateProperty = DependencyProperty.Register(
            nameof(ToggleCheckedContentTemplate),
            typeof(DataTemplate),
            typeof(PopupBox),
            new PropertyMetadata(default(DataTemplate)));

        public static readonly DependencyProperty ToggleCheckedContentCommandProperty = DependencyProperty.Register(
            nameof(ToggleCheckedContentCommand),
            typeof(ICommand),
            typeof(PopupBox),
            new PropertyMetadata(default(ICommand)));

        public static readonly DependencyProperty ToggleCheckedContentCommandParameterProperty = DependencyProperty.Register(
            nameof(ToggleCheckedContentCommandParameter),
            typeof(object),
            typeof(PopupBox),
            new PropertyMetadata(default(object)));

        public static readonly DependencyProperty PopupContentProperty = DependencyProperty.Register(
            nameof(PopupContent),
            typeof(object),
            typeof(PopupBox),
            new PropertyMetadata(default(object)));

        public static readonly DependencyProperty PopupContentTemplateProperty = DependencyProperty.Register(
            nameof(PopupContentTemplate),
            typeof(DataTemplate),
            typeof(PopupBox),
            new PropertyMetadata(default(DataTemplate)));

        public static readonly DependencyProperty IsPopupOpenProperty = DependencyProperty.Register(
            nameof(IsPopupOpen),
            typeof(bool),
            typeof(PopupBox),
            new FrameworkPropertyMetadata(default(bool), IsPopupOpenPropertyChangedCallback));

        public static readonly DependencyProperty StaysOpenProperty = DependencyProperty.Register(
            nameof(StaysOpen),
            typeof(bool),
            typeof(PopupBox),
            new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty PlacementModeProperty = DependencyProperty.Register(
            nameof(PlacementMode),
            typeof(EPopupBoxPlacementMode),
            typeof(PopupBox),
            new PropertyMetadata(default(EPopupBoxPlacementMode), PlacementModePropertyChangedCallback));

        public static readonly DependencyProperty PopupModeProperty = DependencyProperty.Register(
            nameof(PopupMode),
            typeof(EPopupBoxPopupMode),
            typeof(PopupBox),
            new PropertyMetadata(default(EPopupBoxPopupMode)));

        /// <summary>
        ///     Get or sets how to unfurl controls when opening the popups. Only child elements of
        ///     type <see cref="ButtonBase"/> are animated.
        /// </summary>
        public static readonly DependencyProperty UnfurlOrientationProperty = DependencyProperty.Register(
            nameof(UnfurlOrientation),
            typeof(Orientation),
            typeof(PopupBox),
            new PropertyMetadata(Orientation.Vertical));

        /// <summary>Get or sets the popup horizontal offset in relation to the button.</summary>
        public static readonly DependencyProperty PopupHorizontalOffsetProperty = DependencyProperty.Register(
            nameof(PopupHorizontalOffset),
            typeof(double),
            typeof(PopupBox),
            new PropertyMetadata(default(double)));

        /// <summary>Get or sets the popup vertical offset in relation to the button.</summary>
        public static readonly DependencyProperty PopupVerticalOffsetProperty = DependencyProperty.Register(
            nameof(PopupVerticalOffset),
            typeof(double),
            typeof(PopupBox),
            new PropertyMetadata(default(double)));

        #endregion DEPENDENCY PROPERTIES

        static PopupBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PopupBox), new FrameworkPropertyMetadata(typeof(PopupBox)));
            ToolTipService.IsEnabledProperty.OverrideMetadata(typeof(PopupBox), new FrameworkPropertyMetadata(null, CoerceToolTipIsEnabled));
            EventManager.RegisterClassHandler(typeof(PopupBox), Mouse.LostMouseCaptureEvent, new MouseEventHandler(OnLostMouseCapture));
            EventManager.RegisterClassHandler(typeof(PopupBox), Mouse.MouseDownEvent, new MouseButtonEventHandler(OnMouseButtonDown), true);
        }

        public PopupBox()
        {
            LayoutUpdated += OnLayoutUpdated;
        }

        #region PROPERTIES

        /// <summary>Content to display in the toggle button.</summary>
        public object ToggleContent
        {
            get => GetValue(ToggleContentProperty);
            set => SetValue(ToggleContentProperty, value);
        }

        /// <summary>Template for <see cref="ToggleContent"/>.</summary>
        public DataTemplate ToggleContentTemplate
        {
            get => (DataTemplate)GetValue(ToggleContentTemplateProperty);
            set => SetValue(ToggleContentTemplateProperty, value);
        }

        /// <summary>
        ///     Content to display in the toggle when it's checked (when the popup is open).
        ///     Optional; if not provided the <see cref="ToggleContent"/> is used.
        /// </summary>
        public object ToggleCheckedContent
        {
            get => GetValue(ToggleCheckedContentProperty);
            set => SetValue(ToggleCheckedContentProperty, value);
        }

        /// <summary>Template for <see cref="ToggleCheckedContent"/>.</summary>
        public DataTemplate ToggleCheckedContentTemplate
        {
            get => (DataTemplate)GetValue(ToggleCheckedContentTemplateProperty);
            set => SetValue(ToggleCheckedContentTemplateProperty, value);
        }

        /// <summary>
        ///     Command to execute if toggle is checked (popup is open) and
        ///     <see cref="ToggleCheckedContent"/> is set.
        /// </summary>
        public ICommand ToggleCheckedContentCommand
        {
            get => (ICommand)GetValue(ToggleCheckedContentCommandProperty);
            set => SetValue(ToggleCheckedContentCommandProperty, value);
        }

        /// <summary>Command parameter to use in conjunction with <see cref="ToggleCheckedContentCommand"/>.</summary>
        public object ToggleCheckedContentCommandParameter
        {
            get => GetValue(ToggleCheckedContentCommandParameterProperty);
            set => SetValue(ToggleCheckedContentCommandParameterProperty, value);
        }

        /// <summary>Content to display in the content.</summary>
        public object PopupContent
        {
            get => GetValue(PopupContentProperty);
            set => SetValue(PopupContentProperty, value);
        }

        /// <summary>Popup content template.</summary>
        public DataTemplate PopupContentTemplate
        {
            get => (DataTemplate)GetValue(PopupContentTemplateProperty);
            set => SetValue(PopupContentTemplateProperty, value);
        }

        /// <summary>Gets or sets whether the popup is currently open.</summary>
        public bool IsPopupOpen
        {
            get => (bool)GetValue(IsPopupOpenProperty);
            set => SetValue(IsPopupOpenProperty, value);
        }

        /// <summary>
        ///     Indicates of the popup should stay open if a click occurs inside the popup.
        /// </summary>
        public bool StaysOpen
        {
            get => (bool)GetValue(StaysOpenProperty);
            set => SetValue(StaysOpenProperty, value);
        }

        /// <summary>Gets or sets how the popup is aligned in relation to the toggle.</summary>
        public EPopupBoxPlacementMode PlacementMode
        {
            get => (EPopupBoxPlacementMode)GetValue(PlacementModeProperty);
            set => SetValue(PlacementModeProperty, value);
        }

        /// <summary>Gets or sets what trigger causes the popup to open.</summary>
        public EPopupBoxPopupMode PopupMode
        {
            get => (EPopupBoxPopupMode)GetValue(PopupModeProperty);
            set => SetValue(PopupModeProperty, value);
        }

        /// <summary>
        ///     Gets or sets how to unfurl controls when opening the popups. Only child elements of
        ///     type <see cref="ButtonBase"/> are animated.
        /// </summary>
        public Orientation UnfurlOrientation
        {
            get => (Orientation)GetValue(UnfurlOrientationProperty);
            set => SetValue(UnfurlOrientationProperty, value);
        }

        /// <summary>Get or sets the popup horizontal offset in relation to the button.</summary>
        public double PopupHorizontalOffset
        {
            get => (double)GetValue(PopupHorizontalOffsetProperty);
            set => SetValue(PopupHorizontalOffsetProperty, value);
        }

        /// <summary>Get or sets the popup vertical offset in relation to the button.</summary>
        public double PopupVerticalOffset
        {
            get => (double)GetValue(PopupVerticalOffsetProperty);
            set => SetValue(PopupVerticalOffsetProperty, value);
        }

        /// <summary>Framework use. Provides the method used to position the popup.</summary>
        public CustomPopupPlacementCallback PopupPlacementMethod => GetPopupPlacement;

        #endregion PROPERTIES

        #region METHODS

        public override void OnApplyTemplate()
        {
            if (_toggleButton != null)
                _toggleButton.PreviewMouseLeftButtonUp -= ToggleButtonOnPreviewMouseLeftButtonUp;

            base.OnApplyTemplate();

            _popup = this.GetTemplateChild<PopupEx>(PopupPartName);
            _popupContentControl = this.GetTemplateChild<ContentControl>(PopupContentControlPartName);
            _toggleButton = this.GetTemplateChild<ToggleButton>(TogglePartName);

            _ = _popup?.CommandBindings.Add(new CommandBinding(ClosePopupCommand, ClosePopupHandler));

            if (_toggleButton != null)
                _toggleButton.PreviewMouseLeftButtonUp += ToggleButtonOnPreviewMouseLeftButtonUp;

            _ = VisualStateManager.GoToState(this, IsPopupOpen ? PopupIsOpenStateName : PopupIsClosedStateName, false);
        }

        protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsKeyboardFocusWithinChanged(e);

            if (IsPopupOpen && !IsKeyboardFocusWithin && !StaysOpen)
                Close();
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (IsEnabled && IsLoaded &&
                (PopupMode == EPopupBoxPopupMode.MouseOverEager || PopupMode == EPopupBoxPopupMode.MouseOver))
            {
                if (_popupContentControl != null)
                {
                    //if the invisible popup that is watching the mouse, isn't where we expected it to be
                    //then the main popup toggle has been moved off screen...so we shouldn't show the popup content
                    var inputSource = PresentationSource.FromVisual(_popupContentControl);
                    if (inputSource != null)
                    {
                        var popupScreenPoint = _popupContentControl.PointToScreen(new Point());
                        popupScreenPoint.Offset(-_popupContentControl.Margin.Left, -_popupContentControl.Margin.Top);
                        var expectedPopupScreenPoint = PointToScreen(_popupPointFromLastRequest);

                        if (Math.Abs(popupScreenPoint.X - expectedPopupScreenPoint.X) > ActualWidth / 3 ||
                            Math.Abs(popupScreenPoint.Y - expectedPopupScreenPoint.Y) > ActualHeight / 3)
                            return;
                    }
                }

                SetCurrentValue(IsPopupOpenProperty, true);
            }
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (PopupMode == EPopupBoxPopupMode.MouseOverEager || PopupMode == EPopupBoxPopupMode.MouseOver)
                Close();

            base.OnMouseLeave(e);
        }

        /// <summary>Raises <see cref="ToggleCheckedContentClickEvent"/>.</summary>
        protected virtual void OnToggleCheckedContentClick()
            => RaiseEvent(new RoutedEventArgs(ToggleCheckedContentClickEvent, this));

        /// <summary>Raises <see cref="OpenedEvent"/>.</summary>
        protected virtual void OnOpened() => RaiseEvent(new RoutedEventArgs(OpenedEvent, this));

        /// <summary>Raises <see cref="ClosedEvent"/>.</summary>
        protected virtual void OnClosed() => RaiseEvent(new RoutedEventArgs(ClosedEvent, this));

        protected void Close()
        {
            if (IsPopupOpen)
                SetCurrentValue(IsPopupOpenProperty, false);
        }

        private void ClosePopupHandler(object sender, ExecutedRoutedEventArgs e) => IsPopupOpen = false;

        private void OnLayoutUpdated(object sender, EventArgs e)
        {
            if (_popupContentControl == null || _popup == null ||
                (PopupMode != EPopupBoxPopupMode.MouseOver && PopupMode != EPopupBoxPopupMode.MouseOverEager))
                return;

            var relativePosition = _popupContentControl.TranslatePoint(new Point(), this);
            if (relativePosition != _lastRelativePosition)
            {
                _popup.RefreshPosition();
                _lastRelativePosition = _popupContentControl.TranslatePoint(new Point(), this);
            }
        }

        private CustomPopupPlacement[] GetPopupPlacement(Size popupSize, Size targetSize, Point offset)
        {
            if (FlowDirection == FlowDirection.RightToLeft)
                offset.X += targetSize.Width / 2;

            var (x, y) = PlacementMode switch
            {
                EPopupBoxPlacementMode.BottomAndAlignLeftEdges => (
                    0 - Math.Abs(offset.X * 3),
                    targetSize.Height - Math.Abs(offset.Y)),

                EPopupBoxPlacementMode.BottomAndAlignRightEdges => (
                    0 - popupSize.Width + targetSize.Width - offset.X,
                    targetSize.Height - Math.Abs(offset.Y)),

                EPopupBoxPlacementMode.BottomAndAlignCentres => (
                    (targetSize.Width / 2) - (popupSize.Width / 2) - Math.Abs(offset.X * 2),
                    targetSize.Height - Math.Abs(offset.Y)),

                EPopupBoxPlacementMode.TopAndAlignLeftEdges => (
                    0 - Math.Abs(offset.X * 3),
                    0 - popupSize.Height - Math.Abs(offset.Y * 2)),

                EPopupBoxPlacementMode.TopAndAlignRightEdges => (
                    0 - popupSize.Width + targetSize.Width - offset.X,
                    0 - popupSize.Height - Math.Abs(offset.Y * 2)),

                EPopupBoxPlacementMode.TopAndAlignCentres => (
                    (targetSize.Width / 2) - (popupSize.Width / 2) - Math.Abs(offset.X * 2),
                    0 - popupSize.Height - Math.Abs(offset.Y * 2)),

                EPopupBoxPlacementMode.LeftAndAlignTopEdges => (
                     0 - popupSize.Width - Math.Abs(offset.X * 2),
                    0 - Math.Abs(offset.Y * 3)),

                EPopupBoxPlacementMode.LeftAndAlignBottomEdges => (
                    0 - popupSize.Width - Math.Abs(offset.X * 2),
                    0 - (popupSize.Height - targetSize.Height)),

                EPopupBoxPlacementMode.LeftAndAlignMiddles => (
                    0 - popupSize.Width - Math.Abs(offset.X * 2),
                    (targetSize.Height / 2) - (popupSize.Height / 2) - Math.Abs(offset.Y * 2)),

                EPopupBoxPlacementMode.RightAndAlignTopEdges => (
                    targetSize.Width,
                    0 - Math.Abs(offset.X * 3)),

                EPopupBoxPlacementMode.RightAndAlignBottomEdges => (
                    targetSize.Width,
                    0 - (popupSize.Height - targetSize.Height)),

                EPopupBoxPlacementMode.RightAndAlignMiddles => (
                    targetSize.Width,
                    (targetSize.Height / 2) - (popupSize.Height / 2) - Math.Abs(offset.Y * 2)),

                _ => throw new ArgumentOutOfRangeException()
            };

            _popupPointFromLastRequest = new Point(x, y);
            return new[] { new CustomPopupPlacement(_popupPointFromLastRequest, PopupPrimaryAxis.Horizontal) };
        }

        private void AnimateChildrenIn(bool reverse)
        {
            if (_popupContentControl == null) 
                return;
            if (VisualTreeHelper.GetChildrenCount(_popupContentControl) != 1) 
                return;

            if (!(VisualTreeHelper.GetChild(_popupContentControl, 0) is ContentPresenter contentPresenter))
                return;

            var controls = contentPresenter.VisualDepthFirstTraversal().OfType<ButtonBase>();
            double translateCoordinateFrom;
            if (PlacementMode == EPopupBoxPlacementMode.TopAndAlignCentres || 
                PlacementMode == EPopupBoxPlacementMode.TopAndAlignLeftEdges || 
                PlacementMode == EPopupBoxPlacementMode.TopAndAlignRightEdges ||
                PlacementMode == EPopupBoxPlacementMode.LeftAndAlignBottomEdges ||
                PlacementMode == EPopupBoxPlacementMode.RightAndAlignBottomEdges ||
                (UnfurlOrientation == Orientation.Horizontal &&
                    (
                        PlacementMode == EPopupBoxPlacementMode.LeftAndAlignBottomEdges ||
                        PlacementMode == EPopupBoxPlacementMode.LeftAndAlignMiddles ||
                        PlacementMode == EPopupBoxPlacementMode.LeftAndAlignTopEdges
                    ))
                )
            {
                controls = controls.Reverse();
                translateCoordinateFrom = 80;
            }
            else
                translateCoordinateFrom = -80;

            var translateCoordinatePath =
                "(UIElement.RenderTransform).(TransformGroup.Children)[1].(TranslateTransform."
                + (UnfurlOrientation == Orientation.Horizontal ? "X)" : "Y)");

            var sineEase = new SineEase();

            var i = 0;
            foreach (var uiElement in controls)
            {
                var deferredStart = i++ * 20;
                var deferredEnd = deferredStart + 200.0;

                var absoluteZeroKeyTime = KeyTime.FromPercent(0.0);
                var deferredStartKeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(deferredStart));
                var deferredEndKeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(deferredEnd));

                var elementTranslateCoordinateFrom = translateCoordinateFrom * i;
                var translateTransform = new TranslateTransform(
                    UnfurlOrientation == Orientation.Vertical ? 0 : elementTranslateCoordinateFrom,
                    UnfurlOrientation == Orientation.Vertical ? elementTranslateCoordinateFrom : 0);

                var transformGroup = new TransformGroup
                {
                    Children = new TransformCollection(new Transform[]
                    {
                        new ScaleTransform(0, 0),
                        translateTransform
                    })
                };
                uiElement.SetCurrentValue(RenderTransformOriginProperty, new Point(.5, .5));
                uiElement.RenderTransform = transformGroup;

                var opacityAnimation = new DoubleAnimationUsingKeyFrames();
                opacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(0, absoluteZeroKeyTime, sineEase));
                opacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(0, deferredStartKeyTime, sineEase));
                opacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame((double)uiElement.GetAnimationBaseValue(OpacityProperty), deferredEndKeyTime, sineEase));
                Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("Opacity"));
                Storyboard.SetTarget(opacityAnimation, uiElement);

                var scaleXAnimation = new DoubleAnimationUsingKeyFrames();
                scaleXAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(0, absoluteZeroKeyTime, sineEase));
                scaleXAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(0, deferredStartKeyTime, sineEase));
                scaleXAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(1, deferredEndKeyTime, sineEase));
                Storyboard.SetTargetProperty(scaleXAnimation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));
                Storyboard.SetTarget(scaleXAnimation, uiElement);

                var scaleYAnimation = new DoubleAnimationUsingKeyFrames();
                scaleYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(0, absoluteZeroKeyTime, sineEase));
                scaleYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(0, deferredStartKeyTime, sineEase));
                scaleYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(1, deferredEndKeyTime, sineEase));
                Storyboard.SetTargetProperty(scaleYAnimation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                Storyboard.SetTarget(scaleYAnimation, uiElement);

                var translateCoordinateAnimation = new DoubleAnimationUsingKeyFrames();
                translateCoordinateAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(elementTranslateCoordinateFrom, absoluteZeroKeyTime, sineEase));
                translateCoordinateAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(elementTranslateCoordinateFrom, deferredStartKeyTime, sineEase));
                translateCoordinateAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(0, deferredEndKeyTime, sineEase));

                Storyboard.SetTargetProperty(translateCoordinateAnimation, new PropertyPath(translateCoordinatePath));
                Storyboard.SetTarget(translateCoordinateAnimation, uiElement);

                var storyboard = new Storyboard();

                storyboard.Children.Add(opacityAnimation);
                storyboard.Children.Add(scaleXAnimation);
                storyboard.Children.Add(scaleYAnimation);
                storyboard.Children.Add(translateCoordinateAnimation);

                if (reverse)
                {
                    storyboard.AutoReverse = true;
                    storyboard.Begin();
                    storyboard.Seek(TimeSpan.FromMilliseconds(deferredEnd));
                    storyboard.Resume();
                }
                else
                    storyboard.Begin();
            }
        }

        private static void IsPopupOpenPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is PopupBox popupBox))
                return;

            var newValue = (bool)e.NewValue;
            if (popupBox.PopupMode == EPopupBoxPopupMode.Click)
            {
                _ = newValue
                    ? Mouse.Capture(popupBox, CaptureMode.SubTree)
                    : Mouse.Capture(null);
            }

            popupBox.AnimateChildrenIn(!newValue);
            popupBox._popup?.RefreshPosition();

            _ = VisualStateManager.GoToState(popupBox, newValue ? PopupIsOpenStateName : PopupIsClosedStateName, true);

            if (newValue)
                popupBox.OnOpened();
            else
                popupBox.OnClosed();
        }

        private static void PlacementModePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is PopupBox popupBox))
                return;

            popupBox._popup?.RefreshPosition();
        }

        #region Capture

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetCapture();

        private static void OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            var popupBox = (PopupBox)sender;

            if (Equals(Mouse.Captured, popupBox)) return;

            if (Equals(e.OriginalSource, popupBox))
            {
                if (Mouse.Captured == null || popupBox._popup == null)
                {
                    if (!(Mouse.Captured as DependencyObject).IsDescendantOf(popupBox._popup))
                    {
                        popupBox.Close();
                    }
                }
            }
            else
            {
                if ((Mouse.Captured as DependencyObject).GetVisualAncestry().Contains(popupBox._popup.Child))
                {
                    // Take capture if one of our children gave up capture (by closing their drop down)
                    if (!popupBox.IsPopupOpen || Mouse.Captured != null || GetCapture() != IntPtr.Zero) return;

                    Mouse.Capture(popupBox, CaptureMode.SubTree);
                    e.Handled = true;
                }
                else
                {
                    if (popupBox.StaysOpen && popupBox.IsPopupOpen)
                    {
                        // allow scrolling
                        if (GetCapture() != IntPtr.Zero) return;

                        // Take capture back because click happend outside of control
                        Mouse.Capture(popupBox, CaptureMode.SubTree);
                        e.Handled = true;
                    }
                    else
                    {
                        popupBox.Close();
                    }
                }
            }
        }

        private static void OnMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            var popupBox = (PopupBox)sender;

            if (!popupBox.IsKeyboardFocusWithin)
            {
                popupBox.Focus();
            }

            e.Handled = true;

            if (Mouse.Captured == popupBox && e.OriginalSource == popupBox)
            {
                popupBox.Close();
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (IsPopupOpen && !StaysOpen)
            {
                Close();
                e.Handled = true;
            }
            else
                base.OnMouseLeftButtonUp(e);
        }

        #endregion Capture

        private void ToggleButtonOnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (PopupMode == EPopupBoxPopupMode.Click || !IsPopupOpen) return;

            if (ToggleCheckedContent != null)
            {
                OnToggleCheckedContentClick();

                if (ToggleCheckedContentCommand != null
                    && ToggleCheckedContentCommand.CanExecute(ToggleCheckedContentCommandParameter)
                    )
                {
                    ToggleCheckedContentCommand.Execute(ToggleCheckedContentCommandParameter);
                }
            }

            Close();
            Mouse.Capture(null);
            mouseButtonEventArgs.Handled = true;
        }

        private static object CoerceToolTipIsEnabled(DependencyObject dependencyObject, object value)
        {
            var popupBox = (PopupBox)dependencyObject;
            return popupBox.IsPopupOpen ? false : value;
        }

        #endregion METHODS

        /// <summary>Event raised when the checked toggled content (if set) is clicked.</summary>
        [Category("Behavior")]
        public event RoutedEventHandler ToggleCheckedContentClick
        {
            add { AddHandler(ToggleCheckedContentClickEvent, value); }
            remove { RemoveHandler(ToggleCheckedContentClickEvent, value); }
        }

        /// <summary>Raised when the popup is opened.</summary>
        public event RoutedEventHandler Opened
        {
            add { AddHandler(OpenedEvent, value); }
            remove { RemoveHandler(OpenedEvent, value); }
        }

        /// <summary>Raised when the popup is closed.</summary>
        public event RoutedEventHandler Closed
        {
            add { AddHandler(ClosedEvent, value); }
            remove { RemoveHandler(ClosedEvent, value); }
        }
    }
}
