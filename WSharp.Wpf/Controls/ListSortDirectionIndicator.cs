using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace WSharp.Wpf.Controls
{
    [TemplateVisualState(GroupName = DirectionGroupName, Name = NoneStateName)]
    [TemplateVisualState(GroupName = DirectionGroupName, Name = AscendingStateName)]
    [TemplateVisualState(GroupName = DirectionGroupName, Name = DescendingStateName)]
    public class ListSortDirectionIndicator : Control
    {
        public const string DirectionGroupName = "Direction";
        public const string NoneStateName = "None";
        public const string AscendingStateName = "Ascending";
        public const string DescendingStateName = "Descending";

        public static readonly DependencyProperty ListSortDirectionProperty = DependencyProperty.Register(
            nameof(ListSortDirection),
            typeof(ListSortDirection?),
            typeof(ListSortDirectionIndicator),
            new PropertyMetadata(default(ListSortDirection?), ListSortDirectionPropertyChangedCallback));

        private static readonly DependencyPropertyKey isNeutralPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(IsNeutral),
            typeof(bool),
            typeof(ListSortDirectionIndicator),
            new PropertyMetadata(true));

        public static readonly DependencyProperty IsNeutralProperty = isNeutralPropertyKey.DependencyProperty;

        static ListSortDirectionIndicator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ListSortDirectionIndicator), new FrameworkPropertyMetadata(typeof(ListSortDirectionIndicator)));
        }

        public ListSortDirection? ListSortDirection
        {
            get => (ListSortDirection?)GetValue(ListSortDirectionProperty);
            set => SetValue(ListSortDirectionProperty, value);
        }

        public bool IsNeutral
        {
            get => (bool)GetValue(IsNeutralProperty);
            private set => SetValue(isNeutralPropertyKey, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            GotoVisualState(true, ListSortDirection);
        }

        private void GotoVisualState(bool useTransitions, ListSortDirection? direction)
        {
            var stateName = direction.HasValue
                ? (direction.Value == System.ComponentModel.ListSortDirection.Ascending
                    ? AscendingStateName
                    : DescendingStateName)
                : NoneStateName;

            _ = VisualStateManager.GoToState(this, stateName, useTransitions);
        }

        private static void ListSortDirectionPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ListSortDirectionIndicator indicator))
                return;

            indicator.GotoVisualState(true, indicator.ListSortDirection);
            indicator.IsNeutral = !indicator.ListSortDirection.HasValue;
        }
    }
}
