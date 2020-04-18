// <copyright file="ClipboardUserControl.xaml.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.Clipboard
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;

    /// <summary>
    /// Interaction logic for ClipboardUserControl.xaml.
    /// </summary>
    public partial class ClipboardUserControl : UserControl
    {
        IntPtr nextClipboardViewer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClipboardUserControl"/> class.
        /// </summary>
        public ClipboardUserControl()
        {
            this.InitializeComponent();
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case ClipboardNativeMethods.WM_DRAWCLIPBOARD:
                    this.ClipboardDataGrid.Items.Add(new { Timepoint = DateTime.UtcNow, Type = string.Empty, ClipboardContent = Clipboard.GetText() });
                    ClipboardNativeMethods.SendMessage(this.nextClipboardViewer, (uint)msg, wParam, lParam);
                    break;

                case ClipboardNativeMethods.WM_CHANGECBCHAIN:
                    if (wParam == this.nextClipboardViewer)
                    {
                        this.nextClipboardViewer = lParam;
                    }
                    else
                    {
                        ClipboardNativeMethods.SendMessage(this.nextClipboardViewer, (uint)msg, wParam, lParam);
                    }

                    break;
            }

            return IntPtr.Zero;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(Window.GetWindow(this)).Handle);
            source.AddHook(new HwndSourceHook(this.WndProc));

            this.nextClipboardViewer = ClipboardNativeMethods.SetClipboardViewer(new WindowInteropHelper(Window.GetWindow(this)).Handle);
        }
    }
}
