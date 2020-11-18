// <copyright file="GlobalHotKeyHost.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.UI.WPF.Tools.HotKey
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Windows.Input;
    using System.Windows.Interop;
    using SoluiNet.DevTools.Core.UI.WPF.Application;

    /// <summary>
    /// The HotKeyHost needed for working with hot keys (taken from https://www.codeproject.com/Tips/274003/Global-Hotkeys-in-WPF).
    /// </summary>
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.NamingRules",
        "SA1310:FieldNamesMustNotContainUnderscore",
        Justification = "Use the default windows notification name.")]
    public sealed class GlobalHotKeyHost : IDisposable
    {
        // ReSharper disable once InconsistentNaming
        private const int WM_HotKey = 786;

        // Can be replaced with "Random"-class
        private static readonly SerialCounter IdGenerator = new SerialCounter(1);

        private readonly HwndSourceHook hook;
        private readonly HwndSource handleSource;

        private readonly Dictionary<int, GlobalHotKey> hotKeys = new Dictionary<int, GlobalHotKey>();

        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalHotKeyHost"/> class.
        /// </summary>
        /// <param name="sourceHandle">The handle of the window. Must not be null.</param>
        public GlobalHotKeyHost(HwndSource sourceHandle)
        {
            this.hook = new HwndSourceHook(this.WndProc);
            this.handleSource = sourceHandle ?? throw new ArgumentNullException(nameof(sourceHandle));
            sourceHandle.AddHook(this.hook);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="GlobalHotKeyHost"/> class.
        /// </summary>
        ~GlobalHotKeyHost()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Will be raised if any registered hotKey is pressed
        /// </summary>
        public event EventHandler<HotKeyEventArgs> HotKeyPressed;

        /// <summary>
        /// Gets all registered hot keys.
        /// </summary>
        public IEnumerable<GlobalHotKey> HotKeys
        {
            get
            {
                return this.hotKeys.Values;
            }
        }

        /// <summary>
        /// Disposes of the global hot key host.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Adds an hot key.
        /// </summary>
        /// <param name="hotKey">The hotKey which will be added. Must not be null and can be registered only once.</param>
        public void AddHotKey(GlobalHotKey hotKey)
        {
            if (hotKey == null)
            {
                throw new ArgumentNullException(nameof(hotKey));
            }

            if (hotKey.Key == 0)
            {
                throw new ArgumentException("hotKey.Key is 0", nameof(hotKey));
            }

            if (this.hotKeys.ContainsValue(hotKey))
            {
                throw new HotKeyAlreadyRegisteredException("HotKey already registered!", hotKey);
            }

            int id = IdGenerator.Next();
            if (hotKey.Enabled)
            {
                this.RegisterHotKey(id, hotKey);
            }

            hotKey.PropertyChanged += this.HotKey_PropertyChanged;
            this.hotKeys[id] = hotKey;
        }

        /// <summary>
        /// Removes an hotKey.
        /// </summary>
        /// <param name="hotKey">The hotKey to be removed.</param>
        /// <returns>True if success, otherwise false.</returns>
        public bool RemoveHotKey(GlobalHotKey hotKey)
        {
            var kvPair = this.hotKeys.FirstOrDefault(h => h.Value.Equals(hotKey));

            if (kvPair.Value == null)
            {
                return false;
            }

            kvPair.Value.PropertyChanged -= this.HotKey_PropertyChanged;
            if (kvPair.Value.Enabled)
            {
                this.UnregisterHotKey(kvPair.Key);
            }

            return this.hotKeys.Remove(kvPair.Key);
        }

        /// <summary>
        /// Disposes of the global hot key host.
        /// </summary>
        /// <param name="disposing">The disposing instance.</param>
        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.handleSource.RemoveHook(this.hook);
            }

            for (var i = this.hotKeys.Count - 1; i >= 0; i--)
            {
                this.RemoveHotKey(this.hotKeys.Values.ElementAt(i));
            }

            this.disposed = true;
        }

        private void RegisterHotKey(int id, GlobalHotKey hotKey)
        {
            if ((int)this.handleSource.Handle != 0)
            {
                NativeMethods.RegisterHotKey(this.handleSource.Handle, id, (int)hotKey.Modifiers, KeyInterop.VirtualKeyFromKey(hotKey.Key));
                var error = Marshal.GetLastWin32Error();
                if (error != 0)
                {
                    Exception e = new Win32Exception(error);

                    if (error == 1409)
                    {
                        throw new HotKeyAlreadyRegisteredException(e.Message, hotKey, e);
                    }
                    else
                    {
                        throw e;
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("Handle is invalid");
            }
        }

        private void UnregisterHotKey(int id)
        {
            if ((int)this.handleSource.Handle != 0)
            {
                var result = NativeMethods.UnregisterHotKey(this.handleSource.Handle, id);

                int error = Marshal.GetLastWin32Error();
                if (error != 0)
                {
                    throw new Win32Exception(error);
                }
            }
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HotKey)
            {
                if (this.hotKeys.ContainsKey((int)wParam))
                {
                    GlobalHotKey h = this.hotKeys[(int)wParam];
                    h.RaiseOnHotKeyPressed();
                    this.HotKeyPressed?.Invoke(this, new HotKeyEventArgs(h));
                }
            }

            return new IntPtr(0);
        }

        private void HotKey_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var kvPair = this.hotKeys.FirstOrDefault(h => Equals(h.Value, sender));
            if (kvPair.Value != null)
            {
                if (e.PropertyName == "Enabled")
                {
                    if (kvPair.Value.Enabled)
                    {
                        this.RegisterHotKey(kvPair.Key, kvPair.Value);
                    }
                    else
                    {
                        this.UnregisterHotKey(kvPair.Key);
                    }
                }
                else if (e.PropertyName == "Key" || e.PropertyName == "Modifiers")
                {
                    if (kvPair.Value.Enabled)
                    {
                        this.UnregisterHotKey(kvPair.Key);
                        this.RegisterHotKey(kvPair.Key, kvPair.Value);
                    }
                }
            }
        }

        /// <summary>
        /// A class which allows to use a counter.
        /// </summary>
        public class SerialCounter
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SerialCounter"/> class.
            /// </summary>
            /// <param name="start">The start value.</param>
            public SerialCounter(int start)
            {
                this.Current = start;
            }

            /// <summary>
            /// Gets the current value.
            /// </summary>
            public int Current { get; private set; }

            /// <summary>
            /// Get to the next value.
            /// </summary>
            /// <returns>Returns the next value.</returns>
            public int Next()
            {
                return ++this.Current;
            }
        }
    }
}