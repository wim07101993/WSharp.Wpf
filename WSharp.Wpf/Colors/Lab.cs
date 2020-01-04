namespace WSharp.Wpf.Colors
{
    internal struct Lab
    {
        public double L { get; }
        public double A { get; }
        public double B { get; }

        public Lab(double l, double a, double b)
        {
            L = l;
            A = a;
            B = b;
        }
    }
}
