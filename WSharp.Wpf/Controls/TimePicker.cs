using System;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using WSharp.Extensions;
using WSharp.Wpf.Extensions;

namespace WSharp.Wpf.Controls
{
    [TemplatePart(Name = ButtonPartName, Type = typeof(Button))]
    [TemplatePart(Name = PopupPartName, Type = typeof(Popup))]
    [TemplatePart(Name = TextBoxPartName, Type = typeof(TextBox))]
    public class TimePicker : ValueControl<DateTime?>
    {
        public const string ButtonPartName = "PART_Button";
        public const string PopupPartName = "PART_Popup";
        public const string TextBoxPartName = "PART_TextBox";

        #region dependency properties

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(TimePicker),
            new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, TextPropertyChangedCallback));

        public static readonly DependencyProperty SelectedTimeFormatProperty = DependencyProperty.Register(
            nameof(SelectedTimeFormat),
            typeof(DatePickerFormat),
            typeof(TimePicker),
            new PropertyMetadata(DatePickerFormat.Short));

        public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register(
            nameof(IsDropDownOpen),
            typeof(bool),
            typeof(TimePicker),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsDropDownOpenChanged, OnCoerceIsDropDownOpen));

        public static readonly DependencyProperty Is24HoursProperty = DependencyProperty.Register(
            nameof(Is24Hours),
            typeof(bool),
            typeof(TimePicker),
            new PropertyMetadata(default(bool), Is24HoursChanged));

        public static readonly DependencyProperty ClockStyleProperty = DependencyProperty.Register(
            nameof(ClockStyle),
            typeof(Style),
            typeof(TimePicker),
            new PropertyMetadata(default(Style)));

        public static readonly DependencyProperty ClockHostContentControlStyleProperty = DependencyProperty.Register(
            nameof(ClockHostContentControlStyle),
            typeof(Style),
            typeof(TimePicker),
            new PropertyMetadata(default(Style)));

        public static readonly DependencyProperty IsInvalidTextAllowedProperty = DependencyProperty.Register(
            nameof(IsInvalidTextAllowed),
            typeof(bool),
            typeof(TimePicker),
            new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty WithSecondsProperty = DependencyProperty.Register(
            nameof(WithSeconds),
            typeof(bool),
            typeof(TimePicker),
            new PropertyMetadata(default(bool), WithSecondsPropertyChanged));

        #endregion dependency properties

        private readonly ContentControl _clockHostContentControl;
        private readonly Clock _clock;

        private TextBox _textBox;
        private Popup _popup;
        private Button _dropDownButton;
        private bool _disablePopupReopen;
        private DateTime? _lastValidTime;
        private bool _isManuallyMutatingText;

        static TimePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimePicker), new FrameworkPropertyMetadata(typeof(TimePicker)));
        }

        public TimePicker()
        {
            _clock = new Clock
            {
                DisplayAutomation = EClockDisplayAutomation.ToMinutesOnly
            };
            _clockHostContentControl = new ContentControl
            {
                Content = _clock
            };
            InitializeClock();
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public DatePickerFormat SelectedTimeFormat
        {
            get => (DatePickerFormat)GetValue(SelectedTimeFormatProperty);
            set => SetValue(SelectedTimeFormatProperty, value);
        }

        public bool IsDropDownOpen
        {
            get => (bool)GetValue(IsDropDownOpenProperty);
            set => SetValue(IsDropDownOpenProperty, value);
        }

        public bool Is24Hours
        {
            get => (bool)GetValue(Is24HoursProperty);
            set => SetValue(Is24HoursProperty, value);
        }

        public Style ClockStyle
        {
            get => (Style)GetValue(ClockStyleProperty);
            set => SetValue(ClockStyleProperty, value);
        }

        public Style ClockHostContentControlStyle
        {
            get => (Style)GetValue(ClockHostContentControlStyleProperty);
            set => SetValue(ClockHostContentControlStyleProperty, value);
        }

        /// <summary>
        ///     Set to true to stop invalid text reverting back to previous valid value. Useful in
        ///     cases where you want to display validation messages and allow the user to correct
        ///     the data without it reverting.
        /// </summary>
        public bool IsInvalidTextAllowed
        {
            get => (bool)GetValue(IsInvalidTextAllowedProperty);
            set => SetValue(IsInvalidTextAllowedProperty, value);
        }

        /// <summary>
        ///     Set to true to display seconds in the time and allow the user to select seconds.
        /// </summary>
        public bool WithSeconds
        {
            get => (bool)GetValue(WithSecondsProperty);
            set => SetValue(WithSecondsProperty, value);
        }

        public override void OnApplyTemplate()
        {
            if (_popup != null)
            {
                _popup.RemoveHandler(PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(PopupOnPreviewMouseLeftButtonDown));
                _popup.Opened -= PopupOnOpened;
                _popup.Closed -= PopupOnClosed;
                _popup.Child = null;
            }
            if (_dropDownButton != null)
                _dropDownButton.Click -= DropDownButtonOnClick;

            if (_textBox != null)
            {
                _textBox.RemoveHandler(KeyDownEvent, new KeyEventHandler(TextBoxOnKeyDown));
                _textBox.RemoveHandler(TextBoxBase.TextChangedEvent, new TextChangedEventHandler(TextBoxOnTextChanged));
                _textBox.AddHandler(LostFocusEvent, new RoutedEventHandler(TextBoxOnLostFocus));
            }

            _textBox = this.GetTemplateChild<TextBox>(TextBoxPartName);
            if (_textBox != null)
            {
                _textBox.AddHandler(KeyDownEvent, new KeyEventHandler(TextBoxOnKeyDown));
                _textBox.AddHandler(TextBoxBase.TextChangedEvent, new TextChangedEventHandler(TextBoxOnTextChanged));
                _textBox.AddHandler(LostFocusEvent, new RoutedEventHandler(TextBoxOnLostFocus));
                _textBox.Text = Text;
            }

            _popup = this.GetTemplateChild<Popup>(PopupPartName);
            if (_popup != null)
            {
                _popup.AddHandler(PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(PopupOnPreviewMouseLeftButtonDown));
                _popup.Opened += PopupOnOpened;
                _popup.Closed += PopupOnClosed;
                _popup.Child = _clockHostContentControl;
                if (IsDropDownOpen)
                    _popup.IsOpen = true;
            }

            _dropDownButton = this.GetTemplateChild<Button>(ButtonPartName);
            if (_dropDownButton != null)
                _dropDownButton.Click += DropDownButtonOnClick;

            base.OnApplyTemplate();
        }

        protected override void OnValueChanged(DateTime? oldValue, DateTime? newValue)
        {
            _isManuallyMutatingText = true;
            SetCurrentValue(TextProperty, DateTimeToString(Value));
            _isManuallyMutatingText = false;
            _lastValidTime = Value;
            RaiseValueChanged(oldValue, newValue);
        }

        private void SetInvalidTime()
        {
            if (IsInvalidTextAllowed) return;

            if (_lastValidTime != null)
            {
                //SetCurrentValue(SelectedTimeProperty, _lastValidTime.Value);
                //_textBox.Text = _lastValidTime.Value.ToString(_lastValidTime.Value.Hour % 12 > 9 ? "hh:mm tt" : "h:mm tt");
                _textBox.Text = DateTimeToString(_lastValidTime.Value, DatePickerFormat.Short);
            }
            else
            {
                SetCurrentValue(ValueProperty, null);
                _textBox.Text = "";
            }
        }

        private void SetSelectedTime(bool beCautious = false)
        {
            if (string.IsNullOrEmpty(_textBox?.Text))
                SetCurrentValue(ValueProperty, null);
            else
            {
                ParseTime(_textBox.Text, t =>
                {
                    if (!beCautious || DateTimeToString(t) == _textBox.Text)
                        SetCurrentValue(ValueProperty, t);
                });
            }
        }

        private void ParseTime(string s, Action<DateTime> successContinuation)
        {
            if (IsTimeValid(s, out DateTime time))
                successContinuation(time);
        }

        private bool IsTimeValid(string s, out DateTime time)
        {
            CultureInfo culture = Language.GetSpecificCulture();

            return DateTime.TryParse(s,
                culture,
                DateTimeStyles.AssumeLocal | DateTimeStyles.AllowWhiteSpaces,
                out time);
        }

        private string DateTimeToString(DateTime? d) => d.HasValue ? DateTimeToString(d.Value) : null;

        private string DateTimeToString(DateTime d) => DateTimeToString(d, SelectedTimeFormat);

        private string DateTimeToString(DateTime datetime, DatePickerFormat format)
        {
            CultureInfo culture = Language.GetSpecificCulture();
            DateTimeFormatInfo dtfi = culture.GetDateFormat();

            string hourFormatChar = Is24Hours ? "H" : "h";

            var sb = new StringBuilder();
            sb.Append(hourFormatChar);
            if (format == DatePickerFormat.Long)
                sb.Append(hourFormatChar);

            sb.Append(dtfi.TimeSeparator);
            sb.Append("mm");
            if (WithSeconds)
            {
                sb.Append(dtfi.TimeSeparator);
                sb.Append("ss");
            }

            if (!Is24Hours && (!string.IsNullOrEmpty(dtfi.AMDesignator) || !string.IsNullOrEmpty(dtfi.PMDesignator)))
                sb.Append(" tt");

            return datetime.ToString(sb.ToString(), culture);
        }

        private void InitializeClock()
        {
            _clock.AddHandler(Clock.ClockChoiceMadeEvent, new ClockChoiceMadeEventHandler(ClockChoiceMadeHandler));
            _clock.SetBinding(ForegroundProperty, this.CreateBinding(ForegroundProperty));
            _clock.SetBinding(StyleProperty, this.CreateBinding(ClockStyleProperty));
            _clock.SetBinding(Clock.ValueProperty, this.CreateBinding(ValueProperty, converter: new NullableDateTimeToDateTimeConverter()));
            _clock.SetBinding(Clock.Is24HoursProperty, this.CreateBinding(Is24HoursProperty));
            _clockHostContentControl.SetBinding(StyleProperty, this.CreateBinding(ClockHostContentControlStyleProperty));
        }

        private void TogglePopup()
        {
            if (IsDropDownOpen)
                SetCurrentValue(IsDropDownOpenProperty, false);
            else
            {
                if (_disablePopupReopen)
                    _disablePopupReopen = false;
                else
                {
                    SetSelectedTime();
                    SetCurrentValue(IsDropDownOpenProperty, true);
                }
            }
        }

        #region event handlers

        private void TextBoxOnLostFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            if (string.IsNullOrEmpty(_textBox?.Text))
            {
                SetCurrentValue(ValueProperty, null);
                return;
            }

            if (IsTimeValid(_textBox.Text, out DateTime time))
                SetCurrentValue(ValueProperty, time);
            else // Invalid time, jump back to previous good time
                SetInvalidTime();
        }

        private void TextBoxOnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            keyEventArgs.Handled = ProcessKey(keyEventArgs) || keyEventArgs.Handled;
        }

        private bool ProcessKey(KeyEventArgs keyEventArgs)
        {
            switch (keyEventArgs.Key)
            {
                case Key.System:
                    switch (keyEventArgs.SystemKey)
                    {
                        case Key.Down:
                            if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                            {
                                TogglePopup();
                                return true;
                            }
                            break;
                    }
                    break;

                case Key.Enter:
                    SetSelectedTime();
                    return true;
            }

            return false;
        }

        private void TextBoxOnTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            if (_popup?.IsOpen == true || IsInvalidTextAllowed)
                SetCurrentValue(TextProperty, _textBox.Text);

            if (_popup?.IsOpen == false)
                SetSelectedTime(true);
        }

        private void PopupOnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (!(sender is Popup popup) || popup.StaysOpen) 
                return;

            if (_dropDownButton?.InputHitTest(mouseButtonEventArgs.GetPosition(_dropDownButton)) != null)
            {
                // This popup is being closed by a mouse press on the drop down button The following
                // mouse release will cause the closed popup to immediately reopen. Raise a flag to
                // block reopeneing the popup
                _disablePopupReopen = true;
            }
        }

        private void PopupOnClosed(object sender, EventArgs eventArgs)
        {
            if (IsDropDownOpen)
                SetCurrentValue(IsDropDownOpenProperty, false);

            if (_clock.IsKeyboardFocusWithin)
                MoveFocus(new TraversalRequest(FocusNavigationDirection.First));

            //TODO Clock closed event
            //OnCalendarClosed(new RoutedEventArgs());
        }

        private void PopupOnOpened(object sender, EventArgs eventArgs)
        {
            if (!IsDropDownOpen)
                SetCurrentValue(IsDropDownOpenProperty, true);

            if (_clock != null)
            {
                _clock.DisplayMode = EClockDisplayMode.Hours;
                _clock.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
            }

            //TODO ClockOpenedEvent
            //this.OnCalendarOpened(new RoutedEventArgs());
        }

        private void ClockChoiceMadeHandler(object sender, ClockChoiceMadeEventArgs clockChoiceMadeEventArgs)
        {
            if ((WithSeconds && (clockChoiceMadeEventArgs.Mode == EClockDisplayMode.Seconds)) ||
                (!WithSeconds && (clockChoiceMadeEventArgs.Mode == EClockDisplayMode.Minutes)))
            {
                TogglePopup();
                if (Value == null)
                    Value = _clock.Value;
            }
        }

        private void DropDownButtonOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            TogglePopup();
        }

        #endregion event handlers

        #region callbacks

        private static void TextPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var timePicker = (TimePicker)dependencyObject;
            if (!timePicker._isManuallyMutatingText)
                timePicker.SetSelectedTime();
            if (timePicker._textBox != null)
                timePicker._textBox.Text = dependencyPropertyChangedEventArgs.NewValue as string ?? "";
        }

        private static void Is24HoursChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var timePicker = (TimePicker)dependencyObject;
            timePicker._isManuallyMutatingText = true;
            timePicker.SetCurrentValue(TextProperty, timePicker.DateTimeToString(timePicker.Value));
            timePicker._isManuallyMutatingText = false;
        }

        private static object OnCoerceIsDropDownOpen(DependencyObject d, object baseValue)
        {
            var timePicker = (TimePicker)d;

            if (!timePicker.IsEnabled)
            {
                return false;
            }

            return baseValue;
        }

        /// <summary>IsDropDownOpenProperty property changed handler.</summary>
        /// <param name="d">DatePicker that changed its IsDropDownOpen.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var timePicker = (TimePicker)d;

            var newValue = (bool)e.NewValue;
            if (timePicker._popup == null || timePicker._popup.IsOpen == newValue) return;

            timePicker._popup.IsOpen = newValue;
            if (newValue)
            {
                //TODO set time
                //dp._originalSelectedDate = dp.SelectedDate;

                timePicker.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
                {
                    timePicker._clock.Focus();
                }));
            }
        }

        private static void WithSecondsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is TimePicker picker)
            {
                // update the clock's behavior as needed when the WithSeconds value changes
                picker._clock.DisplayAutomation = picker.WithSeconds ? EClockDisplayAutomation.ToSeconds : EClockDisplayAutomation.ToMinutesOnly;
            }
        }

        #endregion callbacks
    }
}