using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

using WSharp.Wpf.Controls;

namespace WSharp.Wpf.Transitions
{
    public class SlideWipe : ITransitionWipe
    {
        private readonly SineEase _sineEase = new SineEase();

        /// <summary>Direction of the slide wipe</summary>
        public ESlideDirection Direction { get; set; }

        /// <summary>Duration of the animation</summary>
        public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(0.5);

        public void Wipe(TransitionerSlide fromSlide, TransitionerSlide toSlide, Point origin, IZIndexController zIndexController)
        {
            if (fromSlide == null)
                throw new ArgumentNullException(nameof(fromSlide));
            if (toSlide == null)
                throw new ArgumentNullException(nameof(toSlide));
            if (zIndexController == null)
                throw new ArgumentNullException(nameof(zIndexController));

            // Set up time points
            var zeroKeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero);
            var endKeyTime = KeyTime.FromTimeSpan(Duration);

            // Set up coordinates
            double fromStartX = 0, fromEndX = 0, toStartX = 0, toEndX = 0;
            double fromStartY = 0, fromEndY = 0, toStartY = 0, toEndY = 0;

            switch (Direction)
            {
                case ESlideDirection.Left:
                    fromEndX = -fromSlide.ActualWidth;
                    toStartX = toSlide.ActualWidth;
                    break;

                case ESlideDirection.Right:
                    fromEndX = fromSlide.ActualWidth;
                    toStartX = -toSlide.ActualWidth;
                    break;

                case ESlideDirection.Up:
                    fromEndY = -fromSlide.ActualHeight;
                    toStartY = toSlide.ActualHeight;
                    break;

                case ESlideDirection.Down:
                    fromEndY = fromSlide.ActualHeight;
                    toStartY = -toSlide.ActualHeight;
                    break;
            }

            // From
            var fromTransform = new TranslateTransform(fromStartX, fromStartY);
            fromSlide.RenderTransform = fromTransform;
            var fromXAnimation = new DoubleAnimationUsingKeyFrames();
            _ = fromXAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(fromStartX, zeroKeyTime));
            _ = fromXAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(fromEndX, endKeyTime, _sineEase));
            var fromYAnimation = new DoubleAnimationUsingKeyFrames();
            _ = fromYAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(fromStartY, zeroKeyTime));
            _ = fromYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(fromEndY, endKeyTime, _sineEase));

            // To
            var toTransform = new TranslateTransform(toStartX, toStartY);
            toSlide.RenderTransform = toTransform;
            var toXAnimation = new DoubleAnimationUsingKeyFrames();
            _ = toXAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(toStartX, zeroKeyTime));
            _ = toXAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(toEndX, endKeyTime, _sineEase));
            var toYAnimation = new DoubleAnimationUsingKeyFrames();
            _ = toYAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(toStartY, zeroKeyTime));
            _ = toYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(toEndY, endKeyTime, _sineEase));

            // Set up events
            fromXAnimation.Completed += (sender, args) =>
            {
                fromTransform.BeginAnimation(TranslateTransform.XProperty, null);
                fromTransform.X = fromEndX;
                fromSlide.RenderTransform = null;
            };
            fromYAnimation.Completed += (sender, args) =>
            {
                fromTransform.BeginAnimation(TranslateTransform.YProperty, null);
                fromTransform.Y = fromEndY;
                fromSlide.RenderTransform = null;
            };
            toXAnimation.Completed += (sender, args) =>
            {
                toTransform.BeginAnimation(TranslateTransform.XProperty, null);
                toTransform.X = toEndX;
                toSlide.RenderTransform = null;
            };
            toYAnimation.Completed += (sender, args) =>
            {
                toTransform.BeginAnimation(TranslateTransform.YProperty, null);
                toTransform.Y = toEndY;
                toSlide.RenderTransform = null;
            };

            // Animate
            fromTransform.BeginAnimation(TranslateTransform.XProperty, fromXAnimation);
            fromTransform.BeginAnimation(TranslateTransform.YProperty, fromYAnimation);
            toTransform.BeginAnimation(TranslateTransform.XProperty, toXAnimation);
            toTransform.BeginAnimation(TranslateTransform.YProperty, toYAnimation);
            zIndexController.Stack(toSlide, fromSlide);
        }
    }
}
