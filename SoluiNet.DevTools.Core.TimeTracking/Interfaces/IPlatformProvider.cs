// <copyright file="IPlatformProvider.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.TimeTracking.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;
    using SoluiNet.DevTools.Core.TimeTracking.Models;

    /// <summary>
    /// Interface for platform-specific window and process information providers.
    /// </summary>
    public interface IPlatformProvider
    {
        /// <summary>
        /// Gets the name of the platform this provider supports.
        /// </summary>
        string PlatformName { get; }

        /// <summary>
        /// Gets a value indicating whether this platform provider is supported on the current system.
        /// </summary>
        bool IsSupported { get; }

        /// <summary>
        /// Gets the currently active window information asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation that returns the active window information.</returns>
        Task<WindowInfo?> GetActiveWindowAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the currently active process information asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation that returns the active process information.</returns>
        Task<ProcessInfo?> GetActiveProcessAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Initializes the platform provider.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task InitializeAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Disposes of platform-specific resources.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DisposeAsync(CancellationToken cancellationToken = default);
    }
}