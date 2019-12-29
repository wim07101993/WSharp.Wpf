using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

using MaterialDesignColors.ColorManipulation;

using WSharp.Wpf.Extensions;

namespace WSharp.Wpf.Controls
{
    [TemplatePart(Name = HueSliderPartName, Type = typeof(Slider))]
    [TemplatePart(Name = SaturationBrightnessPickerPartName, Type = typeof(Canvas))]
    [TemplatePart(Name = SaturationBrightnessPickerThumbPartName, Type = typeof(Thumb))]
    public class ColorPicker : Control
    {
        public const string HueSliderPartName = "PART_HueSlider";
        public const string SaturationBrightnessPickerPartName = "PART_SaturationBrightnessPicker";
        public const string SaturationBrightnessPickerThumbPartName = "PART_SaturationBrightnessPickerThumb";

        private Thumb _saturationBrightnessThumb;
        private Canvas _saturationBrightnessCanvas;
        private Slider _hueSlider;
        private bool _inCallback;

        public static readonly RoutedEvent ColorChangedEvent = EventManager.RegisterRoutedEvent(
                nameof(Color),
                RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<Color>),
                typeof(ColorPicker));

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
            nameof(Color),
            typeof(Color),
            typeof(ColorPicker),
            new FrameworkPropertyMetadata(default(Color),
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                ColorPropertyChangedCallback));

        internal static readonly DependencyProperty HsbProperty = DependencyProperty.Register(
            nameof(Hsb),
            typeof(Hsb),
            typeof(ColorPicker),
            new FrameworkPropertyMetadata(default(Hsb),
                FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                HsbPropertyChangedCallback));

        public static readonly DependencyProperty HueSliderPositionProperty = DependencyProperty.Register(
            nameof(HueSliderPosition),
            typeof(Dock),
            typeof(ColorPicker),
            new PropertyMetadata(Dock.Bottom));

        static ColorPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPicker), new FrameworkPropertyMetadata(typeof(ColorPicker)));
        }

        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        internal Hsb Hsb
        {
            get => (Hsb)GetValue(HsbProperty);
            set => SetValue(HsbProperty, value);
        }

        public Dock HueSliderPosition
        {
            get => (Dock)GetValue(HueSliderPositionProperty);
            set => SetValue(HueSliderPositionProperty, value);
        }

        public override void OnApplyTemplate()
        {
            if (_saturationBrightnessCanvas != null)
            {
                _saturationBrightnessCanvas.MouseDown -= SaturationBrightnessCanvasMouseDown;
                _saturationBrightnessCanvas.MouseMove -= SaturationBrightnessCanvasMouseMove;
                _saturationBrightnessCanvas.MouseUp -= SaturationBrightnessCanvasMouseUp;
            }

            if (this.TryGetTemplateChild(SaturationBrightnessPickerPartName, out _saturationBrightnessCanvas))
            {
                _saturationBrightnessCanvas.MouseDown += SaturationBrightnessCanvasMouseDown;
                _saturationBrightnessCanvas.MouseMove += SaturationBrightnessCanvasMouseMove;
                _saturationBrightnessCanvas.MouseUp += SaturationBrightnessCanvasMouseUp;
            }

            if (_saturationBrightnessThumb != null)
                _saturationBrightnessThumb.DragDelta -= SaturationBrightnessThumbDragDelta;

            if (this.TryGetTemplateChild(SaturationBrightnessPickerThumbPartName, out _saturationBrightnessThumb))
                _saturationBrightnessThumb.DragDelta += SaturationBrightnessThumbDragDelta;

            if (_hueSlider != null)
                _hueSlider.ValueChanged -= HueSliderOnValueChanged;

            if (this.TryGetTemplateChild(HueSliderPartName, out _hueSlider))
                _hueSlider.ValueChanged += HueSliderOnValueChanged;

            base.OnApplyTemplate();
        }

        private void HueSliderOnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!(Hsb is Hsb hsb))
                return;

            Hsb = new Hsb(e.NewValue, hsb.Saturation, hsb.Brightness);
        }

        private void SaturationBrightnessCanvasMouseDown(object sender, MouseButtonEventArgs e) => _saturationBrightnessThumb.CaptureMouse();

        private void SaturationBrightnessCanvasMouseMove(object sender, MouseEventArgs e)
        {
            switch (e.LeftButton)
            {
                case MouseButtonState.Pressed:
                    var position = e.GetPosition(_saturationBrightnessCanvas);
                    ApplyThumbPosition(position.X, position.Y);
                    break;
            }
        }

        private void SaturationBrightnessCanvasMouseUp(object sender, MouseButtonEventArgs e) => _saturationBrightnessThumb.ReleaseMouseCapture();

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var result = base.ArrangeOverride(arrangeBounds);
            SetThumbLeft();
            SetThumbTop();
            return result;
        }

        private void SaturationBrightnessThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            if (!(e.Source is UIElement thumb))
                return;

            var left = Canvas.GetLeft(thumb) + e.HorizontalChange;
            var top = Canvas.GetTop(thumb) + e.VerticalChange;
            ApplyThumbPosition(left, top);
        }

        private void ApplyThumbPosition(double left, double top)
        {
            if (left < 0) left = 0;
            if (top < 0) top = 0;

            if (left > _saturationBrightnessCanvas.ActualWidth)
                left = _saturationBrightnessCanvas.ActualWidth;
            if (top > _saturationBrightnessCanvas.ActualHeight)
                top = _saturationBrightnessCanvas.ActualHeight;

            var saturation = 1 / (_saturationBrightnessCanvas.ActualWidth / left);
            var brightness = 1 - (top / _saturationBrightnessCanvas.ActualHeight);

            SetCurrentValue(HsbProperty, new Hsb(Hsb.Hue, saturation, brightness));
        }

        private void SetThumbLeft()
        {
            if (_saturationBrightnessCanvas == null)
                return;

            var left = _saturationBrightnessCanvas.ActualWidth / (1 / Hsb.Saturation);
            Canvas.SetLeft(_saturationBrightnessThumb, left);
        }

        private void SetThumbTop()
        {
            if (_saturationBrightnessCanvas == null)
                return;

            var top = (1 - Hsb.Brightness) * _saturationBrightnessCanvas.ActualHeight;
            Canvas.SetTop(_saturationBrightnessThumb, top);
        }

        private static void ColorPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var colorPicker = (ColorPicker)d;
            if (colorPicker._inCallback)
                return;

            colorPicker._inCallback = true;
            colorPicker.SetCurrentValue(HsbProperty, ((Color)e.NewValue).ToHsb());
            var args = new RoutedPropertyChangedEventArgs<Color>((Color)e.OldValue, (Color)e.NewValue)
            { 
                RoutedEvent = ColorChangedEvent 
            };

            colorPicker.RaiseEvent(args);
            colorPicker._inCallback = false;
        }

        private static void HsbPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var colorPicker = (ColorPicker)d;
            if (colorPicker._inCallback)
                return;

            colorPicker._inCallback = true;

            var color = default(Color);
            if (e.NewValue is Hsb hsb)
                color = hsb.ToColor();

            colorPicker.SetCurrentValue(ColorProperty, color);
            colorPicker._inCallback = false;
        }
       
        public event RoutedPropertyChangedEventHandler<Color> ColorChanged
        {
            add => AddHandler(ColorChangedEvent, value);
            remove => RemoveHandler(ColorChangedEvent, value);
        }
    }
}
