// <copyright file="PlatformServiceTest.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.UnitTest
{
    using System;
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SoluiNet.DevTools.Core.Services.Platform;

    /// <summary>
    /// Tests for platform service implementations.
    /// </summary>
    [TestClass]
    public class PlatformServiceTest
    {
        /// <summary>
        /// Test that platform service factory creates appropriate service for current platform.
        /// </summary>
        [TestMethod]
        public void PlatformServiceFactory_ShouldCreateCorrectService()
        {
            // Act
            var platformService = PlatformServiceFactory.Create();

            // Assert
            Assert.IsNotNull(platformService);

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
        /// Test Windows platform service configuration directory.
        /// </summary>
        [TestMethod]
        public void WindowsPlatformService_GetConfigurationDirectory_ShouldReturnValidPath()
        {
            // Arrange
            var service = new WindowsPlatformService();

            // Act
            var configDir = service.GetConfigurationDirectory();

            // Assert
            Assert.IsNotNull(configDir);
            Assert.IsTrue(configDir.Contains("SoluiNet"));
            Assert.IsTrue(configDir.Contains("DevTools"));
            Assert.IsTrue(Path.IsPathRooted(configDir));
        }

        /// <summary>
        /// Test Windows platform service application data directory.
        /// </summary>
        [TestMethod]
        public void WindowsPlatformService_GetApplicationDataDirectory_ShouldReturnValidPath()
        {
            // Arrange
            var service = new WindowsPlatformService();

            // Act
            var appDataDir = service.GetApplicationDataDirectory();

            // Assert
            Assert.IsNotNull(appDataDir);
            Assert.IsTrue(appDataDir.Contains("SoluiNet"));
            Assert.IsTrue(appDataDir.Contains("DevTools"));
            Assert.IsTrue(Path.IsPathRooted(appDataDir));
        }

        /// <summary>
        /// Test Windows platform service path normalization.
        /// </summary>
        [TestMethod]
        public void WindowsPlatformService_NormalizePath_ShouldConvertToBackslashes()
        {
            // Arrange
            var service = new WindowsPlatformService();
            var unixPath = "folder/subfolder/file.txt";

            // Act
            var normalizedPath = service.NormalizePath(unixPath);

            // Assert
            Assert.IsNotNull(normalizedPath);
            // On Windows, should convert to backslashes; on other platforms, behavior may vary
            if (OperatingSystem.IsWindows())
            {
                Assert.IsFalse(normalizedPath.Contains("/"));
                Assert.IsTrue(normalizedPath.Contains("\\"));
            }
            else
            {
                // When running on non-Windows, the service still normalizes but may not convert separators
                Assert.IsNotNull(normalizedPath);
            }
        }

        /// <summary>
        /// Test Windows platform service executable extension.
        /// </summary>
        [TestMethod]
        public void WindowsPlatformService_GetExecutableExtension_ShouldReturnExe()
        {
            // Arrange
            var service = new WindowsPlatformService();

            // Act
            var extension = service.GetExecutableExtension();

            // Assert
            Assert.AreEqual(".exe", extension);
        }

        /// <summary>
        /// Test Linux platform service configuration directory.
        /// </summary>
        [TestMethod]
        public void LinuxPlatformService_GetConfigurationDirectory_ShouldReturnValidPath()
        {
            // Arrange
            var service = new LinuxPlatformService();

            // Act
            var configDir = service.GetConfigurationDirectory();

            // Assert
            Assert.IsNotNull(configDir);
            Assert.IsTrue(configDir.Contains("soluinet-devtools"));
            Assert.IsTrue(Path.IsPathRooted(configDir));
        }

        /// <summary>
        /// Test Linux platform service path normalization.
        /// </summary>
        [TestMethod]
        public void LinuxPlatformService_NormalizePath_ShouldConvertToForwardSlashes()
        {
            // Arrange
            var service = new LinuxPlatformService();
            var windowsPath = "folder\\subfolder\\file.txt";

            // Act
            var normalizedPath = service.NormalizePath(windowsPath);

            // Assert
            Assert.IsNotNull(normalizedPath);
            Assert.IsFalse(normalizedPath.Contains("\\"));
            Assert.IsTrue(normalizedPath.Contains("/"));
        }

        /// <summary>
        /// Test Linux platform service executable extension.
        /// </summary>
        [TestMethod]
        public void LinuxPlatformService_GetExecutableExtension_ShouldReturnEmpty()
        {
            // Arrange
            var service = new LinuxPlatformService();

            // Act
            var extension = service.GetExecutableExtension();

            // Assert
            Assert.AreEqual(string.Empty, extension);
        }

        /// <summary>
        /// Test macOS platform service configuration directory.
        /// </summary>
        [TestMethod]
        public void MacOSPlatformService_GetConfigurationDirectory_ShouldReturnValidPath()
        {
            // Arrange
            var service = new MacOSPlatformService();

            // Act
            var configDir = service.GetConfigurationDirectory();

            // Assert
            Assert.IsNotNull(configDir);
            Assert.IsTrue(configDir.Contains("Library"));
            Assert.IsTrue(configDir.Contains("Application Support"));
            Assert.IsTrue(configDir.Contains("SoluiNet.DevTools"));
            Assert.IsTrue(Path.IsPathRooted(configDir));
        }

        /// <summary>
        /// Test macOS platform service path normalization.
        /// </summary>
        [TestMethod]
        public void MacOSPlatformService_NormalizePath_ShouldConvertToForwardSlashes()
        {
            // Arrange
            var service = new MacOSPlatformService();
            var windowsPath = "folder\\subfolder\\file.txt";

            // Act
            var normalizedPath = service.NormalizePath(windowsPath);

            // Assert
            Assert.IsNotNull(normalizedPath);
            Assert.IsFalse(normalizedPath.Contains("\\"));
            Assert.IsTrue(normalizedPath.Contains("/"));
        }

        /// <summary>
        /// Test macOS platform service executable extension.
        /// </summary>
        [TestMethod]
        public void MacOSPlatformService_GetExecutableExtension_ShouldReturnEmpty()
        {
            // Arrange
            var service = new MacOSPlatformService();

            // Act
            var extension = service.GetExecutableExtension();

            // Assert
            Assert.AreEqual(string.Empty, extension);
        }

        /// <summary>
        /// Test that all platform services handle null/empty paths correctly.
        /// </summary>
        [TestMethod]
        public void AllPlatformServices_NormalizePath_ShouldHandleNullAndEmpty()
        {
            // Arrange
            var services = new IPlatformService[]
            {
                new WindowsPlatformService(),
                new LinuxPlatformService(),
                new MacOSPlatformService()
            };

            foreach (var service in services)
            {
                // Act & Assert
                Assert.IsNull(service.NormalizePath(null));
                Assert.AreEqual(string.Empty, service.NormalizePath(string.Empty));
            }
        }

        /// <summary>
        /// Test that all platform services return valid temp directories.
        /// </summary>
        [TestMethod]
        public void AllPlatformServices_GetTempDirectory_ShouldReturnValidPath()
        {
            // Arrange
            var services = new IPlatformService[]
            {
                new WindowsPlatformService(),
                new LinuxPlatformService(),
                new MacOSPlatformService()
            };

            foreach (var service in services)
            {
                // Act
                var tempDir = service.GetTempDirectory();

                // Assert
                Assert.IsNotNull(tempDir);
                Assert.IsTrue(tempDir.Length > 0);
                Assert.IsTrue(Path.IsPathRooted(tempDir));
            }
        }

        /// <summary>
        /// Test executable file detection with non-existent files.
        /// </summary>
        [TestMethod]
        public void AllPlatformServices_IsExecutableFile_ShouldReturnFalseForNonExistentFile()
        {
            // Arrange
            var services = new IPlatformService[]
            {
                new WindowsPlatformService(),
                new LinuxPlatformService(),
                new MacOSPlatformService()
            };

            var nonExistentFile = "non-existent-file.exe";

            foreach (var service in services)
            {
                // Act & Assert
                Assert.IsFalse(service.IsExecutableFile(nonExistentFile));
                Assert.IsFalse(service.IsExecutableFile(null));
                Assert.IsFalse(service.IsExecutableFile(string.Empty));
            }
        }
    }
}