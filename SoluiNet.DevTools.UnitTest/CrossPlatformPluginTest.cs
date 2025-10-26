// <copyright file="CrossPlatformPluginTest.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.UnitTest
{
    using System;
    using System.IO;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SoluiNet.DevTools.Core.Plugin.CrossPlatform;
    using SoluiNet.DevTools.Core.Services.Platform;

    /// <summary>
    /// Tests for the cross-platform plugin system.
    /// </summary>
    [TestClass]
    public class CrossPlatformPluginTest
    {
        /// <summary>
        /// Test that the assembly resolver can be created.
        /// </summary>
        [TestMethod]
        public void AssemblyResolverCreationTest()
        {
            var platformService = PlatformServiceFactory.Create();
            var assemblyResolver = new CrossPlatformAssemblyResolver(platformService);

            Assert.IsNotNull(assemblyResolver);
        }

        /// <summary>
        /// Test that plugin search paths are returned.
        /// </summary>
        [TestMethod]
        public void GetPluginSearchPathsTest()
        {
            var assemblyResolver = CrossPlatformPluginFactory.GetAssemblyResolver();
            var searchPaths = assemblyResolver.GetPluginSearchPaths();

            Assert.IsNotNull(searchPaths);
            Assert.IsTrue(searchPaths.Any(), "Should return at least one search path");
        }

        /// <summary>
        /// Test that plugin discovery can be created.
        /// </summary>
        [TestMethod]
        public void PluginDiscoveryCreationTest()
        {
            var pluginDiscovery = CrossPlatformPluginFactory.GetPluginDiscovery();

            Assert.IsNotNull(pluginDiscovery);
        }

        /// <summary>
        /// Test that plugin loader can be created.
        /// </summary>
        [TestMethod]
        public void PluginLoaderCreationTest()
        {
            var pluginLoader = CrossPlatformPluginFactory.GetPluginLoader();

            Assert.IsNotNull(pluginLoader);
        }

        /// <summary>
        /// Test that compatibility validator can be created.
        /// </summary>
        [TestMethod]
        public void CompatibilityValidatorCreationTest()
        {
            var validator = CrossPlatformPluginFactory.CreateCompatibilityValidator();

            Assert.IsNotNull(validator);
        }

        /// <summary>
        /// Test assembly validation with a non-existent file.
        /// </summary>
        [TestMethod]
        public void ValidateNonExistentAssemblyTest()
        {
            var assemblyResolver = CrossPlatformPluginFactory.GetAssemblyResolver();
            var isValid = assemblyResolver.ValidateAssembly("non-existent-file.dll");

            Assert.IsFalse(isValid, "Non-existent assembly should not be valid");
        }

        /// <summary>
        /// Test compatibility validation with a non-existent file.
        /// </summary>
        [TestMethod]
        public void ValidateNonExistentPluginCompatibilityTest()
        {
            var validator = CrossPlatformPluginFactory.CreateCompatibilityValidator();
            var result = validator.ValidatePlugin("non-existent-file.dll");

            Assert.IsFalse(result.IsValid, "Non-existent plugin should not be compatible");
            Assert.IsTrue(result.Reason.Contains("does not exist"), "Reason should mention file does not exist");
        }

        /// <summary>
        /// Test that plugin metadata can be created.
        /// </summary>
        [TestMethod]
        public void PluginMetadataCreationTest()
        {
            var metadata = new PluginMetadata("TestPlugin", "1.0.0");

            Assert.IsNotNull(metadata);
            Assert.AreEqual("TestPlugin", metadata.Name);
            Assert.AreEqual("1.0.0", metadata.Version);
            Assert.IsTrue(metadata.SupportedPlatforms.Any(), "Should have default supported platforms");
            Assert.IsTrue(metadata.SupportedArchitectures.Any(), "Should have default supported architectures");
        }

        /// <summary>
        /// Test that incompatible plugin info can be created.
        /// </summary>
        [TestMethod]
        public void IncompatiblePluginInfoCreationTest()
        {
            var info = new IncompatiblePluginInfo("test.dll", "Test reason", new Exception("Test exception"));

            Assert.IsNotNull(info);
            Assert.AreEqual("test.dll", info.AssemblyPath);
            Assert.AreEqual("Test reason", info.Reason);
            Assert.IsNotNull(info.Exception);
            Assert.AreEqual("Test exception", info.Exception.Message);
        }
    }
}