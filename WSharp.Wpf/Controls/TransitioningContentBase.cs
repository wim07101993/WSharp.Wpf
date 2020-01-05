using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

using WSharp.Extensions;
using WSharp.Wpf.Extensions;
using WSharp.Wpf.Transitions;

namespace WSharp.Wpf.Controls
{
    public class TransitioningContentBase : ContentControl, ITransitionEffectSubject
    {
        public const string MatrixTransformPartName = "PART_MatrixTransform";
        public const string RotateTransformPartName = "PART_RotateTransform";
        public const string ScaleTransformPartName = "PART_ScaleTransform";
        public const string SkewTransformPartName = "PART_SkewTransform";
        public const string TranslateTransformPartName = "PART_TranslateTransform";

        private MatrixTransform _matrixTransform;
        private RotateTransform _rotateTransform;
        private ScaleTransform _scaleTransform;
        private SkewTransform _skewTransform;
        private TranslateTransform _translateTransform;

        public static readonly DependencyProperty OpeningEffectProperty = DependencyProperty.Register(
            nameof(OpeningEffect),
            typeof(TransitionEffectBase),
            typeof(TransitioningContentBase),
            new PropertyMetadata(default(TransitionEffectBase)));

        public static readonly DependencyProperty OpeningEffectsOffsetProperty = DependencyProperty.Register(
            nameof(OpeningEffectsOffset),
            typeof(TimeSpan),
            typeof(TransitioningContentBase),
            new PropertyMetadata(default(TimeSpan)));

        static TransitioningContentBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TransitioningContentBase), new FrameworkPropertyMetadata(typeof(TransitioningContentBase)));
        }

        #region PROPERTIES

        /// <summary>
        ///     Gets or sets the transition to run when the content is loaded and made visible.
        /// </summary>
        [TypeConverter(typeof(TransitionEffectTypeConverter))]
        public TransitionEffectBase OpeningEffect
        {
            get => (TransitionEffectBase)GetValue(OpeningEffectProperty);
            set => SetValue(OpeningEffectProperty, value);
        }

        /// <summary>Delay offset to be applied to all opening effect transitions.</summary>
        public TimeSpan OpeningEffectsOffset
        {
            get => (TimeSpan)GetValue(OpeningEffectsOffsetProperty);
            set => SetValue(OpeningEffectsOffsetProperty, value);
        }

        /// <summary>
        ///     Allows multiple transition effects to be combined and run upon the content loading
        ///     or being made visible.
        /// </summary>
        public ObservableCollection<TransitionEffectBase> OpeningEffects { get; } = new ObservableCollection<TransitionEffectBase>();

        string ITransitionEffectSubject.MatrixTransformName => MatrixTransformPartName;

        string ITransitionEffectSubject.RotateTransformName => RotateTransformPartName;

        string ITransitionEffectSubject.ScaleTransformName => ScaleTransformPartName;

        string ITransitionEffectSubject.SkewTransformName => SkewTransformPartName;

        string ITransitionEffectSubject.TranslateTransformName => TranslateTransformPartName;

        TimeSpan ITransitionEffectSubject.Offset => OpeningEffectsOffset;

        #endregion PROPERTIES

        public override void OnApplyTemplate()
        {
            var nameScopeRoot = GetNameScopeRoot();

            _matrixTransform = this.GetTemplateChild<MatrixTransform>(MatrixTransformPartName);
            _rotateTransform = this.GetTemplateChild<RotateTransform>(RotateTransformPartName);
            _scaleTransform = this.GetTemplateChild<ScaleTransform>(ScaleTransformPartName);
            _skewTransform = this.GetTemplateChild<SkewTransform>(SkewTransformPartName);
            _translateTransform = this.GetTemplateChild<TranslateTransform>(TranslateTransformPartName);

            new[] { MatrixTransformPartName, RotateTransformPartName, ScaleTransformPartName, SkewTransformPartName, TranslateTransformPartName }
                .Where(n => FindName(n) != null)
                .ForEach(UnregisterName);

            if (_matrixTransform != null)
                nameScopeRoot.RegisterName(MatrixTransformPartName, _matrixTransform);
            if (_rotateTransform != null)
                nameScopeRoot.RegisterName(RotateTransformPartName, _rotateTransform);
            if (_scaleTransform != null)
                nameScopeRoot.RegisterName(ScaleTransformPartName, _scaleTransform);
            if (_skewTransform != null)
                nameScopeRoot.RegisterName(SkewTransformPartName, _skewTransform);
            if (_translateTransform != null)
                nameScopeRoot.RegisterName(TranslateTransformPartName, _translateTransform);

            base.OnApplyTemplate();

            RunOpeningEffects();
        }

        protected virtual void RunOpeningEffects()
        {
            if (!IsLoaded || _matrixTransform == null)
                return;

            var storyboard = new Storyboard();
            var openingEffect = OpeningEffect?.Build(this);
            if (openingEffect != null)
                storyboard.Children.Add(openingEffect);

            OpeningEffects
                .Select(e => e.Build(this))
                .Where(tl => tl != null)
                .ForEach(storyboard.Children.Add);

            storyboard.Begin(GetNameScopeRoot());
        }

        private FrameworkElement GetNameScopeRoot()
        {
            //https://github.com/ButchersBoy/MaterialDesignInXamlToolkit/issues/950
            //Only set the NameScope if the child does not already have a TemplateNameScope set
            if (VisualChildrenCount > 0 &&
                GetVisualChild(0) is FrameworkElement f && 
                NameScope.GetNameScope(f) != null)
                return f;

            if (NameScope.GetNameScope(this) == null)
                NameScope.SetNameScope(this, new NameScope());

            return this;
        }
    }
}
