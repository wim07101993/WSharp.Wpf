using System.Windows;
using WSharp.Logging;

namespace WSharp.Wpf.NetFramework.Controls
{
    public class LogEntry : AControl
    {
        #region DEPENDENCY PROPERTIES

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value),
            typeof(ILogEntry),
            typeof(LogEntry));

        #endregion DEPENDENCY PROPERTIES


        #region CONSTRUCTORS

        static LogEntry()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LogEntry), new FrameworkPropertyMetadata(typeof(LogEntry)));
        }

        #endregion CONSTRUCTORS


        #region PROPERTIES

        public ILogEntry Value
        {
            get => (ILogEntry)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        #endregion PROPERTIES
    }
}
