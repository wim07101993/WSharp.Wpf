using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WSharp.Wpf.Controls
{
    /// <summary>
    ///     A custom control implementing a rating bar. The icon aka content may be set as a
    ///     DataTemplate via the ButtonContentTemplate property.
    /// </summary>
    public class RatingBar : ValueControl<int>
    {
        public static RoutedCommand SelectRatingCommand = new RoutedCommand();

        private readonly ObservableCollection<RatingBarButton> _ratingButtonsInternal = new ObservableCollection<RatingBarButton>();

        #region DEPENDENCY PROPERTIES

        public static readonly DependencyProperty MinProperty = DependencyProperty.Register(
            nameof(Min),
            typeof(int),
            typeof(RatingBar),
            new PropertyMetadata(1, MinPropertyChangedCallback));

        public static readonly DependencyProperty MaxProperty = DependencyProperty.Register(
            nameof(Max),
            typeof(int),
            typeof(RatingBar),
            new PropertyMetadata(5, MaxPropertyChangedCallback));

        public static readonly DependencyProperty ValueItemContainerButtonStyleProperty = DependencyProperty.Register(
            nameof(ValueItemContainerButtonStyle),
            typeof(Style),
            typeof(RatingBar),
            new PropertyMetadata(default(Style)));

        public static readonly DependencyProperty ValueItemTemplateProperty = DependencyProperty.Register(
            nameof(ValueItemTemplate),
            typeof(DataTemplate),
            typeof(RatingBar),
            new PropertyMetadata(default(DataTemplate)));

        public static readonly DependencyProperty ValueItemTemplateSelectorProperty = DependencyProperty.Register(
            nameof(ValueItemTemplateSelector),
            typeof(DataTemplateSelector),
            typeof(RatingBar),
            new PropertyMetadata(default(DataTemplateSelector)));

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            nameof(Orientation),
            typeof(Orientation),
            typeof(RatingBar),
            new PropertyMetadata(default(Orientation)));

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(
            nameof(IsReadOnly),
            typeof(bool),
            typeof(RatingBar),
            new PropertyMetadata(default(bool)));

        #endregion DEPENDENCY PROPERTIES

        static RatingBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RatingBar), new FrameworkPropertyMetadata(typeof(RatingBar)));
        }

        public RatingBar()
        {
            _ = CommandBindings.Add(new CommandBinding(SelectRatingCommand, SelectItemHandler));
            RatingButtons = new ReadOnlyObservableCollection<RatingBarButton>(_ratingButtonsInternal);
        }

        #region PROPERTIES

        public int Min
        {
            get => (int)GetValue(MinProperty);
            set => SetValue(MinProperty, value);
        }

        public int Max
        {
            get => (int)GetValue(MaxProperty);
            set => SetValue(MaxProperty, value);
        }

        public ReadOnlyObservableCollection<RatingBarButton> RatingButtons { get; }

        public Style ValueItemContainerButtonStyle
        {
            get => (Style)GetValue(ValueItemContainerButtonStyleProperty);
            set => SetValue(ValueItemContainerButtonStyleProperty, value);
        }

        public DataTemplate ValueItemTemplate
        {
            get => (DataTemplate)GetValue(ValueItemTemplateProperty);
            set => SetValue(ValueItemTemplateProperty, value);
        }

        public DataTemplateSelector ValueItemTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(ValueItemTemplateSelectorProperty);
            set => SetValue(ValueItemTemplateSelectorProperty, value);
        }

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        #endregion PROPERTIES

        public override void OnApplyTemplate()
        {
            RebuildButtons();

            base.OnApplyTemplate();
        }

        protected override void OnValueChanged(int oldValue, int newValue)
        {
            foreach (var button in RatingButtons)
                button.IsWithinSelectedValue = button.Value <= newValue;

            base.OnValueChanged(oldValue, newValue);
        }

        private void RebuildButtons()
        {
            _ratingButtonsInternal.Clear();
            for (var i = Min; i <= Max; i++)
            {
                _ratingButtonsInternal.Add(new RatingBarButton
                {
                    Content = i,
                    ContentTemplate = ValueItemTemplate,
                    ContentTemplateSelector = ValueItemTemplateSelector,
                    IsWithinSelectedValue = i <= Value,
                    Style = ValueItemContainerButtonStyle,
                    Value = i,
                });
            }
        }

        private void SelectItemHandler(object sender, ExecutedRoutedEventArgs args)
        {
            if (!(args.Parameter is int) || IsReadOnly)
                return;

            Value = (int)args.Parameter;
        }

        private static void MaxPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is RatingBar ratingBar))
                return;

            ratingBar.RebuildButtons();
        }

        private static void MinPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is RatingBar ratingBar))
                return;

            ratingBar.RebuildButtons();
        }
    }
}
