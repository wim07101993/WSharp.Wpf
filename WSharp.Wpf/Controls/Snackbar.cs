using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace WSharp.Wpf.Controls
{
    /// <summary>
    ///     Implements a <see cref="Snackbar"/> inspired by the Material Design specs (https://material.google.com/components/snackbars-toasts.html).
    /// </summary>
    [ContentProperty(nameof(Message))]
    public class Snackbar : Control
    {
        private const string ActivateStoryboardName = "ActivateStoryboard";
        private const string DeactivateStoryboardName = "DeactivateStoryboard";

        private Action _messageQueueRegistrationCleanUp = null;

        #region DEPENDENCY PROPERTIES

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
            nameof(Message),
            typeof(SnackbarMessage),
            typeof(Snackbar),
            new PropertyMetadata(default(SnackbarMessage)));

        public static readonly DependencyProperty MessageQueueProperty = DependencyProperty.Register(
            nameof(MessageQueue),
            typeof(SnackbarMessageQueue),
            typeof(Snackbar), new PropertyMetadata(default(SnackbarMessageQueue), MessageQueuePropertyChangedCallback));

        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
            nameof(IsActive),
            typeof(bool),
            typeof(Snackbar),
            new PropertyMetadata(default(bool), IsActivePropertyChangedCallback));

        public static readonly DependencyProperty ActionButtonStyleProperty = DependencyProperty.Register(
            nameof(ActionButtonStyle),
            typeof(Style),
            typeof(Snackbar),
            new PropertyMetadata(default(Style)));

        #endregion DEPENDENCY PROPERTIES

        public static readonly RoutedEvent IsActiveChangedEvent = EventManager.RegisterRoutedEvent(
            nameof(IsActiveChanged),
            RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<bool>),
            typeof(Snackbar));

        public static readonly RoutedEvent DeactivateStoryboardCompletedEvent = EventManager.RegisterRoutedEvent(
            nameof(DeactivateStoryboardCompleted),
            RoutingStrategy.Bubble,
            typeof(SnackbarMessageEventArgs),
            typeof(Snackbar));

        static Snackbar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Snackbar), new FrameworkPropertyMetadata(typeof(Snackbar)));
        }

        #region PROPERTIES

        public SnackbarMessage Message
        {
            get => (SnackbarMessage)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        public SnackbarMessageQueue MessageQueue
        {
            get => (SnackbarMessageQueue)GetValue(MessageQueueProperty);
            set => SetValue(MessageQueueProperty, value);
        }

        public bool IsActive
        {
            get => (bool)GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }

        public TimeSpan ActivateStoryboardDuration { get; private set; }

        public TimeSpan DeactivateStoryboardDuration { get; private set; }

        public Style ActionButtonStyle
        {
            get => (Style)GetValue(ActionButtonStyleProperty);
            set => SetValue(ActionButtonStyleProperty, value);
        }

        #endregion PROPERTIES

        #region METHODS

        public override void OnApplyTemplate()
        {
            //we regards to notification of deactivate storyboard finishing,
            //we either build a storyboard in code and subscribe to completed event,
            //or take the not 100% proof of the storyboard duration from the storyboard itself
            //...HOWEVER...we can both methods result can work under the same public API so
            //we can flip the implementation if this version doesnt pan out

            //(currently we have no even on the activate animation; don't
            // need it just now, but it would mirror the deactivate)

            ActivateStoryboardDuration = GetStoryboardResourceDuration(ActivateStoryboardName);
            DeactivateStoryboardDuration = GetStoryboardResourceDuration(DeactivateStoryboardName);

            base.OnApplyTemplate();
        }

        private TimeSpan GetStoryboardResourceDuration(string resourceName)
        {
            var storyboard = Template.Resources.Contains(resourceName)
                ? (Storyboard)Template.Resources[resourceName]
                : null;

            return storyboard != null && storyboard.Duration.HasTimeSpan
                ? storyboard.Duration.TimeSpan
                : new Func<TimeSpan>(() =>
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"Warning, no Duration was specified at root of storyboard '{resourceName}'.");
                    return TimeSpan.Zero;
                })();
        }

        private static void MessageQueuePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is Snackbar snackbar))
                return;

            (snackbar._messageQueueRegistrationCleanUp ?? (() => { }))();

            if (e.NewValue is SnackbarMessageQueue messageQueue)
                snackbar._messageQueueRegistrationCleanUp = messageQueue.Pair(snackbar);
        }

        private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is Snackbar instance))
                return;

            var args = new RoutedPropertyChangedEventArgs<bool>((bool)e.OldValue, (bool)e.NewValue) { RoutedEvent = IsActiveChangedEvent };
            instance.RaiseEvent(args);
        }

        private static void OnDeactivateStoryboardCompleted(IInputElement snackbar, SnackbarMessage message)
        {
            var args = new SnackbarMessageEventArgs(DeactivateStoryboardCompletedEvent, message);
            snackbar.RaiseEvent(args);
        }

        private static void IsActivePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            OnIsActiveChanged(d, args);

            if ((bool)args.NewValue || 
                !(d is Snackbar snackbar) || snackbar.Message == null)
                return;

            var dispatcherTimer = new DispatcherTimer
            {
                Tag = new Tuple<Snackbar, SnackbarMessage>(snackbar, snackbar.Message),
                Interval = snackbar.DeactivateStoryboardDuration
            };

            dispatcherTimer.Tick += DeactivateStoryboardDispatcherTimerOnTick;
            dispatcherTimer.Start();
        }

        private static void DeactivateStoryboardDispatcherTimerOnTick(object sender, EventArgs args)
        {
            if (!(sender is DispatcherTimer dispatcherTimer))
                return;

            dispatcherTimer.Stop();
            dispatcherTimer.Tick -= DeactivateStoryboardDispatcherTimerOnTick;
            var source = (Tuple<Snackbar, SnackbarMessage>)dispatcherTimer.Tag;
            OnDeactivateStoryboardCompleted(source.Item1, source.Item2);
        }

        #endregion METHODS

        public event RoutedPropertyChangedEventHandler<SnackbarMessage> DeactivateStoryboardCompleted
        {
            add => AddHandler(DeactivateStoryboardCompletedEvent, value);
            remove => RemoveHandler(DeactivateStoryboardCompletedEvent, value);
        }

        public event RoutedPropertyChangedEventHandler<bool> IsActiveChanged
        {
            add => AddHandler(IsActiveChangedEvent, value);
            remove => RemoveHandler(IsActiveChangedEvent, value);
        }
    }
}
