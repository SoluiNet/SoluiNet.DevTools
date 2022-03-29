// <copyright file="ApplicationIdentificationTools.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.TimeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
#if BUILD_FOR_WINDOWS
    using System.Windows.Media;
#endif
    using SoluiNet.DevTools.Core.Reference;

    /// <summary>
    /// Provides a collection of methods to identify applications.
    /// </summary>
    public static class ApplicationIdentificationTools
    {
        /// <summary>
        /// An enum which gives an oversight over all identifiable applications.
        /// </summary>
        public enum IdentifiableApplication
        {
            /// <summary>
            /// Google Chrome
            /// </summary>
            Chrome,

            /// <summary>
            /// Editor
            /// </summary>
            Editor,

            /// <summary>
            /// Microsoft Excel
            /// </summary>
            Excel,

            /// <summary>
            /// Git
            /// </summary>
            Git,

            /// <summary>
            /// KeePass
            /// </summary>
            KeePass,

            /// <summary>
            /// Notepad++
            /// </summary>
            NotepadPlusPlus,

            /// <summary>
            /// Microsoft Outlook
            /// </summary>
            Outlook,

            /// <summary>
            /// Remote Desktop Manager
            /// </summary>
            RemoteDesktopManager,

            /// <summary>
            /// Microsoft Teams
            /// </summary>
            MicrosoftTeams,

            /// <summary>
            /// TortoiseGit
            /// </summary>
            TortoiseGit,

            /// <summary>
            /// Microsoft Visual Studio
            /// </summary>
            VisualStudio,
        }

        /// <summary>
        /// Gets the search string for Microsoft Visual Studio.
        /// </summary>
        public static string VisualStudio
        {
            // RegEx: "^(.+ -)?\s*Microsoft Visual Studio$"
            get { return "Microsoft Visual Studio"; }
        }

        /// <summary>
        /// Gets the search string for Microsoft Outlook.
        /// </summary>
        public static string Outlook
        {
            // RegEx: "^(.+ -)?\s*(Outlook|Besprechung|Nachricht \(HTML\))\s*$"
            get { return "Outlook"; }
        }

        /// <summary>
        /// Gets the search string for Remote Desktop Manager.
        /// </summary>
        public static string RemoteDesktopManager
        {
            // RegEx:  "^Remote Desktop Manager\s*(\[.+\])$"
            get { return "Remote Desktop Manager"; }
        }

        /// <summary>
        /// Gets the search string for Microsoft Teams.
        /// </summary>
        public static string Teams
        {
            // RegEx: "^((.+ |)?\s*(Microsoft )?Teams$"
            get { return "Microsoft Teams"; }
        }

        /// <summary>
        /// Gets the search string for Microsoft Excel.
        /// </summary>
        public static string Excel
        {
            // RegEx: "^(.+ -)?\s*Excel$"
            get { return "Excel"; }
        }

        /// <summary>
        /// Gets the search string for Notepad++.
        /// </summary>
        public static string NotepadPlusPlus
        {
            // RegEx: "^(.+ -)?\s*Notepad\+\+$"
            get { return "Notepad++"; }
        }

        /// <summary>
        /// Gets the search string for Editor.
        /// </summary>
        public static string Editor
        {
            // RegEx: "^(.+ -)?\s*Editor$"
            get { return "Editor"; }
        }

        /// <summary>
        /// Gets the search string for Google Chrome.
        /// </summary>
        public static string Chrome
        {
            // RegEx: "^(.+ -)?\s*Google Chrome$"
            get { return "Google Chrome"; }
        }

        /// <summary>
        /// Gets the search string for Git.
        /// </summary>
        public static string Git
        {
            // RegEx: "^(MINGW64\:).+$"
            get { return "Git"; }
        }

        /// <summary>
        /// Gets the search string for TortoiseGit.
        /// </summary>
        public static string TortoiseGit
        {
            // RegEx: "^(.+ -)?\s*TortoiseGit$"
            get { return "TortoiseGit"; }
        }

        /// <summary>
        /// Gets the search string for KeePass.
        /// </summary>
        public static string KeePass
        {
            // RegEx: "^(.+ -)?\s*KeePass"
            get { return "KeePass"; }
        }

        /// <summary>
        /// Extract the application name from a window caption.
        /// </summary>
        /// <param name="windowTitle">The window caption.</param>
        /// <returns>Returns the application identification.</returns>
        public static string ExtractApplicationName(this string windowTitle)
        {
            if (string.IsNullOrEmpty(windowTitle))
            {
                return string.Empty;
            }

            if (windowTitle.Contains(VisualStudio, StringComparison.Ordinal))
            {
                return VisualStudio;
            }
            else if (windowTitle.Contains(Outlook, StringComparison.Ordinal))
            {
                return Outlook;
            }
            else if (windowTitle.Contains(RemoteDesktopManager, StringComparison.Ordinal))
            {
                return RemoteDesktopManager;
            }
            else if (windowTitle.Contains(Teams, StringComparison.Ordinal))
            {
                return Teams;
            }
            else if (windowTitle.Contains(Excel, StringComparison.Ordinal))
            {
                return Excel;
            }
            else if (windowTitle.Contains(NotepadPlusPlus, StringComparison.Ordinal))
            {
                return NotepadPlusPlus;
            }
            else if (windowTitle.Contains(Editor, StringComparison.Ordinal))
            {
                return Editor;
            }
            else if (windowTitle.Contains(Chrome, StringComparison.Ordinal))
            {
                return Chrome;
            }
            else if (windowTitle.Contains(TortoiseGit, StringComparison.Ordinal))
            {
                return TortoiseGit;
            }
            else if (windowTitle.Contains(KeePass, StringComparison.Ordinal))
            {
                return KeePass;
            }

            return string.Empty;
        }

        /// <summary>
        /// Get the background accents for the overgiven application identification.
        /// </summary>
        /// <param name="applicationName">The application identification.</param>
        /// <returns>Returns a <see cref="IBrush"/> for the overgiven application identification.</returns>
        public static IBrush GetBackgroundAccent(string applicationName)
        {
            if (string.IsNullOrEmpty(applicationName))
            {
                return new SolidColorBrush(Colors.White);
            }

            if (applicationName.Equals(VisualStudio, StringComparison.Ordinal))
            {
                return new LinearGradientBrush(Colors.BlueViolet, Color.FromRgb(50, 50, 50), 0.75);
            }
            else if (applicationName.Equals(Outlook, StringComparison.Ordinal))
            {
                return new LinearGradientBrush(Colors.DodgerBlue, Colors.WhiteSmoke, 0.75);
            }
            else if (applicationName.Equals(Teams, StringComparison.Ordinal))
            {
                return new LinearGradientBrush(Colors.DarkViolet, Colors.WhiteSmoke, 0.75);
            }
            else if (applicationName.Equals(RemoteDesktopManager, StringComparison.Ordinal))
            {
                return new LinearGradientBrush(Colors.WhiteSmoke, Colors.DeepSkyBlue, 0.75);
            }
            else if (applicationName.Equals(Excel, StringComparison.Ordinal))
            {
                return new LinearGradientBrush(Colors.DarkGreen, Colors.WhiteSmoke, 0.75);
            }
            else if (applicationName.Equals(NotepadPlusPlus, StringComparison.Ordinal))
            {
                return new LinearGradientBrush(Colors.Lime, Colors.WhiteSmoke, 0.75);
            }
            else if (applicationName.Equals(Editor, StringComparison.Ordinal))
            {
                return new LinearGradientBrush(Colors.LightBlue, Colors.WhiteSmoke, 0.75);
            }
            else if (applicationName.Equals(Chrome, StringComparison.Ordinal))
            {
                var linearGradient = new LinearGradientBrush();

                linearGradient.GradientStops.Add(new GradientStop(Colors.OrangeRed, 0.2));
                linearGradient.GradientStops.Add(new GradientStop(Colors.Yellow, 0.4));
                linearGradient.GradientStops.Add(new GradientStop(Colors.Green, 0.6));
                linearGradient.GradientStops.Add(new GradientStop(Colors.DeepSkyBlue, 0.8));

                linearGradient.StartPoint = new Point(0, 0);
                linearGradient.EndPoint = new Point(1, 0.2);

                return linearGradient;
            }
            else if (applicationName.Equals(TortoiseGit, StringComparison.Ordinal))
            {
                return new LinearGradientBrush(Colors.DarkGray, Colors.LightBlue, 0.75);
            }
            else if (applicationName.Equals(KeePass, StringComparison.Ordinal))
            {
                return new LinearGradientBrush(Colors.DarkBlue, Colors.LightBlue, 0.75);
            }

            return new SolidColorBrush(Colors.White);
        }
    }
}
