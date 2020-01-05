using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

using WSharp.Wpf.Extensions;

namespace WSharp.Wpf.Controls
{
    [TemplatePart(Name = Plane3DPartName, Type = typeof(Plane3D))]
    [TemplateVisualState(GroupName = TemplateFlipGroupName, Name = TemplateFlippedStateName)]
    [TemplateVisualState(GroupName = TemplateFlipGroupName, Name = TemplateUnflippedStateName)]
    public class Flipper : Control
    {
        #region FIELDS

        public static RoutedCommand FlipCommand = new RoutedCommand();

        public const string Plane3DPartName = "PART_Plane3D";
        public const string TemplateFlipGroupName = "FlipStates";
        public const string TemplateFlippedStateName = "Flipped";
        public const string TemplateUnflippedStateName = "Unflipped";

        private Plane3D _plane3D;

        #endregion FIELDS

        public static readonly RoutedEvent IsFlippedChangedEvent = EventManager.RegisterRoutedEvent(
                nameof(IsFlipped),
                RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<bool>),
                typeof(Flipper));

        #region DEPENDENCY PROPERTIES

        public static readonly DependencyProperty FrontContentProperty = DependencyProperty.Register(
            nameof(FrontContent),
            typeof(object),
            typeof(Flipper),
            new PropertyMetadata(default(object)));

        public static readonly DependencyProperty FrontContentTemplateProperty = DependencyProperty.Register(
            nameof(FrontContentTemplate),
            typeof(DataTemplate),
            typeof(Flipper),
            new PropertyMetadata(default(DataTemplate)));

        public static readonly DependencyProperty FrontContentTemplateSelectorProperty = DependencyProperty.Register(
            nameof(FrontContentTemplateSelector),
            typeof(DataTemplateSelector),
            typeof(Flipper),
            new PropertyMetadata(default(DataTemplateSelector)));

        public static readonly DependencyProperty FrontContentStringFormatProperty = DependencyProperty.Register(
            nameof(FrontContentStringFormat),
            typeof(string),
            typeof(Flipper),
            new PropertyMetadata(default(string)));

        public static readonly DependencyProperty BackContentProperty = DependencyProperty.Register(
            nameof(BackContent),
            typeof(object),
            typeof(Flipper),
            new PropertyMetadata(default(object)));

        public static readonly DependencyProperty BackContentTemplateProperty = DependencyProperty.Register(
            nameof(BackContentTemplate),
            typeof(DataTemplate),
            typeof(Flipper),
            new PropertyMetadata(default(DataTemplate)));

        public static readonly DependencyProperty BackContentTemplateSelectorProperty = DependencyProperty.Register(
            nameof(BackContentTemplateSelector),
            typeof(DataTemplateSelector),
            typeof(Flipper),
            new PropertyMetadata(default(DataTemplateSelector)));

        public static readonly DependencyProperty BackContentStringFormatProperty = DependencyProperty.Register(
            nameof(BackContentStringFormat),
            typeof(string),
            typeof(Flipper),
            new PropertyMetadata(default(string)));

        public static readonly DependencyProperty IsFlippedProperty = DependencyProperty.Register(
            nameof(IsFlipped),
            typeof(bool),
            typeof(Flipper),
            new PropertyMetadata(default(bool), IsFlippedPropertyChangedCallback));

        #endregion DEPENDENCY PROPERTIES

        static Flipper()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Flipper), new FrameworkPropertyMetadata(typeof(Flipper)));
        }

        public Flipper()
        {
            _ = CommandBindings.Add(new CommandBinding(FlipCommand, FlipHandler));
        }

        #region PROPERTIES

        public object FrontContent
        {
            get => GetValue(FrontContentProperty);
            set => SetValue(FrontContentProperty, value);
        }

        public DataTemplate FrontContentTemplate
        {
            get => (DataTemplate)GetValue(FrontContentTemplateProperty);
            set => SetValue(FrontContentTemplateProperty, value);
        }

        public DataTemplateSelector FrontContentTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(FrontContentTemplateSelectorProperty);
            set => SetValue(FrontContentTemplateSelectorProperty, value);
        }

        public string FrontContentStringFormat
        {
            get => (string)GetValue(FrontContentStringFormatProperty);
            set => SetValue(FrontContentStringFormatProperty, value);
        }

        public object BackContent
        {
            get => (object)GetValue(BackContentProperty);
            set => SetValue(BackContentProperty, value);
        }

        public DataTemplate BackContentTemplate
        {
            get => (DataTemplate)GetValue(BackContentTemplateProperty);
            set => SetValue(BackContentTemplateProperty, value);
        }

        public DataTemplateSelector BackContentTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(BackContentTemplateSelectorProperty);
            set => SetValue(BackContentTemplateSelectorProperty, value);
        }

        public string BackContentStringFormat
        {
            get => (string)GetValue(BackContentStringFormatProperty);
            set => SetValue(BackContentStringFormatProperty, value);
        }

        public bool IsFlipped
        {
            get => (bool)GetValue(IsFlippedProperty);
            set => SetValue(IsFlippedProperty, value);
        }

        #endregion PROPERTIES

        #region METHODS

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            UpdateVisualStates(false);

            _plane3D = this.GetTemplateChild<Plane3D>(Plane3DPartName);
        }

        private void RemeasureDuringFlip()
        {
            //not entirely happy hardcoding this, but I have explored other options I am not happy with, and this will do for now
            const int StoryboardMs = 400;
            const int Granularity = 6;

            var remeasureInterval = new TimeSpan(0, 0, 0, 0, StoryboardMs / Granularity);
            var refreshCount = 0;
            var plane3D = _plane3D;
            if (plane3D == null)
                return;

            DispatcherTimer dt = null;
            dt = new DispatcherTimer(remeasureInterval, DispatcherPriority.Normal,
                (sender, args) =>
                {
                    plane3D.InvalidateMeasure();
                    if (refreshCount++ == Granularity)
                        dt.Stop();
                }, Dispatcher);
            dt.Start();
        }

        private void UpdateVisualStates(bool useTransitions)
        {
            _ = VisualStateManager.GoToState(this, IsFlipped ? TemplateFlippedStateName : TemplateUnflippedStateName,
                useTransitions);
        }

        private void FlipHandler(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs) => SetCurrentValue(IsFlippedProperty, !IsFlipped);

        private static void IsFlippedPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is Flipper flipper))
                return;

            flipper.UpdateVisualStates(true);
            flipper.RemeasureDuringFlip();
            OnIsFlippedChanged(flipper, e);
        }

        private static void OnIsFlippedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is Flipper flipper))
                return;

            var args = new RoutedPropertyChangedEventArgs<bool>((bool)e.OldValue, (bool)e.NewValue)
            {
                RoutedEvent = IsFlippedChangedEvent
            };
            flipper.RaiseEvent(args);
        }

        #endregion METHODS

        public event RoutedPropertyChangedEventHandler<bool> IsFlippedChanged
        {
            add => AddHandler(IsFlippedChangedEvent, value);
            remove => RemoveHandler(IsFlippedChangedEvent, value);
        }
    }
}
