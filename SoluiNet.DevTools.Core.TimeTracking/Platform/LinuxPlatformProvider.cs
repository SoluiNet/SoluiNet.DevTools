// <copyright file="LinuxPlatformProvider.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.TimeTracking.Platform
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using SoluiNet.DevTools.Core.TimeTracking.Interfaces;
    using SoluiNet.DevTools.Core.TimeTracking.Models;

    /// <summary>
    /// Linux-specific platform provider for window and process information.
    /// Supports both X11 and Wayland display servers.
    /// </summary>
    public class LinuxPlatformProvider : IPlatformProvider
    {
        private readonly ILogger? logger;
        private IntPtr display = IntPtr.Zero;
        private bool isX11Available = false;
        private bool isWaylandAvailable = false;
        private string displayServer = "Unknown";

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
                this.logger?.LogDebug("Getting active window on Linux using {DisplayServer}", this.displayServer);

                // Try X11 native APIs first if available
                if (this.isX11Available && this.display != IntPtr.Zero)
                {
                    var x11Result = await this.GetActiveWindowX11Async(cancellationToken).ConfigureAwait(false);
                    if (x11Result != null)
                    {
                        return x11Result;
                    }
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
                // Detect display server type
                this.DetectDisplayServer();

                // Try to initialize X11 connection if available
                if (this.displayServer == "X11")
                {
                    try
                    {
                        this.display = X11NativeMethods.XOpenDisplay(null);
                        this.isX11Available = this.display != IntPtr.Zero;

                        if (this.isX11Available)
                        {
                            this.logger?.LogDebug("X11 native API connection established");
                        }
                        else
                        {
                            this.logger?.LogDebug("X11 detected but native API connection failed, will use command-line tools");
                        }
                    }
                    catch (Exception ex)
                    {
                        this.logger?.LogDebug(ex, "Failed to establish X11 native connection, falling back to command-line tools");
                        this.isX11Available = false;
                    }
                }

                this.logger?.LogInformation("Linux platform provider initialized. Display server: {DisplayServer}, Native API: {NativeAPI}",
                    this.displayServer, this.isX11Available ? "Available" : "Command-line fallback");
            }
            catch (Exception ex)
            {
                this.logger?.LogWarning(ex, "Failed to initialize Linux platform provider, will attempt command-line fallback");
                this.displayServer = "Unknown";
                this.isX11Available = false;
                this.isWaylandAvailable = false;
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
                    this.logger?.LogDebug("No active window found via X11 _NET_ACTIVE_WINDOW");
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
                    this.logger?.LogDebug("Could not determine process ID for active window");
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
                windowInfo.PlatformSpecificData["RootWindow"] = rootWindow.ToInt64();

                this.logger?.LogDebug("Retrieved X11 window info: {ProcessName} - {Title}", processInfo?.ProcessName, windowTitle);
                return windowInfo;
            }
            catch (Exception ex)
            {
                this.logger?.LogDebug(ex, "Failed to get active window using X11 native APIs");
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
                this.logger?.LogDebug("Attempting to get active window using command-line tools for {DisplayServer}", this.displayServer);

                // Try Wayland-specific tools first
                if (this.displayServer == "Wayland" || this.isWaylandAvailable)
                {
                    var waylandResult = await this.TryWaylandCommandsAsync(cancellationToken).ConfigureAwait(false);
                    if (waylandResult != null)
                    {
                        return waylandResult;
                    }
                }

                // Try X11-specific tools
                if (this.displayServer == "X11" || Environment.GetEnvironmentVariable("DISPLAY") != null)
                {
                    var x11Result = await this.TryX11CommandsAsync(cancellationToken).ConfigureAwait(false);
                    if (x11Result != null)
                    {
                        return x11Result;
                    }
                }

                // Try generic approaches as last resort
                var genericResult = await this.TryGenericCommandsAsync(cancellationToken).ConfigureAwait(false);
                if (genericResult != null)
                {
                    return genericResult;
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
        /// Detects the current display server type.
        /// </summary>
        private void DetectDisplayServer()
        {
            // Check for Wayland first
            var waylandDisplay = Environment.GetEnvironmentVariable("WAYLAND_DISPLAY");
            var xdgSessionType = Environment.GetEnvironmentVariable("XDG_SESSION_TYPE");

            if (!string.IsNullOrEmpty(waylandDisplay) || "wayland".Equals(xdgSessionType, StringComparison.OrdinalIgnoreCase))
            {
                this.displayServer = "Wayland";
                this.isWaylandAvailable = true;
                this.logger?.LogDebug("Wayland display server detected");
                return;
            }

            // Check for X11
            var displayEnv = Environment.GetEnvironmentVariable("DISPLAY");
            if (!string.IsNullOrEmpty(displayEnv) || "x11".Equals(xdgSessionType, StringComparison.OrdinalIgnoreCase))
            {
                this.displayServer = "X11";
                this.logger?.LogDebug("X11 display server detected");
                return;
            }

            // Try to detect from running processes
            try
            {
                var processes = Process.GetProcesses();
                if (processes.Any(p => p.ProcessName.Contains("wayland", StringComparison.OrdinalIgnoreCase) ||
                                      p.ProcessName.Contains("sway", StringComparison.OrdinalIgnoreCase) ||
                                      p.ProcessName.Contains("weston", StringComparison.OrdinalIgnoreCase)))
                {
                    this.displayServer = "Wayland";
                    this.isWaylandAvailable = true;
                    this.logger?.LogDebug("Wayland detected from running processes");
                    return;
                }

                if (processes.Any(p => p.ProcessName.Contains("Xorg", StringComparison.OrdinalIgnoreCase) ||
                                      p.ProcessName.Contains("X11", StringComparison.OrdinalIgnoreCase)))
                {
                    this.displayServer = "X11";
                    this.logger?.LogDebug("X11 detected from running processes");
                    return;
                }
            }
            catch (Exception ex)
            {
                this.logger?.LogDebug(ex, "Failed to detect display server from processes");
            }

            this.logger?.LogWarning("Could not detect display server type, will try both X11 and Wayland approaches");
            this.displayServer = "Unknown";
        }

        /// <summary>
        /// Tries Wayland-specific commands to get active window information.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Window information or null if not available.</returns>
        private async Task<WindowInfo?> TryWaylandCommandsAsync(CancellationToken cancellationToken)
        {
            // Try swaymsg for Sway compositor
            var swayResult = await this.TrySwayCommandAsync(cancellationToken).ConfigureAwait(false);
            if (swayResult != null)
            {
                return swayResult;
            }

            // Try hyprctl for Hyprland
            var hyprResult = await this.TryHyprlandCommandAsync(cancellationToken).ConfigureAwait(false);
            if (hyprResult != null)
            {
                return hyprResult;
            }

            // Try wlrctl for wlroots-based compositors
            var wlrResult = await this.TryWlrootCommandAsync(cancellationToken).ConfigureAwait(false);
            if (wlrResult != null)
            {
                return wlrResult;
            }

            return null;
        }

        /// <summary>
        /// Tries X11-specific commands to get active window information.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Window information or null if not available.</returns>
        private async Task<WindowInfo?> TryX11CommandsAsync(CancellationToken cancellationToken)
        {
            // Try xdotool (most reliable for X11)
            var xdotoolResult = await this.TryXdotoolCommandAsync(cancellationToken).ConfigureAwait(false);
            if (xdotoolResult != null)
            {
                return xdotoolResult;
            }

            // Try wmctrl as fallback
            var wmctrlResult = await this.TryWmctrlCommandAsync(cancellationToken).ConfigureAwait(false);
            if (wmctrlResult != null)
            {
                return wmctrlResult;
            }

            // Try xprop as another fallback
            var xpropResult = await this.TryXpropCommandAsync(cancellationToken).ConfigureAwait(false);
            if (xpropResult != null)
            {
                return xpropResult;
            }

            return null;
        }

        /// <summary>
        /// Tries generic commands that might work on various systems.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Window information or null if not available.</returns>
        private async Task<WindowInfo?> TryGenericCommandsAsync(CancellationToken cancellationToken)
        {
            // Try ps with window title matching (very basic fallback)
            try
            {
                var psResult = await this.RunCommandAsync("ps", "aux", cancellationToken).ConfigureAwait(false);
                if (!string.IsNullOrEmpty(psResult))
                {
                    // This is a very basic approach - look for common GUI applications
                    var lines = psResult.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var line in lines)
                    {
                        if (line.Contains("firefox", StringComparison.OrdinalIgnoreCase) ||
                            line.Contains("chrome", StringComparison.OrdinalIgnoreCase) ||
                            line.Contains("code", StringComparison.OrdinalIgnoreCase) ||
                            line.Contains("terminal", StringComparison.OrdinalIgnoreCase))
                        {
                            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length > 1 && int.TryParse(parts[1], out var pid))
                            {
                                var processInfo = await this.GetProcessInfoAsync(pid, cancellationToken).ConfigureAwait(false);
                                if (processInfo != null)
                                {
                                    return new WindowInfo
                                    {
                                        Title = "Unknown",
                                        ProcessName = processInfo.ProcessName,
                                        ProcessId = pid,
                                        ProcessPath = processInfo.ProcessPath,
                                        Platform = this.PlatformName,
                                        CapturedAt = DateTime.UtcNow,
                                        PlatformSpecificData = { ["DisplayServer"] = "Generic", ["Method"] = "ps fallback" },
                                    };
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger?.LogDebug(ex, "Generic ps command failed");
            }

            return null;
        }

        /// <summary>
        /// Tries to get active window using swaymsg (Sway compositor).
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Window information or null if not available.</returns>
        private async Task<WindowInfo?> TrySwayCommandAsync(CancellationToken cancellationToken)
        {
            try
            {
                var result = await this.RunCommandAsync("swaymsg", "-t get_tree", cancellationToken).ConfigureAwait(false);
                if (string.IsNullOrEmpty(result))
                {
                    return null;
                }

                // Parse JSON output to find focused window
                var jsonDoc = JsonDocument.Parse(result);
                var focusedNode = this.FindFocusedNode(jsonDoc.RootElement);

                if (focusedNode.HasValue)
                {
                    var node = focusedNode.Value;
                    var name = node.TryGetProperty("name", out var nameProperty) ? nameProperty.GetString() : null;
                    var pid = node.TryGetProperty("pid", out var pidProperty) ? pidProperty.GetInt32() : 0;

                    if (!string.IsNullOrEmpty(name) && pid > 0)
                    {
                        var processInfo = await this.GetProcessInfoAsync(pid, cancellationToken).ConfigureAwait(false);
                        return new WindowInfo
                        {
                            Title = name,
                            ProcessName = processInfo?.ProcessName ?? string.Empty,
                            ProcessId = pid,
                            ProcessPath = processInfo?.ProcessPath ?? string.Empty,
                            Platform = this.PlatformName,
                            CapturedAt = DateTime.UtcNow,
                            PlatformSpecificData = { ["DisplayServer"] = "Wayland", ["Compositor"] = "Sway" },
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger?.LogDebug(ex, "swaymsg command failed");
            }

            return null;
        }

        /// <summary>
        /// Tries to get active window using hyprctl (Hyprland compositor).
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Window information or null if not available.</returns>
        private async Task<WindowInfo?> TryHyprlandCommandAsync(CancellationToken cancellationToken)
        {
            try
            {
                var result = await this.RunCommandAsync("hyprctl", "activewindow -j", cancellationToken).ConfigureAwait(false);
                if (string.IsNullOrEmpty(result))
                {
                    return null;
                }

                var jsonDoc = JsonDocument.Parse(result);
                var root = jsonDoc.RootElement;

                var title = root.TryGetProperty("title", out var titleProperty) ? titleProperty.GetString() : null;
                var className = root.TryGetProperty("class", out var classProperty) ? classProperty.GetString() : null;
                var pid = root.TryGetProperty("pid", out var pidProperty) ? pidProperty.GetInt32() : 0;

                if (!string.IsNullOrEmpty(title) && pid > 0)
                {
                    var processInfo = await this.GetProcessInfoAsync(pid, cancellationToken).ConfigureAwait(false);
                    return new WindowInfo
                    {
                        Title = title,
                        ProcessName = processInfo?.ProcessName ?? string.Empty,
                        ProcessId = pid,
                        ProcessPath = processInfo?.ProcessPath ?? string.Empty,
                        WindowClass = className ?? string.Empty,
                        Platform = this.PlatformName,
                        CapturedAt = DateTime.UtcNow,
                        PlatformSpecificData = { ["DisplayServer"] = "Wayland", ["Compositor"] = "Hyprland" },
                    };
                }
            }
            catch (Exception ex)
            {
                this.logger?.LogDebug(ex, "hyprctl command failed");
            }

            return null;
        }

        /// <summary>
        /// Tries to get active window using wlrctl (wlroots-based compositors).
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Window information or null if not available.</returns>
        private async Task<WindowInfo?> TryWlrootCommandAsync(CancellationToken cancellationToken)
        {
            try
            {
                // wlrctl doesn't have a direct "active window" command, so this is a placeholder
                // for future implementation or alternative wlroots tools
                this.logger?.LogDebug("wlrctl support not yet implemented");
            }
            catch (Exception ex)
            {
                this.logger?.LogDebug(ex, "wlrctl command failed");
            }

            return null;
        }

        /// <summary>
        /// Tries to get active window using xdotool.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Window information or null if not available.</returns>
        private async Task<WindowInfo?> TryXdotoolCommandAsync(CancellationToken cancellationToken)
        {
            try
            {
                // Get active window ID
                var windowIdResult = await this.RunCommandAsync("xdotool", "getactivewindow", cancellationToken).ConfigureAwait(false);
                if (string.IsNullOrEmpty(windowIdResult) || !long.TryParse(windowIdResult.Trim(), out var windowId))
                {
                    return null;
                }

                // Get window name
                var titleResult = await this.RunCommandAsync("xdotool", $"getwindowname {windowId}", cancellationToken).ConfigureAwait(false);
                var title = titleResult?.Trim() ?? string.Empty;

                // Get process ID
                var pidResult = await this.RunCommandAsync("xdotool", $"getwindowpid {windowId}", cancellationToken).ConfigureAwait(false);
                if (string.IsNullOrEmpty(pidResult) || !int.TryParse(pidResult.Trim(), out var pid))
                {
                    return null;
                }

                var processInfo = await this.GetProcessInfoAsync(pid, cancellationToken).ConfigureAwait(false);
                return new WindowInfo
                {
                    Title = title,
                    ProcessName = processInfo?.ProcessName ?? string.Empty,
                    ProcessId = pid,
                    ProcessPath = processInfo?.ProcessPath ?? string.Empty,
                    Platform = this.PlatformName,
                    CapturedAt = DateTime.UtcNow,
                    PlatformSpecificData = { ["DisplayServer"] = "X11", ["WindowId"] = windowId, ["Tool"] = "xdotool" },
                };
            }
            catch (Exception ex)
            {
                this.logger?.LogDebug(ex, "xdotool command failed");
            }

            return null;
        }

        /// <summary>
        /// Tries to get active window using wmctrl.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Window information or null if not available.</returns>
        private async Task<WindowInfo?> TryWmctrlCommandAsync(CancellationToken cancellationToken)
        {
            try
            {
                var result = await this.RunCommandAsync("wmctrl", "-l -p", cancellationToken).ConfigureAwait(false);
                if (string.IsNullOrEmpty(result))
                {
                    return null;
                }

                // wmctrl -l -p output format: <window_id> <desktop> <pid> <hostname> <window_title>
                var lines = result.Split('\n', StringSplitOptions.RemoveEmptyEntries);

                // This is a simplified approach - we'd need additional logic to determine the active window
                // For now, we'll take the first window as a fallback
                foreach (var line in lines)
                {
                    var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 4 && int.TryParse(parts[2], out var pid) && pid > 0)
                    {
                        var title = string.Join(" ", parts.Skip(4));
                        var processInfo = await this.GetProcessInfoAsync(pid, cancellationToken).ConfigureAwait(false);

                        return new WindowInfo
                        {
                            Title = title,
                            ProcessName = processInfo?.ProcessName ?? string.Empty,
                            ProcessId = pid,
                            ProcessPath = processInfo?.ProcessPath ?? string.Empty,
                            Platform = this.PlatformName,
                            CapturedAt = DateTime.UtcNow,
                            PlatformSpecificData = { ["DisplayServer"] = "X11", ["Tool"] = "wmctrl" },
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger?.LogDebug(ex, "wmctrl command failed");
            }

            return null;
        }

        /// <summary>
        /// Tries to get active window using xprop.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Window information or null if not available.</returns>
        private async Task<WindowInfo?> TryXpropCommandAsync(CancellationToken cancellationToken)
        {
            try
            {
                var result = await this.RunCommandAsync("xprop", "-root _NET_ACTIVE_WINDOW", cancellationToken).ConfigureAwait(false);
                if (string.IsNullOrEmpty(result))
                {
                    return null;
                }

                // Parse window ID from xprop output
                var parts = result.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var windowIdStr = parts.LastOrDefault()?.Trim();

                if (string.IsNullOrEmpty(windowIdStr) || !windowIdStr.StartsWith("0x"))
                {
                    return null;
                }

                // Get window properties
                var propsResult = await this.RunCommandAsync("xprop", $"-id {windowIdStr} WM_NAME _NET_WM_PID", cancellationToken).ConfigureAwait(false);
                if (string.IsNullOrEmpty(propsResult))
                {
                    return null;
                }

                var title = string.Empty;
                var pid = 0;

                var propLines = propsResult.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in propLines)
                {
                    if (line.StartsWith("WM_NAME"))
                    {
                        var titleMatch = line.Split('=', 2);
                        if (titleMatch.Length > 1)
                        {
                            title = titleMatch[1].Trim().Trim('"');
                        }
                    }
                    else if (line.StartsWith("_NET_WM_PID"))
                    {
                        var pidMatch = line.Split('=', 2);
                        if (pidMatch.Length > 1 && int.TryParse(pidMatch[1].Trim(), out var parsedPid))
                        {
                            pid = parsedPid;
                        }
                    }
                }

                if (pid > 0)
                {
                    var processInfo = await this.GetProcessInfoAsync(pid, cancellationToken).ConfigureAwait(false);
                    return new WindowInfo
                    {
                        Title = title,
                        ProcessName = processInfo?.ProcessName ?? string.Empty,
                        ProcessId = pid,
                        ProcessPath = processInfo?.ProcessPath ?? string.Empty,
                        Platform = this.PlatformName,
                        CapturedAt = DateTime.UtcNow,
                        PlatformSpecificData = { ["DisplayServer"] = "X11", ["Tool"] = "xprop" },
                    };
                }
            }
            catch (Exception ex)
            {
                this.logger?.LogDebug(ex, "xprop command failed");
            }

            return null;
        }

        /// <summary>
        /// Finds the focused node in a Sway tree JSON structure.
        /// </summary>
        /// <param name="element">The JSON element to search.</param>
        /// <returns>The focused node or null if not found.</returns>
        private JsonElement? FindFocusedNode(JsonElement element)
        {
            if (element.TryGetProperty("focused", out var focusedProperty) && focusedProperty.GetBoolean())
            {
                return element;
            }

            if (element.TryGetProperty("nodes", out var nodesProperty) && nodesProperty.ValueKind == JsonValueKind.Array)
            {
                foreach (var node in nodesProperty.EnumerateArray())
                {
                    var result = this.FindFocusedNode(node);
                    if (result.HasValue)
                    {
                        return result;
                    }
                }
            }

            if (element.TryGetProperty("floating_nodes", out var floatingProperty) && floatingProperty.ValueKind == JsonValueKind.Array)
            {
                foreach (var node in floatingProperty.EnumerateArray())
                {
                    var result = this.FindFocusedNode(node);
                    if (result.HasValue)
                    {
                        return result;
                    }
                }
            }

            return null;
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
                    // /proc/pid/exe is a symlink, so we need to read the link target
                    var linkInfo = new FileInfo(exePath);
                    if (linkInfo.Exists)
                    {
                        // Try to resolve the symlink
                        var target = linkInfo.LinkTarget;
                        if (!string.IsNullOrEmpty(target))
                        {
                            return target;
                        }

                        // Fallback: try to read the symlink using readlink command
                        var process = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = "readlink",
                                Arguments = exePath,
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                CreateNoWindow = true,
                            },
                        };

                        process.Start();
                        var output = process.StandardOutput.ReadToEnd().Trim();
                        process.WaitForExit();

                        if (process.ExitCode == 0 && !string.IsNullOrEmpty(output))
                        {
                            return output;
                        }
                    }
                }

                // Fallback: try to get from process info
                using var proc = Process.GetProcessById(processId);
                return proc.MainModule?.FileName ?? string.Empty;
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
            try
            {
                if (this.display == IntPtr.Zero || window == IntPtr.Zero || property == IntPtr.Zero)
                {
                    return IntPtr.Zero;
                }

                var result = X11NativeMethods.XGetWindowProperty(
                    this.display,
                    window,
                    property,
                    0,
                    1,
                    false,
                    IntPtr.Zero,
                    out var actualType,
                    out var actualFormat,
                    out var nItems,
                    out var bytesAfter,
                    out var prop);

                if (result == 0 && nItems > 0 && prop != IntPtr.Zero)
                {
                    var windowPtr = Marshal.ReadIntPtr(prop);
                    X11NativeMethods.XFree(prop);
                    return windowPtr;
                }

                if (prop != IntPtr.Zero)
                {
                    X11NativeMethods.XFree(prop);
                }

                return IntPtr.Zero;
            }
            catch (Exception ex)
            {
                this.logger?.LogDebug(ex, "Failed to get X11 window property");
                return IntPtr.Zero;
            }
        }

        /// <summary>
        /// Gets the window title using X11.
        /// </summary>
        /// <param name="window">The window handle.</param>
        /// <returns>The window title.</returns>
        private string GetWindowTitle(IntPtr window)
        {
            try
            {
                if (this.display == IntPtr.Zero || window == IntPtr.Zero)
                {
                    return string.Empty;
                }

                // Try _NET_WM_NAME first (UTF-8)
                var netWmNameAtom = X11NativeMethods.XInternAtom(this.display, "_NET_WM_NAME", false);
                var title = this.GetStringProperty(window, netWmNameAtom);
                if (!string.IsNullOrEmpty(title))
                {
                    return title;
                }

                // Fall back to WM_NAME
                var wmNameAtom = X11NativeMethods.XInternAtom(this.display, "WM_NAME", false);
                title = this.GetStringProperty(window, wmNameAtom);
                if (!string.IsNullOrEmpty(title))
                {
                    return title;
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                this.logger?.LogDebug(ex, "Failed to get X11 window title");
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the window class using X11.
        /// </summary>
        /// <param name="window">The window handle.</param>
        /// <returns>The window class.</returns>
        private string GetWindowClass(IntPtr window)
        {
            try
            {
                if (this.display == IntPtr.Zero || window == IntPtr.Zero)
                {
                    return string.Empty;
                }

                var wmClassAtom = X11NativeMethods.XInternAtom(this.display, "WM_CLASS", false);
                return this.GetStringProperty(window, wmClassAtom);
            }
            catch (Exception ex)
            {
                this.logger?.LogDebug(ex, "Failed to get X11 window class");
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the process ID for a window using X11.
        /// </summary>
        /// <param name="window">The window handle.</param>
        /// <returns>The process ID.</returns>
        private int GetWindowProcessId(IntPtr window)
        {
            try
            {
                if (this.display == IntPtr.Zero || window == IntPtr.Zero)
                {
                    return 0;
                }

                var netWmPidAtom = X11NativeMethods.XInternAtom(this.display, "_NET_WM_PID", false);
                var result = X11NativeMethods.XGetWindowProperty(
                    this.display,
                    window,
                    netWmPidAtom,
                    0,
                    1,
                    false,
                    IntPtr.Zero,
                    out var actualType,
                    out var actualFormat,
                    out var nItems,
                    out var bytesAfter,
                    out var prop);

                if (result == 0 && nItems > 0 && prop != IntPtr.Zero)
                {
                    var pid = Marshal.ReadInt32(prop);
                    X11NativeMethods.XFree(prop);
                    return pid;
                }

                if (prop != IntPtr.Zero)
                {
                    X11NativeMethods.XFree(prop);
                }

                return 0;
            }
            catch (Exception ex)
            {
                this.logger?.LogDebug(ex, "Failed to get X11 window process ID");
                return 0;
            }
        }

        /// <summary>
        /// Gets a string property from an X11 window.
        /// </summary>
        /// <param name="window">The window handle.</param>
        /// <param name="property">The property atom.</param>
        /// <returns>The property value as string.</returns>
        private string GetStringProperty(IntPtr window, IntPtr property)
        {
            try
            {
                if (this.display == IntPtr.Zero || window == IntPtr.Zero || property == IntPtr.Zero)
                {
                    return string.Empty;
                }

                var result = X11NativeMethods.XGetWindowProperty(
                    this.display,
                    window,
                    property,
                    0,
                    65536,
                    false,
                    IntPtr.Zero,
                    out var actualType,
                    out var actualFormat,
                    out var nItems,
                    out var bytesAfter,
                    out var prop);

                if (result == 0 && nItems > 0 && prop != IntPtr.Zero)
                {
                    var str = Marshal.PtrToStringAnsi(prop);
                    X11NativeMethods.XFree(prop);
                    return str ?? string.Empty;
                }

                if (prop != IntPtr.Zero)
                {
                    X11NativeMethods.XFree(prop);
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                this.logger?.LogDebug(ex, "Failed to get X11 string property");
                return string.Empty;
            }
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

            /// <summary>
            /// Gets a window property.
            /// </summary>
            /// <param name="display">The display pointer.</param>
            /// <param name="window">The window.</param>
            /// <param name="property">The property atom.</param>
            /// <param name="longOffset">The long offset.</param>
            /// <param name="longLength">The long length.</param>
            /// <param name="delete">Whether to delete the property.</param>
            /// <param name="reqType">The requested type.</param>
            /// <param name="actualTypeReturn">The actual type return.</param>
            /// <param name="actualFormatReturn">The actual format return.</param>
            /// <param name="nItemsReturn">The number of items return.</param>
            /// <param name="bytesAfterReturn">The bytes after return.</param>
            /// <param name="propReturn">The property return.</param>
            /// <returns>Success status.</returns>
            [DllImport("libX11.so.6", EntryPoint = "XGetWindowProperty")]
            internal static extern int XGetWindowProperty(
                IntPtr display,
                IntPtr window,
                IntPtr property,
                long longOffset,
                long longLength,
                bool delete,
                IntPtr reqType,
                out IntPtr actualTypeReturn,
                out int actualFormatReturn,
                out ulong nItemsReturn,
                out ulong bytesAfterReturn,
                out IntPtr propReturn);

            /// <summary>
            /// Frees memory allocated by X11.
            /// </summary>
            /// <param name="data">The data to free.</param>
            /// <returns>Success status.</returns>
            [DllImport("libX11.so.6", EntryPoint = "XFree")]
            internal static extern int XFree(IntPtr data);
        }
    }
}