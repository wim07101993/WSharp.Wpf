using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WSharp.Wpf.Controls
{
    [TemplatePart(Name = BadgeContainerPartName, Type = typeof(UIElement))]
    public class BadgedEx : ContentControl
    {
        public const string BadgeContainerPartName = "PART_BadgeContainer";
        protected FrameworkElement badgeContainer;

        public static readonly RoutedEvent BadgeChangedEvent = EventManager.RegisterRoutedEvent(
            nameof(BadgeChanged),
            RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<object>),
            typeof(BadgedEx));

        public static readonly DependencyProperty BadgeProperty = DependencyProperty.Register(
            nameof(Badge),
            typeof(object),
            typeof(BadgedEx),
            new FrameworkPropertyMetadata(default, FrameworkPropertyMetadataOptions.AffectsArrange, OnBadgeChanged));

        public static readonly DependencyProperty BadgeBackgroundProperty = DependencyProperty.Register(
            nameof(BadgeBackground),
            typeof(Brush),
            typeof(BadgedEx),
            new PropertyMetadata(default(Brush)));

        public static readonly DependencyProperty BadgeForegroundProperty = DependencyProperty.Register(
            nameof(BadgeForeground),
            typeof(Brush),
            typeof(BadgedEx),
            new PropertyMetadata(default(Brush)));

        public static readonly DependencyProperty BadgePlacementModeProperty = DependencyProperty.Register(
            nameof(BadgePlacementMode),
            typeof(EBadgePlacementMode),
            typeof(BadgedEx),
            new PropertyMetadata(default(EBadgePlacementMode)));

        private static readonly DependencyPropertyKey isBadgeSetPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(IsBadgeSet),
            typeof(bool),
            typeof(BadgedEx),
            new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty IsBadgeSetProperty = isBadgeSetPropertyKey.DependencyProperty;

        public object Badge
        {
            get => GetValue(BadgeProperty);
            set => SetValue(BadgeProperty, value);
        }

        public Brush BadgeBackground
        {
            get => (Brush)GetValue(BadgeBackgroundProperty);
            set => SetValue(BadgeBackgroundProperty, value);
        }

        public Brush BadgeForeground
        {
            get => (Brush)GetValue(BadgeForegroundProperty);
            set => SetValue(BadgeForegroundProperty, value);
        }

        public EBadgePlacementMode BadgePlacementMode
        {
            get => (EBadgePlacementMode)GetValue(BadgePlacementModeProperty);
            set => SetValue(BadgePlacementModeProperty, value);
        }

        public bool IsBadgeSet
        {
            get => (bool)GetValue(IsBadgeSetProperty);
            private set => SetValue(isBadgeSetPropertyKey, value);
        }

        private static void OnBadgeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is BadgedEx badge))
                return;

            badge.IsBadgeSet = !string.IsNullOrWhiteSpace(e.NewValue as string) || (e.NewValue != null && !(e.NewValue is string));

            var args = new RoutedPropertyChangedEventArgs<object>(e.OldValue, e.NewValue)
            {
                RoutedEvent = BadgeChangedEvent
            };
            badge.RaiseEvent(args);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            badgeContainer = GetTemplateChild(BadgeContainerPartName) as FrameworkElement;
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var result = base.ArrangeOverride(arrangeBounds);

            if (badgeContainer == null)
                return result;

            var containerDesiredSize = badgeContainer.DesiredSize;

            if ((containerDesiredSize.Width <= 0.0 || containerDesiredSize.Height <= 0.0) &&
                !double.IsNaN(badgeContainer.ActualWidth) &&
                !double.IsInfinity(badgeContainer.ActualWidth) &&
                !double.IsNaN(badgeContainer.ActualHeight) &&
                !double.IsInfinity(badgeContainer.ActualHeight))
                containerDesiredSize = new Size(badgeContainer.ActualWidth, badgeContainer.ActualHeight);

            var h = 0 - (containerDesiredSize.Width / 2);
            var v = 0 - (containerDesiredSize.Height / 2);
            badgeContainer.Margin = new Thickness(0);
            badgeContainer.Margin = new Thickness(h, v, h, v);

            return result;
        }

        public event RoutedPropertyChangedEventHandler<object> BadgeChanged
        {
            add { AddHandler(BadgeChangedEvent, value); }
            remove { RemoveHandler(BadgeChangedEvent, value); }
        }
    }
}
