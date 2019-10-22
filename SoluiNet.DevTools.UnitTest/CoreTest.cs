// <copyright file="CoreTest.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.UnitTest
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SoluiNet.DevTools.Core.Tools.Number;

    /// <summary>
    /// The tests for the Core library.
    /// </summary>
    [TestClass]
    public class CoreTest
    {
        /// <summary>
        /// Test the number tools.
        /// </summary>
        [TestMethod]
        public void NumberTest()
        {
            var seconds = 3675;

            Assert.AreEqual("1h 1m 15s", seconds.ToDurationString());
            Assert.AreEqual("1h 1m 15s", Convert.ToDouble(seconds).ToDurationString());
        }
    }
}
