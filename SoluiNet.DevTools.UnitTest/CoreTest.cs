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

            Assert.AreEqual(61.25, Convert.ToDouble(seconds).SecondsToMinutes(), 0.000005);
            Assert.AreEqual(1.02083, Convert.ToDouble(seconds).SecondsToHours(), 0.000005);

            var minutes = 61.25;

            Assert.AreEqual(3675.00, minutes.MinutesToSeconds(), 0.000005);
            Assert.AreEqual(1.02083, minutes.MinutesToHours(), 0.000005);

            var hours = 1.625;

            Assert.AreEqual(5850.00, hours.HoursToSeconds(), 0.000005);
            Assert.AreEqual(97.50, hours.HoursToMinutes(), 0.000005);
        }
    }
}
