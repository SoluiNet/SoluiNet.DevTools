// <copyright file="Program.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using SoluiNet.DevTools.UI.Blazor;

/// <summary>
/// The solui.net blazor app program.
/// </summary>
public class Program
{
    /// <summary>
    /// The main entry point.
    /// </summary>
    /// <param name="args">The arguments.</param>
    public static void Main(string[] args)
    {
        var host = CreateWebHostBuilder(args).Build();
        host.Run();
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
}