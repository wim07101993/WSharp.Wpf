using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Effects;

namespace WSharp.Wpf
{
    internal static class ShadowInfo
    {
        private static readonly IDictionary<EShadowDepth, DropShadowEffect> ShadowsDictionary;

        static ShadowInfo()
        {
            var resourceDictionary = new ResourceDictionary { Source = new Uri("pack://application:,,,/WSharp.Wpf;component/Themes/Shadows.xaml", UriKind.Absolute) };

            ShadowsDictionary = new Dictionary<EShadowDepth, DropShadowEffect>
            {
                { EShadowDepth.Depth0, null },
                { EShadowDepth.Depth1, (DropShadowEffect)resourceDictionary["ShadowDepth1"] },
                { EShadowDepth.Depth2, (DropShadowEffect)resourceDictionary["ShadowDepth2"] },
                { EShadowDepth.Depth3, (DropShadowEffect)resourceDictionary["ShadowDepth3"] },
                { EShadowDepth.Depth4, (DropShadowEffect)resourceDictionary["ShadowDepth4"] },
                { EShadowDepth.Depth5, (DropShadowEffect)resourceDictionary["ShadowDepth5"] },
            };
        }

        public static DropShadowEffect GetDropShadow(EShadowDepth depth) => ShadowsDictionary[depth];
    }
}
