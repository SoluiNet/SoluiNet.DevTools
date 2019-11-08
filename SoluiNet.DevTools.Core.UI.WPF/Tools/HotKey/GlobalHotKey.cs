// <copyright file="GlobalHotKey.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.UI.WPF.Tools.HotKey
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Windows.Input;
    using System.Windows.Interop;

    /// <summary>
    /// Represents an hot key (taken from https://www.codeproject.com/Tips/274003/Global-Hotkeys-in-WPF).
    /// </summary>
    [Serializable]
    public class GlobalHotKey : INotifyPropertyChanged, ISerializable, IEquatable<GlobalHotKey>
    {
        private Key key;
        private ModifierKeys modifiers;
        private bool enabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalHotKey"/> class. This instance has to be registered in an HotKeyHost.
        /// </summary>
        public GlobalHotKey()
        {
            // do nothing
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalHotKey"/> class. This instance has to be registered in an HotKeyHost.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="modifiers">The modifier. Multiple modifiers can be combined with or.</param>
        public GlobalHotKey(Key key, ModifierKeys modifiers)
            : this(key, modifiers, true)
        {
            // do nothing
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalHotKey"/> class. This instance has to be registered in an HotKeyHost.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="modifiers">The modifier. Multiple modifiers can be combined with or.</param>
        /// <param name="enabled">Specifies whether the HotKey will be enabled when registered to an HotKeyHost.</param>
        public GlobalHotKey(Key key, ModifierKeys modifiers, bool enabled)
        {
            this.Key = key;
            this.Modifiers = modifiers;
            this.Enabled = enabled;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalHotKey"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected GlobalHotKey(SerializationInfo info, StreamingContext context)
        {
            this.Key = (Key)info.GetValue("Key", typeof(Key));
            this.Modifiers = (ModifierKeys)info.GetValue("Modifiers", typeof(ModifierKeys));
            this.Enabled = info.GetBoolean("Enabled");
        }

        /// <summary>
        /// The property changed event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Will be raised if the hot key is pressed (works only if registered in HotKeyHost)
        /// </summary>
        public event EventHandler<HotKeyEventArgs> HotKeyPressed;

        /// <summary>
        /// Gets or sets the Key. Must not be null when registering to an HotKeyHost.
        /// </summary>
        public Key Key
        {
            get
            {
                return this.key;
            }

            set
            {
                if (this.key != value)
                {
                    this.key = value;
                    this.OnPropertyChanged("Key");
                }
            }
        }

        /// <summary>
        /// Gets or sets the modifiers. Multiple modifiers can be combined with or.
        /// </summary>
        public ModifierKeys Modifiers
        {
            get
            {
                return this.modifiers;
            }

            set
            {
                if (this.modifiers != value)
                {
                    this.modifiers = value;
                    this.OnPropertyChanged("Modifiers");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this hot key is enabled or not.
        /// </summary>
        public bool Enabled
        {
            get
            {
                return this.enabled;
            }

            set
            {
                if (value != this.enabled)
                {
                    this.enabled = value;
                    this.OnPropertyChanged("Enabled");
                }
            }
        }

        /// <summary>
        /// Check if this hot key is equal to the overgiven hot key instance.
        /// </summary>
        /// <param name="hotKeyInstance">The hot key instance.</param>
        /// <returns>Returns true if both objects are equal.</returns>
        public override bool Equals(object hotKeyInstance)
        {
            if (hotKeyInstance is GlobalHotKey hotKey)
            {
                return this.Equals(hotKey);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check if this hot key is equal to the overgiven hot key instance.
        /// </summary>
        /// <param name="hotKeyInstance">The hot key instance.</param>
        /// <returns>Returns true if both objects are equal.</returns>
        public bool Equals(GlobalHotKey hotKeyInstance)
        {
            return this.Key == hotKeyInstance?.Key && this.Modifiers == hotKeyInstance.Modifiers;
        }

        /// <summary>
        /// Calculate the hash code.
        /// </summary>
        /// <returns>Returns an hash code identifying this instance.</returns>
        public override int GetHashCode()
        {
            return (int)this.Modifiers + (10 * (int)this.Key);
        }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>Returns a string representation of the instance.</returns>
        public override string ToString()
        {
            return $"{this.Key} + {this.Modifiers} ({(this.Enabled ? string.Empty : "Not ")}Enabled)";
        }

        /// <summary>
        /// Get object data from instance.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Key", this.Key, typeof(Key));
            info.AddValue("Modifiers", this.Modifiers, typeof(ModifierKeys));
            info.AddValue("Enabled", this.Enabled);
        }

        /// <summary>
        /// Raise event handler for hot key pressed.
        /// </summary>
        internal void RaiseOnHotKeyPressed()
        {
            this.OnHotKeyPress();
        }

        /// <summary>
        /// Event handler for hot key pressed.
        /// </summary>
        protected virtual void OnHotKeyPress()
        {
            this.HotKeyPressed?.Invoke(this, new HotKeyEventArgs(this));
        }

        /// <summary>
        /// The property changed event handler.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
