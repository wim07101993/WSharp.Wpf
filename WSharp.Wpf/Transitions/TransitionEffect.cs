using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

using WSharp.Wpf.Controls;

namespace WSharp.Wpf.Transitions
{
    public class TransitionEffect : TransitionEffectBase
    {
        public TransitionEffect()
        {
        }

        public TransitionEffect(ETransitionEffectKind kind)
        {
            Kind = kind;
        }

        public TransitionEffect(ETransitionEffectKind kind, TimeSpan duration)
        {
            Kind = kind;
            Duration = duration;
        }

        public ETransitionEffectKind Kind { get; set; }

        public TimeSpan OffsetTime { get; set; } = TimeSpan.Zero;

        public TimeSpan Duration { get; set; } = TimeSpan.FromMilliseconds(400);

        public override Timeline Build<TSubject>(TSubject effectSubject)
        {
            if (effectSubject == null)
                throw new ArgumentNullException(nameof(effectSubject));

            Timeline timeline = null;
            DependencyProperty property = null;
            DependencyObject target = null;
            string targetName = null;
            switch (Kind)
            {
                //we need these long winded property paths as combined storyboards wont play directly on transforms
                case ETransitionEffectKind.None:
                    break;

                case ETransitionEffectKind.ExpandIn:
                    return CreateExpandIn(effectSubject);

                case ETransitionEffectKind.SlideInFromLeft:
                    timeline = CreateSlide(-300, 0, effectSubject.Offset);
                    property = TranslateTransform.XProperty;
                    targetName = effectSubject.TranslateTransformName;
                    break;

                case ETransitionEffectKind.SlideInFromTop:
                    timeline = CreateSlide(-300, 0, effectSubject.Offset);
                    property = TranslateTransform.YProperty;
                    targetName = effectSubject.TranslateTransformName;
                    break;

                case ETransitionEffectKind.SlideInFromRight:
                    timeline = CreateSlide(300, 0, effectSubject.Offset);
                    property = TranslateTransform.XProperty;
                    targetName = effectSubject.TranslateTransformName;
                    break;

                case ETransitionEffectKind.SlideInFromBottom:
                    timeline = CreateSlide(300, 0, effectSubject.Offset);
                    property = TranslateTransform.YProperty;
                    targetName = effectSubject.TranslateTransformName;
                    break;

                case ETransitionEffectKind.FadeIn:
                    timeline = CreateFadeIn(effectSubject.Offset);
                    property = OpacityProperty;
                    target = effectSubject;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (timeline == null || (target == null && targetName == null)) 
                return null;

            timeline.Duration = Duration + effectSubject.Offset;

            if (target != null)
                Storyboard.SetTarget(timeline, target);
            if (targetName != null)
                Storyboard.SetTargetName(timeline, targetName);

            Storyboard.SetTargetProperty(timeline, new PropertyPath(property));

            return timeline;
        }

        private Timeline CreateFadeIn(TimeSpan initialOffset) => CreateSlide(0, 1, initialOffset);

        private Timeline CreateSlide(double from, double to, TimeSpan initialOffset)
        {
            var zeroFrame = new DiscreteDoubleKeyFrame(from);
            var startFrame = new DiscreteDoubleKeyFrame(from, initialOffset + OffsetTime);
            var endFrame = new EasingDoubleKeyFrame(to, initialOffset + OffsetTime + Duration) { EasingFunction = new SineEase() };

            var slideAnimation = new DoubleAnimationUsingKeyFrames();
            _ = slideAnimation.KeyFrames.Add(zeroFrame);
            _ = slideAnimation.KeyFrames.Add(startFrame);
            _ = slideAnimation.KeyFrames.Add(endFrame);

            return slideAnimation;
        }

        private Timeline CreateExpandIn(ITransitionEffectSubject effectSubject)
        {
            var scaleXAnimation = new DoubleAnimationUsingKeyFrames();
            var zeroFrame = new DiscreteDoubleKeyFrame(0.0);
            var startFrame = new DiscreteDoubleKeyFrame(.5, effectSubject.Offset + OffsetTime);
            var endFrame = new EasingDoubleKeyFrame(1, effectSubject.Offset + OffsetTime + Duration) { EasingFunction = new SineEase() };

            _ = scaleXAnimation.KeyFrames.Add(zeroFrame);
            _ = scaleXAnimation.KeyFrames.Add(startFrame);
            _ = scaleXAnimation.KeyFrames.Add(endFrame);

            Storyboard.SetTargetName(scaleXAnimation, effectSubject.ScaleTransformName);
            Storyboard.SetTargetProperty(scaleXAnimation, new PropertyPath(ScaleTransform.ScaleXProperty));

            var scaleYAnimation = scaleXAnimation.Clone();

            Storyboard.SetTargetName(scaleYAnimation, effectSubject.ScaleTransformName);
            Storyboard.SetTargetProperty(scaleYAnimation, new PropertyPath(ScaleTransform.ScaleYProperty));

            var parallelTimeline = new ParallelTimeline();
            parallelTimeline.Children.Add(scaleXAnimation);
            parallelTimeline.Children.Add(scaleYAnimation);

            return parallelTimeline;
        }
    }
}
