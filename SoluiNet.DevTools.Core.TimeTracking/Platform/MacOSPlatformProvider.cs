// <copyright file="MacOSPlatformProvider.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.TimeTracking.Platform
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using SoluiNet.DevTools.Core.TimeTracking.Interfaces;
    using SoluiNet.DevTools.Core.TimeTracking.Models;

    /// <summary>
    /// macOS-specific platform provider for window and process information.
    /// Uses Cocoa APIs and AppleScript for comprehensive window monitoring.
    /// </summary>
    public class MacOSPlatformProvider : IPlatformProvider
    {
        private readonly ILogger? logger;
        private bool accessibilityPermissionsChecked = false;
        private bool hasAccessibilityPermissions = false;

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

                // Try using Cocoa APIs first if accessibility permissions are available
                if (this.hasAccessibilityPermissions)
                {
                    var cocoaResult = await this.GetActiveWindowCocoaAsync(cancellationToken).ConfigureAwait(false);
                    if (cocoaResult != null)
                    {
                        return cocoaResult;
                    }
                }

                // Fall back to AppleScript approach
                var appleScriptResult = await this.GetActiveWindowAppleScriptAsync(cancellationToken).ConfigureAwait(false);
                if (appleScriptResult != null)
                {
                    return appleScriptResult;
                }

                // Final fallback to basic process detection
                return await this.GetActiveWindowBasicAsync(cancellationToken).ConfigureAwait(false);
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

            try
            {
                // Check accessibility permissions
                this.hasAccessibilityPermissions = this.CheckAccessibilityPermissions();
                this.accessibilityPermissionsChecked = true;

                if (!this.hasAccessibilityPermissions)
                {
                    this.logger?.LogWarning("Accessibility permissions not granted. Some window detection features may be limited. " +
                        "To enable full functionality, grant accessibility permissions in System Preferences > Security & Privacy > Privacy > Accessibility.");
                }
                else
                {
                    this.logger?.LogInformation("Accessibility permissions granted. Full window detection capabilities available.");
                }
            }
            catch (Exception ex)
            {
                this.logger?.LogWarning(ex, "Failed to check accessibility permissions, will use fallback methods");
                this.hasAccessibilityPermissions = false;
                this.accessibilityPermissionsChecked = true;
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
        /// Gets active window information using Cocoa APIs with Accessibility framework.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Window information or null if not available.</returns>
        private async Task<WindowInfo?> GetActiveWindowCocoaAsync(CancellationToken cancellationToken)
        {
            try
            {
                this.logger?.LogDebug("Attempting to get active window using Cocoa APIs");

                // Get the frontmost application using NSWorkspace
                var workspace = CocoaNativeMethods.objc_msgSend(CocoaNativeMethods.objc_getClass("NSWorkspace"), CocoaNativeMethods.sel_registerName("sharedWorkspace"));
                if (workspace == IntPtr.Zero)
                {
                    this.logger?.LogDebug("Failed to get NSWorkspace shared instance");
                    return null;
                }

                var frontmostApp = CocoaNativeMethods.objc_msgSend(workspace, CocoaNativeMethods.sel_registerName("frontmostApplication"));
                if (frontmostApp == IntPtr.Zero)
                {
                    this.logger?.LogDebug("No frontmost application found");
                    return null;
                }

                // Get process identifier
                var processId = (int)CocoaNativeMethods.objc_msgSend(frontmostApp, CocoaNativeMethods.sel_registerName("processIdentifier"));
                if (processId <= 0)
                {
                    this.logger?.LogDebug("Invalid process identifier: {ProcessId}", processId);
                    return null;
                }

                // Get bundle identifier
                var bundleIdPtr = CocoaNativeMethods.objc_msgSend(frontmostApp, CocoaNativeMethods.sel_registerName("bundleIdentifier"));
                var bundleId = bundleIdPtr != IntPtr.Zero ? Marshal.PtrToStringAuto(bundleIdPtr) : string.Empty;

                // Get localized name
                var localizedNamePtr = CocoaNativeMethods.objc_msgSend(frontmostApp, CocoaNativeMethods.sel_registerName("localizedName"));
                var localizedName = localizedNamePtr != IntPtr.Zero ? Marshal.PtrToStringAuto(localizedNamePtr) : string.Empty;

                // Try to get window title using Accessibility APIs
                var windowTitle = await this.GetActiveWindowTitleAccessibilityAsync(processId, cancellationToken).ConfigureAwait(false);

                // Get process information
                var processInfo = await this.GetProcessInfoAsync(processId, cancellationToken).ConfigureAwait(false);

                var windowInfo = new WindowInfo
                {
                    Title = windowTitle ?? localizedName ?? processInfo?.ProcessName ?? string.Empty,
                    ProcessName = processInfo?.ProcessName ?? localizedName ?? string.Empty,
                    ProcessId = processId,
                    ProcessPath = processInfo?.ProcessPath ?? string.Empty,
                    WindowClass = bundleId ?? string.Empty,
                    Platform = this.PlatformName,
                    CapturedAt = DateTime.UtcNow,
                };

                // Add macOS-specific data
                windowInfo.PlatformSpecificData["BundleIdentifier"] = bundleId ?? string.Empty;
                windowInfo.PlatformSpecificData["LocalizedName"] = localizedName ?? string.Empty;
                windowInfo.PlatformSpecificData["DetectionMethod"] = "Cocoa";

                this.logger?.LogDebug("Retrieved Cocoa window info: {ProcessName} - {Title}", processInfo?.ProcessName, windowTitle);
                return windowInfo;
            }
            catch (Exception ex)
            {
                this.logger?.LogDebug(ex, "Failed to get active window using Cocoa APIs");
                return null;
            }
        }

        /// <summary>
        /// Gets active window title using Accessibility APIs.
        /// </summary>
        /// <param name="processId">The process ID of the target application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The window title or null if not available.</returns>
        private async Task<string?> GetActiveWindowTitleAccessibilityAsync(int processId, CancellationToken cancellationToken)
        {
            try
            {
                // Create AXUIElementRef for the application
                var appRef = AccessibilityNativeMethods.AXUIElementCreateApplication(processId);
                if (appRef == IntPtr.Zero)
                {
                    return null;
                }

                try
                {
                    // Get the focused window
                    var focusedWindowRef = IntPtr.Zero;
                    var result = AccessibilityNativeMethods.AXUIElementCopyAttributeValue(
                        appRef,
                        AccessibilityNativeMethods.kAXFocusedWindowAttribute,
                        out focusedWindowRef);

                    if (result != 0 || focusedWindowRef == IntPtr.Zero)
                    {
                        return null;
                    }

                    try
                    {
                        // Get the window title
                        var titleRef = IntPtr.Zero;
                        result = AccessibilityNativeMethods.AXUIElementCopyAttributeValue(
                            focusedWindowRef,
                            AccessibilityNativeMethods.kAXTitleAttribute,
                            out titleRef);

                        if (result == 0 && titleRef != IntPtr.Zero)
                        {
                            var title = Marshal.PtrToStringAuto(titleRef);
                            CocoaNativeMethods.CFRelease(titleRef);
                            return title;
                        }
                    }
                    finally
                    {
                        CocoaNativeMethods.CFRelease(focusedWindowRef);
                    }
                }
                finally
                {
                    CocoaNativeMethods.CFRelease(appRef);
                }
            }
            catch (Exception ex)
            {
                this.logger?.LogDebug(ex, "Failed to get window title using Accessibility APIs");
            }

            return null;
        }

        /// <summary>
        /// Gets active window information using AppleScript.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Window information or null if not available.</returns>
        private async Task<WindowInfo?> GetActiveWindowAppleScriptAsync(CancellationToken cancellationToken)
        {
            try
            {
                this.logger?.LogDebug("Attempting to get active window using AppleScript");

                // Enhanced AppleScript to get more detailed information
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

                        try
                            set bundleId to bundle identifier of frontApp
                        on error
                            set bundleId to """"
                        end try

                        try
                            set windowCount to count of windows of frontApp
                        on error
                            set windowCount to 0
                        end try

                        return appName & ""|"" & appPID & ""|"" & windowTitle & ""|"" & bundleId & ""|"" & windowCount
                    end tell";

                var result = await this.RunAppleScriptAsync(script, cancellationToken).ConfigureAwait(false);
                if (string.IsNullOrEmpty(result))
                {
                    this.logger?.LogDebug("AppleScript returned empty result");
                    return null;
                }

                var parts = result.Split('|');
                if (parts.Length < 3)
                {
                    this.logger?.LogDebug("AppleScript result has insufficient parts: {Result}", result);
                    return null;
                }

                var appName = parts[0].Trim();
                if (!int.TryParse(parts[1].Trim(), out var processId))
                {
                    this.logger?.LogDebug("Failed to parse process ID: {ProcessIdString}", parts[1]);
                    return null;
                }

                var windowTitle = parts[2].Trim();
                var bundleId = parts.Length > 3 ? parts[3].Trim() : string.Empty;
                var windowCountStr = parts.Length > 4 ? parts[4].Trim() : "0";
                int.TryParse(windowCountStr, out var windowCount);

                // Get additional process information
                var processInfo = await this.GetProcessInfoAsync(processId, cancellationToken).ConfigureAwait(false);

                var windowInfo = new WindowInfo
                {
                    Title = !string.IsNullOrEmpty(windowTitle) ? windowTitle : appName,
                    ProcessName = processInfo?.ProcessName ?? appName,
                    ProcessId = processId,
                    ProcessPath = processInfo?.ProcessPath ?? string.Empty,
                    WindowClass = bundleId,
                    Platform = this.PlatformName,
                    CapturedAt = DateTime.UtcNow,
                };

                // Add macOS-specific data
                windowInfo.PlatformSpecificData["ApplicationName"] = appName;
                windowInfo.PlatformSpecificData["BundleIdentifier"] = bundleId;
                windowInfo.PlatformSpecificData["WindowCount"] = windowCount;
                windowInfo.PlatformSpecificData["DetectionMethod"] = "AppleScript";

                this.logger?.LogDebug("Retrieved AppleScript window info: {ProcessName} - {Title}", appName, windowTitle);
                return windowInfo;
            }
            catch (Exception ex)
            {
                this.logger?.LogDebug(ex, "Failed to get active window using AppleScript");
                return null;
            }
        }

        /// <summary>
        /// Gets active window information using basic process detection.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Window information or null if not available.</returns>
        private async Task<WindowInfo?> GetActiveWindowBasicAsync(CancellationToken cancellationToken)
        {
            try
            {
                this.logger?.LogDebug("Attempting basic window detection using process list");

                // Get all GUI processes and try to identify the most likely active one
                var processes = Process.GetProcesses();

                // Look for common GUI applications that are likely to be active
                var guiProcesses = new[]
                {
                    "Safari", "Google Chrome", "Firefox", "Microsoft Edge",
                    "Visual Studio Code", "Xcode", "Terminal", "iTerm2",
                    "Finder", "System Preferences", "Activity Monitor",
                    "TextEdit", "Notes", "Mail", "Messages", "Slack",
                    "Discord", "Zoom", "Microsoft Teams"
                };

                foreach (var processName in guiProcesses)
                {
                    var matchingProcesses = Array.FindAll(processes, p =>
                        p.ProcessName.Contains(processName, StringComparison.OrdinalIgnoreCase));

                    if (matchingProcesses.Length > 0)
                    {
                        var process = matchingProcesses[0];
                        var processInfo = await this.GetProcessInfoAsync(process.Id, cancellationToken).ConfigureAwait(false);

                        if (processInfo != null)
                        {
                            var windowInfo = new WindowInfo
                            {
                                Title = processInfo.ProcessName,
                                ProcessName = processInfo.ProcessName,
                                ProcessId = processInfo.ProcessId,
                                ProcessPath = processInfo.ProcessPath,
                                Platform = this.PlatformName,
                                CapturedAt = DateTime.UtcNow,
                            };

                            windowInfo.PlatformSpecificData["DetectionMethod"] = "BasicProcessList";
                            windowInfo.PlatformSpecificData["Confidence"] = "Low";

                            this.logger?.LogDebug("Basic detection found likely active process: {ProcessName}", processInfo.ProcessName);
                            return windowInfo;
                        }
                    }
                }

                this.logger?.LogDebug("No suitable GUI processes found in basic detection");
                return null;
            }
            catch (Exception ex)
            {
                this.logger?.LogDebug(ex, "Failed basic window detection");
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
        /// Checks if accessibility permissions are granted using the Accessibility framework.
        /// </summary>
        /// <returns>True if accessibility permissions are granted.</returns>
        private bool CheckAccessibilityPermissions()
        {
            try
            {
                // Use AXIsProcessTrusted() to check if the current process has accessibility permissions
                var isTrusted = AccessibilityNativeMethods.AXIsProcessTrusted();
                this.logger?.LogDebug("Accessibility permissions check result: {IsTrusted}", isTrusted);
                return isTrusted;
            }
            catch (Exception ex)
            {
                this.logger?.LogDebug(ex, "Failed to check accessibility permissions, assuming not granted");
                return false;
            }
        }

        /// <summary>
        /// Requests accessibility permissions from the user.
        /// </summary>
        /// <returns>True if permissions are granted or already available.</returns>
        private bool RequestAccessibilityPermissions()
        {
            try
            {
                // Create options dictionary to prompt for accessibility permissions
                var options = CocoaNativeMethods.CFDictionaryCreateMutable(
                    IntPtr.Zero, 0, IntPtr.Zero, IntPtr.Zero);

                if (options != IntPtr.Zero)
                {
                    try
                    {
                        // Set the prompt option to true
                        var key = CocoaNativeMethods.CFStringCreateWithCString(
                            IntPtr.Zero, "AXTrustedCheckOptionPrompt", 0);
                        var value = CocoaNativeMethods.CFBooleanGetTrue();

                        if (key != IntPtr.Zero)
                        {
                            CocoaNativeMethods.CFDictionarySetValue(options, key, value);
                            CocoaNativeMethods.CFRelease(key);
                        }

                        // Check with prompt
                        var result = AccessibilityNativeMethods.AXIsProcessTrustedWithOptions(options);
                        this.logger?.LogDebug("Accessibility permissions request result: {Result}", result);
                        return result;
                    }
                    finally
                    {
                        CocoaNativeMethods.CFRelease(options);
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                this.logger?.LogWarning(ex, "Failed to request accessibility permissions");
                return false;
            }
        }

        /// <summary>
        /// Contains Cocoa native method declarations for Objective-C runtime.
        /// </summary>
        private static class CocoaNativeMethods
        {
            /// <summary>
            /// Gets a class by name.
            /// </summary>
            /// <param name="name">The class name.</param>
            /// <returns>The class pointer.</returns>
            [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_getClass")]
            internal static extern IntPtr objc_getClass(string name);

            /// <summary>
            /// Registers a selector.
            /// </summary>
            /// <param name="name">The selector name.</param>
            /// <returns>The selector pointer.</returns>
            [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "sel_registerName")]
            internal static extern IntPtr sel_registerName(string name);

            /// <summary>
            /// Sends a message to an object.
            /// </summary>
            /// <param name="receiver">The receiver object.</param>
            /// <param name="selector">The selector.</param>
            /// <returns>The result pointer.</returns>
            [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
            internal static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector);

            /// <summary>
            /// Releases a Core Foundation object.
            /// </summary>
            /// <param name="cf">The Core Foundation object.</param>
            [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
            internal static extern void CFRelease(IntPtr cf);

            /// <summary>
            /// Creates a mutable dictionary.
            /// </summary>
            /// <param name="allocator">The allocator.</param>
            /// <param name="capacity">The capacity.</param>
            /// <param name="keyCallBacks">The key callbacks.</param>
            /// <param name="valueCallBacks">The value callbacks.</param>
            /// <returns>The dictionary reference.</returns>
            [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
            internal static extern IntPtr CFDictionaryCreateMutable(
                IntPtr allocator, long capacity, IntPtr keyCallBacks, IntPtr valueCallBacks);

            /// <summary>
            /// Sets a value in a dictionary.
            /// </summary>
            /// <param name="theDict">The dictionary.</param>
            /// <param name="key">The key.</param>
            /// <param name="value">The value.</param>
            [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
            internal static extern void CFDictionarySetValue(IntPtr theDict, IntPtr key, IntPtr value);

            /// <summary>
            /// Creates a CFString from a C string.
            /// </summary>
            /// <param name="alloc">The allocator.</param>
            /// <param name="cStr">The C string.</param>
            /// <param name="encoding">The encoding.</param>
            /// <returns>The CFString reference.</returns>
            [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
            internal static extern IntPtr CFStringCreateWithCString(IntPtr alloc, string cStr, uint encoding);

            /// <summary>
            /// Gets the CFBoolean true value.
            /// </summary>
            /// <returns>The CFBoolean true reference.</returns>
            [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
            internal static extern IntPtr CFBooleanGetTrue();
        }

        /// <summary>
        /// Contains Accessibility framework native method declarations.
        /// </summary>
        private static class AccessibilityNativeMethods
        {
            /// <summary>
            /// The kAXFocusedWindowAttribute constant.
            /// </summary>
            internal static readonly IntPtr kAXFocusedWindowAttribute =
                CocoaNativeMethods.CFStringCreateWithCString(IntPtr.Zero, "AXFocusedWindow", 0);

            /// <summary>
            /// The kAXTitleAttribute constant.
            /// </summary>
            internal static readonly IntPtr kAXTitleAttribute =
                CocoaNativeMethods.CFStringCreateWithCString(IntPtr.Zero, "AXTitle", 0);

            /// <summary>
            /// Checks if the current process is trusted for accessibility.
            /// </summary>
            /// <returns>True if the process is trusted.</returns>
            [DllImport("/System/Library/Frameworks/ApplicationServices.framework/ApplicationServices")]
            internal static extern bool AXIsProcessTrusted();

            /// <summary>
            /// Checks if the current process is trusted for accessibility with options.
            /// </summary>
            /// <param name="options">The options dictionary.</param>
            /// <returns>True if the process is trusted.</returns>
            [DllImport("/System/Library/Frameworks/ApplicationServices.framework/ApplicationServices")]
            internal static extern bool AXIsProcessTrustedWithOptions(IntPtr options);

            /// <summary>
            /// Creates an AXUIElement for an application.
            /// </summary>
            /// <param name="pid">The process ID.</param>
            /// <returns>The AXUIElement reference.</returns>
            [DllImport("/System/Library/Frameworks/ApplicationServices.framework/ApplicationServices")]
            internal static extern IntPtr AXUIElementCreateApplication(int pid);

            /// <summary>
            /// Copies an attribute value from an AXUIElement.
            /// </summary>
            /// <param name="element">The AXUIElement.</param>
            /// <param name="attribute">The attribute name.</param>
            /// <param name="value">The output value.</param>
            /// <returns>The result code (0 for success).</returns>
            [DllImport("/System/Library/Frameworks/ApplicationServices.framework/ApplicationServices")]
            internal static extern int AXUIElementCopyAttributeValue(
                IntPtr element, IntPtr attribute, out IntPtr value);
        }
    }
}