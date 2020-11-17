// <copyright file="XmlDataExtension.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.UI.WPF.XmlData
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Media;
    using SoluiNet.DevTools.Core.UI.WPF.Tools.String;
    using SoluiNet.DevTools.Core.XmlData;

    /// <summary>
    /// A collection of extension methods for xml data classes.
    /// </summary>
    public static class XmlDataExtension
    {
        /// <summary>
        /// Convert a SoluiNet.BrushDefinition to <see cref="Brush"/>.
        /// </summary>
        /// <param name="soluiNetBrush">The SoluiNet.BrushDefinition.</param>
        /// <returns>Returns a <see cref="Brush"/> object which represents the overgiven SoluiNet.BrushDefinition.</returns>
        public static Brush ToBrush(this SoluiNetBrushDefinitionType soluiNetBrush)
        {
            if (soluiNetBrush == null)
            {
                throw new ArgumentNullException(nameof(soluiNetBrush));
            }

            Brush brush = null;

            switch (soluiNetBrush.type)
            {
                case SoluiNetBrushType.SimpleLinearGradient:
                    brush = new LinearGradientBrush(soluiNetBrush.startColour.ToColour(), soluiNetBrush.endColour.ToColour(), Convert.ToDouble(soluiNetBrush.angle));
                    break;
                case SoluiNetBrushType.LinearGradient:
                    brush = new LinearGradientBrush();

                    foreach (var gradientStop in soluiNetBrush.SoluiNetGradientStop)
                    {
                        (brush as LinearGradientBrush).GradientStops.Add(new GradientStop(gradientStop.colour.ToColour(), Convert.ToDouble(gradientStop.offset)));
                    }

                    (brush as LinearGradientBrush).StartPoint = new System.Windows.Point(Convert.ToDouble(soluiNetBrush.SoluiNetStartPoint.xAxis), Convert.ToDouble(soluiNetBrush.SoluiNetStartPoint.yAxis));
                    (brush as LinearGradientBrush).EndPoint = new System.Windows.Point(Convert.ToDouble(soluiNetBrush.SoluiNetEndPoint.xAxis), Convert.ToDouble(soluiNetBrush.SoluiNetEndPoint.yAxis));
                    break;
            }

            return brush;
        }
    }
}
