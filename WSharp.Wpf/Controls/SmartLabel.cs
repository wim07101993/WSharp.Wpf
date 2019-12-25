using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace WSharp.Wpf.Controls
{

    /// <summary>
    /// A control that implement placeholder behavior. Can work as a simple placeholder either as a floating label, see <see cref="UseFloating"/> property.
    /// <para/>
    /// To set a target control you should set the LabelProxy property. Use the <see cref="LabelProxyFabricConverter.Instance"/> converter which converts a control into the ILabelProxy interface.
    /// </summary>
    [TemplateVisualState(GroupName = ContentStatesGroupName, Name = LabelRestingPositionName)]
    [TemplateVisualState(GroupName = ContentStatesGroupName, Name = LabelFloatingPositionName)]
    public class SmartLabel : Control
    {
        public const string ContentStatesGroupName = "ContentStates";

        public const string LabelRestingPositionName = "LabelRestingPosition";
        public const string LabelFloatingPositionName = "LabelFloatingPosition";

        #region dependency properties

        public static readonly DependencyProperty LabelProxyProperty = DependencyProperty.Register(
            nameof(LabelProxy),
            typeof(ILabelProxy),
            typeof(SmartLabel),
            new PropertyMetadata(default(ILabelProxy), LabelProxyPropertyChangedCallback));

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
            nameof(Label), 
            typeof(object), 
            typeof(SmartLabel), 
            new PropertyMetadata(null));

        private static readonly DependencyPropertyKey IsContentNullOrEmptyPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(IsContentNullOrEmpty), 
            typeof(bool), 
            typeof(SmartLabel),
            new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty IsContentNullOrEmptyProperty = IsContentNullOrEmptyPropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey IsLabelInFloatingPositionPropertyKey = DependencyProperty.RegisterReadOnly(
                nameof(IsLabelInFloatingPosition), 
                typeof(bool), 
                typeof(SmartLabel),
                new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty IsLabelInFloatingPositionProperty = IsLabelInFloatingPositionPropertyKey.DependencyProperty;

        public static readonly DependencyProperty UseFloatingProperty = DependencyProperty.Register(
            nameof(UseFloating), 
            typeof(bool), 
            typeof(SmartLabel), 
            new PropertyMetadata(false));

        public static readonly DependencyProperty FloatingScaleProperty = DependencyProperty.Register(
            nameof(FloatingScale), 
            typeof(double), 
            typeof(SmartLabel), 
            new PropertyMetadata(.74));

        public static readonly DependencyProperty FloatingOffsetProperty = DependencyProperty.Register(
            nameof(FloatingOffset), 
            typeof(Point), 
            typeof(SmartLabel), 
            new PropertyMetadata(new Point(1, -16)));

        public static readonly DependencyProperty LabelOpacityProperty = DependencyProperty.Register(
            nameof(LabelOpacity), 
            typeof(double), 
            typeof(SmartLabel), 
            new PropertyMetadata(.46));

        #endregion dependency properties

        static SmartLabel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SmartLabel), new FrameworkPropertyMetadata(typeof(SmartLabel)));
        }

        public SmartLabel()
        {
            IsHitTestVisible = false;
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
        }

        public ILabelProxy LabelProxy
        {
            get => (ILabelProxy)GetValue(LabelProxyProperty);
            set => SetValue(LabelProxyProperty, value);
        }

        public object Label
        {
            get => GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public bool IsContentNullOrEmpty
        {
            get => (bool)GetValue(IsContentNullOrEmptyProperty);
            private set => SetValue(IsContentNullOrEmptyPropertyKey, value);
        }

        public bool IsLabelInFloatingPosition
        {
            get => (bool)GetValue(IsLabelInFloatingPositionProperty);
            private set => SetValue(IsLabelInFloatingPositionPropertyKey, value);
        }

        public bool UseFloating
        {
            get => (bool)GetValue(UseFloatingProperty);
            set => SetValue(UseFloatingProperty, value);
        }

        public double FloatingScale
        {
            get => (double)GetValue(FloatingScaleProperty);
            set => SetValue(FloatingScaleProperty, value);
        }

        public Point FloatingOffset
        {
            get => (Point)GetValue(FloatingOffsetProperty);
            set => SetValue(FloatingOffsetProperty, value);
        }

        public double LabelOpacity
        {
            get => (double)GetValue(LabelOpacityProperty);
            set => SetValue(LabelOpacityProperty, value);
        }

        private void LabelProxySetStateOnLoaded(object sender, EventArgs e)
        {
            RefreshState(false);
            LabelProxy.Loaded -= LabelProxySetStateOnLoaded;
        }

        private void RefreshState(bool useTransitions)
        {
            ILabelProxy proxy = LabelProxy;

            if (proxy == null) return;
            if (!proxy.IsVisible) return;

            var action = new Action(() =>
            {
                string state = string.Empty;

                bool isEmpty = proxy.IsEmpty();
                bool isFocused = proxy.IsFocused();

                if (UseFloating)
                    state = !isEmpty || isFocused ? LabelFloatingPositionName : LabelRestingPositionName;
                else
                    state = !isEmpty ? LabelFloatingPositionName : LabelRestingPositionName;

                IsLabelInFloatingPosition = state == LabelFloatingPositionName;

                VisualStateManager.GoToState(this, state, useTransitions);
            });

            if (DesignerProperties.GetIsInDesignMode(this))
            {
                action();
            }
            else
            {
                Dispatcher.BeginInvoke(action);
            }
        }

        protected virtual void OnLabelProxyFocusedChanged(object sender, EventArgs e)
        {
            if (LabelProxy.IsLoaded)
                RefreshState(true);
            else
                LabelProxy.Loaded += LabelProxySetStateOnLoaded;
        }

        protected virtual void OnLabelProxyContentChanged(object sender, EventArgs e)
        {
            IsContentNullOrEmpty = LabelProxy.IsEmpty();

            if (LabelProxy.IsLoaded)
                RefreshState(true);
            else
                LabelProxy.Loaded += LabelProxySetStateOnLoaded;
        }

        protected virtual void OnLabelProxyIsVisibleChanged(object sender, EventArgs e)
        {
            RefreshState(false);
        }

        private static void LabelProxyPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (!(dependencyObject is SmartLabel smartLabel)) 
                return;

            if (dependencyPropertyChangedEventArgs.OldValue is ILabelProxy labelProxy)
            {
                labelProxy.IsVisibleChanged -= smartLabel.OnLabelProxyIsVisibleChanged;
                labelProxy.ContentChanged -= smartLabel.OnLabelProxyContentChanged;
                labelProxy.Loaded -= smartLabel.OnLabelProxyContentChanged;
                labelProxy.FocusedChanged -= smartLabel.OnLabelProxyFocusedChanged;
                labelProxy.Dispose();
            }

            labelProxy = dependencyPropertyChangedEventArgs.NewValue as ILabelProxy;
            if (labelProxy == null) return;

            labelProxy.IsVisibleChanged += smartLabel.OnLabelProxyIsVisibleChanged;
            labelProxy.ContentChanged += smartLabel.OnLabelProxyContentChanged;
            labelProxy.Loaded += smartLabel.OnLabelProxyContentChanged;
            labelProxy.FocusedChanged += smartLabel.OnLabelProxyFocusedChanged;
            smartLabel.RefreshState(false);
        }
    }
}
