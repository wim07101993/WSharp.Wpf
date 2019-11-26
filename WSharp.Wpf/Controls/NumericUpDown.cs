using System;
using System.ComponentModel;
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
    [TemplatePart(Name = ElementNumericUp, Type = typeof(RepeatButton))]
    [TemplatePart(Name = ElementNumericDown, Type = typeof(RepeatButton))]
    [TemplatePart(Name = ElementTextBox, Type = typeof(TextBox))]
    public class NumericUpDown : Control
    {
        #region ROUTED EVENTS

        public static readonly RoutedEvent ValueIncrementedEvent = EventManager.RegisterRoutedEvent(
            nameof(ValueIncremented),
            RoutingStrategy.Bubble,
            typeof(NumbericUpDownChangedRoutedEventHandler),
            typeof(NumericUpDown));

        public static readonly RoutedEvent ValueDecrementedEvent = EventManager.RegisterRoutedEvent(
            nameof(ValueDecremented),
            RoutingStrategy.Bubble,
            typeof(NumbericUpDownChangedRoutedEventHandler),
            typeof(NumericUpDown));

        public static readonly RoutedEvent DelayChangedEvent = EventManager.RegisterRoutedEvent(
            nameof(DelayChanged),
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(NumericUpDown));

        public static readonly RoutedEvent MaximumReachedEvent = EventManager.RegisterRoutedEvent(
            nameof(MaximumReached),
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(NumericUpDown));

        public static readonly RoutedEvent MinimumReachedEvent = EventManager.RegisterRoutedEvent(
            nameof(MinimumReached),
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(NumericUpDown));

        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(
            nameof(ValueChanged),
            RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<double?>),
            typeof(NumericUpDown));

        #endregion ROUTED EVENTS


        #region DEPENDENCY PROPERTIES

        public static readonly DependencyProperty TextAlignmentProperty = TextBox.TextAlignmentProperty.AddOwner(
            typeof(NumericUpDown));

        public static readonly DependencyProperty IsReadOnlyProperty = TextBoxBase.IsReadOnlyProperty.AddOwner(
            typeof(NumericUpDown),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits, OnIsReadOnlyPropertyChanged));

        public static readonly DependencyProperty DelayProperty = DependencyProperty.Register(
            nameof(Delay),
            typeof(int),
            typeof(NumericUpDown),
            new FrameworkPropertyMetadata(DefaultDelay, OnDelayChanged),
            ValidateDelay);

        public static readonly DependencyProperty SpeedupProperty = DependencyProperty.Register(
            nameof(Speedup),
            typeof(bool),
            typeof(NumericUpDown),
            new FrameworkPropertyMetadata(true, OnSpeedupChanged));

        public static readonly DependencyProperty StringFormatProperty = DependencyProperty.Register(
            nameof(StringFormat),
            typeof(string),
            typeof(NumericUpDown),
            new FrameworkPropertyMetadata(string.Empty, OnStringFormatChanged, CoerceStringFormat));

        public static readonly DependencyProperty InterceptArrowKeysProperty = DependencyProperty.Register(
            nameof(InterceptArrowKeys),
            typeof(bool),
            typeof(NumericUpDown),
            new FrameworkPropertyMetadata(true));

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value),
            typeof(double?),
            typeof(NumericUpDown),
            new FrameworkPropertyMetadata(default(double?), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged, CoerceValue));

        public static readonly DependencyProperty ButtonsAlignmentProperty = DependencyProperty.Register(
           nameof(ButtonsAlignment),
           typeof(ButtonsAlignment),
           typeof(NumericUpDown),
           new FrameworkPropertyMetadata(ButtonsAlignment.Right, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
            nameof(Minimum),
            typeof(double),
            typeof(NumericUpDown),
            new FrameworkPropertyMetadata(double.MinValue, OnMinimumChanged));

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
            nameof(Maximum),
            typeof(double),
            typeof(NumericUpDown),
            new FrameworkPropertyMetadata(double.MaxValue, OnMaximumChanged, CoerceMaximum));

        public static readonly DependencyProperty IntervalProperty = DependencyProperty.Register(
            nameof(Interval),
            typeof(double),
            typeof(NumericUpDown),
            new FrameworkPropertyMetadata(DefaultInterval, IntervalChanged));

        public static readonly DependencyProperty InterceptMouseWheelProperty = DependencyProperty.Register(
            nameof(InterceptMouseWheel),
            typeof(bool),
            typeof(NumericUpDown),
            new FrameworkPropertyMetadata(true));

        public static readonly DependencyProperty TrackMouseWheelWhenMouseOverProperty = DependencyProperty.Register(
            nameof(TrackMouseWheelWhenMouseOver),
            typeof(bool),
            typeof(NumericUpDown),
            new FrameworkPropertyMetadata(default(bool)));

        public static readonly DependencyProperty HideUpDownButtonsProperty = DependencyProperty.Register(
            nameof(HideUpDownButtons),
            typeof(bool),
            typeof(NumericUpDown),
            new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty UpDownButtonsWidthProperty = DependencyProperty.Register(
            nameof(UpDownButtonsWidth),
            typeof(double),
            typeof(NumericUpDown),
            new PropertyMetadata(25d));

        public static readonly DependencyProperty UpDownButtonsHeightProperty = DependencyProperty.Register(
            nameof(UpDownButtonsHeight),
            typeof(double),
            typeof(NumericUpDown),
            new PropertyMetadata(20d));

        public static readonly DependencyProperty InterceptManualEnterProperty = DependencyProperty.Register(
            nameof(InterceptManualEnter),
            typeof(bool),
            typeof(NumericUpDown),
            new PropertyMetadata(true, OnInterceptManualEnterChanged));

        public static readonly DependencyProperty CultureProperty = DependencyProperty.Register(
            nameof(Culture),
            typeof(CultureInfo),
            typeof(NumericUpDown),
            new PropertyMetadata(null, OnCulturePropertyChanged));

        public static readonly DependencyProperty NumericInputModeProperty = DependencyProperty.Register(
            nameof(NumericInputMode),
            typeof(NumericInput),
            typeof(NumericUpDown),
            new FrameworkPropertyMetadata(NumericInput.All, OnNumericInputModeChanged));

        public static readonly DependencyProperty SnapToMultipleOfIntervalProperty = DependencyProperty.Register(
            nameof(SnapToMultipleOfInterval),
            typeof(bool),
            typeof(NumericUpDown),
            new PropertyMetadata(default(bool), OnSnapToMultipleOfIntervalChanged));

        #endregion DEPENDENCY PROPERTIES


        #region FIELDS

        private static readonly Regex RegexStringFormatHexadecimal = new Regex(@"^(?<complexHEX>.*{\d:X\d+}.*)?(?<simpleHEX>X\d+)?$", RegexOptions.Compiled);
        private static readonly Regex RegexStringFormatNumber = new Regex(@"[-+]?(?<![0-9][.,])\b[0-9]+(?:[.,\s][0-9]+)*[.,]?[0-9]?(?:[eE][-+]?[0-9]+)?\b(?!\.[0-9])", RegexOptions.Compiled);

        private const double DefaultInterval = 1d;
        private const int DefaultDelay = 500;
        private const string ElementNumericDown = "DownButton";
        private const string ElementNumericUp = "UpButton";
        private const string ElementTextBox = "TextBox";
        private const string ScientificNotationChar = "E";
        private const StringComparison StrComp = StringComparison.InvariantCultureIgnoreCase;

        private Lazy<PropertyInfo> _handlesMouseWheelScrolling = new Lazy<PropertyInfo>();
        private double _internalIntervalMultiplierForCalculation = DefaultInterval;
        private double _internalLargeChange = DefaultInterval * 100;
        private double _intervalValueSinceReset;
        private bool _manualChange;
        private RepeatButton _repeatDown;
        private RepeatButton _repeatUp;
        private TextBox _valueTextBox;
        private ScrollViewer _scrollViewer;

        #endregion FIELDS


        #region CONSTRUCTOR

        static NumericUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(typeof(NumericUpDown)));

            VerticalContentAlignmentProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(VerticalAlignment.Center));
            HorizontalContentAlignmentProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(HorizontalAlignment.Right));

            EventManager.RegisterClassHandler(typeof(NumericUpDown), GotFocusEvent, new RoutedEventHandler(OnGotFocus));
        }

        #endregion CONSTRUCTOR

        #region PROPERTIES

        /// <summary>
        ///     Gets or sets the amount of time, in milliseconds, the NumberBox waits while the up/down button is pressed
        ///     before it starts increasing/decreasing the
        ///     <see cref="Value" /> for the specified <see cref="Interval" /> . The value must be
        ///     non-negative.
        /// </summary>
        [Bindable(true)]
        [DefaultValue(DefaultDelay)]
        [Category("Behavior")]
        public int Delay
        {
            get => (int)GetValue(DelayProperty);
            set => SetValue(DelayProperty, value);
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the user can use the arrow keys <see cref="Key.Up"/> and <see cref="Key.Down"/> to change values. 
        /// </summary>
        [Bindable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool InterceptArrowKeys
        {
            get => (bool)GetValue(InterceptArrowKeysProperty);
            set => SetValue(InterceptArrowKeysProperty, value);
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the user can use the mouse wheel to change values.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool InterceptMouseWheel
        {
            get => (bool)GetValue(InterceptMouseWheelProperty);
            set => SetValue(InterceptMouseWheelProperty, value);
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the control must have the focus in order to change values using the mouse wheel.
        /// <remarks>
        ///     If the value is true then the value changes when the mouse wheel is over the control. If the value is false then the value changes only if the control has the focus. If <see cref="InterceptMouseWheel"/> is set to "false" then this property has no effect.
        /// </remarks>
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool TrackMouseWheelWhenMouseOver
        {
            get => (bool)GetValue(TrackMouseWheelWhenMouseOverProperty);
            set => SetValue(TrackMouseWheelWhenMouseOverProperty, value);
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the user can enter text in the control.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool InterceptManualEnter
        {
            get => (bool)GetValue(InterceptManualEnterProperty);
            set => SetValue(InterceptManualEnterProperty, value);
        }

        /// <summary>
        ///     Gets or sets a value indicating the culture to be used in string formatting operations.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(null)]
        public CultureInfo Culture
        {
            get => (CultureInfo)GetValue(CultureProperty);
            set => SetValue(CultureProperty, value);
        }

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
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool HideUpDownButtons
        {
            get => (bool)GetValue(HideUpDownButtonsProperty);
            set => SetValue(HideUpDownButtonsProperty, value);
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(25d)]
        public double UpDownButtonsWidth
        {
            get => (double)GetValue(UpDownButtonsWidthProperty);
            set => SetValue(UpDownButtonsWidthProperty, value);
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(20d)]
        public double UpDownButtonsHeight
        {
            get => (double)GetValue(UpDownButtonsHeightProperty);
            set => SetValue(UpDownButtonsHeightProperty, value);
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(ButtonsAlignment.Right)]
        public ButtonsAlignment ButtonsAlignment
        {
            get => (ButtonsAlignment)GetValue(ButtonsAlignmentProperty);
            set => SetValue(ButtonsAlignmentProperty, value);
        }

        [Bindable(true)]
        [Category("Behavior")]
        [DefaultValue(DefaultInterval)]
        public double Interval
        {
            get => (double)GetValue(IntervalProperty);
            set => SetValue(IntervalProperty, value);
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        [Bindable(true)]
        [Category("Common")]
        [DefaultValue(double.MaxValue)]
        public double Maximum
        {
            get => (double)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        [Bindable(true)]
        [Category("Common")]
        [DefaultValue(double.MinValue)]
        public double Minimum
        {
            get => (double)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        [Category("Common")]
        [DefaultValue(true)]
        public bool Speedup
        {
            get => (bool)GetValue(SpeedupProperty);
            set => SetValue(SpeedupProperty, value);
        }

        [Category("Common")]
        public string StringFormat
        {
            get => (string)GetValue(StringFormatProperty);
            set => SetValue(StringFormatProperty, value);
        }

        [Bindable(true)]
        [Category("Common")]
        [DefaultValue(TextAlignment.Right)]
        public TextAlignment TextAlignment
        {
            get => (TextAlignment)GetValue(TextAlignmentProperty);
            set => SetValue(TextAlignmentProperty, value);
        }

        [Bindable(true)]
        [Category("Common")]
        [DefaultValue(null)]
        public double? Value
        {
            get => (double?)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        [Category("Common")]
        [DefaultValue(NumericInput.All)]
        public NumericInput NumericInputMode
        {
            get => (NumericInput)GetValue(NumericInputModeProperty);
            set => SetValue(NumericInputModeProperty, value);
        }

        [Bindable(true)]
        [Category("Common")]
        [DefaultValue(false)]
        public bool SnapToMultipleOfInterval
        {
            get => (bool)GetValue(SnapToMultipleOfIntervalProperty);
            set => SetValue(SnapToMultipleOfIntervalProperty, value);
        }

        private CultureInfo SpecificCultureInfo => Culture ?? Language.GetSpecificCulture();

        #endregion PROPERTIES


        #region METHODS

        #region callbacks

        /// <summary> 
        ///     Called when this element or any below gets focus.
        /// </summary>
        private static void OnGotFocus(object sender, RoutedEventArgs e)
        {
            // When NumberBox gets logical focus, select the text inside us.
            // If we're an editable NumberBox, forward focus to the TextBox element
            if (e.Handled ||
                !(sender is NumericUpDown numericUpDown))
                return;

            if (!numericUpDown.InterceptManualEnter &&
                !numericUpDown.IsReadOnly ||
                !numericUpDown.Focusable ||
                e.OriginalSource != numericUpDown)
                return;

            // MoveFocus takes a TraversalRequest as its argument.
            var request = new TraversalRequest((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift ? FocusNavigationDirection.Previous : FocusNavigationDirection.Next);
            // Gets the element with keyboard focus.
            // Change keyboard focus.
            if (Keyboard.FocusedElement is UIElement elementWithFocus)
                elementWithFocus.MoveFocus(request);
            else
                numericUpDown.Focus();

            e.Handled = true;
        }

        private static void OnCulturePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue ||
                !(d is NumericUpDown numUpDown))
                return;

            numUpDown.OnValueChanged(numUpDown.Value, numUpDown.Value);
        }

        private static void OnIsReadOnlyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue ||
                !(d is NumericUpDown numUpDown))
                return;

            var isReadOnly = (bool)e.NewValue;
            numUpDown.ToggleReadOnlyMode(isReadOnly || !numUpDown.InterceptManualEnter);
        }

        private static void OnInterceptManualEnterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue ||
                !(d is NumericUpDown numUpDown))
                return;

            var interceptManualEnter = (bool)e.NewValue;
            numUpDown.ToggleReadOnlyMode(!interceptManualEnter || numUpDown.IsReadOnly);
        }

        private static void IntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is NumericUpDown numericUpDown))
                return;

            numericUpDown.ResetInternal();
        }

        private static void OnDelayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is NumericUpDown numericUpDown))
                return;

            numericUpDown.RaiseChangeDelay();
            numericUpDown.OnDelayChanged((int)e.OldValue, (int)e.NewValue);
        }

        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is NumericUpDown numericUpDown))
                return;

            numericUpDown.CoerceValue(ValueProperty);
            numericUpDown.OnMaximumChanged((double)e.OldValue, (double)e.NewValue);
            numericUpDown.EnableDisableUpDown();
        }

        private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is NumericUpDown numericUpDown))
                return;

            numericUpDown.CoerceValue(MaximumProperty);
            numericUpDown.CoerceValue(ValueProperty);
            numericUpDown.OnMinimumChanged((double)e.OldValue, (double)e.NewValue);
            numericUpDown.EnableDisableUpDown();
        }

        private static void OnSpeedupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is NumericUpDown numericUpDown))
                return;

            numericUpDown.OnSpeedupChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        private static void OnStringFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is NumericUpDown numericUpDown))
                return;

            if (numericUpDown._valueTextBox != null && numericUpDown.Value.HasValue)
                numericUpDown.InternalSetText(numericUpDown.Value);

            var value = (string)e.NewValue;

            if (!numericUpDown.NumericInputMode.HasFlag(NumericInput.Decimal) &&
                !string.IsNullOrEmpty(value) &&
                RegexStringFormatHexadecimal.IsMatch(value))
                numericUpDown.SetCurrentValue(NumericInputModeProperty, numericUpDown.NumericInputMode | NumericInput.Decimal);
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is NumericUpDown numericUpDown))
                return;

            numericUpDown.OnValueChanged((double?)e.OldValue, (double?)e.NewValue);
        }

        private static void OnNumericInputModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is NumericUpDown numericUpDown) ||
                e.NewValue == e.OldValue || !(e.NewValue is NumericInput) || numericUpDown.Value == null)
                return;

            var numericInput = (NumericInput)e.NewValue;

            if (!numericInput.HasFlag(NumericInput.Decimal))
                numericUpDown.Value = System.Math.Truncate(numericUpDown.Value.GetValueOrDefault());
        }

        private static void OnSnapToMultipleOfIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is NumericUpDown numericUpDown))
                return;

            var value = numericUpDown.Value.GetValueOrDefault();

            if ((bool)e.NewValue && System.Math.Abs(numericUpDown.Interval) > 0)
                numericUpDown.Value = System.Math.Round(value / numericUpDown.Interval) * numericUpDown.Interval;
        }

        #endregion callback

        #region coercers

        private static object CoerceStringFormat(DependencyObject d, object basevalue) => basevalue ?? string.Empty;

        private static object CoerceMaximum(DependencyObject d, object value)
        {
            double minimum = ((NumericUpDown)d).Minimum;
            double val = (double)value;
            return val < minimum ? minimum : val;
        }

        private static object CoerceValue(DependencyObject d, object value)
        {
            if (value == null)
                return null;

            var numericUpDown = (NumericUpDown)d;
            double val = ((double?)value).Value;

            if (!numericUpDown.NumericInputMode.HasFlag(NumericInput.Decimal))
                val = System.Math.Truncate(val);

            if (val < numericUpDown.Minimum)
                return numericUpDown.Minimum;

            if (val > numericUpDown.Maximum)
                return numericUpDown.Maximum;

            return val;
        }

        #endregion coercers

        /// <summary>
        ///     When overridden in a derived class, is invoked whenever application code or internal processes call
        ///     <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _repeatUp = this.GetTemplateChild<RepeatButton>(ElementNumericUp);
            _repeatDown = this.GetTemplateChild<RepeatButton>(ElementNumericDown);
            _valueTextBox = this.GetTemplateChild<TextBox>(ElementTextBox);

            ToggleReadOnlyMode(IsReadOnly | !InterceptManualEnter);

            _repeatUp.Click += OnRepeatUpClick;
            _repeatUp.PreviewMouseUp += OnRepeatUpPreviewMouseUp;

            _repeatDown.Click += OnRepeatDownClick;
            _repeatDown.PreviewMouseUp += OnRepeatDownPreviewMouseUp;

            OnValueChanged(Value, Value);

            _scrollViewer = TryFindScrollViewer();
        }

        private void OnRepeatUpPreviewMouseUp(object sender, MouseButtonEventArgs e) => ResetInternal();
        private void OnRepeatDownPreviewMouseUp(object sender, MouseButtonEventArgs e) => ResetInternal();

        private void OnRepeatDownClick(object sender, RoutedEventArgs e) => ChangeValueWithSpeedUp(false);
        private void OnRepeatUpClick(object sender, RoutedEventArgs e) => ChangeValueWithSpeedUp(true);

        private void ToggleReadOnlyMode(bool isReadOnly)
        {
            if (_repeatUp == null || _repeatDown == null || _valueTextBox == null)
                return;

            if (isReadOnly)
            {
                _valueTextBox.LostFocus -= OnTextBoxLostFocus;
                _valueTextBox.PreviewTextInput -= OnPreviewTextInput;
                _valueTextBox.PreviewKeyDown -= OnTextBoxKeyDown;
                _valueTextBox.TextChanged -= OnTextChanged;
                DataObject.RemovePastingHandler(_valueTextBox, OnValueTextBoxPaste);
            }
            else
            {
                _valueTextBox.LostFocus += OnTextBoxLostFocus;
                _valueTextBox.PreviewTextInput += OnPreviewTextInput;
                _valueTextBox.PreviewKeyDown += OnTextBoxKeyDown;
                _valueTextBox.TextChanged += OnTextChanged;
                DataObject.AddPastingHandler(_valueTextBox, OnValueTextBoxPaste);
            }
        }

        public void SelectAll()
        {
            if (_valueTextBox == null)
                return;

            _valueTextBox.SelectAll();
        }

        protected virtual void OnDelayChanged(int oldDelay, int newDelay)
        {
            if (oldDelay == newDelay)
                return;

            if (_repeatDown != null)
                _repeatDown.Delay = newDelay;

            if (_repeatUp != null)
                _repeatUp.Delay = newDelay;
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

            if (InterceptMouseWheel && (IsFocused || _valueTextBox.IsFocused || TrackMouseWheelWhenMouseOver))
            {
                bool increment = e.Delta > 0;
                _manualChange = false;
                ChangeValueInternal(increment);
            }

            if (_scrollViewer != null && _handlesMouseWheelScrolling.Value != null)
            {
                if (TrackMouseWheelWhenMouseOver)
                    _handlesMouseWheelScrolling.Value.SetValue(_scrollViewer, true, null);
                else if (InterceptMouseWheel)
                    _handlesMouseWheelScrolling.Value.SetValue(_scrollViewer, _valueTextBox.IsFocused, null);
                else
                    _handlesMouseWheelScrolling.Value.SetValue(_scrollViewer, true, null);
            }
        }

        protected void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = true;
            if (string.IsNullOrWhiteSpace(e.Text) || e.Text.Length != 1)
                return;

            TextBox textBox = (TextBox)sender;
            CultureInfo equivalentCulture = SpecificCultureInfo;
            NumberFormatInfo numberFormatInfo = equivalentCulture.NumberFormat;

            string text = e.Text;

            if (char.IsDigit(text[0]))
            {
                var i = textBox.SelectionStart + textBox.SelectionLength;
                if (textBox.Text.IndexOf(numberFormatInfo.NegativeSign, i, StrComp) < 0 &&
                    textBox.Text.IndexOf(numberFormatInfo.PositiveSign, i, StrComp) < 0)
                    e.Handled = false;
            }
            else
            {
                bool allTextSelected = textBox.SelectedText == textBox.Text;

                if (numberFormatInfo.NumberDecimalSeparator == text)
                {
                    if ((textBox.Text.All(i => i.ToString(equivalentCulture) != numberFormatInfo.NumberDecimalSeparator) || allTextSelected) &&
                        NumericInputMode.HasFlag(NumericInput.Decimal))
                        e.Handled = false;
                }
                else
                {
                    if (numberFormatInfo.NegativeSign == text ||
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
                                {
                                    e.Handled = false;
                                }
                            }
                        }
                        else if (textBox.SelectionStart > 0)
                        {
                            string elementBeforeCaret = textBox.Text.ElementAt(textBox.SelectionStart - 1).ToString(equivalentCulture);
                            if (elementBeforeCaret.Equals(ScientificNotationChar, StrComp) && NumericInputMode.HasFlag(NumericInput.Decimal))
                                e.Handled = false;
                        }
                    }
                    else if (text.Equals(ScientificNotationChar, StrComp) &&
                             NumericInputMode.HasFlag(NumericInput.Decimal) &&
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
        protected virtual void OnValueChanged(double? oldValue, double? newValue)
        {
            if (!_manualChange)
            {
                if (!newValue.HasValue)
                {
                    if (_valueTextBox != null)
                        _valueTextBox.Text = null;

                    if (oldValue != newValue)
                        RaiseEvent(new RoutedPropertyChangedEventArgs<double?>(oldValue, newValue, ValueChangedEvent));

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
                        RaiseEvent(new RoutedEventArgs(MinimumReachedEvent));
                }

                if (newValue >= Maximum)
                {
                    if (_repeatUp != null)
                        _repeatUp.IsEnabled = false;

                    ResetInternal();

                    if (IsLoaded)
                        RaiseEvent(new RoutedEventArgs(MaximumReachedEvent));
                }

                if (_valueTextBox != null)
                    InternalSetText(newValue);
            }

            if (oldValue != newValue)
                RaiseEvent(new RoutedPropertyChangedEventArgs<double?>(oldValue, newValue, ValueChangedEvent));
        }

        private static bool ValidateDelay(object value) => Convert.ToInt32(value) >= 0;

        private void InternalSetText(double? newValue)
        {
            if (!newValue.HasValue)
            {
                _valueTextBox.Text = null;
                return;
            }

            _valueTextBox.Text = FormattedValue(newValue, StringFormat, SpecificCultureInfo);

            if ((bool)GetValue(TextBoxHelper.IsMonitoringProperty))
                SetValue(TextBoxHelper.TextLengthProperty, _valueTextBox.Text.Length);
        }

        private string FormattedValue(double? newValue, string format, CultureInfo culture)
        {
            format = format.Replace("{}", string.Empty);
            if (string.IsNullOrWhiteSpace(format))
                return newValue.Value.ToString(culture);

            var match = RegexStringFormatHexadecimal.Match(format);
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

        private ScrollViewer TryFindScrollViewer()
        {
            _valueTextBox.ApplyTemplate();

            var scrollViewer = _valueTextBox.Template.FindName("PART_ContentHost", _valueTextBox) as ScrollViewer;
            if (scrollViewer != null)
                _handlesMouseWheelScrolling = new Lazy<PropertyInfo>(() => _scrollViewer.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance).SingleOrDefault(i => i.Name == "HandlesMouseWheelScrolling"));

            return scrollViewer;
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
                double d = Interval * _internalLargeChange;
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

            NumericUpDownChangedRoutedEventArgs routedEvent = interval > 0 ?
                new NumericUpDownChangedRoutedEventArgs(ValueIncrementedEvent, interval) :
                new NumericUpDownChangedRoutedEventArgs(ValueDecrementedEvent, interval);

            RaiseEvent(routedEvent);

            if (!routedEvent.Handled)
            {
                ChangeValueBy(routedEvent.Interval);
                _valueTextBox.CaretIndex = _valueTextBox.Text.Length;
            }
        }

        private void ChangeValueBy(double difference)
        {
            var newValue = Value.GetValueOrDefault() + difference;
            SetCurrentValue(ValueProperty, CoerceValue(this, newValue));
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

            if (!NumericInputMode.HasFlag(NumericInput.Decimal) || e.Key != Key.Decimal && e.Key != Key.OemPeriod)
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

                if (ValidateText(tb.Text, out double convertedValue))
                {
                    if (SnapToMultipleOfInterval && System.Math.Abs(Interval) > 0)
                        convertedValue = System.Math.Round(convertedValue / Interval) * Interval;

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
            else if (_manualChange && ValidateText(((TextBox)sender).Text, out double convertedValue))
                SetCurrentValue(ValueProperty, convertedValue);
        }

        private void OnValueTextBoxPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (!(sender is TextBox textBox))
                return;

            string textPresent = textBox.Text;

            var isText = e.SourceDataObject.GetDataPresent(DataFormats.Text, true);
            if (!isText)
            {
                e.CancelCommand();
                return;
            }

            var text = e.SourceDataObject.GetData(DataFormats.Text) as string;

            string newText = string.Concat(textPresent.Substring(0, textBox.SelectionStart), text, textPresent.Substring(textBox.SelectionStart + textBox.SelectionLength));
            if (!ValidateText(newText, out _))
                e.CancelCommand();
            else
                _manualChange = true;
        }

        private void RaiseChangeDelay() => RaiseEvent(new RoutedEventArgs(DelayChangedEvent));

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
            var matches = RegexStringFormatNumber.Matches(text);

            if (matches.Count > 0)
                return matches[0].Value;

            return text;
        }

        #endregion methods


        #region EVENTS

        public event RoutedPropertyChangedEventHandler<double?> ValueChanged
        {
            add => AddHandler(ValueChangedEvent, value);
            remove => RemoveHandler(ValueChangedEvent, value);
        }

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

        public event NumbericUpDownChangedRoutedEventHandler ValueIncremented
        {
            add => AddHandler(ValueIncrementedEvent, value);
            remove => RemoveHandler(ValueIncrementedEvent, value);
        }

        public event NumbericUpDownChangedRoutedEventHandler ValueDecremented
        {
            add => AddHandler(ValueDecrementedEvent, value);
            remove => RemoveHandler(ValueDecrementedEvent, value);
        }

        public event RoutedEventHandler DelayChanged
        {
            add => AddHandler(DelayChangedEvent, value);
            remove => RemoveHandler(DelayChangedEvent, value);
        }

        #endregion EVENTS
    }
}
