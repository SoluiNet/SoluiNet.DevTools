// <copyright file="GeneralToolsUserControl.xaml.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.General
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using SoluiNet.DevTools.Core.Tools.String;

    /// <summary>
    /// Interaction logic for GeneralToolsUserControl.xaml.
    /// </summary>
    public partial class GeneralToolsUserControl : UserControl
    {
        private static Dictionary<char, string> readableCharacters = new Dictionary<char, string>
        {
            { '\0', "NUL" },
            { ((char)1), "SOH" }, // start of heading
            { ((char)2), "STX" }, // start of text
            { ((char)3), "ETX" }, // end of text
            { ((char)4), "EOT" }, // end of transmission
            { ((char)5), "ENQ" }, // enquiry
            { ((char)6), "ACK" }, // acknowledge
            { ((char)7), "BEL (\\a)" }, // bell
            { ((char)8), "BS (\\b)" }, // backspace
            { ((char)9), "TAB (\\t)" }, // horizontal tab
            { ((char)10), "LF (\\n)" }, // line feed, new line
            { ((char)11), "VT (\\v)" }, // vertical tab
            { ((char)12), "FF (\\f)" }, // form feed, new page
            { ((char)13), "CR (\\r)" }, // carriage return
            { ((char)14), "SO" }, // shift out
            { ((char)15), "SI" }, // shift in
            { ((char)16), "DLE" }, // data link escape
            { ((char)17), "DC1" }, // device control 1
            { ((char)18), "DC2" }, // device control 2
            { ((char)19), "DC3" }, // device control 3
            { ((char)20), "DC4" }, // device control 4
            { ((char)21), "NAK" }, // negative acknowledge
            { ((char)22), "SYN" }, // synchronous idle
            { ((char)23), "ETB" }, // end of transmission block
            { ((char)24), "CAN" }, // cancel
            { ((char)25), "EM" }, // end of medium
            { ((char)26), "SUB" }, // substitute
            { ((char)27), "ESC (\\e)" }, // escape
            { ((char)28), "FS" }, // file separator
            { ((char)29), "GS" }, // group separator
            { ((char)30), "RS" }, // record separator
            { ((char)31), "US" }, // unit separator
            { ((char)32), "SPC" }, // space
            { ((char)127), "DEL" }, // delete
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralToolsUserControl"/> class.
        /// </summary>
        public GeneralToolsUserControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the readable characters.
        /// </summary>
        public static Dictionary<char, string> ReadableCharacters
        {
            get
            {
                return readableCharacters;
            }
        }

        private static string MapCharToReadableString(char c)
        {
            return ReadableCharacters.ContainsKey(c) ? ReadableCharacters[c] : c.ToString(CultureInfo.InvariantCulture);
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            var selectedDirection = (this.Direction.SelectedItem as ComboBoxItem).Content;
            var selectedBinaryType = (this.BinaryType.SelectedItem as ComboBoxItem).Content;

            if (selectedDirection.ToString() == "To Binary")
            {
                int baseForCalculation;

                switch (selectedBinaryType.ToString())
                {
                    case "Hexadecimal":
                        baseForCalculation = 16;
                        break;
                    case "Octal":
                        baseForCalculation = 8;
                        break;
                    case "Binary":
                        baseForCalculation = 2;
                        break;
                    case "Decimal":
                    default:
                        baseForCalculation = 10;
                        break;
                }

                var decimalValue = Convert.ToInt64(this.NumberOutput.Text, CultureInfo.InvariantCulture);

                var binaryResult = string.Empty;

                var numberArray = GeneralTools.GetNumberArrayByBase(decimalValue, baseForCalculation);

                for (int i = 0; i < numberArray.Length; i++)
                {
                    binaryResult += baseForCalculation == 16 ? GeneralTools.GetHexadecimalValue(numberArray[i]) : numberArray[i].ToString(CultureInfo.InvariantCulture);
                }

                this.BinaryInput.Text = binaryResult;
            }
            else if (selectedDirection.ToString() == "To Decimal")
            {
                int baseForCalculation;

                switch (selectedBinaryType.ToString())
                {
                    case "Hexadecimal":
                        baseForCalculation = 16;
                        break;
                    case "Octal":
                        baseForCalculation = 8;
                        break;
                    case "Binary":
                        baseForCalculation = 2;
                        break;
                    case "Decimal":
                    default:
                        baseForCalculation = 10;
                        break;
                }

                long decimalValue = 0;

                for (int i = 0; i < this.BinaryInput.Text.Length; i++)
                {
                    var valence = Convert.ToInt64(Math.Pow(baseForCalculation, this.BinaryInput.Text.Length - 1 - i));
                    var quantity = baseForCalculation == 16 ? GeneralTools.GetDecimalValueForHex(this.BinaryInput.Text[i]) : Convert.ToInt64(this.BinaryInput.Text[i].ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);

                    decimalValue += quantity * valence;
                }

                this.NumberOutput.Text = decimalValue.ToString(CultureInfo.InvariantCulture);
            }
        }

        private void TabControl_Initialized(object sender, EventArgs e)
        {
            var asciiRow = new string[16];

            for (var i = 0; i <= 255; i++)
            {
                if (i % 16 == 0 && i > 0)
                {
                    var tempArray = new string[16];
                    asciiRow.CopyTo(tempArray, 0);

                    this.AsciiTable.Items.Add(tempArray);
                }

                asciiRow[i % 16] = GeneralToolsUserControl.MapCharToReadableString((char)i);
            }

            this.AsciiTable.Items.Add(asciiRow);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase", Justification = "A lower cast is inteded at this point.")]
        private void CalculateString_Click(object sender, RoutedEventArgs e)
        {
            var inputValue = this.StringInput.Text;
            var selectedModification = (this.StringModification.SelectedItem as ComboBoxItem).Content;

            string outputValue;

            switch (selectedModification.ToString())
            {
                case "Upper":
                    outputValue = inputValue.ToUpperInvariant();
                    break;
                case "Lower":
                    outputValue = inputValue.ToLowerInvariant();
                    break;
                case "Substring":
                    var additionalParameters = this.AdditionalParameters.Text;
                    var startIndex = 0;
                    var length = inputValue.Length;

                    var lengthProvided = false;

                    if (!string.IsNullOrEmpty(additionalParameters))
                    {
                        if (additionalParameters.Contains(','))
                        {
                            startIndex = Convert.ToInt32(additionalParameters.Split(',')[0], CultureInfo.InvariantCulture);
                            length = Convert.ToInt32(additionalParameters.Split(',')[1], CultureInfo.InvariantCulture);

                            lengthProvided = true;
                        }
                        else
                        {
                            startIndex = Convert.ToInt32(additionalParameters, CultureInfo.InvariantCulture);
                            length -= startIndex;
                        }
                    }

                    if (lengthProvided)
                    {
                        outputValue = inputValue.Substring(startIndex, length);
                    }
                    else
                    {
                        outputValue = inputValue.Substring(startIndex);
                    }

                    break;
                case "ToBase64":
                    outputValue = inputValue.ToBase64();
                    break;
                default:
                    outputValue = string.Format(CultureInfo.InvariantCulture, "no change ({0})", inputValue);
                    break;
            }

            this.StringOutput.Text = outputValue;
        }

        private void GenerateGuid_Click(object sender, RoutedEventArgs e)
        {
            var guid = Guid.NewGuid();

            this.GuidContent.Text = guid.ToString();

            if (this.GuidOptions.SelectedValue != null
                && (this.GuidOptions.SelectedValue is ComboBoxItem)
                && !string.IsNullOrEmpty((this.GuidOptions.SelectedValue as ComboBoxItem).Content.ToString())
                && (this.GuidOptions.SelectedValue as ComboBoxItem).Content.ToString() == "Upper")
            {
                this.GuidContent.Text = guid.ToString().ToUpperInvariant();
            }

            if (this.GuidOptions.SelectedValue != null
                && (this.GuidOptions.SelectedValue is ComboBoxItem)
                && !string.IsNullOrEmpty((this.GuidOptions.SelectedValue as ComboBoxItem).Content.ToString())
                && (this.GuidOptions.SelectedValue as ComboBoxItem).Content.ToString() == "Lower")
            {
#pragma warning disable CA1308 // A change to lower case is intended
                this.GuidContent.Text = guid.ToString().ToLowerInvariant();
#pragma warning restore CA1308 // A change to lower case is intended
            }
        }
    }
}
