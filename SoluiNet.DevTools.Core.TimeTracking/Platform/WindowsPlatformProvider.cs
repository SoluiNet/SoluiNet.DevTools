// <copyright file="WindowsPlatformProvider.cs" company="SoluiNet">
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
    /// Windows-specific platform provider for window and process information.
    /// </summary>
    public class WindowsPlatformProvider : IPlatformProvider
    {
        private readonly ILogger? logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsPlatformProvider"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        public WindowsPlatformProvider(ILogger? logger = null)
        {
            this.logger = logger;
        }

        /// <inheritdoc />
        public string PlatformName => "Windows";

        /// <inheritdoc />
        public bool IsSupported => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        /// <inheritdoc />
        public Task<WindowInfo?> GetActiveWindowAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                this.logger?.LogDebug("Getting active window on Windows");

                var foregroundWindow = NativeMethods.GetForegroundWindow();
                if (foregroundWindow == IntPtr.Zero)
                {
                    this.logger?.LogDebug("No foreground window found");
                    return Task.FromResult<WindowInfo?>(null);
                }

                // Get window title
                const int titleBufferSize = 256;
                var titleBuffer = new StringBuilder(titleBufferSize);
                var titleLength = NativeMethods.GetWindowText(foregroundWindow, titleBuffer, titleBufferSize);
                var windowTitle = titleLength > 0 ? titleBuffer.ToString() : string.Empty;

                // Get window class name
                const int classBufferSize = 256;
                var classBuffer = new StringBuilder(classBufferSize);
                var classLength = NativeMethods.GetClassName(foregroundWindow, classBuffer, classBufferSize);
                var windowClass = classLength > 0 ? classBuffer.ToString() : string.Empty;

                // Get process ID
                var processId = NativeMethods.GetWindowThreadProcessId(foregroundWindow, out var windowProcessId);
                if (windowProcessId == 0)
                {
                    this.logger?.LogDebug("Could not get process ID for window");
                    return Task.FromResult<WindowInfo?>(null);
                }

                // Get process information
                string processName = string.Empty;
                string processPath = string.Empty;

                try
                {
                    using var process = Process.GetProcessById((int)windowProcessId);
                    processName = process.ProcessName;
                    processPath = process.MainModule?.FileName ?? string.Empty;
                }
                catch (Exception ex)
                {
                    this.logger?.LogDebug(ex, "Could not get process information for PID {ProcessId}", windowProcessId);
                }

                var windowInfo = new WindowInfo
                {
                    Title = windowTitle,
                    ProcessName = processName,
                    ProcessId = (int)windowProcessId,
                    ProcessPath = processPath,
                    WindowClass = windowClass,
                    Platform = this.PlatformName,
                    CapturedAt = DateTime.UtcNow,
                };

                // Add Windows-specific data
                windowInfo.PlatformSpecificData["WindowHandle"] = foregroundWindow.ToInt64();
                windowInfo.PlatformSpecificData["ThreadId"] = processId;

                this.logger?.LogDebug("Retrieved window info: {ProcessName} - {Title}", processName, windowTitle);
                return Task.FromResult<WindowInfo?>(windowInfo);
            }
            catch (Exception ex)
            {
                this.logger?.LogError(ex, "Failed to get active window information on Windows");
                return Task.FromResult<WindowInfo?>(null);
            }
        }

        /// <inheritdoc />
        public Task<ProcessInfo?> GetActiveProcessAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                this.logger?.LogDebug("Getting active process on Windows");

                var foregroundWindow = NativeMethods.GetForegroundWindow();
                if (foregroundWindow == IntPtr.Zero)
                {
                    this.logger?.LogDebug("No foreground window found");
                    return Task.FromResult<ProcessInfo?>(null);
                }

                // Get process ID from window
                NativeMethods.GetWindowThreadProcessId(foregroundWindow, out var windowProcessId);
                if (windowProcessId == 0)
                {
                    this.logger?.LogDebug("Could not get process ID for window");
                    return Task.FromResult<ProcessInfo?>(null);
                }

                // Get process information
                try
                {
                    using var process = Process.GetProcessById((int)windowProcessId);

                    var processInfo = new ProcessInfo
                    {
                        ProcessId = process.Id,
                        ProcessName = process.ProcessName,
                        ProcessPath = process.MainModule?.FileName ?? string.Empty,
                        CommandLine = GetProcessCommandLine(process.Id),
                        Platform = this.PlatformName,
                        CapturedAt = DateTime.UtcNow,
                    };

                    // Add Windows-specific data
                    processInfo.PlatformSpecificData["WindowHandle"] = foregroundWindow.ToInt64();
                    processInfo.PlatformSpecificData["StartTime"] = process.StartTime;
                    processInfo.PlatformSpecificData["WorkingSet"] = process.WorkingSet64;

                    this.logger?.LogDebug("Retrieved process info: {ProcessName} (PID: {ProcessId})", processInfo.ProcessName, processInfo.ProcessId);
                    return Task.FromResult<ProcessInfo?>(processInfo);
                }
                catch (Exception ex)
                {
                    this.logger?.LogDebug(ex, "Could not get process information for PID {ProcessId}", windowProcessId);
                    return Task.FromResult<ProcessInfo?>(null);
                }
            }
            catch (Exception ex)
            {
                this.logger?.LogError(ex, "Failed to get active process information on Windows");
                return Task.FromResult<ProcessInfo?>(null);
            }
        }

        /// <inheritdoc />
        public Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            this.logger?.LogDebug("Initializing Windows platform provider");
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task DisposeAsync(CancellationToken cancellationToken = default)
        {
            this.logger?.LogDebug("Disposing Windows platform provider");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets the command line for a process.
        /// </summary>
        /// <param name="processId">The process ID.</param>
        /// <returns>The command line string, or empty string if not available.</returns>
        private string GetProcessCommandLine(int processId)
        {
            try
            {
                // This is a simplified implementation. A more complete implementation
                // would use WMI or other Windows APIs to get the full command line.
                using var process = Process.GetProcessById(processId);
                return process.MainModule?.FileName ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Contains Windows native method declarations.
        /// </summary>
        private static class NativeMethods
        {
            /// <summary>
            /// Retrieves a handle to the foreground window.
            /// </summary>
            /// <returns>Returns a handle to the foreground window. Can be null in certain circumstances.</returns>
            [DllImport("user32.dll")]
            [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
            internal static extern IntPtr GetForegroundWindow();

            /// <summary>
            /// Copies the text of the specified window's title bar (if it has one) into a buffer.
            /// </summary>
            /// <param name="hWnd">A handle to the window or control containing the text.</param>
            /// <param name="text">The buffer that will receive the text.</param>
            /// <param name="count">The maximum number of characters to copy to the buffer.</param>
            /// <returns>If the function succeeds, the return value is the length, in characters, of the copied string.</returns>
            [DllImport("user32.dll", CharSet = CharSet.Unicode)]
            [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1838:Avoid 'StringBuilder' parameters for P/Invokes", Justification = "Required for Win32 API.")]
            internal static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

            /// <summary>
            /// Retrieves the name of the class to which the specified window belongs.
            /// </summary>
            /// <param name="hWnd">A handle to the window.</param>
            /// <param name="className">The class name string.</param>
            /// <param name="maxCount">The length of the buffer.</param>
            /// <returns>The number of characters copied to the buffer.</returns>
            [DllImport("user32.dll", CharSet = CharSet.Unicode)]
            [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1838:Avoid 'StringBuilder' parameters for P/Invokes", Justification = "Required for Win32 API.")]
            internal static extern int GetClassName(IntPtr hWnd, StringBuilder className, int maxCount);

            /// <summary>
            /// Retrieves the identifier of the thread that created the specified window and the identifier of the process that created the window.
            /// </summary>
            /// <param name="hWnd">A handle to the window.</param>
            /// <param name="processId">A pointer to a variable that receives the process identifier.</param>
            /// <returns>The thread identifier.</returns>
            [DllImport("user32.dll")]
            [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
            internal static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);
        }
    }
}