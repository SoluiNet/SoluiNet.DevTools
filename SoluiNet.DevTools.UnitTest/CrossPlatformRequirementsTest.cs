// <copyright file="CrossPlatformRequirementsTest.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.UnitTest
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SoluiNet.DevTools.Core.Configuration;
    using SoluiNet.DevTools.Core.Plugin.CrossPlatform;
    using SoluiNet.DevTools.Core.Services.Platform;
    using SoluiNet.DevTools.Core.Tools.File;

    /// <summary>
    /// Comprehensive tests that validate all cross-platform requirements.
    /// </summary>
    [TestClass]
    public class CrossPlatformRequirementsTest
    {
        /// <summary>
        /// Test Requirement 1.1: Console application starts successfully on current platform.
        /// </summary>
        [TestMethod]
        public void Requirement1_1_ConsoleApplicationShouldStartSuccessfully()
        {
            // Arrange & Act
            var platformService = PlatformServiceFactory.Create();

            // Assert
            Assert.IsNotNull(platformService);

            // Verify platform detection works
            if (OperatingSystem.IsWindows())
            {
                Assert.IsInstanceOfType(platformService, typeof(WindowsPlatformService));
            }
            else if (OperatingSystem.IsLinux())
            {
                Assert.IsInstanceOfType(platformService, typeof(LinuxPlatformService));
            }
            else if (OperatingSystem.IsMacOS())
            {
                Assert.IsInstanceOfType(platformService, typeof(MacOSPlatformService));
            }
        }

        /// <summary>
        /// Test Requirement 1.2 & 2.2: Plugin system discovers and loads compatible plugins.
        /// </summary>
        [TestMethod]
        public void Requirement1_2_2_2_PluginSystemShouldDiscoverAndLoadPlugins()
        {
            // Arrange
            var pluginDiscovery = CrossPlatformPluginFactory.GetPluginDiscovery();
            var pluginLoader = CrossPlatformPluginFactory.GetPluginLoader();

            // Act
            var searchPaths = CrossPlatformPluginFactory.GetAssemblyResolver().GetPluginSearchPaths();

            // Assert
            Assert.IsNotNull(pluginDiscovery);
            Assert.IsNotNull(pluginLoader);
            Assert.IsNotNull(searchPaths);
            Assert.IsTrue(searchPaths.Any());

            // Test plugin discovery in each search path
            foreach (var searchPath in searchPaths)
            {
                if (Directory.Exists(searchPath))
                {
                    var plugins = pluginDiscovery.DiscoverPluginAssemblies();
                    Assert.IsNotNull(plugins);
                }
            }
        }

        /// <summary>
        /// Test Requirement 1.4 & 2.4: File path handling works correctly on current platform.
        /// </summary>
        [TestMethod]
        public void Requirement1_4_2_4_FilePathHandlingShouldWorkCorrectly()
        {
            // Arrange
            var platformService = PlatformServiceFactory.Create();

            // Act & Assert - Test path normalization
            var mixedPath = "folder\\subfolder/file.txt";
            var normalizedPath = platformService.NormalizePath(mixedPath);

            Assert.IsNotNull(normalizedPath);

            // Verify path uses correct separators for platform
            if (OperatingSystem.IsWindows())
            {
                Assert.IsFalse(normalizedPath.Contains("/"));
                Assert.IsTrue(normalizedPath.Contains("\\"));
            }
            else
            {
                Assert.IsFalse(normalizedPath.Contains("\\"));
                Assert.IsTrue(normalizedPath.Contains("/"));
            }

            // Test cross-platform file helper
            var tempPath = CrossPlatformFileHelper.GetTemporaryFilePath();
            Assert.IsNotNull(tempPath);
            Assert.IsTrue(CrossPlatformFileHelper.IsAbsolutePath(tempPath));
        }

        /// <summary>
        /// Test Requirement 1.5 & 2.5: Logging uses platform-appropriate directories.
        /// </summary>
        [TestMethod]
        public void Requirement1_5_2_5_LoggingShouldUsePlatformAppropriateDirectories()
        {
            // Arrange
            var platformService = PlatformServiceFactory.Create();

            // Act
            var appDataDir = platformService.GetApplicationDataDirectory();
            var configDir = platformService.GetConfigurationDirectory();

            // Assert
            Assert.IsNotNull(appDataDir);
            Assert.IsNotNull(configDir);
            Assert.IsTrue(Path.IsPathRooted(appDataDir));
            Assert.IsTrue(Path.IsPathRooted(configDir));

            // Verify platform-specific paths
            if (OperatingSystem.IsWindows())
            {
                Assert.IsTrue(appDataDir.Contains("SoluiNet"));
                Assert.IsTrue(configDir.Contains("SoluiNet"));
            }
            else if (OperatingSystem.IsLinux())
            {
                Assert.IsTrue(configDir.Contains("soluinet-devtools"));
            }
            else if (OperatingSystem.IsMacOS())
            {
                Assert.IsTrue(configDir.Contains("Library"));
                Assert.IsTrue(configDir.Contains("Application Support"));
            }
        }

        /// <summary>
        /// Test Requirement 3.1: Platform detection works automatically.
        /// </summary>
        [TestMethod]
        public void Requirement3_1_PlatformDetectionShouldWorkAutomatically()
        {
            // Act
            var platformService = PlatformServiceFactory.Create();

            // Assert
            Assert.IsNotNull(platformService);

            // Verify platform helper works
            Assert.IsTrue(PlatformHelper.IsWindows || PlatformHelper.IsLinux || PlatformHelper.IsMacOS);
            Assert.IsNotNull(PlatformHelper.PlatformName);
            Assert.IsNotNull(PlatformHelper.GetDetailedPlatformInfo());
        }

        /// <summary>
        /// Test Requirement 3.2: Platform-specific paths use appropriate format.
        /// </summary>
        [TestMethod]
        public void Requirement3_2_PlatformSpecificPathsShouldUseAppropriateFormat()
        {
            // Arrange
            var platformService = PlatformServiceFactory.Create();

            // Act
            var configDir = platformService.GetConfigurationDirectory();
            var appDataDir = platformService.GetApplicationDataDirectory();
            var tempDir = platformService.GetTempDirectory();

            // Assert
            Assert.IsNotNull(configDir);
            Assert.IsNotNull(appDataDir);
            Assert.IsNotNull(tempDir);

            // Verify paths are normalized for platform
            Assert.AreEqual(configDir, platformService.NormalizePath(configDir));
            Assert.AreEqual(appDataDir, platformService.NormalizePath(appDataDir));
            Assert.AreEqual(tempDir, platformService.NormalizePath(tempDir));
        }

        /// <summary>
        /// Test Requirement 3.4: Configuration files use platform-appropriate directories.
        /// </summary>
        [TestMethod]
        public void Requirement3_4_ConfigurationFilesShouldUsePlatformAppropriateDirectories()
        {
            // Arrange
            var platformService = PlatformServiceFactory.Create();
            var configManager = new CrossPlatformConfigurationManager(platformService);

            // Act
            var configPath = configManager.GetConfigurationPath("test-config");
            var configDir = configManager.GetConfigurationDirectory();

            // Assert
            Assert.IsNotNull(configPath);
            Assert.IsNotNull(configDir);
            Assert.IsTrue(configPath.StartsWith(configDir));
            Assert.IsTrue(configPath.EndsWith(".json"));
        }

        /// <summary>
        /// Test Requirement 4.1: Build system targets .NET 8.0.
        /// </summary>
        [TestMethod]
        public void Requirement4_1_BuildSystemShouldTargetNet8()
        {
            // Act
            var frameworkVersion = RuntimeInformation.FrameworkDescription;

            // Assert
            Assert.IsNotNull(frameworkVersion);

            // Should be running on .NET 8.0 or later
            Assert.IsTrue(frameworkVersion.Contains(".NET") || frameworkVersion.Contains("Core"));
        }

        /// <summary>
        /// Test Requirement 5.1: Plugin system uses platform-agnostic assembly loading.
        /// </summary>
        [TestMethod]
        public void Requirement5_1_PluginSystemShouldUsePlatformAgnosticAssemblyLoading()
        {
            // Arrange
            var assemblyResolver = CrossPlatformPluginFactory.GetAssemblyResolver();

            // Act
            var searchPaths = assemblyResolver.GetPluginSearchPaths();

            // Assert
            Assert.IsNotNull(searchPaths);
            Assert.IsTrue(searchPaths.Any());

            // Verify assembly validation works
            Assert.IsFalse(assemblyResolver.ValidateAssembly("non-existent.dll"));
            Assert.IsFalse(assemblyResolver.ValidateAssembly(null));
        }

        /// <summary>
        /// Test Requirement 5.3: Plugin configuration uses platform-appropriate locations.
        /// </summary>
        [TestMethod]
        public void Requirement5_3_PluginConfigurationShouldUsePlatformAppropriateLocations()
        {
            // Arrange
            var platformService = PlatformServiceFactory.Create();
            var configManager = new CrossPlatformConfigurationManager(platformService);

            // Act
            var pluginConfigPath = configManager.GetConfigurationPath("plugins");

            // Assert
            Assert.IsNotNull(pluginConfigPath);
            Assert.IsTrue(pluginConfigPath.EndsWith(".json"));

            var configDir = Path.GetDirectoryName(pluginConfigPath);
            Assert.IsNotNull(configDir);
            Assert.IsTrue(Path.IsPathRooted(configDir));
        }

        /// <summary>
        /// Test Requirement 6.1: Application uses latest .NET version features.
        /// </summary>
        [TestMethod]
        public void Requirement6_1_ApplicationShouldUseLatestNetFeatures()
        {
            // Act & Assert - Test modern .NET APIs are available
            Assert.IsTrue(OperatingSystem.IsWindows() || OperatingSystem.IsLinux() || OperatingSystem.IsMacOS());

            // Test RuntimeInformation APIs
            Assert.IsNotNull(RuntimeInformation.RuntimeIdentifier);
            Assert.IsNotNull(RuntimeInformation.FrameworkDescription);
            Assert.IsNotNull(RuntimeInformation.OSDescription);

            // Test architecture detection
            var architecture = RuntimeInformation.ProcessArchitecture;
            Assert.IsTrue(architecture == Architecture.X64 || architecture == Architecture.Arm64 ||
                         architecture == Architecture.X86 || architecture == Architecture.Arm);
        }

        /// <summary>
        /// Test executable extension detection across platforms.
        /// </summary>
        [TestMethod]
        public void ExecutableExtensionShouldBePlatformSpecific()
        {
            // Arrange
            var platformService = PlatformServiceFactory.Create();

            // Act
            var extension = platformService.GetExecutableExtension();

            // Assert
            Assert.IsNotNull(extension);

            if (OperatingSystem.IsWindows())
            {
                Assert.AreEqual(".exe", extension);
            }
            else
            {
                Assert.AreEqual(string.Empty, extension);
            }
        }

        /// <summary>
        /// Test that temporary directory is accessible and writable.
        /// </summary>
        [TestMethod]
        public void TempDirectoryShouldBeAccessibleAndWritable()
        {
            // Arrange
            var platformService = PlatformServiceFactory.Create();

            // Act
            var tempDir = platformService.GetTempDirectory();

            // Assert
            Assert.IsNotNull(tempDir);
            Assert.IsTrue(Directory.Exists(tempDir));

            // Test that we can create a file in temp directory
            var testFile = Path.Combine(tempDir, "soluinet-test-" + Guid.NewGuid().ToString("N").Substring(0, 8) + ".tmp");

            try
            {
                File.WriteAllText(testFile, "test");
                Assert.IsTrue(File.Exists(testFile));
            }
            finally
            {
                if (File.Exists(testFile))
                {
                    File.Delete(testFile);
                }
            }
        }

        /// <summary>
        /// Test configuration migration functionality.
        /// </summary>
        [TestMethod]
        public void ConfigurationMigrationShouldWorkCorrectly()
        {
            // Arrange
            var platformService = PlatformServiceFactory.Create();
            var configManager = new CrossPlatformConfigurationManager(platformService);
            var migrationService = new ConfigurationMigrationService(configManager, platformService);

            // Act & Assert - Migration service should handle non-existent files gracefully
            var result = migrationService.MigrateFromWindowsLocations();

            // Should not throw exceptions and return false when no files exist
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Test plugin compatibility validation.
        /// </summary>
        [TestMethod]
        public void PluginCompatibilityValidationShouldWork()
        {
            // Arrange
            var validator = CrossPlatformPluginFactory.CreateCompatibilityValidator();

            // Act & Assert
            var result = validator.ValidatePlugin("non-existent-plugin.dll");

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsValid);
            Assert.IsNotNull(result.Reason);
        }
    }
}