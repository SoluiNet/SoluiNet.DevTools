using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SoluiNet.DevTools.Utils.TimeTracking
{
    public static class ApplicationIdentificationTools
    {
        public enum IdentifiableApplications
        {
            Chrome,
            Editor,
            Excel,
            Git,
            KeePass,
            NotepadPlusPlus,
            Outlook,
            RemoteDesktopManager,
            MicrosoftTeams,
            TortoiseGit,
            VisualStudio
        }

        public static string VisualStudio
        {
            // RegEx: "^(.+ -)?\s*Microsoft Visual Studio$"
            get { return "Microsoft Visual Studio"; }
        }

        public static string Outlook
        {
            // RegEx: "^(.+ -)?\s*Outlook$"
            get { return "Outlook"; }
        }

        public static string RemoteDesktopManager
        {
            // RegEx:  "^Remote Desktop Manager\s*(\[.+\])$"
            get { return "Remote Desktop Manager"; }
        }

        public static string Teams
        {
            // RegEx: "^((.+ |)?\s*(Microsoft )?Teams$"
            get { return "Microsoft Teams"; }
        }

        public static string Excel
        {
            // RegEx: "^(.+ -)?\s*Excel$"
            get { return "Excel"; }
        }

        public static string NotepadPlusPlus
        {
            // RegEx: "^(.+ -)?\s*Notepad\+\+$"
            get { return "Notepad++"; }
        }

        public static string Editor
        {
            // RegEx: "^(.+ -)?\s*Editor$"
            get { return "Editor"; }
        }

        public static string Chrome
        {
            // RegEx: "^(.+ -)?\s*Google Chrome$"
            get { return "Google Chrome"; }
        }

        public static string Git
        {
            // RegEx: "^(MINGW64\:).+$"
            get { return "Git"; }
        }

        public static string TortoiseGit
        {
            // RegEx: "^(.+ -)?\s*TortoiseGit$"
            get { return "TortoiseGit"; }
        }

        public static string KeePass
        {
            // RegEx: "^(.+ -)?\s*KeePass"
            get { return "KeePass"; }
        }

        public static string ExtractApplicationName(this string windowTitle)
        {
            if (string.IsNullOrEmpty(windowTitle))
            {
                return string.Empty;
            }

            if (windowTitle.Contains(VisualStudio))
            {
                return VisualStudio;
            }
            else if (windowTitle.Contains(Outlook))
            {
                return Outlook;
            }
            else if (windowTitle.Contains(RemoteDesktopManager))
            {
                return RemoteDesktopManager;
            }
            else if (windowTitle.Contains(Teams))
            {
                return Teams;
            }
            else if (windowTitle.Contains(Excel))
            {
                return Excel;
            }
            else if (windowTitle.Contains(NotepadPlusPlus))
            {
                return NotepadPlusPlus;
            }
            else if (windowTitle.Contains(Editor))
            {
                return Editor;
            }
            else if (windowTitle.Contains(Chrome))
            {
                return Chrome;
            }
            else if (windowTitle.Contains(TortoiseGit))
            {
                return TortoiseGit;
            }
            else if (windowTitle.Contains(KeePass))
            {
                return KeePass;
            }

            return string.Empty;
        }

        public static Brush GetBackgroundAccent(string applicationName)
        {
            if (applicationName.Equals(VisualStudio))
            {
                return new LinearGradientBrush(Colors.BlueViolet, Color.FromRgb(50, 50, 50), 0.75);
            }
            else if (applicationName.Equals(Outlook))
            {
                return new LinearGradientBrush(Colors.DodgerBlue, Colors.WhiteSmoke, 0.75);
            }
            else if (applicationName.Equals(Teams))
            {
                return new LinearGradientBrush(Colors.DarkViolet, Colors.WhiteSmoke, 0.75);
            }
            else if (applicationName.Equals(RemoteDesktopManager))
            {
                return new LinearGradientBrush(Colors.WhiteSmoke, Colors.DeepSkyBlue, 0.75);
            }
            else if (applicationName.Equals(Excel))
            {
                return new LinearGradientBrush(Colors.DarkGreen, Colors.WhiteSmoke, 0.75);
            }
            else if (applicationName.Equals(NotepadPlusPlus))
            {
                return new LinearGradientBrush(Colors.Lime, Colors.WhiteSmoke, 0.75);
            }
            else if (applicationName.Equals(Editor))
            {
                return new LinearGradientBrush(Colors.LightBlue, Colors.WhiteSmoke, 0.75);
            }
            else if (applicationName.Equals(Chrome))
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
            else if (applicationName.Equals(TortoiseGit))
            {
                return new LinearGradientBrush(Colors.DarkGray, Colors.LightBlue, 0.75);
            }
            else if (applicationName.Equals(KeePass))
            {
                return new LinearGradientBrush(Colors.DarkBlue, Colors.LightBlue, 0.75);
            }

            return new SolidColorBrush(Colors.White);
        }
    }
}
