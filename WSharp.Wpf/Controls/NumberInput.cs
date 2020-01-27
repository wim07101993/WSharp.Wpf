using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

using WSharp.Wpf.Extensions;
using WSharp.Wpf.Helpers;

namespace WSharp.Wpf.Controls
{
    /// <summary>
    ///     Represents a Windows spin box (also known as an up-down control) that displays numeric values.
    /// </summary>
    [TemplatePart(Name = NumericUpPartName, Type = typeof(RepeatButton))]
    [TemplatePart(Name = NumericDownPartName, Type = typeof(RepeatButton))]
    public class NumberInput : ATextInput<double?>
    {
        #region FIELDS

        private const double DefaultInterval = 1d;
        private const int DefaultDelay = 500;
        private const string NumericDownPartName = "PART_DownButton";
        private const string NumericUpPartName = "PART_UpButton";
        private const string ContentHostPartName = "PART_ContentHost";
        private const string ScientificNotationChar = "E";
        private const StringComparison StrComp = StringComparison.InvariantCultureIgnoreCase;

        private static readonly Regex regexStringFormatHexadecimal = new Regex(@"^(?<complexHEX>.*{\d:X\d+}.*)?(?<simpleHEX>X\d+)?$", RegexOptions.Compiled);
        private static readonly Regex regexStringFormatNumber = new Regex(@"[-+]?(?<![0-9][.,])\b[0-9]+(?:[.,\s][0-9]+)*[.,]?[0-9]?(?:[eE][-+]?[0-9]+)?\b(?!\.[0-9])", RegexOptions.Compiled);

        private Lazy<PropertyInfo> _handlesMouseWheelScrolling = new Lazy<PropertyInfo>();
        private double _internalIntervalMultiplierForCalculation = DefaultInterval;
        private double _internalLargeChange = DefaultInterval * 100;
        private double _intervalValueSinceReset;
        private bool _manualChange;
        private RepeatButton _repeatDown;
        private RepeatButton _repeatUp;
        private ScrollViewer _scrollViewer;

        #endregion FIELDS

        static NumberInput()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumberInput), new FrameworkPropertyMetadata(typeof(NumberInput)));

            VerticalContentAlignmentProperty.OverrideMetadata(typeof(NumberInput), new FrameworkPropertyMetadata(VerticalAlignment.Center));
            HorizontalContentAlignmentProperty.OverrideMetadata(typeof(NumberInput), new FrameworkPropertyMetadata(HorizontalAlignment.Right));

            EventManager.RegisterClassHandler(typeof(NumberInput), GotFocusEvent, new RoutedEventHandler(OnGotFocus));
        }

        private CultureInfo SpecificCultureInfo => Culture ?? Language.GetSpecificCulture();

        #region DEPENDENCY PROPERTIES

        #region Delay

        public static readonly DependencyProperty DelayProperty = DependencyProperty.Register(
            nameof(Delay),
            typeof(int),
            typeof(NumberInput),
            new FrameworkPropertyMetadata(DefaultDelay, OnDelayChanged),
            ValidateDelay);

        /// <summary>
        ///     Gets or sets the amount of time, in milliseconds, the NumberBox waits while the up/down button is pressed
        ///     before it starts increasing/decreasing the
        ///     <see cref="Value" /> for the specified <see cref="Interval" /> . The value must be
        ///     non-negative.
        /// </summary>
        public int Delay
        {
            get => (int)GetValue(DelayProperty);
            set => SetValue(DelayProperty, value);
        }

        private static void OnDelayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is NumberInput numericUpDown) ||
                !(e.OldValue is int oldValue) ||
                !(e.NewValue is int newValue))
                return;

            numericUpDown.OnDelayChanged(oldValue, newValue);
        }

        #endregion Delay

        #region Speedup

        public static readonly DependencyProperty SpeedupProperty = DependencyProperty.Register(
            nameof(Speedup),
            typeof(bool),
            typeof(NumberInput),
            new FrameworkPropertyMetadata(true, OnSpeedupChanged));

        public bool Speedup
        {
            get => (bool)GetValue(SpeedupProperty);
            set => SetValue(SpeedupProperty, value);
        }

        private static void OnSpeedupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is NumberInput numericUpDown) ||
                !(e.NewValue is bool newValue) ||
                !(e.OldValue is bool oldValue))
                return;

            numericUpDown.OnSpeedupChanged(oldValue, newValue);
        }

        #endregion Speedup

        #region StringFormat

        public static readonly DependencyProperty StringFormatProperty = DependencyProperty.Register(
            nameof(StringFormat),
            typeof(string),
            typeof(NumberInput),
            new FrameworkPropertyMetadata(string.Empty, OnStringFormatChanged, CoerceStringFormat));

        public string StringFormat
        {
            get => (string)GetValue(StringFormatProperty);
            set => SetValue(StringFormatProperty, value);
        }

        private static void OnStringFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is NumberInput numericUpDown))
                return;

            if (numericUpDown.Value.HasValue)
                numericUpDown.InternalSetText(numericUpDown.Value);

            if (!(e.NewValue is string value))
                return;

            if (!numericUpDown.NumericInputMode.HasFlag(ENumericInputMode.Decimal) &&
                !string.IsNullOrEmpty(value) &&
                regexStringFormatHexadecimal.IsMatch(value))
                numericUpDown.SetCurrentValue(NumericInputModeProperty, numericUpDown.NumericInputMode | ENumericInputMode.Decimal);
        }

        private static object CoerceStringFormat(DependencyObject d, object basevalue) => basevalue ?? string.Empty;

        #endregion StringFormat

        #region InterceptArrowKeys

        public static readonly DependencyProperty InterceptArrowKeysProperty = DependencyProperty.Register(
            nameof(InterceptArrowKeys),
            typeof(bool),
            typeof(NumberInput),
            new FrameworkPropertyMetadata(true));

        /// <summary>
        ///     Gets or sets a value indicating whether the user can use the arrow keys <see cref="Key.Up"/> and <see cref="Key.Down"/> to change values.
        /// </summary>
        public bool InterceptArrowKeys
        {
            get => (bool)GetValue(InterceptArrowKeysProperty);
            set => SetValue(InterceptArrowKeysProperty, value);
        }

        #endregion InterceptArrowKeys

        #region ButtonsAlignment

        public static readonly DependencyProperty ButtonsAlignmentProperty = DependencyProperty.Register(
           nameof(ButtonsAlignment),
           typeof(EButtonsAlignment),
           typeof(NumberInput),
           new FrameworkPropertyMetadata(EButtonsAlignment.Right, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public EButtonsAlignment ButtonsAlignment
        {
            get => (EButtonsAlignment)GetValue(ButtonsAlignmentProperty);
            set => SetValue(ButtonsAlignmentProperty, value);
        }

        #endregion ButtonsAlignment

        #region Minimum

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
            nameof(Minimum),
            typeof(double),
            typeof(NumberInput),
            new FrameworkPropertyMetadata(double.MinValue, OnMinimumChanged));

        public double Minimum
        {
            get => (double)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is NumberInput numericUpDown) ||
                !(e.NewValue is double newValue) ||
                !(e.OldValue is double oldValue))
                return;

            numericUpDown.CoerceValue(MaximumProperty);
            numericUpDown.CoerceValue(ValueProperty);
            numericUpDown.OnMinimumChanged(oldValue, newValue);
            numericUpDown.EnableDisableUpDown();
        }

        #endregion Minimum

        #region Maximum

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
            nameof(Maximum),
            typeof(double),
            typeof(NumberInput),
            new FrameworkPropertyMetadata(double.MaxValue, OnMaximumChanged, CoerceMaximum));

        public double Maximum
        {
            get => (double)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is NumberInput numericUpDown) ||
                !(e.NewValue is double newValue) ||
                !(e.OldValue is double oldValue))
                return;

            numericUpDown.CoerceValue(ValueProperty);
            numericUpDown.OnMaximumChanged(oldValue, newValue);
            numericUpDown.EnableDisableUpDown();
        }

        private static object CoerceMaximum(DependencyObject d, object value)
        {
            if (!(d is NumberInput numericUpDown) ||
                !(value is double newValue))
                return double.MaxValue;

            var minimum = numericUpDown.Minimum;
            return newValue < minimum
                ? minimum
                : newValue;
        }

        #endregion Maximum

        #region Interval

        public static readonly DependencyProperty IntervalProperty = DependencyProperty.Register(
            nameof(Interval),
            typeof(double),
            typeof(NumberInput),
            new FrameworkPropertyMetadata(DefaultInterval, IntervalChanged));

        public double Interval
        {
            get => (double)GetValue(IntervalProperty);
            set => SetValue(IntervalProperty, value);
        }

        private static void IntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is NumberInput numericUpDown))
                return;

            numericUpDown.ResetInternal();
        }

        #endregion Interval

        #region InterceptMouseWheel

        public static readonly DependencyProperty InterceptMouseWheelProperty = DependencyProperty.Register(
            nameof(InterceptMouseWheel),
            typeof(bool),
            typeof(NumberInput),
            new FrameworkPropertyMetadata(true));

        /// <summary>
        ///     Gets or sets a value indicating whether the user can use the mouse wheel to change values.
        /// </summary>
        public bool InterceptMouseWheel
        {
            get => (bool)GetValue(InterceptMouseWheelProperty);
            set => SetValue(InterceptMouseWheelProperty, value);
        }

        #endregion InterceptMouseWheel

        #region TrackMouseWheelWhenMouseOver

        public static readonly DependencyProperty TrackMouseWheelWhenMouseOverProperty = DependencyProperty.Register(
            nameof(TrackMouseWheelWhenMouseOver),
            typeof(bool),
            typeof(NumberInput),
            new FrameworkPropertyMetadata(default(bool)));

        /// <summary>
        ///     Gets or sets a value indicating whether the control must have the focus in order to change values using the mouse wheel.
        /// <remarks>
        ///     If the value is true then the value changes when the mouse wheel is over the control. If the value is false then the value changes only if the control has the focus. If <see cref="InterceptMouseWheel"/> is set to "false" then this property has no effect.
        /// </remarks>
        /// </summary>
        public bool TrackMouseWheelWhenMouseOver
        {
            get => (bool)GetValue(TrackMouseWheelWhenMouseOverProperty);
            set => SetValue(TrackMouseWheelWhenMouseOverProperty, value);
        }

        #endregion TrackMouseWheelWhenMouseOver

        #region HideUpDownButtons

        public static readonly DependencyProperty HideUpDownButtonsProperty = DependencyProperty.Register(
            nameof(HideUpDownButtons),
            typeof(bool),
            typeof(NumberInput),
            new PropertyMetadata(default(bool)));

        /// <summary>
        ///     Gets or sets a value indicating whether the +/- button of the control is visible.
        /// </summary>
        /// <remarks>
        ///     If the value is false then the <see cref="Value" /> of the control can be changed only if one of the following cases is satisfied:
        ///     <list type="bullet">
        ///         <item>
        ///             <description><see cref="InterceptArrowKeys" /> is true.</description>
        ///         </item>
        ///         <item>
        ///             <description><see cref="InterceptMouseWheel" /> is true.</description>
        ///         </item>
        ///         <item>
        ///             <description><see cref="InterceptManualEnter" /> is true.</description>
        ///         </item>
        ///     </list>
        /// </remarks>
        public bool HideUpDownButtons
        {
            get => (bool)GetValue(HideUpDownButtonsProperty);
            set => SetValue(HideUpDownButtonsProperty, value);
        }

        #endregion HideUpDownButtons

        #region UpDownButtonsWidth

        public static readonly DependencyProperty UpDownButtonsWidthProperty = DependencyProperty.Register(
            nameof(UpDownButtonsWidth),
            typeof(double),
            typeof(NumberInput),
            new PropertyMetadata(25d));

        public double UpDownButtonsWidth
        {
            get => (double)GetValue(UpDownButtonsWidthProperty);
            set => SetValue(UpDownButtonsWidthProperty, value);
        }

        #endregion UpDownButtonsWidth

        #region UpDownButtonsHeight

        public static readonly DependencyProperty UpDownButtonsHeightProperty = DependencyProperty.Register(
            nameof(UpDownButtonsHeight),
            typeof(double),
            typeof(NumberInput),
            new PropertyMetadata(20d));

        public double UpDownButtonsHeight
        {
            get => (double)GetValue(UpDownButtonsHeightProperty);
            set => SetValue(UpDownButtonsHeightProperty, value);
        }

        #endregion UpDownButtonsHeight

        #region InterceptManualEnter

        public static readonly DependencyProperty InterceptManualEnterProperty = DependencyProperty.Register(
            nameof(InterceptManualEnter),
            typeof(bool),
            typeof(NumberInput),
            new PropertyMetadata(true, OnInterceptManualEnterChanged));

        /// <summary>
        ///     Gets or sets a value indicating whether the user can enter text in the control.
        /// </summary>
        public bool InterceptManualEnter
        {
            get => (bool)GetValue(InterceptManualEnterProperty);
            set => SetValue(InterceptManualEnterProperty, value);
        }

        private static void OnInterceptManualEnterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is NumberInput numUpDown) ||
                !(e.NewValue is bool newValue) ||
                e.NewValue == e.OldValue)
                return;

            numUpDown.ToggleReadOnlyMode(!newValue || numUpDown.IsReadOnly);
        }

        #endregion InterceptManualEnter

        #region Culture

        public static readonly DependencyProperty CultureProperty = DependencyProperty.Register(
            nameof(Culture),
            typeof(CultureInfo),
            typeof(NumberInput),
            new PropertyMetadata(null, OnCulturePropertyChanged));

        /// <summary>
        ///     Gets or sets a value indicating the culture to be used in string formatting operations.
        /// </summary>
        public CultureInfo Culture
        {
            get => (CultureInfo)GetValue(CultureProperty);
            set => SetValue(CultureProperty, value);
        }

        private static void OnCulturePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is NumberInput numUpDown) || e.NewValue == e.OldValue)
                return;

            numUpDown.OnValueChanged(numUpDown.Value, numUpDown.Value);
        }

        #endregion Culture

        #region NumericInputMode

        public static readonly DependencyProperty NumericInputModeProperty = DependencyProperty.Register(
            nameof(NumericInputMode),
            typeof(ENumericInputMode),
            typeof(NumberInput),
            new FrameworkPropertyMetadata(ENumericInputMode.All, OnNumericInputModeChanged));

        public ENumericInputMode NumericInputMode
        {
            get => (ENumericInputMode)GetValue(NumericInputModeProperty);
            set => SetValue(NumericInputModeProperty, value);
        }

        private static void OnNumericInputModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is NumberInput numericUpDown) ||
                !(e.NewValue is ENumericInputMode newValue) ||
                e.NewValue == e.OldValue ||
                numericUpDown.Value == null)
                return;

            if (!newValue.HasFlag(ENumericInputMode.Decimal))
                numericUpDown.Value = Math.Truncate(numericUpDown.Value.GetValueOrDefault());
        }

        #endregion NumericInputMode

        #region SnapToMultipleOfInterval

        public static readonly DependencyProperty SnapToMultipleOfIntervalProperty = DependencyProperty.Register(
            nameof(SnapToMultipleOfInterval),
            typeof(bool),
            typeof(NumberInput),
            new PropertyMetadata(default(bool), OnSnapToMultipleOfIntervalChanged));

        public bool SnapToMultipleOfInterval
        {
            get => (bool)GetValue(SnapToMultipleOfIntervalProperty);
            set => SetValue(SnapToMultipleOfIntervalProperty, value);
        }

        private static void OnSnapToMultipleOfIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is NumberInput numericUpDown) ||
                !(e.NewValue is bool newValue))
                return;

            var value = numericUpDown.Value.GetValueOrDefault();

            if (newValue && Math.Abs(numericUpDown.Interval) > 0)
                numericUpDown.Value = Math.Round(value / numericUpDown.Interval) * numericUpDown.Interval;
        }

        #endregion SnapToMultipleOfInterval

        #endregion DEPENDENCY PROPERTIES

        #region METHODS

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_repeatUp != null)
            {
                _repeatUp.Click -= OnRepeatUpClick;
                _repeatUp.PreviewMouseUp -= OnRepeatUpPreviewMouseUp;
            }

            if (_repeatDown != null)
            {
                _repeatDown.Click -= OnRepeatDownClick;
                _repeatDown.PreviewMouseUp -= OnRepeatDownPreviewMouseUp;
            }

            if (this.TryGetTemplateChild(NumericUpPartName, out _repeatUp))
            {
                _repeatUp.Click += OnRepeatUpClick;
                _repeatUp.PreviewMouseUp += OnRepeatUpPreviewMouseUp;
            }

            if (this.TryGetTemplateChild(NumericDownPartName, out _repeatDown))
            {
                _repeatDown.Click += OnRepeatDownClick;
                _repeatDown.PreviewMouseUp += OnRepeatDownPreviewMouseUp;
            }

            ToggleReadOnlyMode(IsReadOnly | !InterceptManualEnter);
            OnValueChanged(Value, Value);

            if (this.TryGetTemplateChild(ContentHostPartName, out _scrollViewer))
                _handlesMouseWheelScrolling = new Lazy<PropertyInfo>(() => _scrollViewer
                    .GetType()
                    .GetProperties(BindingFlags.NonPublic | BindingFlags.Instance)
                    .SingleOrDefault(i => i.Name == "HandlesMouseWheelScrolling"));
        }

        public override double? CoerceValue(double? baseValue)
        {
            if (baseValue == null)
                return null;

            var val = baseValue.Value;

            if (!NumericInputMode.HasFlag(ENumericInputMode.Decimal))
                val = Math.Truncate(val);

            return val < Minimum
                ? Minimum
                : val > Maximum
                ? Maximum
                : val;
        }

        private void OnRepeatUpPreviewMouseUp(object sender, MouseButtonEventArgs e) => ResetInternal();

        private void OnRepeatDownPreviewMouseUp(object sender, MouseButtonEventArgs e) => ResetInternal();

        private void OnRepeatDownClick(object sender, RoutedEventArgs e) => ChangeValueWithSpeedUp(false);

        private void OnRepeatUpClick(object sender, RoutedEventArgs e) => ChangeValueWithSpeedUp(true);

        private void ToggleReadOnlyMode(bool isReadOnly)
        {
            if (_repeatUp == null || _repeatDown == null)
                return;

            if (isReadOnly)
            {
                LostFocus -= OnTextBoxLostFocus;
                PreviewTextInput -= OnPreviewTextInput;
                PreviewKeyDown -= OnTextBoxKeyDown;
                TextChanged -= OnTextChanged;
                DataObject.RemovePastingHandler(this, OnValueTextBoxPaste);
            }
            else
            {
                LostFocus += OnTextBoxLostFocus;
                PreviewTextInput += OnPreviewTextInput;
                PreviewKeyDown += OnTextBoxKeyDown;
                TextChanged += OnTextChanged;
                DataObject.AddPastingHandler(this, OnValueTextBoxPaste);
            }
        }

        protected virtual void OnDelayChanged(int oldValue, int newValue)
        {
            if (oldValue == newValue)
                return;
            
            RaiseDelayChanged(oldValue, newValue);

            if (_repeatDown != null)
                _repeatDown.Delay = newValue;

            if (_repeatUp != null)
                _repeatUp.Delay = newValue;
        }

        protected virtual void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
        }

        protected virtual void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (!InterceptArrowKeys)
                return;

            switch (e.Key)
            {
                case Key.Up:
                    ChangeValueWithSpeedUp(true);
                    e.Handled = true;
                    break;

                case Key.Down:
                    ChangeValueWithSpeedUp(false);
                    e.Handled = true;
                    break;
            }

            if (e.Handled)
            {
                _manualChange = false;
                InternalSetText(Value);
            }
        }

        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            base.OnPreviewKeyUp(e);

            if (e.Key == Key.Down || e.Key == Key.Up)
                ResetInternal();
        }

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            base.OnPreviewMouseWheel(e);

            if (InterceptMouseWheel && (IsFocused || IsFocused || TrackMouseWheelWhenMouseOver))
            {
                var increment = e.Delta > 0;
                _manualChange = false;
                ChangeValueInternal(increment);
            }

            if (_scrollViewer != null && _handlesMouseWheelScrolling.Value != null)
            {
                if (TrackMouseWheelWhenMouseOver)
                    _handlesMouseWheelScrolling.Value.SetValue(_scrollViewer, true, null);
                else if (InterceptMouseWheel)
                    _handlesMouseWheelScrolling.Value.SetValue(_scrollViewer, IsFocused, null);
                else
                    _handlesMouseWheelScrolling.Value.SetValue(_scrollViewer, true, null);
            }
        }

        protected void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = true;
            if (string.IsNullOrWhiteSpace(e.Text) || e.Text.Length != 1 ||
                !(sender is TextBox textBox))
                return;

            var equivalentCulture = SpecificCultureInfo;
            var numberFormatInfo = equivalentCulture.NumberFormat;
            var text = e.Text;

            if (char.IsDigit(text[0]))
            {
                var i = textBox.SelectionStart + textBox.SelectionLength;
                if (textBox.Text.IndexOf(numberFormatInfo.NegativeSign, i, StrComp) < 0 &&
                    textBox.Text.IndexOf(numberFormatInfo.PositiveSign, i, StrComp) < 0)
                    e.Handled = false;
            }
            else
            {
                var allTextSelected = textBox.SelectedText == textBox.Text;

                if (numberFormatInfo.NumberDecimalSeparator == text)
                {
                    var noSeparator = textBox.Text.All(i => i.ToString(equivalentCulture) != numberFormatInfo.NumberDecimalSeparator);
                    if ((noSeparator || allTextSelected) && NumericInputMode.HasFlag(ENumericInputMode.Decimal))
                        e.Handled = false;
                }
                else
                {
                    if (text == numberFormatInfo.NegativeSign ||
                        text == numberFormatInfo.PositiveSign)
                    {
                        if (textBox.SelectionStart == 0)
                        {
                            // check if text already has a + or - sign
                            if (textBox.Text.Length <= 1)
                                e.Handled = false;
                            else
                            {
                                if (allTextSelected ||
                                    (!textBox.Text.StartsWith(numberFormatInfo.NegativeSign, StrComp) &&
                                    !textBox.Text.StartsWith(numberFormatInfo.PositiveSign, StrComp)))
                                    e.Handled = false;
                            }
                        }
                        else if (textBox.SelectionStart > 0)
                        {
                            var elementBeforeCaret = textBox.Text
                                .ElementAt(textBox.SelectionStart - 1)
                                .ToString(equivalentCulture);
                            if (elementBeforeCaret.Equals(ScientificNotationChar, StrComp) &&
                                NumericInputMode.HasFlag(ENumericInputMode.Decimal))
                                e.Handled = false;
                        }
                    }
                    else if (text.Equals(ScientificNotationChar, StrComp) &&
                             NumericInputMode.HasFlag(ENumericInputMode.Decimal) &&
                             textBox.SelectionStart > 0 &&
                             !textBox.Text.Any(i => i.ToString(equivalentCulture).Equals(ScientificNotationChar, StrComp)))
                    {
                        e.Handled = false;
                    }
                }
            }

            _manualChange = _manualChange || !e.Handled;
        }

        protected virtual void OnSpeedupChanged(bool oldSpeedup, bool newSpeedup)
        {
        }

        /// <summary>
        ///     Raises the <see cref="ValueChanged" /> routed event.
        /// </summary>
        /// <param name="oldValue">
        ///     Old value of the <see cref="Value" /> property
        /// </param>
        /// <param name="newValue">
        ///     New value of the <see cref="Value" /> property
        /// </param>
        protected override void OnValueChanged(double? oldValue, double? newValue)
        {
            if (!_manualChange)
            {
                if (!newValue.HasValue)
                {
                    Text = null;

                    if (oldValue != newValue)
                        RaiseValueChanged(oldValue, newValue);

                    return;
                }

                if (_repeatUp != null && !_repeatUp.IsEnabled)
                    _repeatUp.IsEnabled = true;

                if (_repeatDown != null && !_repeatDown.IsEnabled)
                    _repeatDown.IsEnabled = true;

                if (newValue <= Minimum)
                {
                    if (_repeatDown != null)
                        _repeatDown.IsEnabled = false;

                    ResetInternal();

                    if (IsLoaded)
                        RaiseMiminumReached();
                }

                if (newValue >= Maximum)
                {
                    if (_repeatUp != null)
                        _repeatUp.IsEnabled = false;

                    ResetInternal();

                    if (IsLoaded)
                        RaiseMaxinumReached();
                }

                InternalSetText(newValue);
            }

            if (oldValue != newValue)
                RaiseValueChanged(oldValue, newValue);
        }

        private static bool ValidateDelay(object value) => Convert.ToInt32(value) >= 0;

        private void InternalSetText(double? newValue)
        {
            if (!newValue.HasValue)
            {
                Text = null;
                return;
            }

            Text = FormattedValue(newValue, StringFormat, SpecificCultureInfo);

            if ((bool)GetValue(TextBoxHelper.IsMonitoringProperty))
                SetValue(TextBoxHelper.TextLengthProperty, Text.Length);
        }

        private string FormattedValue(double? newValue, string format, CultureInfo culture)
        {
            format = format.Replace("{}", string.Empty);
            if (string.IsNullOrWhiteSpace(format))
                return newValue.Value.ToString(culture);

            var match = regexStringFormatHexadecimal.Match(format);
            if (match.Success)
            {
                // HEX DOES SUPPORT INT ONLY.
                if (match.Groups["simpleHEX"].Success)
                    return ((int)newValue.Value).ToString(match.Groups["simpleHEX"].Value, culture);

                if (match.Groups["complexHEX"].Success)
                    return string.Format(culture, match.Groups["complexHEX"].Value, (int)newValue.Value);
            }
            else
            {
                // then we may have a StringFormat of e.g. "N0"
                if (format.Contains("{"))
                    return string.Format(culture, format, newValue.Value);

                return newValue.Value.ToString(format, culture);
            }

            return newValue.Value.ToString(culture);
        }

        private void ChangeValueWithSpeedUp(bool toPositive)
        {
            if (IsReadOnly)
                return;

            double direction = toPositive ? 1 : -1;
            if (!Speedup)
                ChangeValueInternal(direction * Interval);
            else
            {
                var d = Interval * _internalLargeChange;
                if ((_intervalValueSinceReset += Interval * _internalIntervalMultiplierForCalculation) > d)
                {
                    _internalLargeChange *= 10;
                    _internalIntervalMultiplierForCalculation *= 10;
                }

                ChangeValueInternal(direction * _internalIntervalMultiplierForCalculation);
            }
        }

        private void ChangeValueInternal(bool addInterval) => ChangeValueInternal(addInterval ? Interval : -Interval);

        private void ChangeValueInternal(double interval)
        {
            if (IsReadOnly)
                return;

            var routedEvent = interval > 0 ?
                new NumberInputChangedRoutedEventArgs(ValueIncrementedEvent, interval) :
                new NumberInputChangedRoutedEventArgs(ValueDecrementedEvent, interval);

            RaiseEvent(routedEvent);

            if (!routedEvent.Handled)
            {
                ChangeValueBy(routedEvent.Interval);
                CaretIndex = Text.Length;
            }
        }

        private void ChangeValueBy(double difference)
        {
            var newValue = Value.GetValueOrDefault() + difference;
            SetCurrentValue(ValueProperty, CoerceValue(newValue));
        }

        private void EnableDisableDown()
        {
            if (_repeatDown == null)
                return;

            _repeatDown.IsEnabled = Value > Minimum;
        }

        private void EnableDisableUp()
        {
            if (_repeatUp == null)
                return;

            _repeatUp.IsEnabled = Value < Maximum;
        }

        private void EnableDisableUpDown()
        {
            EnableDisableUp();
            EnableDisableDown();
        }

        private void OnTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            _manualChange = _manualChange || e.Key == Key.Back || e.Key == Key.Delete || e.Key == Key.Decimal || e.Key == Key.OemComma || e.Key == Key.OemPeriod;

            if (!NumericInputMode.HasFlag(ENumericInputMode.Decimal) || (e.Key != Key.Decimal && e.Key != Key.OemPeriod))
                return;

            if (!(sender is TextBox textBox))
                return;

            if (textBox.Text.Contains(SpecificCultureInfo.NumberFormat.NumberDecimalSeparator) == false)
            {
                //the control doesn't contai the decimal separator
                //so we get the current caret index to insert the current culture decimal separator
                var caret = textBox.CaretIndex;
                //update the control text
                textBox.Text = textBox.Text.Insert(caret, SpecificCultureInfo.NumberFormat.CurrencyDecimalSeparator);
                //move the caret to the correct position
                textBox.CaretIndex = caret + 1;
            }

            e.Handled = true;
        }

        private void OnTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            if (!InterceptManualEnter)
                return;

            if (_manualChange && sender is TextBox tb)
            {
                _manualChange = false;

                if (ValidateText(tb.Text, out var convertedValue))
                {
                    if (SnapToMultipleOfInterval && Math.Abs(Interval) > 0)
                        convertedValue = Math.Round(convertedValue / Interval) * Interval;

                    if (convertedValue > Maximum)
                        convertedValue = Maximum;
                    else if (convertedValue < Minimum)
                        convertedValue = Minimum;

                    SetCurrentValue(ValueProperty, convertedValue);
                }
            }

            OnValueChanged(Value, Value);
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(((TextBox)sender).Text))
                Value = null;
            else if (_manualChange && ValidateText(((TextBox)sender).Text, out var convertedValue))
                SetCurrentValue(ValueProperty, convertedValue);
        }

        private void OnValueTextBoxPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (!(sender is TextBox textBox))
                return;

            var textPresent = textBox.Text;

            var isText = e.SourceDataObject.GetDataPresent(DataFormats.Text, true);
            if (!isText)
            {
                e.CancelCommand();
                return;
            }

            var text = e.SourceDataObject.GetData(DataFormats.Text) as string;

            var newText = string.Concat(
                textPresent.Substring(0, textBox.SelectionStart), 
                text,
                textPresent.Substring(textBox.SelectionStart + textBox.SelectionLength));

            if (!ValidateText(newText, out _))
                e.CancelCommand();
            else
                _manualChange = true;
        }

        private void ResetInternal()
        {
            if (IsReadOnly)
                return;

            _internalLargeChange = 100 * Interval;
            _internalIntervalMultiplierForCalculation = Interval;
            _intervalValueSinceReset = 0;
        }

        private bool ValidateText(string text, out double convertedValue)
        {
            text = GetAnyNumberFromText(text);

            return double.TryParse(text, NumberStyles.Any, SpecificCultureInfo, out convertedValue);
        }

        private string GetAnyNumberFromText(string text)
        {
            var matches = regexStringFormatNumber.Matches(text);

            if (matches.Count > 0)
                return matches[0].Value;

            return text;
        }

        /// <summary>
        ///     Called when this element or any below gets focus.
        /// </summary>
        private static void OnGotFocus(object sender, RoutedEventArgs e)
        {
            // When NumberBox gets logical focus, select the text inside us.
            // If we're an editable NumberBox, forward focus to the TextBox element
            if (e.Handled ||
                !(sender is NumberInput numericUpDown))
                return;

            if ((!numericUpDown.InterceptManualEnter && !numericUpDown.IsReadOnly) ||
                !numericUpDown.Focusable ||
                e.OriginalSource != numericUpDown)
                return;

            // MoveFocus takes a TraversalRequest as its argument.
            var request = new TraversalRequest((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift ? FocusNavigationDirection.Previous : FocusNavigationDirection.Next);
            // Gets the element with keyboard focus.
            // Change keyboard focus.
            _ = Keyboard.FocusedElement is UIElement elementWithFocus
                ? elementWithFocus.MoveFocus(request)
                : numericUpDown.Focus();

            e.Handled = true;
        }

        #endregion METHODS

        #region EVENTS

        public static readonly RoutedEvent MaximumReachedEvent = EventManager.RegisterRoutedEvent(nameof(MaximumReached), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NumberInput));
        public static readonly RoutedEvent MinimumReachedEvent = EventManager.RegisterRoutedEvent(nameof(MinimumReached), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NumberInput));
        public static readonly RoutedEvent ValueIncrementedEvent = EventManager.RegisterRoutedEvent(nameof(ValueIncremented), RoutingStrategy.Bubble, typeof(NumberInputChangedRoutedEventHandler), typeof(NumberInput));
        public static readonly RoutedEvent ValueDecrementedEvent = EventManager.RegisterRoutedEvent(nameof(ValueDecremented), RoutingStrategy.Bubble, typeof(NumberInputChangedRoutedEventHandler), typeof(NumberInput));
        public static readonly RoutedEvent DelayChangedEvent = EventManager.RegisterRoutedEvent(nameof(DelayChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NumberInput));

        /// <summary>
        ///     Event fired from this NumberBox when its value has reached the maximum value
        /// </summary>
        public event RoutedEventHandler MaximumReached
        {
            add => AddHandler(MaximumReachedEvent, value);
            remove => RemoveHandler(MaximumReachedEvent, value);
        }

        /// <summary>
        ///     Event fired from this NumberBox when its value has reached the minimum value
        /// </summary>
        public event RoutedEventHandler MinimumReached
        {
            add => AddHandler(MinimumReachedEvent, value);
            remove => RemoveHandler(MinimumReachedEvent, value);
        }

        public event NumberInputChangedRoutedEventHandler ValueIncremented
        {
            add => AddHandler(ValueIncrementedEvent, value);
            remove => RemoveHandler(ValueIncrementedEvent, value);
        }

        public event NumberInputChangedRoutedEventHandler ValueDecremented
        {
            add => AddHandler(ValueDecrementedEvent, value);
            remove => RemoveHandler(ValueDecrementedEvent, value);
        }

        public event RoutedEventHandler DelayChanged
        {
            add => AddHandler(DelayChangedEvent, value);
            remove => RemoveHandler(DelayChangedEvent, value);
        }

        protected void RaiseMiminumReached() => RaiseEvent(new RoutedEventArgs(MinimumReachedEvent));

        protected void RaiseMaxinumReached() => RaiseEvent(new RoutedEventArgs(MaximumReachedEvent));

        protected void RaiseValueIncremented(double interval) => RaiseEvent(new NumberInputChangedRoutedEventArgs(ValueIncrementedEvent, interval));

        protected void RaiseValueDecremented(double interval) => RaiseEvent(new NumberInputChangedRoutedEventArgs(ValueDecrementedEvent, interval));

        protected void RaiseDelayChanged(double oldValue, double newValue)
            => RaiseEvent(new RoutedPropertyChangedEventArgs<double>(oldValue, newValue, ValueDecrementedEvent));

        #endregion EVENTS
    }
}
