using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using WSharp.Wpf.Extensions;
using WSharp.Wpf.Helpers;

namespace WSharp.Wpf.Controls
{
    [TemplateVisualState(GroupName = TemplateAllDrawersGroupName, Name = TemplateAllDrawersAllClosedStateName)]
    [TemplateVisualState(GroupName = TemplateAllDrawersGroupName, Name = TemplateAllDrawersAnyOpenStateName)]
    [TemplateVisualState(GroupName = TemplateLeftDrawerGroupName, Name = TemplateLeftClosedStateName)]
    [TemplateVisualState(GroupName = TemplateLeftDrawerGroupName, Name = TemplateLeftOpenStateName)]
    [TemplateVisualState(GroupName = TemplateTopDrawerGroupName, Name = TemplateTopClosedStateName)]
    [TemplateVisualState(GroupName = TemplateTopDrawerGroupName, Name = TemplateTopOpenStateName)]
    [TemplateVisualState(GroupName = TemplateRightDrawerGroupName, Name = TemplateRightClosedStateName)]
    [TemplateVisualState(GroupName = TemplateRightDrawerGroupName, Name = TemplateRightOpenStateName)]
    [TemplateVisualState(GroupName = TemplateBottomDrawerGroupName, Name = TemplateBottomClosedStateName)]
    [TemplateVisualState(GroupName = TemplateBottomDrawerGroupName, Name = TemplateBottomOpenStateName)]
    [TemplatePart(Name = TemplateContentCoverPartName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = TemplateLeftDrawerPartName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = TemplateTopDrawerPartName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = TemplateRightDrawerPartName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = TemplateBottomDrawerPartName, Type = typeof(FrameworkElement))]
    public class DrawerHost : ContentControl
    {
        #region FIELDS

        public const string TemplateAllDrawersGroupName = "AllDrawers";
        public const string TemplateAllDrawersAllClosedStateName = "AllClosed";
        public const string TemplateAllDrawersAnyOpenStateName = "AnyOpen";
        public const string TemplateLeftDrawerGroupName = "LeftDrawer";
        public const string TemplateLeftClosedStateName = "LeftDrawerClosed";
        public const string TemplateLeftOpenStateName = "LeftDrawerOpen";
        public const string TemplateTopDrawerGroupName = "TopDrawer";
        public const string TemplateTopClosedStateName = "TopDrawerClosed";
        public const string TemplateTopOpenStateName = "TopDrawerOpen";
        public const string TemplateRightDrawerGroupName = "RightDrawer";
        public const string TemplateRightClosedStateName = "RightDrawerClosed";
        public const string TemplateRightOpenStateName = "RightDrawerOpen";
        public const string TemplateBottomDrawerGroupName = "BottomDrawer";
        public const string TemplateBottomClosedStateName = "BottomDrawerClosed";
        public const string TemplateBottomOpenStateName = "BottomDrawerOpen";

        public const string TemplateContentCoverPartName = "PART_ContentCover";
        public const string TemplateLeftDrawerPartName = "PART_LeftDrawer";
        public const string TemplateTopDrawerPartName = "PART_TopDrawer";
        public const string TemplateRightDrawerPartName = "PART_RightDrawer";
        public const string TemplateBottomDrawerPartName = "PART_BottomDrawer";

        public static RoutedCommand OpenDrawerCommand = new RoutedCommand();
        public static RoutedCommand CloseDrawerCommand = new RoutedCommand();

        private FrameworkElement _templateContentCoverElement;
        private FrameworkElement _leftDrawerElement;
        private FrameworkElement _topDrawerElement;
        private FrameworkElement _rightDrawerElement;
        private FrameworkElement _bottomDrawerElement;

        private bool _lockZIndexes;

        private readonly IDictionary<DependencyProperty, DependencyPropertyKey> _zIndexPropertyLookup = new Dictionary<DependencyProperty, DependencyPropertyKey>
        {
            { IsLeftDrawerOpenProperty, leftDrawerZIndexPropertyKey },
            { IsTopDrawerOpenProperty, topDrawerZIndexPropertyKey },
            { IsRightDrawerOpenProperty, rightDrawerZIndexPropertyKey },
            { IsBottomDrawerOpenProperty, bottomDrawerZIndexPropertyKey }
        };

        #endregion FIELDS

        #region DEPENDENCY PROPERTIES

        #region top drawer

        public static readonly DependencyProperty TopDrawerContentProperty = DependencyProperty.Register(
            nameof(TopDrawerContent),
            typeof(object),
            typeof(DrawerHost),
            new PropertyMetadata(default(object)));

        public static readonly DependencyProperty TopDrawerContentTemplateProperty = DependencyProperty.Register(
            nameof(TopDrawerContentTemplate),
            typeof(DataTemplate),
            typeof(DrawerHost),
            new PropertyMetadata(default(DataTemplate)));

        public static readonly DependencyProperty TopDrawerContentTemplateSelectorProperty = DependencyProperty.Register(
            nameof(TopDrawerContentTemplateSelector),
            typeof(DataTemplateSelector),
            typeof(DrawerHost),
            new PropertyMetadata(default(DataTemplateSelector)));

        public static readonly DependencyProperty TopDrawerContentStringFormatProperty = DependencyProperty.Register(
            nameof(TopDrawerContentStringFormat),
            typeof(string),
            typeof(DrawerHost),
            new PropertyMetadata(default(string)));

        public static readonly DependencyProperty TopDrawerBackgroundProperty = DependencyProperty.Register(
            nameof(TopDrawerBackground),
            typeof(Brush),
            typeof(DrawerHost),
            new PropertyMetadata(default(Brush)));

        public static readonly DependencyProperty IsTopDrawerOpenProperty = DependencyProperty.Register(
            nameof(IsTopDrawerOpen),
            typeof(bool),
            typeof(DrawerHost),
            new FrameworkPropertyMetadata(
                default(bool),
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                IsDrawerOpenPropertyChangedCallback));

        private static readonly DependencyPropertyKey topDrawerZIndexPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(TopDrawerZIndex),
            typeof(int),
            typeof(DrawerHost),
            new PropertyMetadata(4));

        public static readonly DependencyProperty TopDrawerZIndexProperty = topDrawerZIndexPropertyKey.DependencyProperty;

        public static readonly DependencyProperty TopDrawerCloseOnClickAwayProperty = DependencyProperty.Register(
            nameof(TopDrawerCloseOnClickAway),
            typeof(bool),
            typeof(DrawerHost),
            new PropertyMetadata(true));

        #endregion top drawer

        #region left drawer

        public static readonly DependencyProperty LeftDrawerContentProperty = DependencyProperty.Register(
            nameof(LeftDrawerContent),
            typeof(object),
            typeof(DrawerHost),
            new PropertyMetadata(default(object)));

        public static readonly DependencyProperty LeftDrawerContentTemplateProperty = DependencyProperty.Register(
            nameof(LeftDrawerContentTemplate),
            typeof(DataTemplate),
            typeof(DrawerHost),
            new PropertyMetadata(default(DataTemplate)));

        public static readonly DependencyProperty LeftDrawerContentTemplateSelectorProperty = DependencyProperty.Register(
            nameof(LeftDrawerContentTemplateSelector),
            typeof(DataTemplateSelector),
            typeof(DrawerHost),
            new PropertyMetadata(default(DataTemplateSelector)));

        public static readonly DependencyProperty LeftDrawerContentStringFormatProperty = DependencyProperty.Register(
            nameof(LeftDrawerContentStringFormat),
            typeof(string),
            typeof(DrawerHost),
            new PropertyMetadata(default(string)));

        public static readonly DependencyProperty LeftDrawerBackgroundProperty = DependencyProperty.Register(
            nameof(LeftDrawerBackground),
            typeof(Brush),
            typeof(DrawerHost),
            new PropertyMetadata(default(Brush)));

        public static readonly DependencyProperty IsLeftDrawerOpenProperty = DependencyProperty.Register(
            nameof(IsLeftDrawerOpen),
            typeof(bool),
            typeof(DrawerHost),
            new FrameworkPropertyMetadata(
                default(bool),
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                IsDrawerOpenPropertyChangedCallback));

        private static readonly DependencyPropertyKey leftDrawerZIndexPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(LeftDrawerZIndex),
            typeof(int),
            typeof(DrawerHost),
            new PropertyMetadata(2));

        public static readonly DependencyProperty LeftDrawerZIndexProperty = leftDrawerZIndexPropertyKey.DependencyProperty;

        public static readonly DependencyProperty LeftDrawerCloseOnClickAwayProperty = DependencyProperty.Register(
            nameof(LeftDrawerCloseOnClickAway),
            typeof(bool),
            typeof(DrawerHost),
            new PropertyMetadata(true));

        #endregion left drawer

        #region right drawer

        public static readonly DependencyProperty RightDrawerContentProperty = DependencyProperty.Register(
            nameof(RightDrawerContent),
            typeof(object),
            typeof(DrawerHost),
            new PropertyMetadata(default(object)));

        public static readonly DependencyProperty RightDrawerContentTemplateProperty = DependencyProperty.Register(
            nameof(RightDrawerContentTemplate),
            typeof(DataTemplate),
            typeof(DrawerHost),
            new PropertyMetadata(default(DataTemplate)));

        public static readonly DependencyProperty RightDrawerContentTemplateSelectorProperty = DependencyProperty.Register(
            nameof(RightDrawerContentTemplateSelector),
            typeof(DataTemplateSelector),
            typeof(DrawerHost),
            new PropertyMetadata(default(DataTemplateSelector)));

        public static readonly DependencyProperty RightDrawerContentStringFormatProperty = DependencyProperty.Register(
            nameof(RightDrawerContentStringFormat),
            typeof(string),
            typeof(DrawerHost),
            new PropertyMetadata(default(string)));

        public static readonly DependencyProperty RightDrawerBackgroundProperty = DependencyProperty.Register(
            nameof(RightDrawerBackground),
            typeof(Brush),
            typeof(DrawerHost),
            new PropertyMetadata(default(Brush)));

        public static readonly DependencyProperty IsRightDrawerOpenProperty = DependencyProperty.Register(
            nameof(IsRightDrawerOpen),
            typeof(bool),
            typeof(DrawerHost),
            new FrameworkPropertyMetadata(
                default(bool),
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                IsDrawerOpenPropertyChangedCallback));

        private static readonly DependencyPropertyKey rightDrawerZIndexPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(RightDrawerZIndex),
            typeof(int),
            typeof(DrawerHost),
            new PropertyMetadata(1));

        public static readonly DependencyProperty RightDrawerZIndexProperty = rightDrawerZIndexPropertyKey.DependencyProperty;

        public static readonly DependencyProperty RightDrawerCloseOnClickAwayProperty = DependencyProperty.Register(
            nameof(RightDrawerCloseOnClickAway),
            typeof(bool),
            typeof(DrawerHost),
            new PropertyMetadata(true));

        #endregion right drawer

        #region bottom drawer

        public static readonly DependencyProperty BottomDrawerContentProperty = DependencyProperty.Register(
            nameof(BottomDrawerContent),
            typeof(object),
            typeof(DrawerHost),
            new PropertyMetadata(default(object)));

        public static readonly DependencyProperty BottomDrawerContentTemplateProperty = DependencyProperty.Register(
            nameof(BottomDrawerContentTemplate),
            typeof(DataTemplate),
            typeof(DrawerHost),
            new PropertyMetadata(default(DataTemplate)));

        public static readonly DependencyProperty BottomDrawerContentTemplateSelectorProperty = DependencyProperty.Register(
            nameof(BottomDrawerContentTemplateSelector),
            typeof(DataTemplateSelector),
            typeof(DrawerHost),
            new PropertyMetadata(default(DataTemplateSelector)));

        public static readonly DependencyProperty BottomDrawerContentStringFormatProperty = DependencyProperty.Register(
            nameof(BottomDrawerContentStringFormat),
            typeof(string),
            typeof(DrawerHost),
            new PropertyMetadata(default(string)));

        public static readonly DependencyProperty BottomDrawerBackgroundProperty = DependencyProperty.Register(
            nameof(BottomDrawerBackground),
            typeof(Brush),
            typeof(DrawerHost),
            new PropertyMetadata(default(Brush)));

        public static readonly DependencyProperty IsBottomDrawerOpenProperty = DependencyProperty.Register(
            nameof(IsBottomDrawerOpen),
            typeof(bool),
            typeof(DrawerHost),
            new FrameworkPropertyMetadata(
                default(bool),
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                IsDrawerOpenPropertyChangedCallback));

        private static readonly DependencyPropertyKey bottomDrawerZIndexPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(BottomDrawerZIndex),
            typeof(int),
            typeof(DrawerHost),
            new PropertyMetadata(3));

        public static readonly DependencyProperty BottomDrawerZIndexProperty = bottomDrawerZIndexPropertyKey.DependencyProperty;

        public static readonly DependencyProperty BottomDrawerCloseOnClickAwayProperty = DependencyProperty.Register(
            nameof(BottomDrawerCloseOnClickAway),
            typeof(bool),
            typeof(DrawerHost),
            new PropertyMetadata(true));

        #endregion bottom drawer

        #endregion DEPENDENCY PROPERTIES

        static DrawerHost()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DrawerHost), new FrameworkPropertyMetadata(typeof(DrawerHost)));
        }

        public DrawerHost()
        {
            _ = CommandBindings.Add(new CommandBinding(OpenDrawerCommand, OpenDrawerHandler));
            _ = CommandBindings.Add(new CommandBinding(CloseDrawerCommand, CloseDrawerHandler));
        }

        #region top drawer

        public object TopDrawerContent
        {
            get => GetValue(TopDrawerContentProperty);
            set => SetValue(TopDrawerContentProperty, value);
        }

        public DataTemplate TopDrawerContentTemplate
        {
            get => (DataTemplate)GetValue(TopDrawerContentTemplateProperty);
            set => SetValue(TopDrawerContentTemplateProperty, value);
        }

        public DataTemplateSelector TopDrawerContentTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(TopDrawerContentTemplateSelectorProperty);
            set => SetValue(TopDrawerContentTemplateSelectorProperty, value);
        }

        public string TopDrawerContentStringFormat
        {
            get => (string)GetValue(TopDrawerContentStringFormatProperty);
            set => SetValue(TopDrawerContentStringFormatProperty, value);
        }

        public Brush TopDrawerBackground
        {
            get => (Brush)GetValue(TopDrawerBackgroundProperty);
            set => SetValue(TopDrawerBackgroundProperty, value);
        }

        public bool IsTopDrawerOpen
        {
            get => (bool)GetValue(IsTopDrawerOpenProperty);
            set => SetValue(IsTopDrawerOpenProperty, value);
        }

        public int TopDrawerZIndex
        {
            get => (int)GetValue(TopDrawerZIndexProperty);
            private set => SetValue(topDrawerZIndexPropertyKey, value);
        }

        public bool TopDrawerCloseOnClickAway
        {
            get => (bool)GetValue(TopDrawerCloseOnClickAwayProperty);
            set => SetValue(TopDrawerCloseOnClickAwayProperty, value);
        }

        #endregion top drawer

        #region left drawer

        public object LeftDrawerContent
        {
            get => GetValue(LeftDrawerContentProperty);
            set => SetValue(LeftDrawerContentProperty, value);
        }

        public DataTemplate LeftDrawerContentTemplate
        {
            get => (DataTemplate)GetValue(LeftDrawerContentTemplateProperty);
            set => SetValue(LeftDrawerContentTemplateProperty, value);
        }

        public DataTemplateSelector LeftDrawerContentTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(LeftDrawerContentTemplateSelectorProperty);
            set => SetValue(LeftDrawerContentTemplateSelectorProperty, value);
        }

        public string LeftDrawerContentStringFormat
        {
            get => (string)GetValue(LeftDrawerContentStringFormatProperty);
            set => SetValue(LeftDrawerContentStringFormatProperty, value);
        }

        public Brush LeftDrawerBackground
        {
            get => (Brush)GetValue(LeftDrawerBackgroundProperty);
            set => SetValue(LeftDrawerBackgroundProperty, value);
        }

        public bool IsLeftDrawerOpen
        {
            get => (bool)GetValue(IsLeftDrawerOpenProperty);
            set => SetValue(IsLeftDrawerOpenProperty, value);
        }

        public int LeftDrawerZIndex
        {
            get => (int)GetValue(LeftDrawerZIndexProperty);
            private set => SetValue(leftDrawerZIndexPropertyKey, value);
        }

        public bool LeftDrawerCloseOnClickAway
        {
            get => (bool)GetValue(LeftDrawerCloseOnClickAwayProperty);
            set => SetValue(LeftDrawerCloseOnClickAwayProperty, value);
        }

        #endregion left drawer

        #region right drawer

        public object RightDrawerContent
        {
            get => GetValue(RightDrawerContentProperty);
            set => SetValue(RightDrawerContentProperty, value);
        }

        public DataTemplate RightDrawerContentTemplate
        {
            get => (DataTemplate)GetValue(RightDrawerContentTemplateProperty);
            set => SetValue(RightDrawerContentTemplateProperty, value);
        }

        public DataTemplateSelector RightDrawerContentTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(RightDrawerContentTemplateSelectorProperty);
            set => SetValue(RightDrawerContentTemplateSelectorProperty, value);
        }

        public string RightDrawerContentStringFormat
        {
            get => (string)GetValue(RightDrawerContentStringFormatProperty);
            set => SetValue(RightDrawerContentStringFormatProperty, value);
        }

        public Brush RightDrawerBackground
        {
            get => (Brush)GetValue(RightDrawerBackgroundProperty);
            set => SetValue(RightDrawerBackgroundProperty, value);
        }

        public bool IsRightDrawerOpen
        {
            get => (bool)GetValue(IsRightDrawerOpenProperty);
            set => SetValue(IsRightDrawerOpenProperty, value);
        }

        public int RightDrawerZIndex
        {
            get => (int)GetValue(RightDrawerZIndexProperty);
            private set => SetValue(rightDrawerZIndexPropertyKey, value);
        }

        public bool RightDrawerCloseOnClickAway
        {
            get => (bool)GetValue(RightDrawerCloseOnClickAwayProperty);
            set => SetValue(RightDrawerCloseOnClickAwayProperty, value);
        }

        #endregion right drawer

        #region bottom drawer

        public object BottomDrawerContent
        {
            get => GetValue(BottomDrawerContentProperty);
            set => SetValue(BottomDrawerContentProperty, value);
        }

        public DataTemplate BottomDrawerContentTemplate
        {
            get => (DataTemplate)GetValue(BottomDrawerContentTemplateProperty);
            set => SetValue(BottomDrawerContentTemplateProperty, value);
        }

        public DataTemplateSelector BottomDrawerContentTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(BottomDrawerContentTemplateSelectorProperty);
            set => SetValue(BottomDrawerContentTemplateSelectorProperty, value);
        }

        public string BottomDrawerContentStringFormat
        {
            get => (string)GetValue(BottomDrawerContentStringFormatProperty);
            set => SetValue(BottomDrawerContentStringFormatProperty, value);
        }

        public Brush BottomDrawerBackground
        {
            get => (Brush)GetValue(BottomDrawerBackgroundProperty);
            set => SetValue(BottomDrawerBackgroundProperty, value);
        }

        public bool IsBottomDrawerOpen
        {
            get => (bool)GetValue(IsBottomDrawerOpenProperty);
            set => SetValue(IsBottomDrawerOpenProperty, value);
        }

        public int BottomDrawerZIndex
        {
            get => (int)GetValue(BottomDrawerZIndexProperty);
            private set => SetValue(bottomDrawerZIndexPropertyKey, value);
        }

        public bool BottomDrawerCloseOnClickAway
        {
            get => (bool)GetValue(BottomDrawerCloseOnClickAwayProperty);
            set => SetValue(BottomDrawerCloseOnClickAwayProperty, value);
        }

        #endregion bottom drawer

        public override void OnApplyTemplate()
        {
            if (_templateContentCoverElement != null)
                _templateContentCoverElement.PreviewMouseLeftButtonUp += TemplateContentCoverElementOnPreviewMouseLeftButtonUp;

            _ = WireDrawer(_leftDrawerElement, true);
            _ = WireDrawer(_topDrawerElement, true);
            _ = WireDrawer(_rightDrawerElement, true);
            _ = WireDrawer(_bottomDrawerElement, true);

            base.OnApplyTemplate();

            if (this.TryGetTemplateChild(TemplateContentCoverPartName, out _templateContentCoverElement))
                _templateContentCoverElement.PreviewMouseLeftButtonUp += TemplateContentCoverElementOnPreviewMouseLeftButtonUp;

            _leftDrawerElement = WireDrawer(this.GetTemplateChild<FrameworkElement>(TemplateLeftDrawerPartName), false);
            _topDrawerElement = WireDrawer(this.GetTemplateChild<FrameworkElement>(TemplateTopDrawerPartName), false);
            _rightDrawerElement = WireDrawer(this.GetTemplateChild<FrameworkElement>(TemplateRightDrawerPartName), false);
            _bottomDrawerElement = WireDrawer(this.GetTemplateChild<FrameworkElement>(TemplateBottomDrawerPartName), false);

            UpdateVisualStates(false);
        }

        private FrameworkElement WireDrawer(FrameworkElement drawerElement, bool unwire)
        {
            if (drawerElement == null)
                return null;

            if (unwire)
            {
                drawerElement.GotFocus -= DrawerElement_GotFocus;
                drawerElement.MouseDown -= DrawerElement_MouseDown;

                return drawerElement;
            }

            drawerElement.GotFocus += DrawerElement_GotFocus;
            drawerElement.MouseDown += DrawerElement_MouseDown;

            return drawerElement;
        }

        private void DrawerElement_MouseDown(object sender, MouseButtonEventArgs e) => ReactToFocus(sender);

        private void DrawerElement_GotFocus(object sender, RoutedEventArgs e) => ReactToFocus(sender);

        private void ReactToFocus(object sender)
        {
            if (sender == _leftDrawerElement)
                PrepareZIndexes(leftDrawerZIndexPropertyKey);
            else if (sender == _topDrawerElement)
                PrepareZIndexes(topDrawerZIndexPropertyKey);
            else if (sender == _rightDrawerElement)
                PrepareZIndexes(rightDrawerZIndexPropertyKey);
            else if (sender == _bottomDrawerElement)
                PrepareZIndexes(bottomDrawerZIndexPropertyKey);
        }

        private void TemplateContentCoverElementOnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (LeftDrawerCloseOnClickAway)
                SetCurrentValue(IsLeftDrawerOpenProperty, false);
            if (RightDrawerCloseOnClickAway)
                SetCurrentValue(IsRightDrawerOpenProperty, false);
            if (TopDrawerCloseOnClickAway)
                SetCurrentValue(IsTopDrawerOpenProperty, false);
            if (BottomDrawerCloseOnClickAway)
                SetCurrentValue(IsBottomDrawerOpenProperty, false);
        }

        private void UpdateVisualStates(bool? useTransitions = null)
        {
            var anyOpen = IsTopDrawerOpen || IsLeftDrawerOpen || IsBottomDrawerOpen || IsRightDrawerOpen;
            var useTransissions = useTransitions ?? !TransitionHelper.GetDisableTransitions(this);
            var names = new[]
            {
                anyOpen ? TemplateAllDrawersAnyOpenStateName : TemplateAllDrawersAllClosedStateName,
                IsLeftDrawerOpen ? TemplateLeftOpenStateName : TemplateLeftClosedStateName,
                IsTopDrawerOpen ? TemplateTopOpenStateName : TemplateTopClosedStateName,
                IsRightDrawerOpen ? TemplateRightOpenStateName : TemplateRightClosedStateName,
                IsBottomDrawerOpen ? TemplateBottomOpenStateName : TemplateBottomClosedStateName
            };

            foreach (var name in names)
                _ = VisualStateManager.GoToState(this, name, useTransissions);
        }

        private static void IsDrawerOpenPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is DrawerHost drawerHost))
                return;

            if (!drawerHost._lockZIndexes && (bool)e.NewValue)
                drawerHost.PrepareZIndexes(drawerHost._zIndexPropertyLookup[e.Property]);
            drawerHost.UpdateVisualStates();
        }

        private void PrepareZIndexes(DependencyPropertyKey d)
        {
            var newOrder = new[] { d }
                .Concat(_zIndexPropertyLookup.Values.Except(new[] { d })
                .OrderByDescending(dpk => (int)GetValue(dpk.DependencyProperty)))
                .Reverse()
                .Select((dpk, idx) => new { dpk, idx });

            foreach (var a in newOrder)
                SetValue(a.dpk, a.idx);
        }

        private void CloseDrawerHandler(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Handled)
                return;

            SetOpenFlag(e, false);

            e.Handled = true;
        }

        private void OpenDrawerHandler(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Handled)
                return;

            SetOpenFlag(e, true);

            e.Handled = true;
        }

        private void SetOpenFlag(ExecutedRoutedEventArgs e, bool value)
        {
            if (e.Parameter is Dock dock)
            {
                switch (dock)
                {
                    case Dock.Left:
                        SetCurrentValue(IsLeftDrawerOpenProperty, value);
                        break;

                    case Dock.Top:
                        SetCurrentValue(IsTopDrawerOpenProperty, value);
                        break;

                    case Dock.Right:
                        SetCurrentValue(IsRightDrawerOpenProperty, value);
                        break;

                    case Dock.Bottom:
                        SetCurrentValue(IsBottomDrawerOpenProperty, value);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                try
                {
                    _lockZIndexes = true;
                    SetCurrentValue(IsLeftDrawerOpenProperty, value);
                    SetCurrentValue(IsTopDrawerOpenProperty, value);
                    SetCurrentValue(IsRightDrawerOpenProperty, value);
                    SetCurrentValue(IsBottomDrawerOpenProperty, value);
                }
                finally
                {
                    _lockZIndexes = false;
                }
            }
        }
    }
}
