using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Animation;

namespace WSharp.Wpf.Controls
{
    [TemplatePart(Name = BadgeContainerPartName, Type = typeof(UIElement))]
    public class Badged : BadgedEx
    {
        public static readonly DependencyProperty BadgeChangedStoryboardProperty = DependencyProperty.Register(
            nameof(BadgeChangedStoryboard),
            typeof(Storyboard),
            typeof(Badged),
            new PropertyMetadata(default(Storyboard)));

        public static readonly DependencyProperty BadgeColorZoneModeProperty = DependencyProperty.Register(
            nameof(BadgeColorZoneMode),
            typeof(EColorZoneMode),
            typeof(Badged),
            new PropertyMetadata(default(EColorZoneMode)));

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            nameof(CornerRadius),
            typeof(CornerRadius),
            typeof(Badged),
            new PropertyMetadata(new CornerRadius(9)));

        static Badged()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Badged), new FrameworkPropertyMetadata(typeof(Badged)));
        }

        public Storyboard BadgeChangedStoryboard
        {
            get => (Storyboard)GetValue(BadgeChangedStoryboardProperty);
            set => SetValue(BadgeChangedStoryboardProperty, value);
        }

        public EColorZoneMode BadgeColorZoneMode
        {
            get => (EColorZoneMode)GetValue(BadgeColorZoneModeProperty);
            set => SetValue(BadgeColorZoneModeProperty, value);
        }

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public override void OnApplyTemplate()
        {
            BadgeChanged -= OnBadgeChanged;

            base.OnApplyTemplate();

            BadgeChanged += OnBadgeChanged;
        }

        private void OnBadgeChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var sb = BadgeChangedStoryboard;
            if (badgeContainer != null && sb != null)
            {
                try
                {
                    badgeContainer.BeginStoryboard(sb);
                }
                catch (Exception exc)
                {
                    Trace.WriteLine("Error during Storyboard execution, exception will be rethrown.");
                    Trace.WriteLine($"{exc.Message} ({exc.GetType().FullName})");
                    Trace.WriteLine(exc.StackTrace);

                    throw;
                }
            }
        }
    }
}
