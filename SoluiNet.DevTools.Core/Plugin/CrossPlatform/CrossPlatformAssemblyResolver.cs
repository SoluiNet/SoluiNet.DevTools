// <copyright file="CrossPlatformAssemblyResolver.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Plugin.CrossPlatform
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using NLog;
    using SoluiNet.DevTools.Core.Services.Platform;

    /// <summary>
    /// Cross-platform assembly resolver for plugin loading.
    /// </summary>
    public class CrossPlatformAssemblyResolver : ICrossPlatformAssemblyResolver
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IPlatformService platformService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CrossPlatformAssemblyResolver"/> class.
        /// </summary>
        /// <param name="platformService">The platform service.</param>
        public CrossPlatformAssemblyResolver(IPlatformService platformService)
        {
            this.platformService = platformService ?? throw new ArgumentNullException(nameof(platformService));
        }

        /// <inheritdoc />
        public Assembly ResolveAssembly(string assemblyName, string[] searchPaths)
        {
            if (string.IsNullOrEmpty(assemblyName))
            {
                return null;
            }

            if (searchPaths == null || searchPaths.Length == 0)
            {
                return null;
            }

            var assemblyFileName = new AssemblyName(assemblyName).Name + ".dll";

            foreach (var searchPath in searchPaths)
            {
                if (!Directory.Exists(searchPath))
                {
                    continue;
                }

                var assemblyPath = Path.Combine(searchPath, assemblyFileName);
                
                if (File.Exists(assemblyPath))
                {
                    try
                    {
                        if (this.IsCompatibleAssembly(assemblyPath) && this.ValidateAssembly(assemblyPath))
                        {
                            Logger.Debug("Loading assembly from: {0}", assemblyPath);
                            return Assembly.LoadFrom(assemblyPath);
                        }
                        else
                        {
                            Logger.Warn("Assembly {0} is not compatible with current platform", assemblyPath);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "Failed to load assembly from: {0}", assemblyPath);
                    }
                }
            }

            return null;
        }

        /// <inheritdoc />
        public IEnumerable<string> GetPluginSearchPaths()
        {
            var searchPaths = new List<string>();
            
            // Get the application directory
            var appDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (!string.IsNullOrEmpty(appDirectory))
            {
                searchPaths.Add(appDirectory);
                
                // Add Plugins subdirectory
                var pluginsDirectory = Path.Combine(appDirectory, "Plugins");
                if (Directory.Exists(pluginsDirectory))
                {
                    searchPaths.Add(pluginsDirectory);
                }

                // Add platform-specific plugin directories
                var currentPlatform = this.GetCurrentPlatformName();
                var currentArchitecture = this.GetCurrentArchitectureName();
                
                var platformSpecificPath = Path.Combine(pluginsDirectory, currentPlatform);
                if (Directory.Exists(platformSpecificPath))
                {
                    searchPaths.Add(platformSpecificPath);
                }

                var archSpecificPath = Path.Combine(pluginsDirectory, currentPlatform, currentArchitecture);
                if (Directory.Exists(archSpecificPath))
                {
                    searchPaths.Add(archSpecificPath);
                }
            }

            return searchPaths;
        }

        /// <inheritdoc />
        public bool IsCompatibleAssembly(string assemblyPath)
        {
            if (string.IsNullOrEmpty(assemblyPath) || !File.Exists(assemblyPath))
            {
                return false;
            }

            try
            {
                // Check if we can get assembly name (basic validation)
                var assemblyName = AssemblyName.GetAssemblyName(assemblyPath);
                
                // Check processor architecture compatibility
                var assemblyArchitecture = assemblyName.ProcessorArchitecture;
                var currentArchitecture = RuntimeInformation.ProcessArchitecture;

                // Allow MSIL (AnyCPU) assemblies on any architecture
                if (assemblyArchitecture == ProcessorArchitecture.MSIL || 
                    assemblyArchitecture == ProcessorArchitecture.None)
                {
                    return true;
                }

                // Check specific architecture compatibility
                return this.IsArchitectureCompatible(assemblyArchitecture, currentArchitecture);
            }
            catch (Exception ex)
            {
                Logger.Debug(ex, "Failed to check assembly compatibility for: {0}", assemblyPath);
                return false;
            }
        }

        /// <inheritdoc />
        public bool ValidateAssembly(string assemblyPath)
        {
            if (string.IsNullOrEmpty(assemblyPath) || !File.Exists(assemblyPath))
            {
                return false;
            }

            try
            {
                // Basic validation - try to load assembly metadata
                var assemblyName = AssemblyName.GetAssemblyName(assemblyPath);
                
                // Check if it's a .NET assembly
                if (assemblyName == null)
                {
                    return false;
                }

                // Additional validation could be added here (e.g., digital signature verification)
                return true;
            }
            catch (BadImageFormatException)
            {
                Logger.Debug("Assembly {0} has invalid format", assemblyPath);
                return false;
            }
            catch (Exception ex)
            {
                Logger.Debug(ex, "Failed to validate assembly: {0}", assemblyPath);
                return false;
            }
        }

        /// <inheritdoc />
        public IPluginMetadata GetPluginMetadata(string assemblyPath)
        {
            if (string.IsNullOrEmpty(assemblyPath) || !File.Exists(assemblyPath))
            {
                return null;
            }

            try
            {
                var assemblyName = AssemblyName.GetAssemblyName(assemblyPath);
                var version = assemblyName.Version?.ToString() ?? "1.0.0.0";

                // Try to load assembly to get more detailed metadata
                var assembly = Assembly.LoadFrom(assemblyPath);
                
                // Look for plugin metadata attributes or embedded resources
                var supportedPlatforms = this.ExtractSupportedPlatforms(assembly);
                var supportedArchitectures = this.ExtractSupportedArchitectures(assembly);
                var dependencies = this.ExtractDependencies(assembly);
                var requiresElevation = this.ExtractElevationRequirement(assembly);
                var minimumDotNetVersion = this.ExtractMinimumDotNetVersion(assembly);

                return new PluginMetadata(
                    assemblyName.Name,
                    version,
                    supportedPlatforms,
                    supportedArchitectures,
                    dependencies,
                    requiresElevation,
                    minimumDotNetVersion);
            }
            catch (Exception ex)
            {
                Logger.Debug(ex, "Failed to extract plugin metadata from: {0}", assemblyPath);
                return null;
            }
        }

        private string GetCurrentPlatformName()
        {
            if (OperatingSystem.IsWindows())
            {
                return "Windows";
            }
            else if (OperatingSystem.IsLinux())
            {
                return "Linux";
            }
            else if (OperatingSystem.IsMacOS())
            {
                return "macOS";
            }

            return "Unknown";
        }

        private string GetCurrentArchitectureName()
        {
            return RuntimeInformation.ProcessArchitecture switch
            {
                Architecture.X64 => "x64",
                Architecture.Arm64 => "arm64",
                Architecture.X86 => "x86",
                Architecture.Arm => "arm",
                _ => "unknown"
            };
        }

        private bool IsArchitectureCompatible(ProcessorArchitecture assemblyArch, Architecture currentArch)
        {
            return (assemblyArch, currentArch) switch
            {
                (ProcessorArchitecture.Amd64, Architecture.X64) => true,
                (ProcessorArchitecture.X86, Architecture.X86) => true,
                (ProcessorArchitecture.X86, Architecture.X64) => true, // x86 can run on x64
                (ProcessorArchitecture.Arm, Architecture.Arm) => true,
                (ProcessorArchitecture.Arm, Architecture.Arm64) => true, // ARM can run on ARM64
                _ => false
            };
        }

        private IEnumerable<string> ExtractSupportedPlatforms(Assembly assembly)
        {
            // Default to all platforms if no specific metadata found
            // In a real implementation, this could check for custom attributes
            return new[] { "Windows", "Linux", "macOS" };
        }

        private IEnumerable<string> ExtractSupportedArchitectures(Assembly assembly)
        {
            // Default to common architectures if no specific metadata found
            // In a real implementation, this could check for custom attributes
            return new[] { "x64", "arm64" };
        }

        private IEnumerable<string> ExtractDependencies(Assembly assembly)
        {
            // Extract dependencies from assembly references
            return assembly.GetReferencedAssemblies()
                .Where(a => !a.Name.StartsWith("System.", StringComparison.OrdinalIgnoreCase) &&
                           !a.Name.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase))
                .Select(a => a.Name)
                .ToList();
        }

        private bool ExtractElevationRequirement(Assembly assembly)
        {
            // Check for elevation requirement attributes or manifest
            // Default to false for now
            return false;
        }

        private Version ExtractMinimumDotNetVersion(Assembly assembly)
        {
            // Try to determine minimum .NET version from target framework
            // Default to .NET 6.0 for now
            return new Version(6, 0);
        }
    }
}