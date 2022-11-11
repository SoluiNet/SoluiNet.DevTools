// <copyright file="Program.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NLog;
using SoluiNet.DevTools.UI.Blazor;

/// <summary>
/// The solui.net blazor app program.
/// </summary>
public class Program : IHostedService
{
    /// <summary>
    /// The main entry point.
    /// </summary>
    /// <param name="args">The arguments.</param>
    public static void Main(string[] args)
    {
        try
        {
            var host = CreateWebHostBuilder(args).Build();
            host.Run();
        }
        catch (Exception exception)
        {
            var logger = LogManager.GetCurrentClassLogger();

            logger.Fatal(exception, "Unhandled Exception while executing SoluiNet.DevTools.UI.Blazor");
        }
    }

    /// <summary>
    /// Create the web host builder.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <returns>Returns a <see cref="IWebHostBuilder"/>.</returns>
    public static IWebHostBuilder CreateWebHostBuilder(string[] args)
    {
        return WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}