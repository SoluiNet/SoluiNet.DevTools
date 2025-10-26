// <copyright file="CrossPlatformFileTest.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.UnitTest
{
    using System;
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SoluiNet.DevTools.Core.Tools.File;

    /// <summary>
    /// Tests for cross-platform file operations.
    /// </summary>
    [TestClass]
    public class CrossPlatformFileTest
    {
        /// <summary>
        /// Test that temporary file path generation works across platforms.
        /// </summary>
        [TestMethod]
        public void GetTemporaryFilePath_ShouldReturnValidPath()
        {
            // Act
            var tempPath = CrossPlatformFileHelper.GetTemporaryFilePath();

            // Assert
            Assert.IsNotNull(tempPath);
            Assert.IsTrue(tempPath.Contains("SoluiNet.DevTools"));
            Assert.IsTrue(tempPath.Contains("temp"));
            Assert.IsTrue(tempPath.EndsWith(".tmp"));

            // Verify the path uses correct separators for the platform
            var normalizedPath = CrossPlatformFileHelper.NormalizePath(tempPath);
            Assert.AreEqual(tempPath, normalizedPath);
        }

        /// <summary>
        /// Test that application data path generation works across platforms.
        /// </summary>
        [TestMethod]
        public void GetApplicationDataPath_ShouldReturnValidPath()
        {
            // Act
            var appDataPath = CrossPlatformFileHelper.GetApplicationDataPath();

            // Assert
            Assert.IsNotNull(appDataPath);
            Assert.IsTrue(appDataPath.Length > 0);

            // Verify the path is absolute
            Assert.IsTrue(CrossPlatformFileHelper.IsAbsolutePath(appDataPath));
        }

        /// <summary>
        /// Test that configuration path generation works across platforms.
        /// </summary>
        [TestMethod]
        public void GetConfigurationPath_ShouldReturnValidPath()
        {
            // Act
            var configPath = CrossPlatformFileHelper.GetConfigurationPath();

            // Assert
            Assert.IsNotNull(configPath);
            Assert.IsTrue(configPath.Length > 0);

            // Verify the path is absolute
            Assert.IsTrue(CrossPlatformFileHelper.IsAbsolutePath(configPath));
        }

        /// <summary>
        /// Test that path normalization works correctly.
        /// </summary>
        [TestMethod]
        public void NormalizePath_ShouldHandleMixedSeparators()
        {
            // Arrange
            var mixedPath = "folder\\subfolder/file.txt";

            // Act
            var normalizedPath = CrossPlatformFileHelper.NormalizePath(mixedPath);

            // Assert
            Assert.IsNotNull(normalizedPath);

            // Verify that the path uses consistent separators
            var expectedSeparator = Path.DirectorySeparatorChar;
            Assert.IsFalse(normalizedPath.Contains(expectedSeparator == '\\' ? '/' : '\\'));
        }

        /// <summary>
        /// Test that path combination works correctly.
        /// </summary>
        [TestMethod]
        public void CombinePaths_ShouldCombineCorrectly()
        {
            // Arrange
            var path1 = "folder1";
            var path2 = "folder2";
            var path3 = "file.txt";

            // Act
            var combinedPath = CrossPlatformFileHelper.CombinePaths(path1, path2, path3);

            // Assert
            Assert.IsNotNull(combinedPath);
            Assert.IsTrue(combinedPath.Contains(path1));
            Assert.IsTrue(combinedPath.Contains(path2));
            Assert.IsTrue(combinedPath.Contains(path3));

            // Verify the path uses correct separators
            var normalizedPath = CrossPlatformFileHelper.NormalizePath(combinedPath);
            Assert.AreEqual(combinedPath, normalizedPath);
        }

        /// <summary>
        /// Test that executable extension detection works correctly.
        /// </summary>
        [TestMethod]
        public void GetExecutableExtension_ShouldReturnPlatformAppropriateExtension()
        {
            // Act
            var extension = CrossPlatformFileHelper.GetExecutableExtension();

            // Assert
            Assert.IsNotNull(extension);

            // On Windows, should return ".exe", on Unix-like systems, should return empty string
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                Assert.AreEqual(".exe", extension);
            }
            else
            {
                Assert.AreEqual(string.Empty, extension);
            }
        }

        /// <summary>
        /// Test that directory creation works correctly.
        /// </summary>
        [TestMethod]
        public void EnsureDirectoryExists_ShouldCreateDirectory()
        {
            // Arrange
            var tempDir = Path.GetTempPath();
            var testDir = Path.Combine(tempDir, "SoluiNetTest_" + Guid.NewGuid().ToString("N").Substring(0, 8));

            try
            {
                // Act
                CrossPlatformFileHelper.EnsureDirectoryExists(testDir);

                // Assert
                Assert.IsTrue(Directory.Exists(testDir));
            }
            finally
            {
                // Cleanup
                if (Directory.Exists(testDir))
                {
                    CrossPlatformFileHelper.SafeDeleteDirectory(testDir);
                }
            }
        }

        /// <summary>
        /// Test that file deletion works correctly.
        /// </summary>
        [TestMethod]
        public void SafeDeleteFile_ShouldDeleteFile()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, "test content");

            // Act
            var result = CrossPlatformFileHelper.SafeDeleteFile(tempFile);

            // Assert
            Assert.IsTrue(result);
            Assert.IsFalse(File.Exists(tempFile));
        }
    }
}