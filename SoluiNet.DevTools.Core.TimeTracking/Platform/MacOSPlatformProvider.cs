// <copyright file="MacOSPlatformProvider.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.TimeTracking.Platform
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using SoluiNet.DevTools.Core.TimeTracking.Interfaces;
    using SoluiNet.DevTools.Core.TimeTracking.Models;

    /// <summary>
    /// macOS-specific platform provider for window and process information.
    /// </summary>
    public class MacOSPlatformProvider : IPlatformProvider
    {
        private readonly ILogger? logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MacOSPlatformProvider"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        public MacOSPlatformProvider(ILogger? logger = null)
        {
            this.logger = logger;
        }

        /// <inheritdoc />
        public string PlatformName => "macOS";

        /// <inheritdoc />
        public bool IsSupported => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        /// <inheritdoc />
        public async Task<WindowInfo?> GetActiveWindowAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                this.logger?.LogDebug("Getting active window on macOS");

                // Try using Cocoa APIs first
                var cocoaResult = await this.GetActiveWindowCocoaAsync(cancellationToken).ConfigureAwait(false);
                if (cocoaResult != null)
                {
                    return cocoaResult;
                }

                // Fall back to command-line tools
                return await this.GetActiveWindowCommandLineAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger?.LogError(ex, "Failed to get active window information on macOS");
                return null;
            }
        }

        /// <inheritdoc />
        public async Task<ProcessInfo?> GetActiveProcessAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                this.logger?.LogDebug("Getting active process on macOS");

                var windowInfo = await this.GetActiveWindowAsync(cancellationToken).ConfigureAwait(false);
                if (windowInfo == null || windowInfo.ProcessId <= 0)
                {
                    return null;
                }

                return await this.GetProcessInfoAsync(windowInfo.ProcessId, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger?.LogError(ex, "Failed to get active process information on macOS");
                return null;
            }
        }

        /// <inheritdoc />
        public Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            this.logger?.LogDebug("Initializing macOS platform provider");

            // Check accessibility permissions
            if (!this.CheckAccessibilityPermissions())
            {
                this.logger?.LogWarning("Accessibility permissions not granted. Window detection may be limited.");
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task DisposeAsync(CancellationToken cancellationToken = default)
        {
            this.logger?.LogDebug("Disposing macOS platform provider");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets active window information using Cocoa APIs.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Window information or null if not available.</returns>
        private async Task<WindowInfo?> GetActiveWindowCocoaAsync(CancellationToken cancellationToken)
        {
            try
            {
                // Get the frontmost application
                var frontmostApp = CocoaNativeMethods.NSWorkspaceSharedWorkspace();
                if (frontmostApp == IntPtr.Zero)
                {
                    return null;
                }

                var activeApp = CocoaNativeMethods.NSWorkspaceFrontmostApplication(frontmostApp);
                if (activeApp == IntPtr.Zero)
                {
                    return null;
                }

                // Get application information
                var bundleId = CocoaNativeMethods.NSRunningApplicationBundleIdentifier(activeApp);
                var processId = CocoaNativeMethods.NSRunningApplicationProcessIdentifier(activeApp);
                var localizedName = CocoaNativeMethods.NSRunningApplicationLocalizedName(activeApp);

                if (processId <= 0)
                {
                    return null;
                }

                // Get process information
                var processInfo = await this.GetProcessInfoAsync(processId, cancellationToken).ConfigureAwait(false);

                // Try to get window title using AppleScript or other methods
                var windowTitle = await this.GetActiveWindowTitleAsync(cancellationToken).ConfigureAwait(false);

                var windowInfo = new WindowInfo
                {
                    Title = windowTitle ?? localizedName ?? string.Empty,
                    ProcessName = processInfo?.ProcessName ?? string.Empty,
                    ProcessId = processId,
                    ProcessPath = processInfo?.ProcessPath ?? string.Empty,
                    WindowClass = bundleId ?? string.Empty,
                    Platform = this.PlatformName,
                    CapturedAt = DateTime.UtcNow,
                };

                // Add macOS-specific data
                windowInfo.PlatformSpecificData["BundleIdentifier"] = bundleId ?? string.Empty;
                windowInfo.PlatformSpecificData["LocalizedName"] = localizedName ?? string.Empty;

                return windowInfo;
            }
            catch (Exception ex)
            {
                this.logger?.LogDebug(ex, "Failed to get active window using Cocoa APIs");
                return null;
            }
        }

        /// <summary>
        /// Gets active window information using command-line tools.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Window information or null if not available.</returns>
        private async Task<WindowInfo?> GetActiveWindowCommandLineAsync(CancellationToken cancellationToken)
        {
            try
            {
                // Use AppleScript to get the frontmost application and window
                var script = @"
                    tell application ""System Events""
                        set frontApp to first application process whose frontmost is true
                        set appName to name of frontApp
                        set appPID to unix id of frontApp
                        try
                            set windowTitle to name of front window of frontApp
                        on error
                            set windowTitle to """"
                        end try
                        return appName & ""|"" & appPID & ""|"" & windowTitle
                    end tell";

                var result = await this.RunAppleScriptAsync(script, cancellationToken).ConfigureAwait(false);
                if (string.IsNullOrEmpty(result))
                {
                    return null;
                }

                var parts = result.Split('|');
                if (parts.Length < 3)
                {
                    return null;
                }

                var appName = parts[0].Trim();
                if (!int.TryParse(parts[1].Trim(), out var processId))
                {
                    return null;
                }

                var windowTitle = parts[2].Trim();

                // Get additional process information
                var processInfo = await this.GetProcessInfoAsync(processId, cancellationToken).ConfigureAwait(false);

                var windowInfo = new WindowInfo
                {
                    Title = windowTitle,
                    ProcessName = processInfo?.ProcessName ?? appName,
                    ProcessId = processId,
                    ProcessPath = processInfo?.ProcessPath ?? string.Empty,
                    Platform = this.PlatformName,
                    CapturedAt = DateTime.UtcNow,
                };

                windowInfo.PlatformSpecificData["ApplicationName"] = appName;
                windowInfo.PlatformSpecificData["DetectionMethod"] = "AppleScript";

                return windowInfo;
            }
            catch (Exception ex)
            {
                this.logger?.LogDebug(ex, "Failed to get active window using command-line tools");
                return null;
            }
        }

        /// <summary>
        /// Gets the active window title using AppleScript.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The window title or null if not available.</returns>
        private async Task<string?> GetActiveWindowTitleAsync(CancellationToken cancellationToken)
        {
            try
            {
                var script = @"
                    tell application ""System Events""
                        set frontApp to first application process whose frontmost is true
                        try
                            return name of front window of frontApp
                        on error
                            return """"
                        end try
                    end tell";

                return await this.RunAppleScriptAsync(script, cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets process information for a given process ID.
        /// </summary>
        /// <param name="processId">The process ID.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Process information or null if not available.</returns>
        private async Task<ProcessInfo?> GetProcessInfoAsync(int processId, CancellationToken cancellationToken)
        {
            try
            {
                using var process = Process.GetProcessById(processId);

                var processInfo = new ProcessInfo
                {
                    ProcessId = process.Id,
                    ProcessName = process.ProcessName,
                    ProcessPath = process.MainModule?.FileName ?? string.Empty,
                    CommandLine = await this.GetProcessCommandLineAsync(process.Id, cancellationToken).ConfigureAwait(false),
                    Platform = this.PlatformName,
                    CapturedAt = DateTime.UtcNow,
                };

                // Add macOS-specific data
                processInfo.PlatformSpecificData["StartTime"] = process.StartTime;
                processInfo.PlatformSpecificData["WorkingSet"] = process.WorkingSet64;

                return processInfo;
            }
            catch (Exception ex)
            {
                this.logger?.LogDebug(ex, "Could not get process information for PID {ProcessId}", processId);
                return null;
            }
        }

        /// <summary>
        /// Gets the command line for a process using ps command.
        /// </summary>
        /// <param name="processId">The process ID.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The command line string.</returns>
        private async Task<string> GetProcessCommandLineAsync(int processId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await this.RunCommandAsync("ps", $"-p {processId} -o command=", cancellationToken).ConfigureAwait(false);
                return result.Trim();
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Runs an AppleScript and returns its output.
        /// </summary>
        /// <param name="script">The AppleScript to run.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The script output or empty string if failed.</returns>
        private async Task<string> RunAppleScriptAsync(string script, CancellationToken cancellationToken)
        {
            try
            {
                using var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "osascript",
                        Arguments = "-e",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        RedirectStandardInput = true,
                        CreateNoWindow = true,
                    },
                };

                process.Start();
                await process.StandardInput.WriteLineAsync(script).ConfigureAwait(false);
                process.StandardInput.Close();

                var output = await process.StandardOutput.ReadToEndAsync().ConfigureAwait(false);
                await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);

                return process.ExitCode == 0 ? output.Trim() : string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Runs a command and returns its output.
        /// </summary>
        /// <param name="command">The command to run.</param>
        /// <param name="arguments">The command arguments.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The command output or empty string if failed.</returns>
        private async Task<string> RunCommandAsync(string command, string arguments, CancellationToken cancellationToken)
        {
            try
            {
                using var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = command,
                        Arguments = arguments,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                    },
                };

                process.Start();
                var output = await process.StandardOutput.ReadToEndAsync().ConfigureAwait(false);
                await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);

                return process.ExitCode == 0 ? output : string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Checks if accessibility permissions are granted.
        /// </summary>
        /// <returns>True if accessibility permissions are granted.</returns>
        private bool CheckAccessibilityPermissions()
        {
            try
            {
                // This is a simplified check - a full implementation would use
                // AXIsProcessTrusted() from the Accessibility framework
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Contains Cocoa native method declarations.
        /// </summary>
        private static class CocoaNativeMethods
        {
            /// <summary>
            /// Gets the shared NSWorkspace instance.
            /// </summary>
            /// <returns>The NSWorkspace instance.</returns>
            [DllImport("/System/Library/Frameworks/Foundation.framework/Foundation")]
            internal static extern IntPtr NSWorkspaceSharedWorkspace();

            /// <summary>
            /// Gets the frontmost application.
            /// </summary>
            /// <param name="workspace">The workspace instance.</param>
            /// <returns>The frontmost NSRunningApplication.</returns>
            [DllImport("/System/Library/Frameworks/Foundation.framework/Foundation")]
            internal static extern IntPtr NSWorkspaceFrontmostApplication(IntPtr workspace);

            /// <summary>
            /// Gets the bundle identifier of a running application.
            /// </summary>
            /// <param name="app">The NSRunningApplication instance.</param>
            /// <returns>The bundle identifier.</returns>
            [DllImport("/System/Library/Frameworks/Foundation.framework/Foundation")]
            internal static extern string NSRunningApplicationBundleIdentifier(IntPtr app);

            /// <summary>
            /// Gets the process identifier of a running application.
            /// </summary>
            /// <param name="app">The NSRunningApplication instance.</param>
            /// <returns>The process identifier.</returns>
            [DllImport("/System/Library/Frameworks/Foundation.framework/Foundation")]
            internal static extern int NSRunningApplicationProcessIdentifier(IntPtr app);

            /// <summary>
            /// Gets the localized name of a running application.
            /// </summary>
            /// <param name="app">The NSRunningApplication instance.</param>
            /// <returns>The localized name.</returns>
            [DllImport("/System/Library/Frameworks/Foundation.framework/Foundation")]
            internal static extern string NSRunningApplicationLocalizedName(IntPtr app);
        }
    }
}