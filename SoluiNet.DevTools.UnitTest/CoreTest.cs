// <copyright file="CoreTest.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.UnitTest
{
    using System;
    using System.IO;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SoluiNet.DevTools.Core.Tools.Number;
    using SoluiNet.DevTools.Core.Tools.Stream;
    using SoluiNet.DevTools.Core.Tools.String;

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

            var roundableNumber = 1.25;
            var secondRoundableNumber = 1.24;

            Assert.AreEqual(1.5, roundableNumber.RoundWithDelta(0.5));
            Assert.AreEqual(1.0, secondRoundableNumber.RoundWithDelta(0.5));
        }

        /// <summary>
        /// Test the string tools.
        /// </summary>
        [TestMethod]
        public void StringTest()
        {
            var simpleString = "Hello foo bar. What's up foo bar?";

            Assert.AreEqual("Hello foobar. What's up foo bar?", StringHelper.ReplaceFirstOccurence(simpleString, "foo bar", "foobar"));
            Assert.AreEqual("Hello . What's up foo bar?", StringHelper.ReplaceFirstOccurence(simpleString, "foo bar"));

            var multiLineStringCrlf = "Hello\r\nfoo bar.\r\nWhat's up\r\nfoo bar?";
            var multiLineStringLf = "Hello\nfoo bar.\nWhat's up\nfoo bar?";

            Assert.AreEqual("01: Hello\r\n02: foo bar.\r\n03: What's up\r\n04: foo bar?", multiLineStringCrlf.AddLineNumbers());
            Assert.AreEqual("01: Hello\r\n02: foo bar.\r\n03: What's up\r\n04: foo bar?", multiLineStringLf.AddLineNumbers());

            var simpleStringBase64 = "SGVsbG8gZm9vIGJhci4gV2hhdCdzIHVwIGZvbyBiYXI/";

            Assert.AreEqual(simpleStringBase64, simpleString.ToBase64());
            Assert.AreEqual(simpleString, simpleStringBase64.FromBase64());

            var durationString = "2w 3d 1h 36m 45s";

            Assert.AreEqual(1474605, durationString.GetSecondsFromDurationString());

            var regExPattern = "(foo\\sbar)+";

            Assert.IsTrue(regExPattern.RegExMatch(simpleString));
            Assert.IsTrue(simpleString.MatchesRegEx(regExPattern));

            Assert.AreEqual("Hello foobar. What's up foobar?", simpleString.ReplaceRegEx(regExPattern, "foobar"));
            Assert.AreEqual("Hello. What's up?", simpleString.ReplaceRegEx("(\\s?foo\\sbar\\s?)"));

            Assert.IsTrue("true".IsAffirmative());
            Assert.IsTrue("True".IsAffirmative());
            Assert.IsTrue("TRUE".IsAffirmative());
            Assert.IsTrue("1".IsAffirmative());
            Assert.IsTrue("wahr".IsAffirmative());
            Assert.IsTrue("Wahr".IsAffirmative());
            Assert.IsTrue("WAHR".IsAffirmative());
            Assert.IsTrue("y".IsAffirmative());
            Assert.IsTrue("yes".IsAffirmative());
            Assert.IsTrue("Yes".IsAffirmative());
            Assert.IsTrue("YES".IsAffirmative());

            Assert.IsFalse("false".IsAffirmative());
            Assert.IsFalse("n".IsAffirmative());
            Assert.IsFalse("no".IsAffirmative());
            Assert.IsFalse("0".IsAffirmative());
        }

        /// <summary>
        /// Test the stream tools.
        /// </summary>
        [TestMethod]
        public void StreamTest()
        {
            var simpleString = "Hello foo bar. What's up foo bar? ÄÖÜß€";

            var utf8Bytes = Encoding.UTF8.GetBytes(simpleString);
            var streamUtf8 = new MemoryStream(utf8Bytes);

            try
            {
                Assert.AreEqual(simpleString, StreamHelper.StreamToString(streamUtf8));
            }
            finally
            {
                streamUtf8.Close();
                streamUtf8.Dispose();
            }

            var utf32Bytes = Encoding.UTF32.GetBytes(simpleString);
            var streamUtf32 = new MemoryStream(utf32Bytes);

            try
            {
                Assert.AreEqual(simpleString, StreamHelper.StreamToString(streamUtf32, Encoding.UTF32));
            }
            finally
            {
                streamUtf32.Close();
                streamUtf32.Dispose();
            }

            var iso88591Bytes = Encoding.GetEncoding("ISO-8859-15").GetBytes(simpleString);
            var streamIso88591 = new MemoryStream(iso88591Bytes);

            try
            {
                Assert.AreEqual(simpleString, StreamHelper.StreamToString(streamIso88591, Encoding.GetEncoding("ISO-8859-15")));
            }
            finally
            {
                streamIso88591.Close();
                streamIso88591.Dispose();
            }
        }
    }
}
