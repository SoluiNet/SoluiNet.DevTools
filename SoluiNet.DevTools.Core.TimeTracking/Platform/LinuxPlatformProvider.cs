// <copyright file="LinuxPlatformProvider.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.TimeTracking.Platform
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using SoluiNet.DevTools.Core.TimeTracking.Interfaces;
    using SoluiNet.DevTools.Core.TimeTracking.Models;

    /// <summary>
    /// Linux-specific platform provider for window and process information.
    /// </summary>
    public class LinuxPlatformProvider : IPlatformProvider
    {
        private readonly ILogger? logger;
        private IntPtr display = IntPtr.Zero;
        private bool isX11Available = false;
        private bool isWaylandAvailable = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="LinuxPlatformProvider"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        public LinuxPlatformProvider(ILogger? logger = null)
        {
            this.logger = logger;
        }

        /// <inheritdoc />
        public string PlatformName => "Linux";

        /// <inheritdoc />
        public bool IsSupported => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        /// <inheritdoc />
        public async Task<WindowInfo?> GetActiveWindowAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                this.logger?.LogDebug("Getting active window on Linux");

                // Try X11 first, then fall back to command-line tools
                if (this.isX11Available)
                {
                    return await this.GetActiveWindowX11Async(cancellationToken).ConfigureAwait(false);
                }

                // Fall back to command-line tools for both X11 and Wayland
                return await this.GetActiveWindowCommandLineAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger?.LogError(ex, "Failed to get active window information on Linux");
                return null;
            }
        }

        /// <inheritdoc />
        public async Task<ProcessInfo?> GetActiveProcessAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                this.logger?.LogDebug("Getting active process on Linux");

                var windowInfo = await this.GetActiveWindowAsync(cancellationToken).ConfigureAwait(false);
                if (windowInfo == null || windowInfo.ProcessId <= 0)
                {
                    return null;
                }

                return await this.GetProcessInfoAsync(windowInfo.ProcessId, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger?.LogError(ex, "Failed to get active process information on Linux");
                return null;
            }
        }

        /// <inheritdoc />
        public Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            this.logger?.LogDebug("Initializing Linux platform provider");

            try
            {
                // Check if X11 is available
                this.display = X11NativeMethods.XOpenDisplay(null);
                this.isX11Available = this.display != IntPtr.Zero;

                if (this.isX11Available)
                {
                    this.logger?.LogDebug("X11 display server detected");
                }
                else
                {
                    this.logger?.LogDebug("X11 not available, will use command-line tools");
                }

                // Check if Wayland is available
                var waylandDisplay = Environment.GetEnvironmentVariable("WAYLAND_DISPLAY");
                this.isWaylandAvailable = !string.IsNullOrEmpty(waylandDisplay);

                if (this.isWaylandAvailable)
                {
                    this.logger?.LogDebug("Wayland display server detected");
                }
            }
            catch (Exception ex)
            {
                this.logger?.LogWarning(ex, "Failed to initialize display server connection, falling back to command-line tools");
                this.isX11Available = false;
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task DisposeAsync(CancellationToken cancellationToken = default)
        {
            this.logger?.LogDebug("Disposing Linux platform provider");

            try
            {
                if (this.display != IntPtr.Zero)
                {
                    X11NativeMethods.XCloseDisplay(this.display);
                    this.display = IntPtr.Zero;
                }
            }
            catch (Exception ex)
            {
                this.logger?.LogWarning(ex, "Error disposing X11 display connection");
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets active window information using X11 APIs.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Window information or null if not available.</returns>
        private async Task<WindowInfo?> GetActiveWindowX11Async(CancellationToken cancellationToken)
        {
            try
            {
                if (this.display == IntPtr.Zero)
                {
                    return null;
                }

                var rootWindow = X11NativeMethods.XDefaultRootWindow(this.display);
                var activeWindowAtom = X11NativeMethods.XInternAtom(this.display, "_NET_ACTIVE_WINDOW", false);

                // Get the active window
                var activeWindow = this.GetWindowProperty(rootWindow, activeWindowAtom);
                if (activeWindow == IntPtr.Zero)
                {
                    return null;
                }

                // Get window title
                var windowTitle = this.GetWindowTitle(activeWindow);

                // Get window class
                var windowClass = this.GetWindowClass(activeWindow);

                // Get process ID
                var processId = this.GetWindowProcessId(activeWindow);

                if (processId <= 0)
                {
                    return null;
                }

                // Get process information
                var processInfo = await this.GetProcessInfoAsync(processId, cancellationToken).ConfigureAwait(false);

                var windowInfo = new WindowInfo
                {
                    Title = windowTitle,
                    ProcessName = processInfo?.ProcessName ?? string.Empty,
                    ProcessId = processId,
                    ProcessPath = processInfo?.ProcessPath ?? string.Empty,
                    WindowClass = windowClass,
                    Platform = this.PlatformName,
                    CapturedAt = DateTime.UtcNow,
                };

                // Add Linux-specific data
                windowInfo.PlatformSpecificData["WindowId"] = activeWindow.ToInt64();
                windowInfo.PlatformSpecificData["DisplayServer"] = "X11";

                return windowInfo;
            }
            catch (Exception ex)
            {
                this.logger?.LogDebug(ex, "Failed to get active window using X11");
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
                // Try different command-line tools based on available display server
                if (this.isWaylandAvailable)
                {
                    // Try swaymsg for Sway compositor
                    var swayResult = await this.RunCommandAsync("swaymsg", "-t get_tree | jq -r '.. | select(.focused? == true) | .name'", cancellationToken).ConfigureAwait(false);
                    if (!string.IsNullOrEmpty(swayResult))
                    {
                        return await this.CreateWindowInfoFromTitle(swayResult.Trim(), "Wayland", cancellationToken).ConfigureAwait(false);
                    }
                }

                // Try xdotool for X11
                var xdotoolResult = await this.RunCommandAsync("xdotool", "getactivewindow getwindowname", cancellationToken).ConfigureAwait(false);
                if (!string.IsNullOrEmpty(xdotoolResult))
                {
                    return await this.CreateWindowInfoFromTitle(xdotoolResult.Trim(), "X11", cancellationToken).ConfigureAwait(false);
                }

                // Try wmctrl as fallback
                var wmctrlResult = await this.RunCommandAsync("wmctrl", "-l | grep $(xdotool getactivewindow) | cut -d' ' -f5-", cancellationToken).ConfigureAwait(false);
                if (!string.IsNullOrEmpty(wmctrlResult))
                {
                    return await this.CreateWindowInfoFromTitle(wmctrlResult.Trim(), "X11", cancellationToken).ConfigureAwait(false);
                }

                this.logger?.LogDebug("No suitable command-line tools found for window detection");
                return null;
            }
            catch (Exception ex)
            {
                this.logger?.LogDebug(ex, "Failed to get active window using command-line tools");
                return null;
            }
        }

        /// <summary>
        /// Creates window information from a window title and attempts to find the associated process.
        /// </summary>
        /// <param name="title">The window title.</param>
        /// <param name="displayServer">The display server type.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Window information or null if process cannot be determined.</returns>
        private async Task<WindowInfo?> CreateWindowInfoFromTitle(string title, string displayServer, CancellationToken cancellationToken)
        {
            try
            {
                // This is a simplified approach - in a real implementation, you would need
                // more sophisticated methods to map window titles to processes
                var processes = Process.GetProcesses();
                foreach (var process in processes)
                {
                    try
                    {
                        if (process.MainWindowTitle.Contains(title, StringComparison.OrdinalIgnoreCase) ||
                            title.Contains(process.ProcessName, StringComparison.OrdinalIgnoreCase))
                        {
                            var windowInfo = new WindowInfo
                            {
                                Title = title,
                                ProcessName = process.ProcessName,
                                ProcessId = process.Id,
                                ProcessPath = GetProcessPath(process.Id),
                                Platform = this.PlatformName,
                                CapturedAt = DateTime.UtcNow,
                            };

                            windowInfo.PlatformSpecificData["DisplayServer"] = displayServer;
                            return windowInfo;
                        }
                    }
                    catch
                    {
                        // Ignore processes we can't access
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                this.logger?.LogDebug(ex, "Failed to create window info from title");
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
                    ProcessPath = GetProcessPath(process.Id),
                    CommandLine = await this.GetProcessCommandLineAsync(process.Id, cancellationToken).ConfigureAwait(false),
                    Platform = this.PlatformName,
                    CapturedAt = DateTime.UtcNow,
                };

                // Add Linux-specific data
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
        /// Gets the command line for a process by reading from /proc filesystem.
        /// </summary>
        /// <param name="processId">The process ID.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The command line string.</returns>
        private async Task<string> GetProcessCommandLineAsync(int processId, CancellationToken cancellationToken)
        {
            try
            {
                var cmdlinePath = $"/proc/{processId}/cmdline";
                if (File.Exists(cmdlinePath))
                {
                    var cmdlineBytes = await File.ReadAllBytesAsync(cmdlinePath, cancellationToken).ConfigureAwait(false);
                    // Replace null bytes with spaces
                    for (int i = 0; i < cmdlineBytes.Length; i++)
                    {
                        if (cmdlineBytes[i] == 0)
                        {
                            cmdlineBytes[i] = (byte)' ';
                        }
                    }
                    return Encoding.UTF8.GetString(cmdlineBytes).Trim();
                }
            }
            catch
            {
                // Ignore errors reading proc filesystem
            }

            return string.Empty;
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
        /// Gets the process path from the /proc filesystem.
        /// </summary>
        /// <param name="processId">The process ID.</param>
        /// <returns>The process path or empty string if not available.</returns>
        private static string GetProcessPath(int processId)
        {
            try
            {
                var exePath = $"/proc/{processId}/exe";
                if (File.Exists(exePath))
                {
                    return File.ReadAllText(exePath).Trim();
                }
            }
            catch
            {
                // Ignore errors
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets a window property using X11.
        /// </summary>
        /// <param name="window">The window handle.</param>
        /// <param name="property">The property atom.</param>
        /// <returns>The property value as IntPtr.</returns>
        private IntPtr GetWindowProperty(IntPtr window, IntPtr property)
        {
            // Simplified implementation - a full implementation would handle different property types
            return IntPtr.Zero;
        }

        /// <summary>
        /// Gets the window title using X11.
        /// </summary>
        /// <param name="window">The window handle.</param>
        /// <returns>The window title.</returns>
        private string GetWindowTitle(IntPtr window)
        {
            // Simplified implementation
            return string.Empty;
        }

        /// <summary>
        /// Gets the window class using X11.
        /// </summary>
        /// <param name="window">The window handle.</param>
        /// <returns>The window class.</returns>
        private string GetWindowClass(IntPtr window)
        {
            // Simplified implementation
            return string.Empty;
        }

        /// <summary>
        /// Gets the process ID for a window using X11.
        /// </summary>
        /// <param name="window">The window handle.</param>
        /// <returns>The process ID.</returns>
        private int GetWindowProcessId(IntPtr window)
        {
            // Simplified implementation
            return 0;
        }

        /// <summary>
        /// Contains X11 native method declarations.
        /// </summary>
        private static class X11NativeMethods
        {
            /// <summary>
            /// Opens a connection to the X server.
            /// </summary>
            /// <param name="displayName">The display name.</param>
            /// <returns>A pointer to the Display structure.</returns>
            [DllImport("libX11.so.6", EntryPoint = "XOpenDisplay")]
            internal static extern IntPtr XOpenDisplay(string? displayName);

            /// <summary>
            /// Closes a connection to the X server.
            /// </summary>
            /// <param name="display">The display pointer.</param>
            /// <returns>Zero on success.</returns>
            [DllImport("libX11.so.6", EntryPoint = "XCloseDisplay")]
            internal static extern int XCloseDisplay(IntPtr display);

            /// <summary>
            /// Gets the default root window.
            /// </summary>
            /// <param name="display">The display pointer.</param>
            /// <returns>The root window.</returns>
            [DllImport("libX11.so.6", EntryPoint = "XDefaultRootWindow")]
            internal static extern IntPtr XDefaultRootWindow(IntPtr display);

            /// <summary>
            /// Interns an atom.
            /// </summary>
            /// <param name="display">The display pointer.</param>
            /// <param name="atomName">The atom name.</param>
            /// <param name="onlyIfExists">Whether to only return existing atoms.</param>
            /// <returns>The atom identifier.</returns>
            [DllImport("libX11.so.6", EntryPoint = "XInternAtom")]
            internal static extern IntPtr XInternAtom(IntPtr display, string atomName, bool onlyIfExists);
        }
    }
}