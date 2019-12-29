using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using WSharp.Wpf.Extensions;
using WSharp.Wpf.Helpers;

namespace WSharp.Wpf.Controls
{
    [TemplatePart(Name = PopupPartName, Type = typeof(Popup))]
    [TemplatePart(Name = PopupPartName, Type = typeof(ContentControl))]
    [TemplatePart(Name = ContentCoverGridName, Type = typeof(Grid))]
    [TemplateVisualState(GroupName = "PopupStates", Name = OpenStateName)]
    [TemplateVisualState(GroupName = "PopupStates", Name = ClosedStateName)]
    public class DialogHost : ContentControl
    {
        #region FIELDS

        public const string PopupPartName = "PART_Popup";
        public const string PopupContentPartName = "PART_PopupContentElement";
        public const string ContentCoverGridName = "PART_ContentCoverGrid";
        public const string OpenStateName = "Open";
        public const string ClosedStateName = "Closed";

        /// <summary>
        ///     Routed command to be used somewhere inside an instance to trigger showing of the
        ///     dialog. Content can be passed to the dialog via a <see cref="Button.CommandParameter"/>.
        /// </summary>
        public static RoutedCommand OpenDialogCommand = new RoutedCommand();

        /// <summary>
        ///     Routed command to be used inside dialog content to close a dialog. Use a
        ///     <see cref="Button.CommandParameter"/> to indicate the result of the parameter.
        /// </summary>
        public static RoutedCommand CloseDialogCommand = new RoutedCommand();

        private static readonly HashSet<DialogHost> loadedInstances = new HashSet<DialogHost>();

        private DialogOpenedEventHandler _asyncShowOpenedEventHandler;
        private DialogClosingEventHandler _asyncShowClosingEventHandler;
        private TaskCompletionSource<object> _dialogTaskCompletionSource;

        private Popup _popup;
        private ContentControl _popupContentControl;
        private Grid _contentCoverGrid;
        private DialogOpenedEventHandler _attachedDialogOpenedEventHandler;
        private DialogClosingEventHandler _attachedDialogClosingEventHandler;
        private IInputElement _restoreFocusDialogClose;
        private IInputElement _restoreFocusWindowReactivation;
        private Action _currentSnackbarMessageQueueUnPauseAction;
        private Action _closeCleanUp = () => { };

        #endregion FIELDS

        public static readonly RoutedEvent DialogOpenedEvent = EventManager.RegisterRoutedEvent(
            nameof(DialogOpened),
            RoutingStrategy.Bubble,
            typeof(DialogOpenedEventHandler),
            typeof(DialogHost));

        public static readonly RoutedEvent DialogClosingEvent = EventManager.RegisterRoutedEvent(
            nameof(DialogClosing),
            RoutingStrategy.Bubble,
            typeof(DialogClosingEventHandler),
            typeof(DialogHost));

        #region DEPENDENCY PROPERTIES

        public static readonly DependencyProperty IdentifierProperty = DependencyProperty.Register(
            nameof(Identifier),
            typeof(object), 
            typeof(DialogHost), 
            new PropertyMetadata(default(object)));

        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(
            nameof(IsOpen), 
            typeof(bool), 
            typeof(DialogHost),
            new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, IsOpenPropertyChangedCallback));

        public static readonly DependencyProperty DialogContentProperty = DependencyProperty.Register(
            nameof(DialogContent), 
            typeof(object), 
            typeof(DialogHost),
            new PropertyMetadata(default(object)));

        public static readonly DependencyProperty DialogContentTemplateProperty = DependencyProperty.Register(
            nameof(DialogContentTemplate),
            typeof(DataTemplate), 
            typeof(DialogHost), 
            new PropertyMetadata(default(DataTemplate)));

        public static readonly DependencyProperty DialogContentTemplateSelectorProperty = DependencyProperty.Register(
            nameof(DialogContentTemplateSelector),
            typeof(DataTemplateSelector), 
            typeof(DialogHost),
            new PropertyMetadata(default(DataTemplateSelector)));

        public static readonly DependencyProperty DialogContentStringFormatProperty = DependencyProperty.Register(
            nameof(DialogContentStringFormat), 
            typeof(string),
            typeof(DialogHost), 
            new PropertyMetadata(default(string)));

        public static readonly DependencyProperty DialogMarginProperty = DependencyProperty.Register(
            nameof(DialogMargin),
            typeof(Thickness), 
            typeof(DialogHost),
            new PropertyMetadata(default(Thickness)));

        public static readonly DependencyProperty OpenDialogCommandDataContextSourceProperty = DependencyProperty.Register(
            nameof(OpenDialogCommandDataContextSource),
            typeof(EDialogHostOpenDialogCommandDataContextSource),
            typeof(DialogHost), 
            new PropertyMetadata(default(EDialogHostOpenDialogCommandDataContextSource)));

        public static readonly DependencyProperty CloseOnClickAwayProperty = DependencyProperty.Register(
            nameof(CloseOnClickAway),
            typeof(bool),
            typeof(DialogHost), 
            new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty CloseOnClickAwayParameterProperty = DependencyProperty.Register(
            nameof(CloseOnClickAwayParameter), 
            typeof(object), 
            typeof(DialogHost),
            new PropertyMetadata(default(object)));

        public static readonly DependencyProperty SnackbarMessageQueueProperty = DependencyProperty.Register(
            nameof(SnackbarMessageQueue), 
            typeof(SnackbarMessageQueue), 
            typeof(DialogHost),
            new PropertyMetadata(default(SnackbarMessageQueue), SnackbarMessageQueuePropertyChangedCallback));

        public static readonly DependencyProperty DialogThemeProperty = DependencyProperty.Register(
            nameof(DialogTheme), 
            typeof(EBaseTheme),
            typeof(DialogHost),
            new PropertyMetadata(default(EBaseTheme)));

        public static readonly DependencyProperty PopupStyleProperty = DependencyProperty.Register(
            nameof(PopupStyle), 
            typeof(Style), 
            typeof(DialogHost),
            new PropertyMetadata(default(Style)));

        public static readonly DependencyProperty OverlayBackgroundProperty = DependencyProperty.Register(
            nameof(OverlayBackground), 
            typeof(Brush), 
            typeof(DialogHost),
            new PropertyMetadata(Brushes.Black));

        /// <summary>
        ///     Attached property which can be used on the <see cref="Button"/> which instigated the
        ///     <see cref="OpenDialogCommand"/> to process the event.
        /// </summary>
        public static readonly DependencyProperty DialogOpenedAttachedProperty = DependencyProperty.RegisterAttached(
            "DialogOpenedAttached", 
            typeof(DialogOpenedEventHandler),
            typeof(DialogHost),
            new PropertyMetadata(default(DialogOpenedEventHandler)));

        public static readonly DependencyProperty DialogOpenedCallbackProperty = DependencyProperty.Register(
            nameof(DialogOpenedCallback),
            typeof(DialogOpenedEventHandler),
            typeof(DialogHost),
            new PropertyMetadata(default(DialogOpenedEventHandler)));

        /// <summary>
        ///     Attached property which can be used on the <see cref="Button"/> which instigated the
        ///     <see cref="OpenDialogCommand"/> to process the closing event.
        /// </summary>
        public static readonly DependencyProperty DialogClosingAttachedProperty = DependencyProperty.RegisterAttached(
            "DialogClosingAttached",
            typeof(DialogClosingEventHandler),
            typeof(DialogHost),
            new PropertyMetadata(default(DialogClosingEventHandler)));

        public static readonly DependencyProperty DialogClosingCallbackProperty = DependencyProperty.Register(
            nameof(DialogClosingCallback), 
            typeof(DialogClosingEventHandler),
            typeof(DialogHost),
            new PropertyMetadata(default(DialogClosingEventHandler)));

        #endregion DEPENDENCY PROPERTIES
        static DialogHost()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DialogHost), new FrameworkPropertyMetadata(typeof(DialogHost)));
        }

        public DialogHost()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;

            _ = CommandBindings.Add(new CommandBinding(CloseDialogCommand, CloseDialogHandler, CloseDialogCanExecute));
            _ = CommandBindings.Add(new CommandBinding(OpenDialogCommand, OpenDialogHandler));
        }

        #region PROPERTIES

        /// <summary>
        ///     Identifier which is used in conjunction with <see cref="ShowAsync(object)"/> to determine
        ///     where a dialog should be shown.
        /// </summary>
        public object Identifier
        {
            get => GetValue(IdentifierProperty);
            set => SetValue(IdentifierProperty, value);
        }

        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }

        public object DialogContent
        {
            get => GetValue(DialogContentProperty);
            set => SetValue(DialogContentProperty, value);
        }

        public DataTemplate DialogContentTemplate
        {
            get => (DataTemplate)GetValue(DialogContentTemplateProperty);
            set => SetValue(DialogContentTemplateProperty, value);
        }

        public DataTemplateSelector DialogContentTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(DialogContentTemplateSelectorProperty);
            set => SetValue(DialogContentTemplateSelectorProperty, value);
        }

        public string DialogContentStringFormat
        {
            get => (string)GetValue(DialogContentStringFormatProperty);
            set => SetValue(DialogContentStringFormatProperty, value);
        }

        public Thickness DialogMargin
        {
            get => (Thickness)GetValue(DialogMarginProperty);
            set => SetValue(DialogMarginProperty, value);
        }

        /// <summary>
        ///     Defines how a data context is sourced for a dialog if a
        ///     <see cref="FrameworkElement"/> is passed as the command parameter when using <see cref="DialogHost.OpenDialogCommand"/>.
        /// </summary>
        public EDialogHostOpenDialogCommandDataContextSource OpenDialogCommandDataContextSource
        {
            get => (EDialogHostOpenDialogCommandDataContextSource)GetValue(OpenDialogCommandDataContextSourceProperty);
            set => SetValue(OpenDialogCommandDataContextSourceProperty, value);
        }

        /// <summary>
        ///     Indicates whether the dialog will close if the user clicks off the dialog, on the
        ///     obscured background.
        /// </summary>
        public bool CloseOnClickAway
        {
            get => (bool)GetValue(CloseOnClickAwayProperty);
            set => SetValue(CloseOnClickAwayProperty, value);
        }

        /// <summary>
        ///     Parameter to provide to close handlers if an close due to click away is instigated.
        /// </summary>
        public object CloseOnClickAwayParameter
        {
            get => GetValue(CloseOnClickAwayParameterProperty);
            set => SetValue(CloseOnClickAwayParameterProperty, value);
        }

        /// <summary>Set the theme (light/dark) for the dialog.</summary>
        public EBaseTheme DialogTheme
        {
            get => (EBaseTheme)GetValue(DialogThemeProperty);
            set => SetValue(DialogThemeProperty, value);
        }

        public Style PopupStyle
        {
            get => (Style)GetValue(PopupStyleProperty);
            set => SetValue(PopupStyleProperty, value);
        }

        /// <summary>
        ///     Represents the overlay brush that is used to dim the background behind the dialog
        /// </summary>
        public Brush OverlayBackground
        {
            get => (Brush)GetValue(OverlayBackgroundProperty);
            set => SetValue(OverlayBackgroundProperty, value);
        }

        /// <summary>
        ///     Returns a DialogSession for the currently open dialog for managing it
        ///     programmatically. If no dialog is open, CurrentSession will return null
        /// </summary>
        public DialogSession CurrentSession { get; private set; }

        /// <summary>
        ///     Allows association of a snackbar, so that notifications can be paused whilst a
        ///     dialog is being displayed.
        /// </summary>
        public SnackbarMessageQueue SnackbarMessageQueue
        {
            get => (SnackbarMessageQueue)GetValue(SnackbarMessageQueueProperty);
            set => SetValue(SnackbarMessageQueueProperty, value);
        }

        /// <summary>
        ///     Callback fired when the <see cref="DialogOpened"/> event is fired, allowing the
        ///     event to be processed from a binding/view model.
        /// </summary>
        public DialogOpenedEventHandler DialogOpenedCallback
        {
            get => (DialogOpenedEventHandler)GetValue(DialogOpenedCallbackProperty);
            set => SetValue(DialogOpenedCallbackProperty, value);
        }

        /// <summary>
        ///     Callback fired when the <see cref="DialogClosing"/> event is fired, allowing the
        ///     event to be processed from a binding/view model.
        /// </summary>
        public DialogClosingEventHandler DialogClosingCallback
        {
            get => (DialogClosingEventHandler)GetValue(DialogClosingCallbackProperty);
            set => SetValue(DialogClosingCallbackProperty, value);
        }

        #endregion PROPERTIES

        #region METHODS

        #region .Show overloads

        /// <summary>
        ///     Shows a modal dialog. To use, a <see cref="DialogHost"/> instance must be in a
        ///     visual tree (typically this may be specified towards the root of a Window's XAML).
        /// </summary>
        /// <param name="content">Content to show (can be a control or view model).</param>
        /// <returns>
        ///     Task result is the parameter used to close the dialog, typically what is passed to
        ///     the <see cref="CloseDialogCommand"/> command.
        /// </returns>
        public static async Task<object> ShowAsync(object content) 
            => await ShowAsync(content, null, null);

        /// <summary>
        ///     Shows a modal dialog. To use, a <see cref="DialogHost"/> instance must be in a
        ///     visual tree (typically this may be specified towards the root of a Window's XAML).
        /// </summary>
        /// <param name="content">Content to show (can be a control or view model).</param>
        /// <param name="openedEventHandler">
        ///     Allows access to opened event which would otherwise have been subscribed to on a instance.
        /// </param>
        /// <returns>
        ///     Task result is the parameter used to close the dialog, typically what is passed to
        ///     the <see cref="CloseDialogCommand"/> command.
        /// </returns>
        public static async Task<object> ShowAsync(object content, DialogOpenedEventHandler openedEventHandler) 
            => await ShowAsync(content, null, openedEventHandler, null);

        /// <summary>
        ///     Shows a modal dialog. To use, a <see cref="DialogHost"/> instance must be in a
        ///     visual tree (typically this may be specified towards the root of a Window's XAML).
        /// </summary>
        /// <param name="content">Content to show (can be a control or view model).</param>
        /// <param name="closingEventHandler">
        ///     Allows access to closing event which would otherwise have been subscribed to on a instance.
        /// </param>
        /// <returns>
        ///     Task result is the parameter used to close the dialog, typically what is passed to
        ///     the <see cref="CloseDialogCommand"/> command.
        /// </returns>
        public static async Task<object> ShowAsync(object content, DialogClosingEventHandler closingEventHandler) 
            => await ShowAsync(content, null, null, closingEventHandler);

        /// <summary>
        ///     Shows a modal dialog. To use, a <see cref="DialogHost"/> instance must be in a
        ///     visual tree (typically this may be specified towards the root of a Window's XAML).
        /// </summary>
        /// <param name="content">Content to show (can be a control or view model).</param>
        /// <param name="openedEventHandler">
        ///     Allows access to opened event which would otherwise have been subscribed to on a instance.
        /// </param>
        /// <param name="closingEventHandler">
        ///     Allows access to closing event which would otherwise have been subscribed to on a instance.
        /// </param>
        /// <returns>
        ///     Task result is the parameter used to close the dialog, typically what is passed to
        ///     the <see cref="CloseDialogCommand"/> command.
        /// </returns>
        public static async Task<object> ShowAsync(object content, DialogOpenedEventHandler openedEventHandler, DialogClosingEventHandler closingEventHandler) 
            => await ShowAsync(content, null, openedEventHandler, closingEventHandler);

        /// <summary>
        ///     Shows a modal dialog. To use, a <see cref="DialogHost"/> instance must be in a
        ///     visual tree (typically this may be specified towards the root of a Window's XAML).
        /// </summary>
        /// <param name="content">Content to show (can be a control or view model).</param>
        /// <param name="dialogIdentifier">
        ///     <see cref="Identifier"/> of the instance where the dialog should be shown. Typically
        ///     this will match an identifer set in XAML. <c>null</c> is allowed.
        /// </param>
        /// <returns>
        ///     Task result is the parameter used to close the dialog, typically what is passed to
        ///     the <see cref="CloseDialogCommand"/> command.
        /// </returns>
        public static async Task<object> ShowAsync(object content, object dialogIdentifier) 
            => await ShowAsync(content, dialogIdentifier, null, null);

        /// <summary>
        ///     Shows a modal dialog. To use, a <see cref="DialogHost"/> instance must be in a
        ///     visual tree (typically this may be specified towards the root of a Window's XAML).
        /// </summary>
        /// <param name="content">Content to show (can be a control or view model).</param>
        /// <param name="dialogIdentifier">
        ///     <see cref="Identifier"/> of the instance where the dialog should be shown. Typically
        ///     this will match an identifer set in XAML. <c>null</c> is allowed.
        /// </param>
        /// <param name="openedEventHandler">
        ///     Allows access to opened event which would otherwise have been subscribed to on a instance.
        /// </param>
        /// <returns>
        ///     Task result is the parameter used to close the dialog, typically what is passed to
        ///     the <see cref="CloseDialogCommand"/> command.
        /// </returns>
        public static Task<object> Show(object content, object dialogIdentifier, DialogOpenedEventHandler openedEventHandler) 
            => ShowAsync(content, dialogIdentifier, openedEventHandler, null);

        /// <summary>
        ///     Shows a modal dialog. To use, a <see cref="DialogHost"/> instance must be in a
        ///     visual tree (typically this may be specified towards the root of a Window's XAML).
        /// </summary>
        /// <param name="content">Content to show (can be a control or view model).</param>
        /// <param name="dialogIdentifier">
        ///     <see cref="Identifier"/> of the instance where the dialog should be shown. Typically
        ///     this will match an identifer set in XAML. <c>null</c> is allowed.
        /// </param>
        /// <param name="closingEventHandler">
        ///     Allows access to closing event which would otherwise have been subscribed to on a instance.
        /// </param>
        /// <returns>
        ///     Task result is the parameter used to close the dialog, typically what is passed to
        ///     the <see cref="CloseDialogCommand"/> command.
        /// </returns>
        public static Task<object> Show(object content, object dialogIdentifier, DialogClosingEventHandler closingEventHandler) 
            => ShowAsync(content, dialogIdentifier, null, closingEventHandler);

        /// <summary>
        ///     Shows a modal dialog. To use, a <see cref="DialogHost"/> instance must be in a
        ///     visual tree (typically this may be specified towards the root of a Window's XAML).
        /// </summary>
        /// <param name="content">Content to show (can be a control or view model).</param>
        /// <param name="dialogIdentifier">
        ///     <see cref="Identifier"/> of the instance where the dialog should be shown. Typically
        ///     this will match an identifer set in XAML. <c>null</c> is allowed.
        /// </param>
        /// <param name="openedEventHandler">
        ///     Allows access to opened event which would otherwise have been subscribed to on a instance.
        /// </param>
        /// <param name="closingEventHandler">
        ///     Allows access to closing event which would otherwise have been subscribed to on a instance.
        /// </param>
        /// <returns>
        ///     Task result is the parameter used to close the dialog, typically what is passed to
        ///     the <see cref="CloseDialogCommand"/> command.
        /// </returns>
        public static async Task<object> ShowAsync(object content, object dialogIdentifier, DialogOpenedEventHandler openedEventHandler, DialogClosingEventHandler closingEventHandler)
        {
            if (content == null) 
                throw new ArgumentNullException(nameof(content));

            if (loadedInstances.Count == 0)
                throw new InvalidOperationException("No loaded DialogHost instances.");

            loadedInstances.First().Dispatcher.VerifyAccess();

            var targets = loadedInstances
                .Where(dh => dialogIdentifier == null || Equals(dh.Identifier, dialogIdentifier))
                .ToList();

            if (targets.Count == 0)
                throw new InvalidOperationException($"No loaded DialogHost have an {nameof(Identifier)} property matching {nameof(dialogIdentifier)} argument.");
            if (targets.Count > 1)
                throw new InvalidOperationException("Multiple viable DialogHosts.  Specify a unique Identifier on each DialogHost, especially where multiple Windows are a concern.");

            return await targets[0].ShowInternalAsync(content, openedEventHandler, closingEventHandler);
        }

        internal async Task<object> ShowInternalAsync(object content, DialogOpenedEventHandler openedEventHandler, DialogClosingEventHandler closingEventHandler)
        {
            if (IsOpen)
                throw new InvalidOperationException("DialogHost is already open.");

            _dialogTaskCompletionSource = new TaskCompletionSource<object>();

            AssertTargetableContent();
            DialogContent = content;
            _asyncShowOpenedEventHandler = openedEventHandler;
            _asyncShowClosingEventHandler = closingEventHandler;
            SetCurrentValue(IsOpenProperty, true);

            var result = await _dialogTaskCompletionSource.Task;

            _asyncShowOpenedEventHandler = null;
            _asyncShowClosingEventHandler = null;

            return result;
        }

        #endregion .Show overloads

        public override void OnApplyTemplate()
        {
            if (_contentCoverGrid != null)
                _contentCoverGrid.MouseLeftButtonUp -= ContentCoverGridOnMouseLeftButtonUp;

            _popup = GetTemplateChild(PopupPartName) as Popup;
            _popupContentControl = GetTemplateChild(PopupContentPartName) as ContentControl;
            _contentCoverGrid = GetTemplateChild(ContentCoverGridName) as Grid;

            if (_contentCoverGrid != null)
                _contentCoverGrid.MouseLeftButtonUp += ContentCoverGridOnMouseLeftButtonUp;

            _ = VisualStateManager.GoToState(this, SelectState(), false);

            base.OnApplyTemplate();
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window != null && !window.IsActive)
                _ = window.Activate();
            base.OnPreviewMouseDown(e);
        }

        protected void OnDialogOpened(DialogOpenedEventArgs e) => RaiseEvent(e);

        protected void OnDialogClosing(DialogClosingEventArgs e) => RaiseEvent(e);

        internal void AssertTargetableContent()
        {
            var existingBinding = BindingOperations.GetBindingExpression(this, DialogContentProperty);
            if (existingBinding != null)
                throw new InvalidOperationException("Content cannot be passed to a dialog via the OpenDialog if DialogContent already has a binding.");
        }

        internal void Close(object parameter)
        {
            var dialogClosingEventArgs = new DialogClosingEventArgs(CurrentSession, parameter, DialogClosingEvent);

            CurrentSession.IsEnded = true;

            //multiple ways of calling back that the dialog is closing:
            // * routed event
            // * the attached property (which should be applied to the button which opened the dialog
            // * straight forward dependency property
            // * handler provided to the async show method
            OnDialogClosing(dialogClosingEventArgs);
            _attachedDialogClosingEventHandler?.Invoke(this, dialogClosingEventArgs);
            DialogClosingCallback?.Invoke(this, dialogClosingEventArgs);
            _asyncShowClosingEventHandler?.Invoke(this, dialogClosingEventArgs);

            if (dialogClosingEventArgs.IsCancelled)
                CurrentSession.IsEnded = false;
            else
            {
                _ = _dialogTaskCompletionSource?.TrySetResult(parameter);
                SetCurrentValue(IsOpenProperty, false);
            }
        }

        /// <summary>Attempts to focus the content of a popup.</summary>
        /// <returns>The popup content.</returns>
        internal UIElement FocusPopup()
        {
            var child = _popup?.Child;
            if (child == null) 
                return null;

            CommandManager.InvalidateRequerySuggested();
            var focusable = child.VisualDepthFirstTraversal().OfType<UIElement>().FirstOrDefault(ui => ui.Focusable && ui.IsVisible);
            _ = focusable?.Dispatcher.InvokeAsync(() =>
              {
                  if (!focusable.Focus()) return;
                  _ = focusable.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
              }, DispatcherPriority.Background);

            return child;
        }

        private void ContentCoverGridOnMouseLeftButtonUp(object sender, MouseButtonEventArgs args)
        {
            if (CloseOnClickAway && CurrentSession != null)
                Close(CloseOnClickAwayParameter);
        }

        private void OpenDialogHandler(object sender, ExecutedRoutedEventArgs args)
        {
            if (args.Handled) return;

            if (args.OriginalSource is DependencyObject dependencyObject)
            {
                _attachedDialogOpenedEventHandler = GetDialogOpenedAttached(dependencyObject);
                _attachedDialogClosingEventHandler = GetDialogClosingAttached(dependencyObject);
            }

            if (args.Parameter != null)
            {
                AssertTargetableContent();

                if (_popupContentControl != null)
                {
                    _popupContentControl.DataContext = OpenDialogCommandDataContextSource switch
                    {
                        EDialogHostOpenDialogCommandDataContextSource.SenderElement => (args.OriginalSource as FrameworkElement)?.DataContext,
                        EDialogHostOpenDialogCommandDataContextSource.DialogHostInstance => DataContext,
                        EDialogHostOpenDialogCommandDataContextSource.None => null,
                        _ => throw new ArgumentOutOfRangeException(),
                    };
                }

                DialogContent = args.Parameter;
            }

            SetCurrentValue(IsOpenProperty, true);

            args.Handled = true;
        }

        private void CloseDialogCanExecute(object sender, CanExecuteRoutedEventArgs args) => args.CanExecute = CurrentSession != null;

        private void CloseDialogHandler(object sender, ExecutedRoutedEventArgs args)
        {
            if (args.Handled) return;

            Close(args.Parameter);

            args.Handled = true;
        }

        private string SelectState() => IsOpen ? OpenStateName : ClosedStateName;

        private void OnUnloaded(object sender, RoutedEventArgs args)
        {
            _ = loadedInstances.Remove(this);
            SetCurrentValue(IsOpenProperty, false);
        }

        private void OnLoaded(object sender, RoutedEventArgs args) => loadedInstances.Add(this);

        private void WindowOnDeactivated(object sender, EventArgs args)
        {
            _restoreFocusWindowReactivation = _popup != null 
                ? FocusManager.GetFocusedElement((Window)sender) 
                : null;
        }

        private void WindowOnActivated(object sender, EventArgs args)
        {
            if (_restoreFocusWindowReactivation == null)
                return;

            _ = Dispatcher.BeginInvoke(new Action(() => Keyboard.Focus(_restoreFocusWindowReactivation)));
        }

        private static void WatchWindowActivation(DialogHost dialogHost)
        {
            var window = Window.GetWindow(dialogHost);
            if (window != null)
            {
                window.Activated += dialogHost.WindowOnActivated;
                window.Deactivated += dialogHost.WindowOnDeactivated;
                dialogHost._closeCleanUp = () =>
                {
                    window.Activated -= dialogHost.WindowOnActivated;
                    window.Deactivated -= dialogHost.WindowOnDeactivated;
                };
            }
            else
            {
                dialogHost._closeCleanUp = () => { };
            }
        }

        private static void IsOpenPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is DialogHost dialogHost))
                return;

            if (dialogHost._popupContentControl != null)
                ValidationHelper.SetSuppress(dialogHost._popupContentControl, !dialogHost.IsOpen);

            _ = VisualStateManager.GoToState(dialogHost, dialogHost.SelectState(), !TransitionHelper.GetDisableTransitions(dialogHost));

            if (dialogHost.IsOpen)
            {
                WatchWindowActivation(dialogHost);
                dialogHost._currentSnackbarMessageQueueUnPauseAction = dialogHost.SnackbarMessageQueue?.Pause();
            }
            else
            {
                dialogHost._attachedDialogClosingEventHandler = null;
                if (dialogHost._currentSnackbarMessageQueueUnPauseAction != null)
                {
                    dialogHost._currentSnackbarMessageQueueUnPauseAction();
                    dialogHost._currentSnackbarMessageQueueUnPauseAction = null;
                }

                dialogHost.CurrentSession.IsEnded = true;
                dialogHost.CurrentSession = null;
                dialogHost._closeCleanUp();
                //NB: _dialogTaskCompletionSource is only set in the case where the dialog is shown with Show
                //To get into this case you need to display the dialog with Show and then hide it by setting IsOpen to false
                //Setting this here ensures the other
                _ = dialogHost._dialogTaskCompletionSource?.TrySetResult(null);

                // Don't attempt to Invoke if _restoreFocusDialogClose hasn't been assigned yet. Can
                // occur if the MainWindow has started up minimized. Even when Show() has been
                // called, this doesn't seem to have been set.
                _ = dialogHost.Dispatcher.InvokeAsync(() => dialogHost._restoreFocusDialogClose?.Focus(), DispatcherPriority.Input);

                return;
            }

            dialogHost.CurrentSession = new DialogSession(dialogHost);
            var window = Window.GetWindow(dialogHost);
            dialogHost._restoreFocusDialogClose = window != null ? FocusManager.GetFocusedElement(window) : null;

            //multiple ways of calling back that the dialog has opened:
            // * routed event
            // * the attached property (which should be applied to the button which opened the dialog
            // * straight forward dependency property
            // * handler provided to the async show method
            var dialogOpenedEventArgs = new DialogOpenedEventArgs(dialogHost.CurrentSession, DialogOpenedEvent);
            dialogHost.OnDialogOpened(dialogOpenedEventArgs);
            dialogHost._attachedDialogOpenedEventHandler?.Invoke(dialogHost, dialogOpenedEventArgs);
            dialogHost.DialogOpenedCallback?.Invoke(dialogHost, dialogOpenedEventArgs);
            dialogHost._asyncShowOpenedEventHandler?.Invoke(dialogHost, dialogOpenedEventArgs);

            _ = dialogHost.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                CommandManager.InvalidateRequerySuggested();
                var child = dialogHost.FocusPopup();

                if (child != null)
                {
                    //https://github.com/ButchersBoy/MaterialDesignInXamlToolkit/issues/187
                    //totally not happy about this, but on immediate validation we can get some weird looking stuff...give WPF a kick to refresh...
                    _ = Task.Delay(300)
                            .ContinueWith(t => child.Dispatcher.BeginInvoke(new Action(() => child.InvalidateVisual())));
                }
            }));
        }

        private static void SnackbarMessageQueuePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is DialogHost dialogHost))
                return;

            if (dialogHost._currentSnackbarMessageQueueUnPauseAction != null)
            {
                dialogHost._currentSnackbarMessageQueueUnPauseAction();
                dialogHost._currentSnackbarMessageQueueUnPauseAction = null;
            }

            if (!dialogHost.IsOpen) 
                return;

            if (!(e.NewValue is SnackbarMessageQueue snackbarMessageQueue))
                return;

            dialogHost._currentSnackbarMessageQueueUnPauseAction = snackbarMessageQueue?.Pause();
        }

        public static void SetDialogOpenedAttached(DependencyObject d, DialogOpenedEventHandler e) => d.SetValue(DialogOpenedAttachedProperty, e);

        public static DialogOpenedEventHandler GetDialogOpenedAttached(DependencyObject d) => (DialogOpenedEventHandler)d.GetValue(DialogOpenedAttachedProperty);

        public static void SetDialogClosingAttached(DependencyObject d, DialogClosingEventHandler e) => d.SetValue(DialogClosingAttachedProperty, e);

        public static DialogClosingEventHandler GetDialogClosingAttached(DependencyObject d) => (DialogClosingEventHandler)d.GetValue(DialogClosingAttachedProperty);

        #endregion METHODS

        /// <summary>Raised just before a dialog is closed.</summary>
        public event DialogClosingEventHandler DialogClosing
        {
            add => AddHandler(DialogClosingEvent, value);
            remove => RemoveHandler(DialogClosingEvent, value);
        }

        /// <summary>Raised when a dialog is opened.</summary>
        public event DialogOpenedEventHandler DialogOpened
        {
            add => AddHandler(DialogOpenedEvent, value);
            remove => RemoveHandler(DialogOpenedEvent, value);
        }
    }
}
