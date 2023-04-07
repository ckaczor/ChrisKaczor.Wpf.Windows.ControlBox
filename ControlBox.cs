using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using JetBrains.Annotations;

namespace ChrisKaczor.Wpf.Windows
{
    [PublicAPI]
    public static class ControlBox
    {
        private const int Style = -16;
        private const int ExtStyle = -20;

        private const int MaximizeBox = 0x10000;
        private const int MinimizeBox = 0x20000;
        private const int ContextHelp = 0x400;
        private const int SystemMenu = 0x00080000;

        public static readonly DependencyProperty HasHelpButtonProperty = DependencyProperty.RegisterAttached(
            "HasHelpButton",
            typeof(bool),
            typeof(ControlBox),
            new UIPropertyMetadata(false, OnControlBoxChanged));

        public static readonly DependencyProperty HasMaximizeButtonProperty = DependencyProperty.RegisterAttached(
            "HasMaximizeButton",
            typeof(bool),
            typeof(ControlBox),
            new UIPropertyMetadata(true, OnControlBoxChanged));

        public static readonly DependencyProperty HasMinimizeButtonProperty = DependencyProperty.RegisterAttached(
            "HasMinimizeButton",
            typeof(bool),
            typeof(ControlBox),
            new UIPropertyMetadata(true, OnControlBoxChanged));

        public static readonly DependencyProperty HasSystemMenuProperty = DependencyProperty.RegisterAttached(
            "HasSystemMenu",
            typeof(bool),
            typeof(ControlBox),
            new UIPropertyMetadata(true, OnControlBoxChanged));

        [AttachedPropertyBrowsableForType(typeof(Window))]
        public static bool GetHasHelpButton(Window element)
        {
            return (bool)element.GetValue(HasHelpButtonProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(Window))]
        public static void SetHasHelpButton(Window element, bool value)
        {
            element.SetValue(HasHelpButtonProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(Window))]
        public static bool GetHasMaximizeButton(Window element)
        {
            return (bool)element.GetValue(HasMaximizeButtonProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(Window))]
        public static void SetHasMaximizeButton(Window element, bool value)
        {
            element.SetValue(HasMaximizeButtonProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(Window))]
        public static bool GetHasMinimizeButton(Window element)
        {
            return (bool)element.GetValue(HasMinimizeButtonProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(Window))]
        public static void SetHasMinimizeButton(Window element, bool value)
        {
            element.SetValue(HasMinimizeButtonProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(Window))]
        public static bool GetHasSystemMenu(Window element)
        {
            return (bool)element.GetValue(HasSystemMenuProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(Window))]
        public static void SetHasSystemMenu(Window element, bool value)
        {
            element.SetValue(HasSystemMenuProperty, value);
        }

        private static void OnControlBoxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not Window window)
                return;

            var hWnd = new WindowInteropHelper(window).Handle;

            if (hWnd == nint.Zero)
            {
                window.SourceInitialized += OnWindowSourceInitialized;
            }
            else
            {
                UpdateStyle(window, hWnd);
                UpdateExtendedStyle(window, hWnd);
            }
        }

        private static void OnWindowSourceInitialized(object? sender, EventArgs e)
        {
            var window = (Window?)sender;

            var hWnd = new WindowInteropHelper(window!).Handle;
            UpdateStyle(window!, hWnd);
            UpdateExtendedStyle(window!, hWnd);

            window!.SourceInitialized -= OnWindowSourceInitialized;
        }

        private static void UpdateStyle(Window window, nint hWnd)
        {
            var style = NativeMethods.GetWindowLong(hWnd, Style);

            if (GetHasMaximizeButton(window))
            {
                style |= MaximizeBox;
            }
            else
            {
                style &= ~MaximizeBox;
            }

            if (GetHasMinimizeButton(window))
            {
                style |= MinimizeBox;
            }
            else
            {
                style &= ~MinimizeBox;
            }

            if (GetHasSystemMenu(window))
            {
                style |= SystemMenu;
            }
            else
            {
                style &= ~SystemMenu;
            }

            NativeMethods.SetWindowLong(hWnd, Style, style);
        }

        private static void UpdateExtendedStyle(Window window, nint hWnd)
        {
            var style = NativeMethods.GetWindowLong(hWnd, ExtStyle);

            if (GetHasHelpButton(window))
            {
                style |= ContextHelp;
            }
            else
            {
                style &= -~ContextHelp;
            }

            NativeMethods.SetWindowLong(hWnd, ExtStyle, style);
        }

        private static class NativeMethods
        {
            [DllImport("user32.dll")]
            internal static extern int GetWindowLong(nint hWnd, int index);

            [DllImport("user32.dll")]
            internal static extern int SetWindowLong(nint hWnd, int index, int newLong);
        }
    }
}