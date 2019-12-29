using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

using WSharp.Wpf.Converters;
using WSharp.Wpf.Extensions;
using WSharp.Wpf.Helpers;

namespace WSharp.Wpf.Controls
{
    [TemplatePart(Name = HoursCanvasPartName, Type = typeof(Canvas))]
    [TemplatePart(Name = MinutesCanvasPartName, Type = typeof(Canvas))]
    [TemplatePart(Name = SecondsCanvasPartName, Type = typeof(Canvas))]
    [TemplatePart(Name = MinuteReadOutPartName, Type = typeof(TextBlock))]
    [TemplatePart(Name = HourReadOutPartName, Type = typeof(TextBlock))]
    [TemplateVisualState(GroupName = DisplayModeStates, Name = HoursVisualStateName)]
    [TemplateVisualState(GroupName = DisplayModeStates, Name = MinutesVisualStateName)]
    public class Clock : ValueControl<DateTime>
    {
        #region FIELDS

        public const string HoursCanvasPartName = "PART_HoursCanvas";
        public const string MinutesCanvasPartName = "PART_MinutesCanvas";
        public const string SecondsCanvasPartName = "PART_SecondsCanvas";
        public const string MinuteReadOutPartName = "PART_MinuteReadOut";
        public const string SecondReadOutPartName = "PART_SecondReadOut";
        public const string HourReadOutPartName = "PART_HourReadOut";

        public const string HoursVisualStateName = "Hours";
        public const string MinutesVisualStateName = "Minutes";
        public const string SecondsVisualStateName = "Seconds";

        public const string DisplayModeStates = "DisplayModeStates";

        private Point _centreCanvas = new Point(0, 0);
        private Point _currentStartPosition = new Point(0, 0);
        private TextBlock _hourReadOutPartName;
        private TextBlock _minuteReadOutPartName;
        private TextBlock _secondReadOutPartName;

        #endregion FIELDS

        public static readonly RoutedEvent ClockChoiceMadeEvent = EventManager.RegisterRoutedEvent(
                nameof(ClockChoiceMade),
                RoutingStrategy.Bubble,
                typeof(ClockChoiceMadeEventHandler),
                typeof(Clock));

        #region DEPENDENCY PROPERTIES

        private static readonly DependencyPropertyKey isMidnightHourPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(IsMidnightHour),
            typeof(bool),
            typeof(Clock),
            new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty IsMidnightHourProperty = isMidnightHourPropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey isMiddayHourPropertyKey = DependencyProperty.RegisterReadOnly(
                nameof(IsMiddayHour),
                typeof(bool),
                typeof(Clock),
                new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty IsMiddayHourProperty = isMiddayHourPropertyKey.DependencyProperty;

        public static readonly DependencyProperty IsPostMeridiemProperty = DependencyProperty.Register(
            nameof(IsPostMeridiem),
            typeof(bool),
            typeof(Clock),
            new PropertyMetadata(default(bool), IsPostMeridiemPropertyChangedCallback));

        public static readonly DependencyProperty Is24HoursProperty = DependencyProperty.Register(
            nameof(Is24Hours),
            typeof(bool),
            typeof(Clock),
            new PropertyMetadata(default(bool), Is24HoursChanged));

        public static readonly DependencyProperty DisplayModeProperty = DependencyProperty.Register(
            nameof(DisplayMode),
            typeof(EClockDisplayMode),
            typeof(Clock),
            new FrameworkPropertyMetadata(EClockDisplayMode.Hours, DisplayModePropertyChangedCallback));

        public static readonly DependencyProperty DisplayAutomationProperty = DependencyProperty.Register(
            nameof(DisplayAutomation),
            typeof(EClockDisplayAutomation),
            typeof(Clock),
            new PropertyMetadata(default(EClockDisplayAutomation)));

        public static readonly DependencyProperty ButtonStyleProperty = DependencyProperty.Register(
            nameof(ButtonStyle),
            typeof(Style),
            typeof(Clock),
            new PropertyMetadata(default(Style)));

        public static readonly DependencyProperty LesserButtonStyleProperty = DependencyProperty.Register(
            nameof(LesserButtonStyle),
            typeof(Style),
            typeof(Clock),
            new PropertyMetadata(default(Style)));

        public static readonly DependencyProperty ButtonRadiusRatioProperty = DependencyProperty.Register(
            nameof(ButtonRadiusRatio),
            typeof(double),
            typeof(Clock),
            new PropertyMetadata(default(double)));

        public static readonly DependencyProperty ButtonRadiusInnerRatioProperty = DependencyProperty.Register(
            nameof(ButtonRadiusInnerRatio),
            typeof(double),
            typeof(Clock),
            new PropertyMetadata(default(double)));

        private static readonly DependencyPropertyKey hourLineAnglePropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(HourLineAngle),
            typeof(double),
            typeof(Clock),
            new PropertyMetadata(default(double)));

        public static readonly DependencyProperty HourLineAngleProperty = hourLineAnglePropertyKey.DependencyProperty;

        #endregion DEPENDENCY PROPERTIES

        static Clock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Clock), new FrameworkPropertyMetadata(typeof(Clock)));
        }

        public Clock()
        {
            AddHandler(ClockItemButton.DragStartedEvent, new DragStartedEventHandler(ClockItemDragStartedHandler));
            AddHandler(ClockItemButton.DragDeltaEvent, new DragDeltaEventHandler(ClockItemDragDeltaHandler));
            AddHandler(ClockItemButton.DragCompletedEvent, new DragCompletedEventHandler(ClockItemDragCompletedHandler));
        }

        #region PROPERTIES

        public bool IsMidnightHour
        {
            get => (bool)GetValue(IsMidnightHourProperty);
            private set => SetValue(isMidnightHourPropertyKey, value);
        }

        public bool IsMiddayHour
        {
            get => (bool)GetValue(IsMiddayHourProperty);
            private set => SetValue(isMiddayHourPropertyKey, value);
        }

        public bool IsPostMeridiem
        {
            get => (bool)GetValue(IsPostMeridiemProperty);
            set => SetValue(IsPostMeridiemProperty, value);
        }

        public bool Is24Hours
        {
            get => (bool)GetValue(Is24HoursProperty);
            set => SetValue(Is24HoursProperty, value);
        }

        public EClockDisplayMode DisplayMode
        {
            get => (EClockDisplayMode)GetValue(DisplayModeProperty);
            set => SetValue(DisplayModeProperty, value);
        }

        public EClockDisplayAutomation DisplayAutomation
        {
            get => (EClockDisplayAutomation)GetValue(DisplayAutomationProperty);
            set => SetValue(DisplayAutomationProperty, value);
        }

        public Style ButtonStyle
        {
            get => (Style)GetValue(ButtonStyleProperty);
            set => SetValue(ButtonStyleProperty, value);
        }

        public Style LesserButtonStyle
        {
            get => (Style)GetValue(LesserButtonStyleProperty);
            set => SetValue(LesserButtonStyleProperty, value);
        }

        public double ButtonRadiusRatio
        {
            get => (double)GetValue(ButtonRadiusRatioProperty);
            set => SetValue(ButtonRadiusRatioProperty, value);
        }

        public double ButtonRadiusInnerRatio
        {
            get => (double)GetValue(ButtonRadiusInnerRatioProperty);
            set => SetValue(ButtonRadiusInnerRatioProperty, value);
        }

        public double HourLineAngle
        {
            get => (double)GetValue(HourLineAngleProperty);
            private set => SetValue(hourLineAnglePropertyKey, value);
        }

        #endregion PROPERTIES

        #region METHODS

        public override void OnApplyTemplate()
        {
            SetFlags();

            GenerateButtons();

            if (_hourReadOutPartName != null)
                _hourReadOutPartName.PreviewMouseLeftButtonDown -= HourReadOutPartNameOnPreviewMouseLeftButtonDown;
            if (_minuteReadOutPartName != null)
                _minuteReadOutPartName.PreviewMouseLeftButtonDown -= MinuteReadOutPartNameOnPreviewMouseLeftButtonDown;
            if (_secondReadOutPartName != null)
                _secondReadOutPartName.PreviewMouseLeftButtonDown -= SecondReadOutPartNameOnPreviewMouseLeftButtonDown;

            _hourReadOutPartName = this.GetTemplateChild<TextBlock>(HourReadOutPartName);
            _minuteReadOutPartName = this.GetTemplateChild<TextBlock>(MinuteReadOutPartName);
            _secondReadOutPartName = this.GetTemplateChild<TextBlock>(SecondReadOutPartName);

            if (_hourReadOutPartName != null)
                _hourReadOutPartName.PreviewMouseLeftButtonDown += HourReadOutPartNameOnPreviewMouseLeftButtonDown;
            if (_minuteReadOutPartName != null)
                _minuteReadOutPartName.PreviewMouseLeftButtonDown += MinuteReadOutPartNameOnPreviewMouseLeftButtonDown;
            if (_secondReadOutPartName != null)
                _secondReadOutPartName.PreviewMouseLeftButtonDown += SecondReadOutPartNameOnPreviewMouseLeftButtonDown;

            base.OnApplyTemplate();

            GotoVisualState(false);
        }

        protected override void OnValueChanged(DateTime oldValue, DateTime newValue) => SetFlags();

        private void GotoVisualState(bool useTransitions)
        {
            string stateName;
            switch (DisplayMode)
            {
                case EClockDisplayMode.Hours:
                    stateName = HoursVisualStateName;
                    break;

                case EClockDisplayMode.Minutes:
                    stateName = MinutesVisualStateName;
                    break;

                case EClockDisplayMode.Seconds:
                    stateName = SecondsVisualStateName;
                    break;

                default:
                    return;
            }

            _ = VisualStateManager.GoToState(this, stateName, useTransitions);
        }

        private void SetFlags()
        {
            IsPostMeridiem = Value.Hour >= 12;
            IsMidnightHour = Value.Hour == 0;
            IsMiddayHour = Value.Hour == 12;
        }

        #region button generation

        private void GenerateButtons()
        {
            if (GetTemplateChild(HoursCanvasPartName) is Canvas hoursCanvas)
            {
                RemoveExistingButtons(hoursCanvas);
                GenerateHourButtons(hoursCanvas);
            }

            if (GetTemplateChild(MinutesCanvasPartName) is Canvas minutesCanvas)
            {
                RemoveExistingButtons(minutesCanvas);
                GenerateMinuteButtons(minutesCanvas);
            }

            if (GetTemplateChild(SecondsCanvasPartName) is Canvas secondsCanvas)
            {
                RemoveExistingButtons(secondsCanvas);
                GenerateSecondButtons(secondsCanvas);
            }
        }

        private void RemoveExistingButtons(Canvas canvas)
        {
            for (var i = canvas.Children.Count - 1; i >= 0; i--)
            {
                if (canvas.Children[i] is ClockItemButton)
                    canvas.Children.RemoveAt(i);
            }
        }

        private void GenerateButtons(
            Panel canvas,
            ICollection<int> range,
            double radiusRatio,
            IValueConverter isCheckedConverter,
            Func<int, string> stylePropertySelector,
            string format,
            EClockDisplayMode clockDisplayMode)
        {
            var anglePerItem = 360.0 / range.Count;
            var radiansPerItem = anglePerItem * (Math.PI / 180);

            //nothing fancy with sizing/measuring...we are demanding a height
            if (canvas.Width < 10.0 || Math.Abs(canvas.Height - canvas.Width) > 0.0)
                return;

            _centreCanvas = new Point(canvas.Width / 2, canvas.Height / 2);
            var hypotenuseRadius = _centreCanvas.X * radiusRatio;

            foreach (var i in range)
            {
                var button = new ClockItemButton();
                _ = button.SetBinding(StyleProperty, this.CreateBinding(stylePropertySelector(i)));

                var adjacent = Math.Cos(i * radiansPerItem) * hypotenuseRadius;
                var opposite = Math.Sin(i * radiansPerItem) * hypotenuseRadius;

                button.CentreX = _centreCanvas.X + opposite;
                button.CentreY = _centreCanvas.Y - adjacent;

                _ = button.SetBinding(ToggleButton.IsCheckedProperty, this.CreateBinding("Value", converter: isCheckedConverter, converterParameter: i));
                _ = button.SetBinding(Canvas.LeftProperty, this.CreateBinding("X", button));
                _ = button.SetBinding(Canvas.TopProperty, this.CreateBinding("Y", button));

                var content = i == 60 || (i == 24 && clockDisplayMode == EClockDisplayMode.Hours)
                    ? 0
                    : i;
                button.Content = content.ToString(format);

                _ = canvas.Children.Add(button);
            }
        }

        private void GenerateHourButtons(Panel canvas)
        {
            if (Is24Hours)
            {
                GenerateButtons(
                        canvas,
                        Enumerable.Range(13, 12).ToList(),
                        ButtonRadiusRatio,
                        new ClockItemIsCheckedConverter(() => Value, EClockDisplayMode.Hours, Is24Hours),
                        i => "ButtonStyle",
                        "00",
                        EClockDisplayMode.Hours);
                GenerateButtons(
                        canvas,
                        Enumerable.Range(1, 12).ToList(),
                        ButtonRadiusInnerRatio,
                        new ClockItemIsCheckedConverter(() => Value, EClockDisplayMode.Hours, Is24Hours),
                        i => "ButtonStyle",
                        "#",
                        EClockDisplayMode.Hours);
            }
            else
                GenerateButtons(
                        canvas,
                        Enumerable.Range(1, 12).ToList(),
                        ButtonRadiusRatio,
                        new ClockItemIsCheckedConverter(() => Value, EClockDisplayMode.Hours, Is24Hours),
                        i => "ButtonStyle",
                        "0",
                        EClockDisplayMode.Hours);
        }

        private void GenerateMinuteButtons(Panel canvas)
            => GenerateButtons(
                    canvas,
                    Enumerable.Range(1, 60).ToList(),
                    ButtonRadiusRatio,
                    new ClockItemIsCheckedConverter(() => Value, EClockDisplayMode.Minutes, Is24Hours),
                    i => (i / 5.0 % 1) == 0.0 ? "ButtonStyle" : "LesserButtonStyle",
                    "0",
                    EClockDisplayMode.Minutes);

        private void GenerateSecondButtons(Panel canvas)
            => GenerateButtons(
                canvas,
                Enumerable.Range(1, 60).ToList(),
                ButtonRadiusRatio,
                new ClockItemIsCheckedConverter(() => Value, EClockDisplayMode.Seconds, Is24Hours),
                i => (i / 5.0 % 1) == 0.0 ? "ButtonStyle" : "LesserButtonStyle",
                "0",
                EClockDisplayMode.Seconds);

        #endregion button generation

        #region event handlers

        private void SecondReadOutPartNameOnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => SetCurrentValue(DisplayModeProperty, EClockDisplayMode.Seconds);

        private void MinuteReadOutPartNameOnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => SetCurrentValue(DisplayModeProperty, EClockDisplayMode.Minutes);

        private void HourReadOutPartNameOnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs) => SetCurrentValue(DisplayModeProperty, EClockDisplayMode.Hours);

        private void ClockItemDragCompletedHandler(object sender, DragCompletedEventArgs e)
        {
            OnClockChoiceMade(this, DisplayMode);

            switch (DisplayAutomation)
            {
                case EClockDisplayAutomation.None:
                    break;

                case EClockDisplayAutomation.Cycle:
                    DisplayMode = DisplayMode == EClockDisplayMode.Hours
                        ? EClockDisplayMode.Minutes
                        : EClockDisplayMode.Hours;
                    break;

                case EClockDisplayAutomation.CycleWithSeconds:
                    DisplayMode = DisplayMode == EClockDisplayMode.Hours
                        ? EClockDisplayMode.Minutes
                        : DisplayMode == EClockDisplayMode.Minutes
                        ? EClockDisplayMode.Seconds
                        : EClockDisplayMode.Hours;
                    break;

                case EClockDisplayAutomation.ToMinutesOnly:
                    if (DisplayMode == EClockDisplayMode.Hours)
                        DisplayMode = EClockDisplayMode.Minutes;
                    break;

                case EClockDisplayAutomation.ToSeconds:
                    if (DisplayMode == EClockDisplayMode.Hours)
                        DisplayMode = EClockDisplayMode.Minutes;
                    else if (DisplayMode == EClockDisplayMode.Minutes)
                        DisplayMode = EClockDisplayMode.Seconds;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ClockItemDragStartedHandler(object sender, DragStartedEventArgs dragStartedEventArgs)
            => _currentStartPosition = new Point(dragStartedEventArgs.HorizontalOffset, dragStartedEventArgs.VerticalOffset);

        private void ClockItemDragDeltaHandler(object sender, DragDeltaEventArgs dragDeltaEventArgs)
        {
            var currentDragPosition = new Point(_currentStartPosition.X + dragDeltaEventArgs.HorizontalChange, _currentStartPosition.Y + dragDeltaEventArgs.VerticalChange);
            var delta = new Point(currentDragPosition.X - _centreCanvas.X, currentDragPosition.Y - _centreCanvas.Y);

            var angle = Math.Atan2(delta.X, -delta.Y);
            if (angle < 0)
                angle += 2 * Math.PI;

            DateTime time;
            if (DisplayMode == EClockDisplayMode.Hours)
            {
                if (Is24Hours)
                {
                    var outerBoundary = (_centreCanvas.X * ButtonRadiusInnerRatio) +
                                         (((_centreCanvas.X * ButtonRadiusRatio) - (_centreCanvas.X * ButtonRadiusInnerRatio)) / 2);

                    var sqrt = Pythagoras(_centreCanvas.X - currentDragPosition.X, _centreCanvas.Y - currentDragPosition.Y);
                    var localIsPostMerdiem = sqrt > outerBoundary;
                    var hour = ((int)Math.Round(6 * angle / Math.PI, MidpointRounding.AwayFromZero) % 12) + (localIsPostMerdiem ? 12 : 0);

                    if (hour == 12)
                        hour = 0;
                    else if (hour == 0)
                        hour = 12;

                    time = new DateTime(Value.Year, Value.Month, Value.Day, hour, Value.Minute, Value.Second);
                }
                else
                {
                    var hour = ((int)Math.Round(6 * angle / Math.PI, MidpointRounding.AwayFromZero) % 12) + (IsPostMeridiem ? 12 : 0);
                    time = new DateTime(Value.Year, Value.Month, Value.Day, hour, Value.Minute, Value.Second);
                }
            }
            else
            {
                var value = (int)Math.Round(30 * angle / Math.PI, MidpointRounding.AwayFromZero) % 60;
                time = DisplayMode == EClockDisplayMode.Minutes
                    ? new DateTime(Value.Year, Value.Month, Value.Day, Value.Hour, value, Value.Second)
                    : new DateTime(Value.Year, Value.Month, Value.Day, Value.Hour, Value.Minute, value);
            }

            SetCurrentValue(ValueProperty, time);
        }

        #endregion event handlers

        #region callbacks

        private static double Pythagoras(double x, double y) => Math.Sqrt((x * x) + (y * y));

        private static void IsPostMeridiemPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (!(d is Clock clock))
                return;

            if (clock.IsPostMeridiem && clock.Value.Hour < 12)
                clock.Value = new DateTime(clock.Value.Year, clock.Value.Month, clock.Value.Day, clock.Value.Hour + 12, clock.Value.Minute, clock.Value.Second);
            else if (!clock.IsPostMeridiem && clock.Value.Hour >= 12)
                clock.Value = new DateTime(clock.Value.Year, clock.Value.Month, clock.Value.Day, clock.Value.Hour - 12, clock.Value.Minute, clock.Value.Second);
        }

        private static void Is24HoursChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is Clock clock))
                return;

            clock.GenerateButtons();
        }

        private static void DisplayModePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (!(d is Clock clock))
                return;

            clock.GotoVisualState(!TransitionHelper.GetDisableTransitions(d));
        }

        private static void OnClockChoiceMade(DependencyObject d, EClockDisplayMode displayMode)
        {
            if (!(d is Clock clock))
                return;

            var dragCompletedEventArgs = new ClockChoiceMadeEventArgs(displayMode)
            {
                RoutedEvent = ClockChoiceMadeEvent,
            };

            clock.RaiseEvent(dragCompletedEventArgs);
        }

        #endregion callbacks

        #endregion METHODS

        public event ClockChoiceMadeEventHandler ClockChoiceMade
        {
            add => AddHandler(ClockChoiceMadeEvent, value);
            remove => RemoveHandler(ClockChoiceMadeEvent, value);
        }
    }
}
