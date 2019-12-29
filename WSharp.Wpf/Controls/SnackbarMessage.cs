using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

using WSharp.Wpf.Converters;
using WSharp.Wpf.Extensions;

namespace WSharp.Wpf.Controls
{
    /// <summary>
    ///     Defines the content of a message within a <see cref="Snackbar"/>. Primary content should
    ///     be set via the standard <see cref="SnackbarMessage.Content"/> property. Where an action
    ///     is allowed, content can be provided in <see cref="SnackbarMessage.ActionContent"/>.
    ///     Standard button properties are provided for actions, includiing <see cref="SnackbarMessage.ActionCommand"/>.
    /// </summary>
    [TypeConverter(typeof(SnackbarMessageTypeConverter))]
    [TemplatePart(Name = ActionButtonPartName, Type = typeof(ButtonBase))]
    public class SnackbarMessage : ContentControl
    {
        public const string ActionButtonPartName = "PART_ActionButton";
        private Action _templateCleanupAction = () => { };

        /// <summary>Event correspond to left mouse button click on the Action button.</summary>
        public static readonly RoutedEvent ActionClickEvent = EventManager.RegisterRoutedEvent(
            nameof(ActionClick),
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(SnackbarMessage));

        #region DEPENDENCY PROPERTIS

        public static readonly DependencyProperty ActionCommandProperty = DependencyProperty.Register(
            nameof(ActionCommand),
            typeof(ICommand),
            typeof(SnackbarMessage),
            new PropertyMetadata(default(ICommand)));

        public static readonly DependencyProperty ActionCommandParameterProperty = DependencyProperty.Register(
            nameof(ActionCommandParameter),
            typeof(object),
            typeof(SnackbarMessage),
            new PropertyMetadata(default(object)));

        public static readonly DependencyProperty ActionContentProperty = DependencyProperty.Register(
            nameof(ActionContent),
            typeof(object),
            typeof(SnackbarMessage),
            new PropertyMetadata(default(object)));

        public static readonly DependencyProperty ActionContentTemplateProperty = DependencyProperty.Register(
            nameof(ActionContentTemplate),
            typeof(DataTemplate),
            typeof(SnackbarMessage),
            new PropertyMetadata(default(DataTemplate)));

        public static readonly DependencyProperty ActionContentStringFormatProperty = DependencyProperty.Register(
            nameof(ActionContentStringFormat), 
            typeof(string), 
            typeof(SnackbarMessage), 
            new PropertyMetadata(default(string)));

        public static readonly DependencyProperty ActionContentTemplateSelectorProperty = DependencyProperty.Register(
            nameof(ActionContentTemplateSelector), 
            typeof(DataTemplateSelector), 
            typeof(SnackbarMessage), 
            new PropertyMetadata(default(DataTemplateSelector)));

        #endregion DEPENDENCY PROPERTIES

        static SnackbarMessage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SnackbarMessage), new FrameworkPropertyMetadata(typeof(SnackbarMessage)));
        }

        #region PROPERTIES

        public ICommand ActionCommand
        {
            get => (ICommand)GetValue(ActionCommandProperty);
            set => SetValue(ActionCommandProperty, value);
        }

        public object ActionCommandParameter
        {
            get => GetValue(ActionCommandParameterProperty);
            set => SetValue(ActionCommandParameterProperty, value);
        }

        public object ActionContent
        {
            get => GetValue(ActionContentProperty);
            set => SetValue(ActionContentProperty, value);
        }

        public DataTemplate ActionContentTemplate
        {
            get => (DataTemplate)GetValue(ActionContentTemplateProperty);
            set => SetValue(ActionContentTemplateProperty, value);
        }

        public string ActionContentStringFormat
        {
            get => (string)GetValue(ActionContentStringFormatProperty);
            set => SetValue(ActionContentStringFormatProperty, value);
        }

        public DataTemplateSelector ActionContentTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(ActionContentTemplateSelectorProperty);
            set => SetValue(ActionContentTemplateSelectorProperty, value);
        }

        #endregion PROPERTIES

        protected virtual void OnActionClick()
        {
            var newEvent = new RoutedEventArgs(ActionClickEvent, this);
            RaiseEvent(newEvent);
        }

        public override void OnApplyTemplate()
        {
            _templateCleanupAction();

            if (this.TryGetTemplateChild<ButtonBase>(ActionButtonPartName, out var buttonBase))
            {
                buttonBase.Click += ButtonBaseOnClick;
                _templateCleanupAction = () => buttonBase.Click -= ButtonBaseOnClick;
            }
            else
                _templateCleanupAction = () => { };

            base.OnApplyTemplate();
        }

        private void ButtonBaseOnClick(object sender, RoutedEventArgs routedEventArgs) => OnActionClick();

        /// <summary>Add / Remove ActionClickEvent handler</summary>
        [Category("Behavior")]
        public event RoutedEventHandler ActionClick 
        { 
            add => AddHandler(ActionClickEvent, value);
            remove => RemoveHandler(ActionClickEvent, value);
        }
    }
}
