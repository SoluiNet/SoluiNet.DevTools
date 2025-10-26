// <copyright file="PluginCompatibilityValidator.cs" company="SoluiNet">
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
    /// Validates plugin compatibility with the current platform and architecture.
    /// </summary>
    public class PluginCompatibilityValidator
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IPlatformService platformService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginCompatibilityValidator"/> class.
        /// </summary>
        /// <param name="platformService">The platform service.</param>
        public PluginCompatibilityValidator(IPlatformService platformService)
        {
            this.platformService = platformService ?? throw new ArgumentNullException(nameof(platformService));
        }

        /// <summary>
        /// Validates that a plugin is compatible with the current platform and architecture.
        /// </summary>
        /// <param name="assemblyPath">The path to the plugin assembly.</param>
        /// <returns>A validation result indicating compatibility status.</returns>
        public PluginValidationResult ValidatePlugin(string assemblyPath)
        {
            if (string.IsNullOrEmpty(assemblyPath) || !File.Exists(assemblyPath))
            {
                return new PluginValidationResult(false, "Assembly file does not exist");
            }

            try
            {
                // Basic assembly validation
                var assemblyName = AssemblyName.GetAssemblyName(assemblyPath);
                if (assemblyName == null)
                {
                    return new PluginValidationResult(false, "Invalid assembly format");
                }

                // Platform compatibility check
                var platformCompatibility = this.ValidatePlatformCompatibility(assemblyPath);
                if (!platformCompatibility.IsValid)
                {
                    return platformCompatibility;
                }

                // Architecture compatibility check
                var architectureCompatibility = this.ValidateArchitectureCompatibility(assemblyName);
                if (!architectureCompatibility.IsValid)
                {
                    return architectureCompatibility;
                }

                // .NET version compatibility check
                var dotNetCompatibility = this.ValidateDotNetCompatibility(assemblyPath);
                if (!dotNetCompatibility.IsValid)
                {
                    return dotNetCompatibility;
                }

                // Dependency validation
                var dependencyCompatibility = this.ValidateDependencies(assemblyPath);
                if (!dependencyCompatibility.IsValid)
                {
                    return dependencyCompatibility;
                }

                return new PluginValidationResult(true, "Plugin is compatible");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error validating plugin: {0}", assemblyPath);
                return new PluginValidationResult(false, $"Validation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets detailed compatibility information for a plugin.
        /// </summary>
        /// <param name="assemblyPath">The path to the plugin assembly.</param>
        /// <returns>Detailed compatibility information.</returns>
        public PluginCompatibilityInfo GetCompatibilityInfo(string assemblyPath)
        {
            var info = new PluginCompatibilityInfo
            {
                AssemblyPath = assemblyPath,
                CurrentPlatform = this.GetCurrentPlatformName(),
                CurrentArchitecture = this.GetCurrentArchitectureName(),
                CurrentDotNetVersion = Environment.Version.ToString()
            };

            if (!File.Exists(assemblyPath))
            {
                info.IsCompatible = false;
                info.IncompatibilityReasons.Add("Assembly file does not exist");
                return info;
            }

            try
            {
                var assemblyName = AssemblyName.GetAssemblyName(assemblyPath);
                info.AssemblyName = assemblyName.Name;
                info.AssemblyVersion = assemblyName.Version?.ToString();
                info.AssemblyArchitecture = assemblyName.ProcessorArchitecture.ToString();

                // Load assembly to get more detailed information
                var assembly = Assembly.LoadFrom(assemblyPath);
                var metadata = this.ExtractPluginMetadata(assembly);
                
                if (metadata != null)
                {
                    info.SupportedPlatforms.AddRange(metadata.SupportedPlatforms);
                    info.SupportedArchitectures.AddRange(metadata.SupportedArchitectures);
                    info.MinimumDotNetVersion = metadata.MinimumDotNetVersion?.ToString();
                    info.RequiresElevation = metadata.RequiresElevation;
                    info.Dependencies.AddRange(metadata.Dependencies);
                }

                // Validate compatibility
                var validationResult = this.ValidatePlugin(assemblyPath);
                info.IsCompatible = validationResult.IsValid;
                
                if (!validationResult.IsValid)
                {
                    info.IncompatibilityReasons.Add(validationResult.Reason);
                }
            }
            catch (Exception ex)
            {
                info.IsCompatible = false;
                info.IncompatibilityReasons.Add($"Error analyzing assembly: {ex.Message}");
                Logger.Debug(ex, "Error getting compatibility info for: {0}", assemblyPath);
            }

            return info;
        }

        private PluginValidationResult ValidatePlatformCompatibility(string assemblyPath)
        {
            try
            {
                var assembly = Assembly.LoadFrom(assemblyPath);
                var metadata = this.ExtractPluginMetadata(assembly);
                
                if (metadata?.SupportedPlatforms != null)
                {
                    var currentPlatform = this.GetCurrentPlatformName();
                    var supportedPlatforms = metadata.SupportedPlatforms.ToList();
                    
                    if (!supportedPlatforms.Contains(currentPlatform, StringComparer.OrdinalIgnoreCase))
                    {
                        return new PluginValidationResult(
                            false,
                            $"Plugin does not support current platform '{currentPlatform}'. Supported platforms: {string.Join(", ", supportedPlatforms)}");
                    }
                }

                return new PluginValidationResult(true, "Platform compatibility validated");
            }
            catch (Exception ex)
            {
                Logger.Debug(ex, "Error validating platform compatibility for: {0}", assemblyPath);
                return new PluginValidationResult(true, "Platform compatibility check skipped due to error");
            }
        }

        private PluginValidationResult ValidateArchitectureCompatibility(AssemblyName assemblyName)
        {
            var assemblyArchitecture = assemblyName.ProcessorArchitecture;
            var currentArchitecture = RuntimeInformation.ProcessArchitecture;

            // Allow MSIL (AnyCPU) assemblies on any architecture
            if (assemblyArchitecture == ProcessorArchitecture.MSIL || 
                assemblyArchitecture == ProcessorArchitecture.None)
            {
                return new PluginValidationResult(true, "Architecture compatibility validated (AnyCPU)");
            }

            // Check specific architecture compatibility
            var isCompatible = (assemblyArchitecture, currentArchitecture) switch
            {
                (ProcessorArchitecture.Amd64, Architecture.X64) => true,
                (ProcessorArchitecture.X86, Architecture.X86) => true,
                (ProcessorArchitecture.X86, Architecture.X64) => true, // x86 can run on x64
                (ProcessorArchitecture.Arm, Architecture.Arm) => true,
                (ProcessorArchitecture.Arm, Architecture.Arm64) => true, // ARM can run on ARM64
                _ => false
            };

            if (!isCompatible)
            {
                return new PluginValidationResult(
                    false,
                    $"Architecture mismatch: Plugin is {assemblyArchitecture}, current system is {currentArchitecture}");
            }

            return new PluginValidationResult(true, "Architecture compatibility validated");
        }

        private PluginValidationResult ValidateDotNetCompatibility(string assemblyPath)
        {
            try
            {
                var assembly = Assembly.LoadFrom(assemblyPath);
                var metadata = this.ExtractPluginMetadata(assembly);
                
                if (metadata?.MinimumDotNetVersion != null)
                {
                    var currentVersion = Environment.Version;
                    
                    if (currentVersion < metadata.MinimumDotNetVersion)
                    {
                        return new PluginValidationResult(
                            false,
                            $"Plugin requires .NET {metadata.MinimumDotNetVersion}, current version is {currentVersion}");
                    }
                }

                return new PluginValidationResult(true, ".NET version compatibility validated");
            }
            catch (Exception ex)
            {
                Logger.Debug(ex, "Error validating .NET compatibility for: {0}", assemblyPath);
                return new PluginValidationResult(true, ".NET compatibility check skipped due to error");
            }
        }

        private PluginValidationResult ValidateDependencies(string assemblyPath)
        {
            try
            {
                var assembly = Assembly.LoadFrom(assemblyPath);
                var referencedAssemblies = assembly.GetReferencedAssemblies();
                
                foreach (var referencedAssembly in referencedAssemblies)
                {
                    try
                    {
                        Assembly.Load(referencedAssembly);
                    }
                    catch (FileNotFoundException)
                    {
                        return new PluginValidationResult(
                            false,
                            $"Missing dependency: {referencedAssembly.Name}");
                    }
                }

                return new PluginValidationResult(true, "Dependencies validated");
            }
            catch (Exception ex)
            {
                Logger.Debug(ex, "Error validating dependencies for: {0}", assemblyPath);
                return new PluginValidationResult(true, "Dependency validation skipped due to error");
            }
        }

        private IPluginMetadata ExtractPluginMetadata(Assembly assembly)
        {
            // This is a simplified implementation
            // In a real scenario, you might look for custom attributes or embedded resources
            var assemblyName = assembly.GetName();
            
            // Use platform service to determine default supported platforms
            var defaultPlatforms = new[] { this.platformService.GetType().Name.Replace("PlatformService", string.Empty) };
            
            return new PluginMetadata(
                assemblyName.Name,
                assemblyName.Version?.ToString() ?? "1.0.0.0",
                defaultPlatforms);
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
    }

    /// <summary>
    /// Represents the result of plugin validation.
    /// </summary>
    public class PluginValidationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginValidationResult"/> class.
        /// </summary>
        /// <param name="isValid">Whether the plugin is valid.</param>
        /// <param name="reason">The reason for the validation result.</param>
        public PluginValidationResult(bool isValid, string reason)
        {
            this.IsValid = isValid;
            this.Reason = reason;
        }

        /// <summary>
        /// Gets a value indicating whether the plugin is valid.
        /// </summary>
        public bool IsValid { get; }

        /// <summary>
        /// Gets the reason for the validation result.
        /// </summary>
        public string Reason { get; }
    }

    /// <summary>
    /// Detailed compatibility information for a plugin.
    /// </summary>
    public class PluginCompatibilityInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginCompatibilityInfo"/> class.
        /// </summary>
        public PluginCompatibilityInfo()
        {
            this.SupportedPlatforms = new List<string>();
            this.SupportedArchitectures = new List<string>();
            this.Dependencies = new List<string>();
            this.IncompatibilityReasons = new List<string>();
        }

        /// <summary>
        /// Gets or sets the assembly path.
        /// </summary>
        public string AssemblyPath { get; set; }

        /// <summary>
        /// Gets or sets the assembly name.
        /// </summary>
        public string AssemblyName { get; set; }

        /// <summary>
        /// Gets or sets the assembly version.
        /// </summary>
        public string AssemblyVersion { get; set; }

        /// <summary>
        /// Gets or sets the assembly architecture.
        /// </summary>
        public string AssemblyArchitecture { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the plugin is compatible.
        /// </summary>
        public bool IsCompatible { get; set; }

        /// <summary>
        /// Gets or sets the current platform.
        /// </summary>
        public string CurrentPlatform { get; set; }

        /// <summary>
        /// Gets or sets the current architecture.
        /// </summary>
        public string CurrentArchitecture { get; set; }

        /// <summary>
        /// Gets or sets the current .NET version.
        /// </summary>
        public string CurrentDotNetVersion { get; set; }

        /// <summary>
        /// Gets the supported platforms.
        /// </summary>
        public List<string> SupportedPlatforms { get; }

        /// <summary>
        /// Gets the supported architectures.
        /// </summary>
        public List<string> SupportedArchitectures { get; }

        /// <summary>
        /// Gets or sets the minimum .NET version.
        /// </summary>
        public string MinimumDotNetVersion { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the plugin requires elevation.
        /// </summary>
        public bool RequiresElevation { get; set; }

        /// <summary>
        /// Gets the plugin dependencies.
        /// </summary>
        public List<string> Dependencies { get; }

        /// <summary>
        /// Gets the incompatibility reasons.
        /// </summary>
        public List<string> IncompatibilityReasons { get; }
    }
}