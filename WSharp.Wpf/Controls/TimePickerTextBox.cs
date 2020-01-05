using System.Windows;
using System.Windows.Controls;

namespace WSharp.Wpf.Controls
{
    public class TimePickerTextBox : TextBox
    {
        static TimePickerTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimePickerTextBox), new FrameworkPropertyMetadata(typeof(TimePickerTextBox)));
        }
    }
}
