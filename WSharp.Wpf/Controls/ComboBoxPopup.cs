using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using WSharp.Wpf.Extensions;
using WSharp.Wpf.Helpers;

namespace WSharp.Wpf.Controls
{
    public class ComboBoxPopup : Popup
    {
        #region DEPENDENCY PROPERTY

        public static readonly DependencyProperty UpContentTemplateProperty = DependencyProperty.Register(
            nameof(UpContentTemplate),
            typeof(ControlTemplate),
            typeof(ComboBoxPopup),
            new UIPropertyMetadata(null, CreateTemplatePropertyChangedCallback(EComboBoxPopupPlacement.Classic)));

        public static readonly DependencyProperty DownContentTemplateProperty = DependencyProperty.Register(
            nameof(DownContentTemplate),
            typeof(ControlTemplate),
            typeof(ComboBoxPopup),
            new UIPropertyMetadata(null, CreateTemplatePropertyChangedCallback(EComboBoxPopupPlacement.Down)));

        public static readonly DependencyProperty ClassicContentTemplateProperty = DependencyProperty.Register(
            nameof(ClassicContentTemplate),
            typeof(ControlTemplate),
            typeof(ComboBoxPopup),
            new UIPropertyMetadata(null, CreateTemplatePropertyChangedCallback(EComboBoxPopupPlacement.Up)));

        public static readonly DependencyProperty UpVerticalOffsetProperty = DependencyProperty.Register(
            nameof(UpVerticalOffset),
            typeof(double),
            typeof(ComboBoxPopup),
            new PropertyMetadata(0.0));

        public static readonly DependencyProperty DownVerticalOffsetProperty = DependencyProperty.Register(
            nameof(DownVerticalOffset),
            typeof(double),
            typeof(ComboBoxPopup),
            new PropertyMetadata(0.0));

        public static readonly DependencyProperty PopupPlacementProperty = DependencyProperty.Register(
            nameof(PopupPlacement),
            typeof(EComboBoxPopupPlacement),
            typeof(ComboBoxPopup),
            new PropertyMetadata(EComboBoxPopupPlacement.Undefined, PopupPlacementPropertyChangedCallback));

        private static readonly DependencyPropertyKey backgroundPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(Background), 
            typeof(Brush), 
            typeof(ComboBoxPopup),
            new PropertyMetadata(default(Brush)));

        public static readonly DependencyProperty BackgroundProperty = backgroundPropertyKey.DependencyProperty;

        public static readonly DependencyProperty DefaultVerticalOffsetProperty = DependencyProperty.Register(
            nameof(DefaultVerticalOffset),
            typeof(double),
            typeof(ComboBoxPopup),
            new PropertyMetadata(0.0));

        public static readonly DependencyProperty VisiblePlacementWidthProperty = DependencyProperty.Register(
            nameof(VisiblePlacementWidth),
            typeof(double),
            typeof(ComboBoxPopup),
            new PropertyMetadata(0.0));

        public static readonly DependencyProperty ClassicModeProperty = DependencyProperty.Register(
            nameof(ClassicMode),
            typeof(bool),
            typeof(ComboBoxPopup),
            new FrameworkPropertyMetadata(true));

        #endregion DEPENDENCY PROPERTY

        public ControlTemplate UpContentTemplate
        {
            get => (ControlTemplate)GetValue(UpContentTemplateProperty);
            set => SetValue(UpContentTemplateProperty, value);
        }

        public ControlTemplate DownContentTemplate
        {
            get => (ControlTemplate)GetValue(DownContentTemplateProperty);
            set => SetValue(DownContentTemplateProperty, value);
        }

        public ControlTemplate ClassicContentTemplate
        {
            get => (ControlTemplate)GetValue(ClassicContentTemplateProperty);
            set => SetValue(ClassicContentTemplateProperty, value);
        }

        public double UpVerticalOffset
        {
            get => (double)GetValue(UpVerticalOffsetProperty);
            set => SetValue(UpVerticalOffsetProperty, value);
        }

        public double DownVerticalOffset
        {
            get => (double)GetValue(DownVerticalOffsetProperty);
            set => SetValue(DownVerticalOffsetProperty, value);
        }

        public EComboBoxPopupPlacement PopupPlacement
        {
            get => (EComboBoxPopupPlacement)GetValue(PopupPlacementProperty);
            set => SetValue(PopupPlacementProperty, value);
        }

        public Brush Background
        {
            get => (Brush)GetValue(BackgroundProperty);
            private set => SetValue(backgroundPropertyKey, value);
        }

        public double DefaultVerticalOffset
        {
            get => (double)GetValue(DefaultVerticalOffsetProperty);
            set => SetValue(DefaultVerticalOffsetProperty, value);
        }

        public double VisiblePlacementWidth
        {
            get => (double)GetValue(VisiblePlacementWidthProperty);
            set => SetValue(VisiblePlacementWidthProperty, value);
        }

        public bool ClassicMode
        {
            get => (bool)GetValue(ClassicModeProperty);
            set => SetValue(ClassicModeProperty, value);
        }

        public ComboBoxPopup()
        {
            CustomPopupPlacementCallback = ComboBoxCustomPopupPlacementCallback;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == ChildProperty && PopupPlacement != EComboBoxPopupPlacement.Undefined)
                UpdateChildTemplate(PopupPlacement);
        }

        private void SetupBackground(IEnumerable<DependencyObject> visualAncestry)
        {
            var background = visualAncestry
                .Select(v => (v as Control)?.Background ?? (v as Panel)?.Background ?? (v as Border)?.Background)
                .FirstOrDefault(v => v != null && !Equals(v, Brushes.Transparent) && v is SolidColorBrush);

            if (background != null)
                Background = background;
        }

        private void SetupVisiblePlacementWidth(IEnumerable<DependencyObject> visualAncestry)
        {
            var parent = visualAncestry.OfType<Panel>().ElementAt(1);
            VisiblePlacementWidth = TreeHelper.GetVisibleWidth((FrameworkElement)PlacementTarget, parent);
        }

        private CustomPopupPlacement[] ComboBoxCustomPopupPlacementCallback(
            Size popupSize, Size targetSize, Point offset)
        {
            var visualAncestry = PlacementTarget.GetVisualAncestry().ToList();

            SetupBackground(visualAncestry);

            SetupVisiblePlacementWidth(visualAncestry);

            var data = GetPositioningData(visualAncestry, popupSize, targetSize, offset);
            var preferUpIfSafe = data.LocationY + data.PopupSize.Height > data.ScreenHeight;

            if (ClassicMode
                || data.LocationX + data.PopupSize.Width - data.RealOffsetX > data.ScreenWidth
                || data.LocationX - data.RealOffsetX < 0
                || (!preferUpIfSafe && data.LocationY - Math.Abs(data.NewDownY) < 0))
            {
                SetCurrentValue(PopupPlacementProperty, EComboBoxPopupPlacement.Classic);
                return new[] { GetClassicPopupPlacement(this, data) };
            }
            if (preferUpIfSafe)
            {
                SetCurrentValue(PopupPlacementProperty, EComboBoxPopupPlacement.Up);
                return new[] { GetUpPopupPlacement(data) };
            }
            SetCurrentValue(PopupPlacementProperty, EComboBoxPopupPlacement.Down);
            return new[] { GetDownPopupPlacement(data) };
        }

        private void SetChildTemplateIfNeed(ControlTemplate template)
        {
            if (!(Child is ContentControl contentControl)) 
                return;
            //throw new InvalidOperationException($"The type of {nameof(Child)} must be {nameof(ContentControl)}");

            if (!ReferenceEquals(contentControl.Template, template))
                contentControl.Template = template;
        }

        private PositioningData GetPositioningData(IEnumerable<DependencyObject> visualAncestry, Size popupSize, Size targetSize, Point offset)
        {
            var locationFromScreen = PlacementTarget.PointToScreen(new Point(0, 0));

            var mainVisual = visualAncestry.OfType<Visual>().LastOrDefault();
            if (mainVisual == null) throw new ArgumentException($"{nameof(visualAncestry)} must contains unless one {nameof(Visual)} control inside.");

            var screen = Screen.FromPoint(locationFromScreen);
            var screenWidth = (int)DpiHelper.TransformToDeviceX(mainVisual, (int)screen.Bounds.Width);
            var screenHeight = (int)DpiHelper.TransformToDeviceY(mainVisual, (int)screen.Bounds.Height);

            //Adjust the location to be in terms of the current screen
            var locationX = (int)(locationFromScreen.X - screen.Bounds.X) % screenWidth;
            var locationY = (int)(locationFromScreen.Y - screen.Bounds.Y) % screenHeight;

            var upVerticalOffsetIndepent = DpiHelper.TransformToDeviceY(mainVisual, UpVerticalOffset);
            var newUpY = upVerticalOffsetIndepent - popupSize.Height + targetSize.Height;
            var newDownY = DpiHelper.TransformToDeviceY(mainVisual, DownVerticalOffset);

            double offsetX;
            const int RtlHorizontalOffset = 20;

            offsetX = FlowDirection == FlowDirection.LeftToRight
                ? DpiHelper.TransformToDeviceX(mainVisual, offset.X)
                : DpiHelper.TransformToDeviceX(mainVisual, offset.X - targetSize.Width - RtlHorizontalOffset);

            return new PositioningData(
                mainVisual, offsetX,
                newUpY, newDownY,
                popupSize, targetSize,
                locationX, locationY,
                screenHeight, screenWidth);
        }

        private static PropertyChangedCallback CreateTemplatePropertyChangedCallback(EComboBoxPopupPlacement popupPlacement)
        {
            return (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            {
                if (!(d is ComboBoxPopup popup)) 
                    return;

                if (!(e.NewValue is ControlTemplate template)) 
                    return;

                if (popup.PopupPlacement == popupPlacement)
                    popup.SetChildTemplateIfNeed(template);
            };
        }

        private void UpdateChildTemplate(EComboBoxPopupPlacement placement)
        {
            switch (placement)
            {
                case EComboBoxPopupPlacement.Classic:
                    SetChildTemplateIfNeed(ClassicContentTemplate);
                    break;

                case EComboBoxPopupPlacement.Down:
                    SetChildTemplateIfNeed(DownContentTemplate);
                    break;

                case EComboBoxPopupPlacement.Up:
                    SetChildTemplateIfNeed(UpContentTemplate);
                    break;
                    // default: throw new NotImplementedException($"Unexpected value {placement} of
                    // the {nameof(PopupPlacement)} property inside the {nameof(ComboBoxPopup)} control.");
            }
        }

        private static void PopupPlacementPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ComboBoxPopup popup))
                return;

            if (!(e.NewValue is EComboBoxPopupPlacement placement))
                return;

            popup.UpdateChildTemplate(placement);
        }

        private static CustomPopupPlacement GetClassicPopupPlacement(ComboBoxPopup popup, PositioningData data)
        {
            var defaultVerticalOffsetIndepent = DpiHelper.TransformToDeviceY(data.MainVisual, popup.DefaultVerticalOffset);
            var newY = data.LocationY + data.PopupSize.Height > data.ScreenHeight
                ? -(defaultVerticalOffsetIndepent + data.PopupSize.Height)
                : defaultVerticalOffsetIndepent + data.TargetSize.Height;

            return new CustomPopupPlacement(new Point(data.OffsetX, newY), PopupPrimaryAxis.Horizontal);
        }

        private static CustomPopupPlacement GetDownPopupPlacement(PositioningData data) 
            => new CustomPopupPlacement(new Point(data.OffsetX, data.NewDownY), PopupPrimaryAxis.None);

        private static CustomPopupPlacement GetUpPopupPlacement(PositioningData data) 
            => new CustomPopupPlacement(new Point(data.OffsetX, data.NewUpY), PopupPrimaryAxis.None);

        private struct PositioningData
        {
            public Visual MainVisual { get; }
            public double OffsetX { get; }
            public double NewUpY { get; }
            public double NewDownY { get; }
            public double RealOffsetX => (PopupSize.Width - TargetSize.Width) / 2.0;
            public Size PopupSize { get; }
            public Size TargetSize { get; }
            public double LocationX { get; }
            public double LocationY { get; }
            public double ScreenHeight { get; }
            public double ScreenWidth { get; }

            public PositioningData(
                Visual mainVisual, 
                double offsetX, 
                double newUpY, 
                double newDownY, 
                Size popupSize, 
                Size targetSize, 
                double locationX, 
                double locationY, 
                double screenHeight, 
                double screenWidth)
            {
                MainVisual = mainVisual;
                OffsetX = offsetX;
                NewUpY = newUpY;
                NewDownY = newDownY;
                PopupSize = popupSize; TargetSize = targetSize;
                LocationX = locationX; LocationY = locationY;
                ScreenWidth = screenWidth; ScreenHeight = screenHeight;
            }
        }
    }
}
