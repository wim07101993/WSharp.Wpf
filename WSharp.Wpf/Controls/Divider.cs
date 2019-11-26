using System.Windows;
using System.Windows.Controls;
using WSharp.Wpf.Extensions;

namespace WSharp.Wpf.Controls
{
    [TemplatePart(Name = BorderPartName, Type = typeof(Divider))]
    public class Divider : Control
    {
        public const string BorderPartName = "PART_Border";

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            nameof(Orientation),
            typeof(Orientation),
            typeof(Divider),
            new PropertyMetadata(default(Orientation)));

        public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register(
            nameof(Thickness),
            typeof(double),
            typeof(Divider),
            new PropertyMetadata((double)1, OnThicknesChanged));

        protected Border border;

        static Divider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Divider), new FrameworkPropertyMetadata(typeof(Divider)));
        }

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        public double Thickness
        {
            get => (double)GetValue(ThicknessProperty);
            set => SetValue(ThicknessProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            border = this.MustHaveTemplateChild<Border>(BorderPartName);

            UpdateBorder();
        }

        public void UpdateBorder()
        {

            switch (Orientation)
            {
                case Orientation.Horizontal:
                    if (border != null)
                    border.BorderThickness = new Thickness(0, 0, 0, Thickness);
                    Height = Thickness;
                    Width = double.NaN;
                    break;
                case Orientation.Vertical:
                    border.BorderThickness = new Thickness(0, 0, Thickness, 0);
                    Width = Thickness;
                    Height = double.NaN;
                    break;
            }
        }

        private static void OnThicknesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is Divider divider))
                return;

            if (divider.border != null)
                divider.UpdateBorder();
        }
    }
}
