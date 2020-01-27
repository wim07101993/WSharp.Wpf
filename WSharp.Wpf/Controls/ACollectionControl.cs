using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WSharp.Extensions;
using WSharp.Wpf.Extensions;

namespace WSharp.Wpf.Controls
{
    [TemplatePart(Name = nameof(PartSearchButton), Type = typeof(Button))]
    [TemplatePart(Name = nameof(PartClearFilterButton), Type = typeof(Button))]
    public abstract class ACollectionControl<T> : Control
    {
        public const string PartSearchButton = "SearchButton";
        public const string PartClearFilterButton = "ClearFilterButton";

        protected Button searchButton;
        protected Button clearFilterButton;

        #region ItemsSource

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            nameof(ItemsSource),
            typeof(IEnumerable<T>),
            typeof(ACollectionControl<T>),
            new PropertyMetadata(default(IEnumerable<T>), OnItemsSourceChanged));

        public IEnumerable<T> ItemsSource
        {
            get => (IEnumerable<T>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ACollectionControl<T> collectionView))
                return;

            var oldValue = e.OldValue is IEnumerable<T>
                ? (IEnumerable<T>)e.OldValue
                : default;

            var newValue = e.NewValue is IEnumerable<T>
                ? (IEnumerable<T>)e.NewValue
                : default;

            collectionView.OnItemsSourceChanged(oldValue, newValue);
        }

        protected virtual void OnItemsSourceChanged(IEnumerable<T> oldValue, IEnumerable<T> newValue)
        {
            if (oldValue is INotifyCollectionChanged oldCollection)
                oldCollection.CollectionChanged -= OnItemsSourceCollectionChanged;
            if (newValue is INotifyCollectionChanged newCollection)
                newCollection.CollectionChanged += OnItemsSourceCollectionChanged;

            UpdateFilteredItemsSource();
        }

        #endregion ItemsSource

        #region FilteredItemsSource

        private static readonly DependencyPropertyKey filteredItemsSourcePropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(FilteredItemsSource),
            typeof(IEnumerable<T>),
            typeof(ACollectionControl<T>),
            new PropertyMetadata(default, OnFilteredItemsSourceChanged));

        public static readonly DependencyProperty FilteredItemsSourceProperty = filteredItemsSourcePropertyKey.DependencyProperty;

        public IEnumerable<T> FilteredItemsSource
        {
            get => (IEnumerable<T>)GetValue(FilteredItemsSourceProperty);
            private set => SetValue(filteredItemsSourcePropertyKey, value);
        }

        private static void OnFilteredItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ACollectionControl<T> collectionView))
                return;

            var oldValue = e.OldValue is IEnumerable<T>
                ? (IEnumerable<T>)e.OldValue
                : default;

            var newValue = e.NewValue is IEnumerable<T>
                ? (IEnumerable<T>)e.NewValue
                : default;

            collectionView.OnFilteredItemsSourceChanged(oldValue, newValue);
        }

        protected virtual void OnFilteredItemsSourceChanged(IEnumerable<T> oldValue, IEnumerable<T> newValue)
        {
        }

        #endregion FilteredItemsSource

        #region SelectedItem

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
            nameof(SelectedItem),
            typeof(T),
            typeof(ACollectionControl<T>),
            new PropertyMetadata(default, OnSelectedItemChanged));

        public T SelectedItem
        {
            get => (T)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ACollectionControl<T> collectionView))
                return;

            var oldValue = e.OldValue is T
                ? (T)e.OldValue
                : default;

            var newValue = e.NewValue is T
                ? (T)e.NewValue
                : default;

            collectionView.OnSelectedItemChanged(oldValue, newValue);
        }

        protected virtual void OnSelectedItemChanged(T oldItem, T newItem)
            => RaiseSelectedItemChanged(oldItem, newItem);

        #endregion SelectedItem

        #region SelectedItemChanged

        public static readonly RoutedEvent SelectedItemChangedEvent = EventManager.RegisterRoutedEvent(
            nameof(SelectedItemChanged),
            RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<T>),
            typeof(ACollectionControl<T>));

        public event RoutedPropertyChangedEventHandler<T> SelectedItemChanged
        {
            add => AddHandler(SelectedItemChangedEvent, value);
            remove => RemoveHandler(SelectedItemChangedEvent, value);
        }

        protected void RaiseSelectedItemChanged(T oldValue, T newValue)
            => RaiseEvent(new RoutedPropertyChangedEventArgs<T>(oldValue, newValue, SelectedItemChangedEvent));

        #endregion SelectedItemChanged

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.TryGetTemplateChild(PartSearchButton, out searchButton))
                searchButton.Click += OnSearchButtonClick;

            if (this.TryGetTemplateChild(PartClearFilterButton, out clearFilterButton))
                clearFilterButton.Click += OnClearFilterButtonClick;
        }

        private void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => UpdateFilteredItemsSource();

        protected virtual void OnSearchButtonClick(object sender, RoutedEventArgs e) => UpdateFilteredItemsSource();

        protected virtual void OnClearFilterButtonClick(object sender, RoutedEventArgs e) => ClearFilter();

        public void UpdateFilteredItemsSource() => FilteredItemsSource = FilterItems(ItemsSource).ToList();

        public abstract IEnumerable<T> FilterItems(IEnumerable<T> items);

        public abstract void ClearFilter();
    }
}
