using System.Windows.Media;
using WSharp.Wpf.Colors;

namespace WSharp.Wpf.Extensions
{
    internal static class LabExtensions
    {
        public static Color ToColor(this Lab lab)
        {
            var xyz = lab.ToXyz();

            return xyz.ToColor();
        }

        public static Xyz ToXyz(this Lab lab)
        {
            static double LabXyz(double d)
            {
                return d > LabConstants.ECubedRoot 
                    ? d * d * d 
                    : ((116 * d) - 16) / LabConstants.K;
            }

            var y = (lab.L + 16.0) / 116.0;
            
            var x = double.IsNaN(lab.A) 
                ? y 
                : y + (lab.A / 500.0);

            var z = double.IsNaN(lab.B) 
                ? y 
                : y - (lab.B / 200.0);

            y = LabConstants.WhitePointY * LabXyz(y);
            x = LabConstants.WhitePointX * LabXyz(x);
            z = LabConstants.WhitePointZ * LabXyz(z);

            return new Xyz(x, y, z);
        }
    }
}
