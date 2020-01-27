﻿using System.Windows;
using System.Windows.Controls;

namespace WSharp.Wpf.Controls
{
    public abstract class ATextInput<T> : TextBox
    {
        #region Value

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value),
            typeof(T),
            typeof(ValueControl<T>),
            new FrameworkPropertyMetadata(default(T), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged, CoerceValue));

        public T Value
        {
            get => (T)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ATextInput<T> control))
                return;

            var oldValue = e.OldValue is T
                ? (T)e.OldValue
                : default;

            var newValue = e.NewValue is T
                ? (T)e.NewValue
                : default;

            control.OnValueChanged(oldValue, newValue);
        }

        protected virtual void OnValueChanged(T oldValue, T newValue)
            => RaiseValueChanged(oldValue, newValue);

        private static object CoerceValue(DependencyObject d, object baseValue)
        {
            if (!(d is ValueControl<T> control) || !(baseValue is T t))
                return baseValue;

            return control.CoerceValue(t);
        }

        public virtual T CoerceValue(T baseValue) => baseValue;

        #endregion Value

        #region ValueChanged

        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(
            nameof(ValueChanged),
            RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<T>),
            typeof(ValueControl<T>));

        public event RoutedPropertyChangedEventHandler<T> ValueChanged
        {
            add => AddHandler(ValueChangedEvent, value);
            remove => RemoveHandler(ValueChangedEvent, value);
        }

        protected virtual void RaiseValueChanged(T oldValue, T newValue)
            => RaiseEvent(new RoutedPropertyChangedEventArgs<T>(oldValue, newValue, ValueChangedEvent));

        #endregion ValueChanged
    }
}