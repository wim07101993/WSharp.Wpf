using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

using WSharp.Extensions;
using WSharp.Wpf.Transitions;

namespace WSharp.Wpf.Controls
{
    /// <summary>
    ///     The transitioner provides an easy way to move between content with a default in-place
    ///     circular transition.
    /// </summary>
    public class Transitioner : Selector, IZIndexController
    {
        private Point? _nextTransitionOrigin;

        /// <summary>Causes the the next slide to be displayed (affectively increments <see cref="Selector.SelectedIndex"/>).</summary>
        public static RoutedCommand MoveNextCommand = new RoutedCommand();

        /// <summary>
        ///     Causes the the previous slide to be displayed (affectively decrements <see cref="Selector.SelectedIndex"/>).
        /// </summary>
        public static RoutedCommand MovePreviousCommand = new RoutedCommand();

        /// <summary>Moves to the first slide.</summary>
        public static RoutedCommand MoveFirstCommand = new RoutedCommand();

        /// <summary>Moves to the last slide.</summary>
        public static RoutedCommand MoveLastCommand = new RoutedCommand();

        public static readonly DependencyProperty AutoApplyTransitionOriginsProperty = DependencyProperty.Register(
            nameof(AutoApplyTransitionOrigins),
            typeof(bool),
            typeof(Transitioner),
            new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty DefaultTransitionOriginProperty = DependencyProperty.Register(
            nameof(DefaultTransitionOrigin),
            typeof(Point),
            typeof(Transitioner),
            new PropertyMetadata(new Point(0.5, 0.5)));

        static Transitioner()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Transitioner), new FrameworkPropertyMetadata(typeof(Transitioner)));
        }

        public Transitioner()
        {
            _ = CommandBindings.Add(new CommandBinding(MoveNextCommand, MoveNextHandler));
            _ = CommandBindings.Add(new CommandBinding(MovePreviousCommand, MovePreviousHandler));
            _ = CommandBindings.Add(new CommandBinding(MoveFirstCommand, MoveFirstHandler));
            _ = CommandBindings.Add(new CommandBinding(MoveLastCommand, MoveLastHandler));

            AddHandler(TransitionerSlide.InTransitionFinished, new RoutedEventHandler(IsTransitionFinishedHandler));

            Loaded += (sender, args) =>
            {
                if (SelectedIndex != -1)
                    ActivateFrame(SelectedIndex, -1);
            };
        }

        /// <summary>
        ///     If enabled, transition origins will be applied to wipes, according to where a
        ///     transition was triggered from. For example, the mouse point where a user clicks a button.
        /// </summary>
        public bool AutoApplyTransitionOrigins
        {
            get => (bool)GetValue(AutoApplyTransitionOriginsProperty);
            set => SetValue(AutoApplyTransitionOriginsProperty, value);
        }

        public Point DefaultTransitionOrigin
        {
            get => (Point)GetValue(DefaultTransitionOriginProperty);
            set => SetValue(DefaultTransitionOriginProperty, value);
        }

        protected override bool IsItemItsOwnContainerOverride(object item) => item is TransitionerSlide;

        protected override DependencyObject GetContainerForItemOverride() => new TransitionerSlide();

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (AutoApplyTransitionOrigins)
                _nextTransitionOrigin = GetNavigationSourcePoint(e);
            base.OnPreviewMouseLeftButtonDown(e);
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            var unselectedIndex = -1;
            if (e.RemovedItems.Count == 1)
                unselectedIndex = Items.IndexOf(e.RemovedItems[0]);

            var selectedIndex = 1;
            if (e.AddedItems.Count == 1)
                selectedIndex = Items.IndexOf(e.AddedItems[0]);

            ActivateFrame(selectedIndex, unselectedIndex);

            base.OnSelectionChanged(e);
        }

        private void IsTransitionFinishedHandler(object sender, RoutedEventArgs routedEventArgs)
        {
            Items
                .OfType<object>()
                .Select(GetSlide)
                .Where(s => s.State == ETransitionerSlideState.Previous)
                .ForEach(s => s.SetCurrentValue(TransitionerSlide.StateProperty, ETransitionerSlideState.None));
        }

        private void MoveNextHandler(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            if (AutoApplyTransitionOrigins)
                _nextTransitionOrigin = GetNavigationSourcePoint(executedRoutedEventArgs);

            var slides = 1;
            if (executedRoutedEventArgs.Parameter is int && (int)executedRoutedEventArgs.Parameter > 0)
                slides = (int)executedRoutedEventArgs.Parameter;

            SetCurrentValue(SelectedIndexProperty, Math.Min(Items.Count - 1, SelectedIndex + slides));
        }

        private void MovePreviousHandler(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            if (AutoApplyTransitionOrigins)
                _nextTransitionOrigin = GetNavigationSourcePoint(executedRoutedEventArgs);

            var slides = 1;
            if (executedRoutedEventArgs.Parameter is int && (int)executedRoutedEventArgs.Parameter > 0)
                slides = (int)executedRoutedEventArgs.Parameter;

            SetCurrentValue(SelectedIndexProperty, Math.Max(0, SelectedIndex - slides));
        }

        private void MoveFirstHandler(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            if (AutoApplyTransitionOrigins)
                _nextTransitionOrigin = GetNavigationSourcePoint(executedRoutedEventArgs);

            SetCurrentValue(SelectedIndexProperty, 0);
        }

        private void MoveLastHandler(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            if (AutoApplyTransitionOrigins)
                _nextTransitionOrigin = GetNavigationSourcePoint(executedRoutedEventArgs);

            SetCurrentValue(SelectedIndexProperty, Items.Count - 1);
        }

        private Point? GetNavigationSourcePoint(RoutedEventArgs executedRoutedEventArgs)
        {
            if (!(executedRoutedEventArgs.OriginalSource is FrameworkElement sourceElement) ||
                !IsAncestorOf(sourceElement) || !IsSafePositive(ActualWidth) ||
                !IsSafePositive(ActualHeight) || !IsSafePositive(sourceElement.ActualWidth) ||
                !IsSafePositive(sourceElement.ActualHeight))
                return null;

            var transitionOrigin = sourceElement.TranslatePoint(new Point(sourceElement.ActualWidth / 2, sourceElement.ActualHeight), this);
            transitionOrigin = new Point(transitionOrigin.X / ActualWidth, transitionOrigin.Y / ActualHeight);
            return transitionOrigin;
        }

        private static bool IsSafePositive(double d) => !double.IsNaN(d) && !double.IsInfinity(d) && d > 0.0;

        private TransitionerSlide GetSlide(object item)
        {
            return IsItemItsOwnContainer(item)
                ? (TransitionerSlide)item
                : (TransitionerSlide)ItemContainerGenerator.ContainerFromItem(item);
        }

        private void ActivateFrame(int selectedIndex, int unselectedIndex)
        {
            if (!IsLoaded)
                return;

            TransitionerSlide oldSlide = null, newSlide = null;
            for (var index = 0; index < Items.Count; index++)
            {
                var slide = GetSlide(Items[index]);
                if (slide == null) continue;
                if (index == selectedIndex)
                {
                    newSlide = slide;
                    slide.SetCurrentValue(TransitionerSlide.StateProperty, ETransitionerSlideState.Current);
                }
                else if (index == unselectedIndex)
                {
                    oldSlide = slide;
                    slide.SetCurrentValue(TransitionerSlide.StateProperty, ETransitionerSlideState.Previous);
                }
                else
                {
                    slide.SetCurrentValue(TransitionerSlide.StateProperty, ETransitionerSlideState.None);
                }
                Panel.SetZIndex(slide, 0);
            }

            if (newSlide != null)
                newSlide.Opacity = 1;

            if (oldSlide != null && newSlide != null)
            {
                var wipe = selectedIndex > unselectedIndex ? oldSlide.ForwardWipe : oldSlide.BackwardWipe;
                if (wipe != null)
                    wipe.Wipe(oldSlide, newSlide, GetTransitionOrigin(newSlide), this);
                else
                    DoStack(newSlide, oldSlide);

                oldSlide.Opacity = 0;
            }
            else if (oldSlide != null || newSlide != null)
            {
                DoStack(oldSlide ?? newSlide);
                if (oldSlide != null)
                    oldSlide.Opacity = 0;
            }

            _nextTransitionOrigin = null;
        }

        private Point GetTransitionOrigin(TransitionerSlide slide)
        {
            return _nextTransitionOrigin != null
                ? _nextTransitionOrigin.Value
                : slide.ReadLocalValue(TransitionerSlide.TransitionOriginProperty) != DependencyProperty.UnsetValue
                ? slide.TransitionOrigin
                : DefaultTransitionOrigin;
        }

        void IZIndexController.Stack(params TransitionerSlide[] highestToLowest) => DoStack(highestToLowest);

        private static void DoStack(params TransitionerSlide[] highestToLowest)
        {
            if (highestToLowest == null)
                return;

            var pos = highestToLowest.Length;
            highestToLowest
                .Where(s => s != null)
                .ForEach(s => Panel.SetZIndex(s, pos--));
        }
    }
}
