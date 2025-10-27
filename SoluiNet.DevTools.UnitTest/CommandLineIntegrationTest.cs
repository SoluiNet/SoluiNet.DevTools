// <copyright file="CommandLineIntegrationTest.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.UnitTest
{
    using System;
    using CommandLine;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SoluiNet.DevTools.Console.Options;

    /// <summary>
    /// End-to-end tests for command-line functionality.
    /// </summary>
    [TestClass]
    public class CommandLineIntegrationTest
    {
        /// <summary>
        /// Test command line parsing with valid help option.
        /// </summary>
        [TestMethod]
        public void CommandLineParsing_ShouldParseHelpOption()
        {
            // Arrange
            var args = new[] { "--help" };

            // Act
            var parseResult = CommandLine.Parser.Default.ParseArguments<RunOptions>(args);
            RunOptions parsedOptions = null;
            parseResult.WithParsed(options => parsedOptions = options);

            // Assert
            Assert.IsNotNull(parsedOptions);
            Assert.IsTrue(parsedOptions.Help);
        }

        /// <summary>
        /// Test command line parsing with valid verbose option.
        /// </summary>
        [TestMethod]
        public void CommandLineParsing_ShouldParseVerboseOption()
        {
            // Arrange
            var args = new[] { "--verbose" };

            // Act
            var parseResult = CommandLine.Parser.Default.ParseArguments<RunOptions>(args);
            RunOptions parsedOptions = null;
            parseResult.WithParsed(options => parsedOptions = options);

            // Assert
            Assert.IsNotNull(parsedOptions);
            Assert.IsTrue(parsedOptions.Verbose);
        }

        /// <summary>
        /// Test command line parsing with short options.
        /// </summary>
        [TestMethod]
        public void CommandLineParsing_ShouldParseShortOptions()
        {
            // Arrange
            var args = new[] { "-h", "-v" };

            // Act
            var parseResult = CommandLine.Parser.Default.ParseArguments<RunOptions>(args);
            RunOptions parsedOptions = null;
            parseResult.WithParsed(options => parsedOptions = options);

            // Assert
            Assert.IsNotNull(parsedOptions);
            Assert.IsTrue(parsedOptions.Help);
            Assert.IsTrue(parsedOptions.Verbose);
        }

        /// <summary>
        /// Test command line parsing with no arguments.
        /// </summary>
        [TestMethod]
        public void CommandLineParsing_ShouldParseEmptyArguments()
        {
            // Arrange
            var args = Array.Empty<string>();

            // Act
            var parseResult = CommandLine.Parser.Default.ParseArguments<RunOptions>(args);
            RunOptions parsedOptions = null;
            parseResult.WithParsed(options => parsedOptions = options);

            // Assert
            Assert.IsNotNull(parsedOptions);
            Assert.IsFalse(parsedOptions.Help);
            Assert.IsFalse(parsedOptions.Verbose);
        }

        /// <summary>
        /// Test command line parsing with invalid arguments.
        /// </summary>
        [TestMethod]
        public void CommandLineParsing_ShouldHandleInvalidArguments()
        {
            // Arrange
            var args = new[] { "--invalid-option" };

            // Act
            var parseResult = CommandLine.Parser.Default.ParseArguments<RunOptions>(args);
            var hasErrors = false;
            parseResult.WithNotParsed(errors => hasErrors = true);

            // Assert
            Assert.IsTrue(hasErrors);
        }

        /// <summary>
        /// Test command line parsing with combined options.
        /// </summary>
        [TestMethod]
        public void CommandLineParsing_ShouldParseCombinedOptions()
        {
            // Arrange
            var args = new[] { "--help", "--verbose" };

            // Act
            var parseResult = CommandLine.Parser.Default.ParseArguments<RunOptions>(args);
            RunOptions parsedOptions = null;
            parseResult.WithParsed(options => parsedOptions = options);

            // Assert
            Assert.IsNotNull(parsedOptions);
            Assert.IsTrue(parsedOptions.Help);
            Assert.IsTrue(parsedOptions.Verbose);
        }

        /// <summary>
        /// Test that RunOptions can be created with default values.
        /// </summary>
        [TestMethod]
        public void RunOptions_ShouldHaveCorrectDefaults()
        {
            // Act
            var options = new RunOptions();

            // Assert
            Assert.IsNotNull(options);
            Assert.IsFalse(options.Help);
            Assert.IsFalse(options.Verbose);
        }

        /// <summary>
        /// Test that RunOptions properties can be set.
        /// </summary>
        [TestMethod]
        public void RunOptions_ShouldAllowPropertySetting()
        {
            // Arrange
            var options = new RunOptions();

            // Act
            options.Help = true;
            options.Verbose = true;

            // Assert
            Assert.IsTrue(options.Help);
            Assert.IsTrue(options.Verbose);
        }
    }
}