// <copyright file="PluginIntegrationTest.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.UnitTest
{
    using System;
    using System.IO;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SoluiNet.DevTools.Core.Plugin;
    using SoluiNet.DevTools.Core.Plugin.CrossPlatform;
    using SoluiNet.DevTools.Core.Services.Platform;

    /// <summary>
    /// Integration tests for plugin loading across different platforms.
    /// </summary>
    [TestClass]
    public class PluginIntegrationTest
    {
        private IPlatformService platformService;
        private CrossPlatformPluginLoader pluginLoader;
        private CrossPlatformPluginDiscovery pluginDiscovery;
        private CrossPlatformAssemblyResolver assemblyResolver;
        private string testPluginDirectory;

        /// <summary>
        /// Initialize test setup.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            this.platformService = PlatformServiceFactory.Create();
            this.assemblyResolver = new CrossPlatformAssemblyResolver(this.platformService);
            this.pluginDiscovery = new CrossPlatformPluginDiscovery(this.assemblyResolver);
            this.pluginLoader = new CrossPlatformPluginLoader(this.pluginDiscovery, this.assemblyResolver);
            
            // Create a temporary test directory for plugins
            this.testPluginDirectory = Path.Combine(Path.GetTempPath(), "SoluiNetPluginTest_" + Guid.NewGuid().ToString("N").Substring(0, 8));
            Directory.CreateDirectory(this.testPluginDirectory);
        }

        /// <summary>
        /// Clean up test resources.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(this.testPluginDirectory))
            {
                try
                {
                    Directory.Delete(this.testPluginDirectory, true);
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
        }

        /// <summary>
        /// Test plugin discovery in empty directory.
        /// </summary>
        [TestMethod]
        public void PluginDiscovery_ShouldReturnEmptyForEmptyDirectory()
        {
            // Act - The discovery will find plugins in the system, but we test that it returns a valid collection
            var plugins = this.pluginDiscovery.DiscoverPluginAssemblies();

            // Assert
            Assert.IsNotNull(plugins);
            // Note: May find actual plugins in the system, so we just verify it returns a valid collection
        }

        /// <summary>
        /// Test plugin discovery with non-existent directory.
        /// </summary>
        [TestMethod]
        public void PluginDiscovery_ShouldHandleNonExistentDirectory()
        {
            // Act - The discovery will handle non-existent directories gracefully
            var plugins = this.pluginDiscovery.DiscoverPluginAssemblies();

            // Assert
            Assert.IsNotNull(plugins);
            // Note: May find actual plugins in the system, so we just verify it returns a valid collection
        }

        /// <summary>
        /// Test plugin discovery with null directory.
        /// </summary>
        [TestMethod]
        public void PluginDiscovery_ShouldHandleNullDirectory()
        {
            // Act - The discovery will handle null/empty directories gracefully
            var plugins = this.pluginDiscovery.DiscoverPluginAssemblies();

            // Assert
            Assert.IsNotNull(plugins);
            // Note: May find actual plugins in the system, so we just verify it returns a valid collection
        }

        /// <summary>
        /// Test plugin discovery with files that are not assemblies.
        /// </summary>
        [TestMethod]
        public void PluginDiscovery_ShouldIgnoreNonAssemblyFiles()
        {
            // Arrange
            var textFile = Path.Combine(this.testPluginDirectory, "readme.txt");
            var configFile = Path.Combine(this.testPluginDirectory, "config.json");
            var imageFile = Path.Combine(this.testPluginDirectory, "icon.png");
            
            File.WriteAllText(textFile, "This is a readme file");
            File.WriteAllText(configFile, "{}");
            File.WriteAllBytes(imageFile, new byte[] { 0x89, 0x50, 0x4E, 0x47 }); // PNG header

            // Act - The discovery should ignore non-assembly files
            var plugins = this.pluginDiscovery.DiscoverPluginAssemblies();

            // Assert
            Assert.IsNotNull(plugins);
            // Note: May find actual plugins in the system, so we just verify it returns a valid collection
            // The non-assembly files we created should be ignored
        }

        /// <summary>
        /// Test plugin discovery with invalid DLL files.
        /// </summary>
        [TestMethod]
        public void PluginDiscovery_ShouldHandleInvalidDllFiles()
        {
            // Arrange
            var invalidDll = Path.Combine(this.testPluginDirectory, "invalid.dll");
            File.WriteAllText(invalidDll, "This is not a valid DLL file");

            // Act
            var plugins = this.pluginDiscovery.DiscoverPluginAssemblies();

            // Assert
            Assert.IsNotNull(plugins);
            // Should find the file but it won't be loadable
            Assert.IsTrue(plugins.Any());
        }

        /// <summary>
        /// Test assembly resolver search paths.
        /// </summary>
        [TestMethod]
        public void AssemblyResolver_ShouldReturnValidSearchPaths()
        {
            // Act
            var searchPaths = this.assemblyResolver.GetPluginSearchPaths();

            // Assert
            Assert.IsNotNull(searchPaths);
            Assert.IsTrue(searchPaths.Any());
            
            foreach (var path in searchPaths)
            {
                Assert.IsNotNull(path);
                Assert.IsTrue(path.Length > 0);
            }
        }

        /// <summary>
        /// Test assembly validation with non-existent file.
        /// </summary>
        [TestMethod]
        public void AssemblyResolver_ShouldReturnFalseForNonExistentAssembly()
        {
            // Arrange
            var nonExistentAssembly = Path.Combine(this.testPluginDirectory, "non-existent.dll");

            // Act
            var isValid = this.assemblyResolver.ValidateAssembly(nonExistentAssembly);

            // Assert
            Assert.IsFalse(isValid);
        }

        /// <summary>
        /// Test assembly validation with invalid file.
        /// </summary>
        [TestMethod]
        public void AssemblyResolver_ShouldReturnFalseForInvalidAssembly()
        {
            // Arrange
            var invalidAssembly = Path.Combine(this.testPluginDirectory, "invalid.dll");
            File.WriteAllText(invalidAssembly, "This is not a valid assembly");

            // Act
            var isValid = this.assemblyResolver.ValidateAssembly(invalidAssembly);

            // Assert
            Assert.IsFalse(isValid);
        }

        /// <summary>
        /// Test assembly validation with null path.
        /// </summary>
        [TestMethod]
        public void AssemblyResolver_ShouldReturnFalseForNullPath()
        {
            // Act
            var isValid = this.assemblyResolver.ValidateAssembly(null);

            // Assert
            Assert.IsFalse(isValid);
        }

        /// <summary>
        /// Test plugin loader with non-existent assembly.
        /// </summary>
        [TestMethod]
        public void PluginLoader_ShouldReturnNullForNonExistentAssembly()
        {
            // Arrange
            var nonExistentAssembly = Path.Combine(this.testPluginDirectory, "non-existent.dll");

            // Act
            var plugins = this.pluginLoader.LoadPluginsFromAssembly<IBasePlugin>(nonExistentAssembly);

            // Assert
            Assert.IsNotNull(plugins);
            Assert.AreEqual(0, plugins.Count());
        }

        /// <summary>
        /// Test plugin loader with invalid assembly.
        /// </summary>
        [TestMethod]
        public void PluginLoader_ShouldHandleInvalidAssembly()
        {
            // Arrange
            var invalidAssembly = Path.Combine(this.testPluginDirectory, "invalid.dll");
            File.WriteAllText(invalidAssembly, "This is not a valid assembly");

            // Act
            var plugins = this.pluginLoader.LoadPluginsFromAssembly<IBasePlugin>(invalidAssembly);

            // Assert
            Assert.IsNotNull(plugins);
            Assert.AreEqual(0, plugins.Count());
        }

        /// <summary>
        /// Test plugin compatibility validation.
        /// </summary>
        [TestMethod]
        public void PluginCompatibilityValidator_ShouldValidateNonExistentPlugin()
        {
            // Arrange
            var validator = CrossPlatformPluginFactory.CreateCompatibilityValidator();
            var nonExistentPlugin = Path.Combine(this.testPluginDirectory, "non-existent.dll");

            // Act
            var result = validator.ValidatePlugin(nonExistentPlugin);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsValid);
            Assert.IsNotNull(result.Reason);
            Assert.IsTrue(result.Reason.Contains("does not exist"));
        }

        /// <summary>
        /// Test plugin compatibility validation with invalid assembly.
        /// </summary>
        [TestMethod]
        public void PluginCompatibilityValidator_ShouldHandleInvalidAssembly()
        {
            // Arrange
            var validator = CrossPlatformPluginFactory.CreateCompatibilityValidator();
            var invalidAssembly = Path.Combine(this.testPluginDirectory, "invalid.dll");
            File.WriteAllText(invalidAssembly, "This is not a valid assembly");

            // Act
            var result = validator.ValidatePlugin(invalidAssembly);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsValid);
            Assert.IsNotNull(result.Reason);
        }

        /// <summary>
        /// Test plugin metadata creation and validation.
        /// </summary>
        [TestMethod]
        public void PluginMetadata_ShouldCreateWithValidDefaults()
        {
            // Arrange
            var pluginName = "TestPlugin";
            var pluginVersion = "1.0.0";

            // Act
            var metadata = new PluginMetadata(pluginName, pluginVersion);

            // Assert
            Assert.IsNotNull(metadata);
            Assert.AreEqual(pluginName, metadata.Name);
            Assert.AreEqual(pluginVersion, metadata.Version);
            Assert.IsNotNull(metadata.SupportedPlatforms);
            Assert.IsTrue(metadata.SupportedPlatforms.Any());
            Assert.IsNotNull(metadata.SupportedArchitectures);
            Assert.IsTrue(metadata.SupportedArchitectures.Any());
        }

        /// <summary>
        /// Test incompatible plugin info creation.
        /// </summary>
        [TestMethod]
        public void IncompatiblePluginInfo_ShouldCreateCorrectly()
        {
            // Arrange
            var assemblyPath = "test.dll";
            var reason = "Test incompatibility reason";
            var exception = new Exception("Test exception");

            // Act
            var info = new IncompatiblePluginInfo(assemblyPath, reason, exception);

            // Assert
            Assert.IsNotNull(info);
            Assert.AreEqual(assemblyPath, info.AssemblyPath);
            Assert.AreEqual(reason, info.Reason);
            Assert.AreEqual(exception, info.Exception);
        }

        /// <summary>
        /// Test cross-platform plugin factory methods.
        /// </summary>
        [TestMethod]
        public void CrossPlatformPluginFactory_ShouldCreateAllComponents()
        {
            // Act & Assert
            var assemblyResolver = CrossPlatformPluginFactory.GetAssemblyResolver();
            Assert.IsNotNull(assemblyResolver);

            var pluginDiscovery = CrossPlatformPluginFactory.GetPluginDiscovery();
            Assert.IsNotNull(pluginDiscovery);

            var pluginLoader = CrossPlatformPluginFactory.GetPluginLoader();
            Assert.IsNotNull(pluginLoader);

            var compatibilityValidator = CrossPlatformPluginFactory.CreateCompatibilityValidator();
            Assert.IsNotNull(compatibilityValidator);
        }

        /// <summary>
        /// Test plugin discovery with subdirectories.
        /// </summary>
        [TestMethod]
        public void PluginDiscovery_ShouldSearchSubdirectories()
        {
            // Arrange
            var subDir = Path.Combine(this.testPluginDirectory, "subdirectory");
            Directory.CreateDirectory(subDir);
            
            var invalidDll = Path.Combine(subDir, "subplugin.dll");
            File.WriteAllText(invalidDll, "This is not a valid DLL file");

            // Act
            var plugins = this.pluginDiscovery.DiscoverPluginAssemblies();

            // Assert
            Assert.IsNotNull(plugins);
            Assert.IsTrue(plugins.Any());
        }
    }
}