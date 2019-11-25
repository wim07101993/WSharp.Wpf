using System.Windows;
using WSharp.Wpf.Controls;

namespace WSharp.Wpf.Material.Controls
{
    public class Card : ClippedContentControl
    {
        static Card()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Card), new FrameworkPropertyMetadata(typeof(Card)));
        }

    }
}
