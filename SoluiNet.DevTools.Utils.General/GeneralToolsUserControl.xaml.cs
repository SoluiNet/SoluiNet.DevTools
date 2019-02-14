﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SoluiNet.DevTools.Utils.General
{
    /// <summary>
    /// Interaktionslogik für GeneralToolsUserControl.xaml
    /// </summary>
    public partial class GeneralToolsUserControl : UserControl
    {
        public static Dictionary<char, string> ReadableCharacters = new Dictionary<char, string>
        {
            { '\0', "NUL"},
            { ((char)1), "SOH"}, // start of heading
            { ((char)2), "STX"}, // start of text
            { ((char)3), "ETX"}, // end of text
            { ((char)4), "EOT"}, // end of transmission
            { ((char)5), "ENQ"}, // enquiry
            { ((char)6), "ACK"}, // acknowledge
            { ((char)7), "BEL (\\a)"}, // bell
            { ((char)8), "BS (\\b)"}, // backspace
            { ((char)9), "TAB (\\t)"}, // horizontal tab
            { ((char)10), "LF (\\n)"}, // line feed, new line
            { ((char)11), "VT (\\v)"}, // vertical tab
            { ((char)12), "FF (\\f)"}, // form feed, new page
            { ((char)13), "CR (\\r)"}, // carriage return
            { ((char)14), "SO"}, // shift out
            { ((char)15), "SI"}, // shift in
            { ((char)16), "DLE"}, // data link escape
            { ((char)17), "DC1"}, // device control 1
            { ((char)18), "DC2"}, // device control 2
            { ((char)19), "DC3"}, // device control 3
            { ((char)20), "DC4"}, // device control 4
            { ((char)21), "NAK"}, // negative acknowledge
            { ((char)22), "SYN"}, // synchronous idle
            { ((char)23), "ETB"}, // end of transmission block
            { ((char)24), "CAN"}, // cancel
            { ((char)25), "EM"}, // end of medium
            { ((char)26), "SUB"}, // substitute
            { ((char)27), "ESC (\\e)"}, // escape
            { ((char)28), "FS"}, // file separator
            { ((char)29), "GS"}, // group separator
            { ((char)30), "RS"}, // record separator
            { ((char)31), "US"}, // unit separator
            { ((char)32), "SPC"}, // space
            { ((char)127), "DEL"}, // delete
        };

        public GeneralToolsUserControl()
        {
            InitializeComponent();
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            if (Direction.SelectedItem.ToString() == "To Binary")
            {

            }
            else if(Direction.SelectedItem.ToString() == "To Decimal")
            {

            }
        }

        private string MapCharToReadableString(char c)
        {
            return ReadableCharacters.ContainsKey(c) ? ReadableCharacters[c] : c.ToString();
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

                    AsciiTable.Items.Add(tempArray);
                }

                asciiRow[i % 16] = MapCharToReadableString((char) i);
            }

            AsciiTable.Items.Add(asciiRow);
        }
    }
}
