using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

using WSharp.Wpf.Extensions;

namespace WSharp.Wpf.Controls
{
    [TemplatePart(Name = DeleteButtonPartName, Type = typeof(Button))]
    public class Chip : ButtonBase
    {
        private ButtonBase _deleteButton;

        public const string DeleteButtonPartName = "PART_DeleteButton";

        #region DEPENDENCY PROPERTIES

        /// <summary>Event correspond to delete button left mouse button click</summary>
        public static readonly RoutedEvent DeleteClickEvent = EventManager.RegisterRoutedEvent(
            nameof(DeleteClick),
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(Chip));

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            nameof(Icon),
            typeof(object),
            typeof(Chip),
            new PropertyMetadata(default(object)));

        public static readonly DependencyProperty IconBackgroundProperty = DependencyProperty.Register(
            nameof(IconBackground),
            typeof(Brush),
            typeof(Chip),
            new PropertyMetadata(default(Brush)));

        public static readonly DependencyProperty IconForegroundProperty = DependencyProperty.Register(
            nameof(IconForeground),
            typeof(Brush),
            typeof(Chip),
            new PropertyMetadata(default(Brush)));

        public static readonly DependencyProperty IsDeletableProperty = DependencyProperty.Register(
            nameof(IsDeletable),
            typeof(bool),
            typeof(Chip),
            new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty DeleteCommandProperty = DependencyProperty.Register(
            nameof(DeleteCommand),
            typeof(ICommand),
            typeof(Chip),
            new PropertyMetadata(default(ICommand)));

        public static readonly DependencyProperty DeleteCommandParameterProperty = DependencyProperty.Register(
            nameof(DeleteCommandParameter),
            typeof(object),
            typeof(Chip),
            new PropertyMetadata(default(object)));

        public static readonly DependencyProperty DeleteToolTipProperty = DependencyProperty.Register(
            nameof(DeleteToolTip),
            typeof(object),
            typeof(Chip),
            new PropertyMetadata(default(object)));

        #endregion DEPENDENCY PROPERTIES

        static Chip()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Chip), new FrameworkPropertyMetadata(typeof(Chip)));
        }

        #region PROPERTIES

        public object Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public Brush IconBackground
        {
            get => (Brush)GetValue(IconBackgroundProperty);
            set => SetValue(IconBackgroundProperty, value);
        }

        public Brush IconForeground
        {
            get => (Brush)GetValue(IconForegroundProperty);
            set => SetValue(IconForegroundProperty, value);
        }

        /// <summary>Indicates if the delete button should be visible.</summary>
        public bool IsDeletable
        {
            get => (bool)GetValue(IsDeletableProperty);
            set => SetValue(IsDeletableProperty, value);
        }

        public ICommand DeleteCommand
        {
            get => (ICommand)GetValue(DeleteCommandProperty);
            set => SetValue(DeleteCommandProperty, value);
        }

        public object DeleteCommandParameter
        {
            get => GetValue(DeleteCommandParameterProperty);
            set => SetValue(DeleteCommandParameterProperty, value);
        }

        public object DeleteToolTip
        {
            get => GetValue(DeleteToolTipProperty);
            set => SetValue(DeleteToolTipProperty, value);
        }

        #endregion PROPERTIES

        public override void OnApplyTemplate()
        {
            if (_deleteButton != null)
                _deleteButton.Click -= DeleteButtonOnClick;

            _deleteButton = this.GetTemplateChild<ButtonBase>(DeleteButtonPartName);
            if (_deleteButton != null)
                _deleteButton.Click += DeleteButtonOnClick;

            base.OnApplyTemplate();
        }

        protected virtual void OnDeleteClick()
        {
            var newEvent = new RoutedEventArgs(DeleteClickEvent, this);
            RaiseEvent(newEvent);

            var command = DeleteCommand;
            if (command != null && command.CanExecute(DeleteCommandParameter))
                command.Execute(DeleteCommandParameter);
        }

        private void DeleteButtonOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            OnDeleteClick();
            routedEventArgs.Handled = true;
        }

        /// <summary>Add / Remove DeleteClickEvent handler</summary>
        [Category("Behavior")]
        public event RoutedEventHandler DeleteClick { add { AddHandler(DeleteClickEvent, value); } remove { RemoveHandler(DeleteClickEvent, value); } }
    }
}
