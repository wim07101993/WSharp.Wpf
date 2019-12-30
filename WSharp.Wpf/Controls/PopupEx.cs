using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;

namespace WSharp.Wpf.Controls
{
    /// <summary>
    ///     This custom popup can be used by validation error templates or something else. It
    ///     provides some additional nice features:
    ///     - repositioning if host-window size or location changed
    ///     - repositioning if host-window gets maximized and vice versa
    ///     - it's only topmost if the host-window is activated
    /// </summary>
    public class PopupEx : Popup
    {
        private Window _hostWindow;
        private bool? _appliedTopMost;
        private static readonly IntPtr hWndTopmost = new IntPtr(-1);
        private static readonly IntPtr hWndNotTopMost = new IntPtr(-2);
        private static readonly IntPtr hWndTop = new IntPtr(0);
        private static readonly IntPtr hWndBottom = new IntPtr(1);

        public static readonly DependencyProperty CloseOnMouseLeftButtonDownProperty = DependencyProperty.Register(
            nameof(CloseOnMouseLeftButtonDown),
            typeof(bool),
            typeof(PopupEx),
            new PropertyMetadata(false));

        public static readonly DependencyProperty AllowTopMostProperty = DependencyProperty.Register(
            nameof(AllowTopMost),
            typeof(bool),
            typeof(PopupEx),
            new PropertyMetadata(true));

        /// <summary>Gets/sets if the popup can be closed by left mouse button down.</summary>
        public bool CloseOnMouseLeftButtonDown
        {
            get => (bool)GetValue(CloseOnMouseLeftButtonDownProperty);
            set => SetValue(CloseOnMouseLeftButtonDownProperty, value);
        }

        public bool AllowTopMost
        {
            get => (bool)GetValue(AllowTopMostProperty);
            set => SetValue(AllowTopMostProperty, value);
        }

        public PopupEx()
        {
            Loaded += PopupExLoaded;
            Opened += PopupExOpened;
        }

        #region METHODS

        /// <summary>Causes the popup to update it's position according to it's current settings.</summary>
        public void RefreshPosition()
        {
            var offset = HorizontalOffset;
            // "bump" the offset to cause the popup to reposition itself on its own
            SetCurrentValue(HorizontalOffsetProperty, offset + 1);
            SetCurrentValue(HorizontalOffsetProperty, offset);
        }

        private void PopupExLoaded(object sender, RoutedEventArgs e)
        {
            if (!(PlacementTarget is FrameworkElement target))
                return;

            _hostWindow = Window.GetWindow(target);
            if (_hostWindow == null)
                return;

            _hostWindow.LocationChanged -= HostWindowSizeOrLocationChanged;
            _hostWindow.LocationChanged += HostWindowSizeOrLocationChanged;
            _hostWindow.SizeChanged -= HostWindowSizeOrLocationChanged;
            _hostWindow.SizeChanged += HostWindowSizeOrLocationChanged;
            target.SizeChanged -= HostWindowSizeOrLocationChanged;
            target.SizeChanged += HostWindowSizeOrLocationChanged;
            _hostWindow.StateChanged -= HostWindowStateChanged;
            _hostWindow.StateChanged += HostWindowStateChanged;
            _hostWindow.Activated -= HostWindowActivated;
            _hostWindow.Activated += HostWindowActivated;
            _hostWindow.Deactivated -= HostWindowDeactivated;
            _hostWindow.Deactivated += HostWindowDeactivated;

            Unloaded -= PopupExUnloaded;
            Unloaded += PopupExUnloaded;
        }

        private void PopupExOpened(object sender, EventArgs e) => SetTopmostState(_hostWindow?.IsActive ?? true);

        private void HostWindowActivated(object sender, EventArgs e) => SetTopmostState(true);

        private void HostWindowDeactivated(object sender, EventArgs e) => SetTopmostState(false);

        private void PopupExUnloaded(object sender, RoutedEventArgs e)
        {
            if (PlacementTarget is FrameworkElement target)
                target.SizeChanged -= HostWindowSizeOrLocationChanged;

            if (_hostWindow != null)
            {
                _hostWindow.LocationChanged -= HostWindowSizeOrLocationChanged;
                _hostWindow.SizeChanged -= HostWindowSizeOrLocationChanged;
                _hostWindow.StateChanged -= HostWindowStateChanged;
                _hostWindow.Activated -= HostWindowActivated;
                _hostWindow.Deactivated -= HostWindowDeactivated;
            }

            Unloaded -= PopupExUnloaded;
            Opened -= PopupExOpened;
            _hostWindow = null;
        }

        private void HostWindowStateChanged(object sender, EventArgs e)
        {
            if (_hostWindow == null || _hostWindow.WindowState == WindowState.Minimized)
                return;
            
            // special handling for validation popup
            var holder = PlacementTarget is FrameworkElement target
                ? target.DataContext as AdornedElementPlaceholder
                : null;

            if (holder != null && holder.AdornedElement != null)
            {
                PopupAnimation = PopupAnimation.None;
                IsOpen = false;
                var errorTemplate = holder.AdornedElement.GetValue(Validation.ErrorTemplateProperty);
                holder.AdornedElement.SetValue(Validation.ErrorTemplateProperty, null);
                holder.AdornedElement.SetValue(Validation.ErrorTemplateProperty, errorTemplate);
            }
        }

        private void HostWindowSizeOrLocationChanged(object sender, EventArgs e) => RefreshPosition();

        private void SetTopmostState(bool isTop)
        {
            isTop &= AllowTopMost;
            // Don’t apply state if it’s the same as incoming state
            if (_appliedTopMost.HasValue && _appliedTopMost == isTop)
                return;

            if (Child == null)
                return;

            if (!(PresentationSource.FromVisual(Child) is HwndSource hwndSource))
                return;

            var hwnd = hwndSource.Handle;

            if (!GetWindowRect(hwnd, out var rect))
                return;

            //Debug.WriteLine("setting z-order " + isTop);

            var left = rect.Left;
            var top = rect.Top;
            var width = rect.Width;
            var height = rect.Height;
            if (isTop)
                _ = InternSetWindowPos(hwnd, hWndTopmost, left, top, width, height, Swp.TOPMOST);
            else
            {
                // Z-Order would only get refreshed/reflected if clicking the the titlebar (as
                // opposed to other parts of the external
                // window) unless I first set the popup to HWND_BOTTOM then HWND_TOP before HWND_NOTOPMOST
                _ = InternSetWindowPos(hwnd, hWndBottom, left, top, width, height, Swp.TOPMOST);
                _ = InternSetWindowPos(hwnd, hWndTop, left, top, width, height, Swp.TOPMOST);
                _ = InternSetWindowPos(hwnd, hWndNotTopMost, left, top, width, height, Swp.TOPMOST);
            }

            _appliedTopMost = isTop;
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (CloseOnMouseLeftButtonDown)
                IsOpen = false;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal static int LOWORD(int i) => (short)(i & 0xFFFF);

        [SecurityCritical]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", EntryPoint = "GetWindowRect", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, out Rect lpRect);

        [SecurityCritical]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", EntryPoint = "SetWindowPos", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, Swp uFlags);

        [SecurityCritical]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private static bool InternSetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, Swp uFlags)
        {
            // If this fails it's never worth taking down the process. Let the caller deal with
            // the error if they want.
            return SetWindowPos(hWnd, hWndInsertAfter, x, y, cx, cy, uFlags);
        }

        #endregion METHODS

        #region TYPES

        /// <summary>SetWindowPos options</summary>
        [Flags]
        internal enum Swp
        {
            ASYNCWINDOWPOS = 0x4000,
            DEFERERASE = 0x2000,
            DRAWFRAME = 0x0020,
            FRAMECHANGED = 0x0020,
            HIDEWINDOW = 0x0080,
            NOACTIVATE = 0x0010,
            NOCOPYBITS = 0x0100,
            NOMOVE = 0x0002,
            NOOWNERZORDER = 0x0200,
            NOREDRAW = 0x0008,
            NOREPOSITION = 0x0200,
            NOSENDCHANGING = 0x0400,
            NOSIZE = 0x0001,
            NOZORDER = 0x0004,
            SHOWWINDOW = 0x0040,
            TOPMOST = Swp.NOACTIVATE | Swp.NOOWNERZORDER | Swp.NOSIZE | Swp.NOMOVE | Swp.NOREDRAW | Swp.NOSENDCHANGING,
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Point
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Size
        {
            public int cx;
            public int cy;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Rect
        {
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            public void Offset(int dx, int dy)
            {
                Left += dx;
                Top += dy;
                Right += dx;
                Bottom += dy;
            }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            public int Left { get; set; }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            public int Right { get; set; }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            public int Top { get; set; }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            public int Bottom { get; set; }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            public int Width => Right - Left;

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            public int Height => Bottom - Top;

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            public Point Position => new Point { x = Left, y = Top };

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            public Size Size => new Size { cx = Width, cy = Height };

            public static Rect Union(Rect rect1, Rect rect2)
            {
                return new Rect
                {
                    Left = Math.Min(rect1.Left, rect2.Left),
                    Top = Math.Min(rect1.Top, rect2.Top),
                    Right = Math.Max(rect1.Right, rect2.Right),
                    Bottom = Math.Max(rect1.Bottom, rect2.Bottom),
                };
            }

            public override bool Equals(object obj)
            {
                try
                {
                    var rc = (Rect)obj;
                    return rc.Bottom == Bottom
                        && rc.Left == Left
                        && rc.Right == Right
                        && rc.Top == Top;
                }
                catch (InvalidCastException)
                {
                    return false;
                }
            }

            public override int GetHashCode() => ((Left << 16) | LOWORD(Right)) ^ ((Top << 16) | LOWORD(Bottom));
        }

        #endregion TYPES
    }
}
